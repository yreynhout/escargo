namespace Escargo

open Giraffe.ViewEngine

module Styled =
    let html (content: XmlNode list) =
        html [ _lang "en"; _class "m-0 p-0 w-full h-full bg-cover bg-composition8" ] content

    let body (content: XmlNode list) =
        body [ _class "m-0 p-0 min-w-320 w-full h-full" ] content
