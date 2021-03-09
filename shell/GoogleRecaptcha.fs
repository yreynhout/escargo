namespace Shell

open Giraffe.ViewEngine

module GoogleRecaptcha =
  let scripts key =
    [ script [ _async
               _defer
               _type "text/javascript"
               _src $"https://www.google.com/recaptcha/enterprise.js?render={key}" ] [] ]
