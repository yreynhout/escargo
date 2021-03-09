namespace Shell

open Giraffe.ViewEngine

module PlausibleAnalytics =
  let scripts domain =
    [ script [ _async
               _defer
               _type "text/javascript"
               _data "domain" domain
               _src $"https://plausible.io/js/plausible.js" ] [] ]
