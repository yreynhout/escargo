namespace Helix

open Expecto

open System
open System.Net
open System.Net.Http
open System.Text.Json

open RichardSzalay.MockHttp

open Bone.GoogleRecaptcha

open Microsoft.Extensions.Logging

module VerifyGoogleRecaptchaIntegration =
  let factory = new LoggerFactory()
  let log = factory.CreateLogger("-")
  let secret = "secret"

  let tests =
    testList
      "integration with Google reCaptcha"
      [ testTask "using an invalid token has the expected result" {
          let handler = new MockHttpMessageHandler()

          let invalid_token = "invalid_token"

          handler
            .When(HttpMethod.Post, verification_url)
            .Respond(
              "application/json",
              JsonSerializer.Serialize(
                { Success = false
                  ChallengeTimestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd\THH:mm:ssZZ")
                  Hostname = ""
                  ErrorCodes = Some [ "invalid-input-response" ] },
                options)
            ) |> ignore

          use client = new HttpClient(handler)

          let! can_trust = can_trust_client client log secret invalid_token

          Expect.isFalse can_trust "yet Google reports the token and request can be trusted"
        }

        testTask "using a valid token has the expected result" {
          let handler = new MockHttpMessageHandler()

          let valid_token = "valid_token"

          handler
            .When(HttpMethod.Post, verification_url)
            .Respond(
              "application/json",
              JsonSerializer.Serialize(
                { Success = true
                  ChallengeTimestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd\THH:mm:ssZZ")
                  Hostname = ""
                  ErrorCodes = None },
                options)
            ) |> ignore

          use client = new HttpClient(handler)

          let! can_trust = can_trust_client client log secret valid_token

          Expect.isTrue can_trust "yet Google reports the token and request can not be trusted"
        }
        ]
