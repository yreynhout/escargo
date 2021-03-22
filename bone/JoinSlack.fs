namespace Bone

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

module JoinSlack =
  // Google reCatpcha integration
  type CanTrustClient = (* token *) string -> Task<bool>
  // Slack integration
  type CanRedirectClient = (* email address *) string -> Task<bool>
  // AWS S3 integration
  type TryToRememberEmailAddressee = (* email address *) string -> Task<bool>

  type Request = 
    {
      // RemoteIP: string
      EmailAddress: string
      Token: string
    }
  
  type BadRequest =
    | BadTokenFormat
    | BadEmailAddressFormat

  type Response =
    | BadRequest of BadRequest
    | DoNotTrustClient
    | RedirectClient
    | ThankClient
    | ApologizeToClient

  let handle 
    (canRedirectClient: CanRedirectClient) 
    (canTrustClient: CanTrustClient) 
    (tryToRememberEmailAddressee: TryToRememberEmailAddressee) 
    (request: Request) = 
    task {
      if String.IsNullOrEmpty(request.Token) then
        return BadRequest BadTokenFormat
      else if String.IsNullOrEmpty(request.EmailAddress) || not(request.EmailAddress.Contains("@")) then
        return BadRequest BadEmailAddressFormat
      else
        let! can_trust_client = canTrustClient request.Token
        if not(can_trust_client) then
          return DoNotTrustClient
        else
          let! can_redirect_client = canRedirectClient request.EmailAddress
          if can_redirect_client then
            return RedirectClient
          else
            let! could_remember_email_addressee = tryToRememberEmailAddressee request.EmailAddress
            if could_remember_email_addressee then
              return ThankClient
            else
              return ApologizeToClient
    }
