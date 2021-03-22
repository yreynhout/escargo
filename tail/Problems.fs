namespace Tail

open System
open System.Collections.Generic
open System.Net
open System.Text.Json

open FSharp.Control.Tasks.NonAffine

open Giraffe

open Microsoft.AspNetCore.Http

module Problems =
  type ProblemDetail =
    { Type: Uri
      Title: string option
      StatusCode: HttpStatusCode option
      Detail: string option
      Instance: Uri option
      Extensions: IDictionary<string, string> option }

  let private problem (named: string) : ProblemDetail =
    {
      Type = Uri($"urn:escargo:{named}")
      Title = None
      StatusCode = None
      Detail = None
      Instance = None
      Extensions = None
    }

  let private handle problem : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        ctx.SetContentType("application/problem+json")
        use writer = new Utf8JsonWriter(ctx.Response.Body)
        writer.WriteStartObject()
        writer.WriteString("type", problem.Type.AbsoluteUri)
        problem.Title
        |> Option.iter (fun title ->
          writer.WriteString("title", title)
        )
        problem.StatusCode
        |> Option.iter (fun status ->
          writer.WriteNumber("status", LanguagePrimitives.EnumToValue status)
        )
        problem.Detail
        |> Option.iter (fun detail ->
          writer.WriteString("detail", detail)
        )
        problem.Instance
        |> Option.iter (fun instance ->
          writer.WriteString("instance", instance.AbsoluteUri)
        )
        problem.Extensions
        |> Option.iter (fun (extensions: IDictionary<string, string>) ->
          extensions
          |> Seq.iter (fun extension ->
            writer.WriteString(extension.Key, extension.Value)
          )
        )
        writer.WriteEndObject()
        do! writer.FlushAsync(ctx.RequestAborted)
        return! next ctx
      }

  let contentTypeNotSupported : HttpHandler =
    handle (problem "content-type-not-supported")
  let contentMalformed : HttpHandler =
      handle (problem "content-malformed")
  let badEmailAddress : HttpHandler =
    handle (problem "bad-email-address")

