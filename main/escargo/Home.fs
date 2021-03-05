namespace Escargo

open Giraffe.ViewEngine

module rec Home =
    let private document domain =
        Styled.html [ head
                          []
                          ([ Page.titled "DDD-CQRS-ES"
                             FavIcon.links
                             Meta.tags
                             PlausibleAnalytics.scripts domain
                             Tailwind.stylesheets
                             Cascading.stylesheet ((nameof Home).ToLowerInvariant()) ]
                           |> List.concat)
                      Styled.body [] ]

    let file domain = ((nameof Home).ToLowerInvariant() + ".html", document domain)
