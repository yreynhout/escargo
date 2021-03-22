namespace Helix

open Expecto

open System
open System.Net
open System.Net.Http

open RichardSzalay.MockHttp

open Microsoft.Extensions.Logging

module VerifySlackIntegration =
  let factory = new LoggerFactory()
  let log = factory.CreateLogger("-")

  let tests =
    testList
      "integration with Slack"
      [ testTask "the resource associated with the invitation link can be found and has not expired" {
          let link =
            Uri("http://localhost/found_non_expired_invitation_link")

          use handler = new MockHttpMessageHandler()

          handler
            .When(HttpMethod.Get, link.AbsoluteUri)
            .Respond("text/html", Pages.Resources.slack_active_link_page)
          |> ignore

          use client = new HttpClient(handler)

          let! can_redirect = Bone.Slack.can_redirect_to_invitation_url client log link

          Expect.isTrue can_redirect "yet slack reports it can not be found or has expired"
        }

        testTask "the resource associated with the invitation link can no longer be found" {
          let link =
            Uri("http://localhost/expired_invitation_link_not_found")

          use handler = new MockHttpMessageHandler()

          handler
            .When(HttpMethod.Get, link.AbsoluteUri)
            .Respond(HttpStatusCode.NotFound)
          |> ignore

          use client = new HttpClient(handler)

          let! can_redirect = Bone.Slack.can_redirect_to_invitation_url client log link

          Expect.isFalse can_redirect "yet slack reports it can still be found"
        }

        testTask "the resource associated with the invitation link can be found but has expired" {
          let link =
            Uri("http://localhost/expired_invitation_link")

          use handler = new MockHttpMessageHandler()

          handler
            .When(HttpMethod.Get, link.AbsoluteUri)
            .Respond("text/html", Pages.Resources.slack_not_active_link_page)
          |> ignore

          use client = new HttpClient(handler)

          let! can_redirect = Bone.Slack.can_redirect_to_invitation_url client log link

          Expect.isFalse can_redirect "yet slack reports it has not expired"
        } ]
