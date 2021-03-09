namespace Shell

open System

module Robots =
  let file domain =
    ("robots.txt",
     String.Join(
       Environment.NewLine,
       [ "User-agent: *"
         "Allow: /"
         ""
         ("Sitemap: https://" + domain + "/sitemap.xml") ]
     ))
