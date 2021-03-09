namespace Shell

open Giraffe.ViewEngine

module Tailwind =
  let stylesheets =
    [ link [ _rel "stylesheet"
             _href "css/tailwind.css" ] ]
