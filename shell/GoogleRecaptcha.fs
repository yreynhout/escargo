namespace Shell

open Giraffe.ViewEngine

module GoogleRecaptcha =
  let scripts =
    [ script [ _async
               _defer
               _type "text/javascript"
               _src "https://www.google.com/recaptcha/api.js" ] [] ]
