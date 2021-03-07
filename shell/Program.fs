namespace Shell

open System.IO

open Argu
open Giraffe.ViewEngine

module Program =
    type Arguments =
        | [<Mandatory; EqualsAssignmentOrSpaced>] Output of path: string
        | [<Mandatory; EqualsAssignmentOrSpaced>] Domain of name: string
        | [<Mandatory; EqualsAssignmentOrSpaced>] GoogleRecaptcha of key: string
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Output _ -> "specify an output directory."
                | Domain _ -> "specify a domain name."
                | GoogleRecaptcha _ -> "specify a Google recaptcha site key."

    [<EntryPoint>]
    let main argv =
        let parser =
            ArgumentParser.Create<Arguments>(programName = "escargo")

        try
            let arguments =
                parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)

            let path = arguments.GetResult(Output)
            let domain = arguments.GetResult(Domain)
            let key = arguments.GetResult(GoogleRecaptcha)

            [ Home.file domain key]
            |> List.iter
                (fun (name, document) ->
                    File.WriteAllText(Path.Combine(path, name), RenderView.AsString.htmlDocument document))

            [ SiteMap.file domain ]
            |> List.iter
                (fun (name, document) -> File.WriteAllText(Path.Combine(path, name), RenderXml.xmlDocument document))

            [ Robots.file domain ]
            |> List.iter (fun (name, contents) -> File.WriteAllText(Path.Combine(path, name), contents))

            0
        with e ->
            printfn "%s" e.Message
            1
