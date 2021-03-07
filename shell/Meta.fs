namespace Shell

open Giraffe.ViewEngine

module Meta =
    let private keywords =
        [ "ddd"
          "cqrs"
          "es"
          "community"
          "slack"
          "domain driven design"
          "command and query responsibility segregation"
          "event sourcing" ]

    let tags =
        [ meta [ _charset "UTF-8" ]
          meta [ _name "viewport"
                 _content "width=device-width, height=device-height, initial-scale=1" ]
          meta [ _name "author"
                 _content "Yves Reynhout" ]
          meta [ _name "keywords"
                 _content (System.String.Join(",", keywords)) ]
          meta [ _name "description"
                 _content "DDD-CQRS-ES Slack" ] ]
