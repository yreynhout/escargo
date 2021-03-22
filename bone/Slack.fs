namespace Bone

open System
open System.Net
open System.Net.Http

open AngleSharp.Dom
open AngleSharp.Html.Parser

open FSharp.Control.Tasks.NonAffine

open Microsoft.Extensions.Logging

module Slack =
  [<Literal>]
  let private LinkActiveContent = "Join ddd-cqrs-es on Slack"

  [<Literal>]
  let private LinkNotActiveContent = "This link is no longer active"

  let private (|ActiveLink|_|) (elements: IHtmlCollection<IElement>) =
    match elements
          |> Seq.exists (fun element -> element.TextContent = LinkActiveContent) with
    | true -> Some ActiveLink
    | false -> None

  let private (|NotActiveLink|_|) (elements: IHtmlCollection<IElement>) =
    match elements
          |> Seq.exists (fun element -> element.TextContent = LinkNotActiveContent) with
    | true -> Some NotActiveLink
    | false -> None

  let can_redirect_client (client: HttpClient) (log: ILogger) (invitationUrl: Uri) =
    task {
      let! response = client.GetAsync(invitationUrl)

      match response.StatusCode with
      | HttpStatusCode.OK ->
          let parser = HtmlParser()

          try
            use document =
              parser.ParseDocument(response.Content.ReadAsStream())

            let h1Elements = document.GetElementsByTagName("h1")

            match h1Elements with
            | ActiveLink -> return true
            | NotActiveLink -> return false
            | _ ->
                log.LogWarning(
                  "Could not find the H1 element related to the link being either active or not active when following this invitation url {Url}. Received content was parsed as {Html}",
                  invitationUrl,
                  document.DocumentElement.OuterHtml
                )

                return false
          with error ->
            log.LogError(
              error,
              "There was a problem parsing the received content when following this invitation url {Url}",
              invitationUrl
            )

            return false
      | HttpStatusCode.NotFound -> return false
      | _ ->
          log.LogWarning(
            "The received status code {StatusCode} when following this invitation url {Url} was unexpected",
            response.StatusCode,
            invitationUrl
          )

          return false
    }
