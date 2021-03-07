namespace Shell

open Giraffe.ViewEngine

module Page =
    let titled value = [ title [] [ encodedText value ] ]
