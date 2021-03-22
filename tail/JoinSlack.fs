namespace Tail

open System

open FSharp.Control.Tasks.NonAffine

open Giraffe

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Microsoft.Net.Http.Headers

module JoinSlack =
  type Model = {
    EmailAddress: string
    Token: string
  }

  //let handle command = Command -> Task<Result<

  let handler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        match MediaType.matches (StringSegment(ctx.Request.ContentType)) (StringSegment("application/json")) with
        | false -> 
          return! RequestErrors.unsupportedMediaType Problems.contentTypeNotSupported next ctx
        | true ->
          try
            let! model = ctx.BindJsonAsync<Model>()
            if not(model.EmailAddress.Contains('@')) then
              return! RequestErrors.badRequest Problems.badEmailAddress next ctx
            else
              return! next ctx
          with
          | e ->
            // TODO: Log it ...
            return! RequestErrors.badRequest Problems.contentMalformed next ctx
      }
// if not an email address
//   BAD REQUEST
// if known email address (verify using storage)
//   OK
// if not expected captcha reponse then
//   BAD REQUEST
// if link expired then
//   store email address
//   OK
// if link not expired then
//   Open link in AngleSharp
//   Fill in email address
//   Submit
//   If page
