namespace Bone

open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Json

open System.Text.Json
open System.Text.Json.Serialization

open FSharp.Control.Tasks.NonAffine

open Microsoft.Extensions.Logging

module GoogleRecaptcha =
  [<Literal>]
  let verification_url =
    "https://www.google.com/recaptcha/api/siteverify"

  let options =
    let options =
      JsonSerializerOptions(IgnoreNullValues = true)

    JsonFSharpConverter(unionEncoding = JsonUnionEncoding.UnwrapOption, allowNullFields = true)
    |> options.Converters.Add

    options

  type Request =
    { [<JsonPropertyName("secret")>]
      Secret: string
      [<JsonPropertyName("response")>]
      Response: string
      [<JsonPropertyName("remoteip")>]
      RemoteIP: string option }

  type Response =
    { [<JsonPropertyName("success")>]
      Success: bool
      [<JsonPropertyName("challenge_ts")>]
      ChallengeTimestamp: string
      [<JsonPropertyName("hostname")>]
      Hostname: string
      [<JsonPropertyName("error-codes")>]
      ErrorCodes: string list option }

  //TODO: Capture and use a User's remote public IP
  let can_trust_client (client: HttpClient) (log: ILogger) (secret: string) (token: string) =
    task {
      let content =
        JsonContent.Create<Request>(
          { Secret = secret
            Response = token
            RemoteIP = None },
          MediaTypeHeaderValue.Parse("application/json"),
          options
        )

      let! response = client.PostAsync(verification_url, content)

      match response.StatusCode with
      | HttpStatusCode.OK ->
          try
            let! verified = JsonSerializer.DeserializeAsync<Response>(response.Content.ReadAsStream(), options)

            match verified.Success with
            | true -> return true
            | false ->
                match verified.ErrorCodes with
                | Some errorCodes ->
                    log.LogWarning(
                      "The received response when verifying this token {Token} indicates failure. The following error codes were reported: {ErrorCodes}",
                      token,
                      System.String.Join(",", errorCodes)
                    )
                | None ->
                    log.LogWarning("The received response when verifying this token {Token} indicates failure.", token)

                return false
          with error ->
            log.LogError(
              error,
              "The received response when verifying this token {Token} could not be deserialized.",
              token
            )

            return false
      | _ ->
          log.LogWarning(
            "The received status code {StatusCode} when verifying this token {Token} is unexpected",
            response.StatusCode,
            token
          )

          return false
    }
