namespace Shell

open Giraffe.ViewEngine

module Cascading =
    let stylesheet name =
        [ link [ _rel "stylesheet"
                 _href $"css/{name}.css" ] ]
