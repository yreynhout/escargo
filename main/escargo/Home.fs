namespace Escargo

open Giraffe.ViewEngine

module rec Home =
    let private bodyContent =
        [ div [ _class "flex h-screen justify-center items-center" ] [
              div [ _class "flex opacity-95 w-1/2 h-1/2" ] [
                  div [ _class "flex-1 bg-gray-900 rounded-l-lg w-1/2 h-full" ] [
                      img [ _class "mx-auto h-full"
                            _src "images/slack-icon.svg" ]
                  ]
                  div [ _class "flex-1 bg-gray-800 rounded-r-lg w-1/2 h-full px-4 py-12" ] [
                      div [] [
                          // TODO: Need to center all this content
                          h1 [ _class "text-2xl text-center font-extrabold uppercase tracking-wider text-gray-300" ] [
                              str "join the ddd-cqrs-es slack"
                          ]
                          form [ _class "flex flex-col mt-5 space-y-6 h-full"
                                 _action "#"
                                 _method "POST" ] [
                              div [ _class "w-full" ] [
                                  label [ _for "email"; _class "sr-only" ] [
                                      str "Email"
                                  ]
                                  input [ _type "text"
                                          _name "email"
                                          _id "email"
                                          _autocomplete "email"
                                          _required
                                          _class
                                              "mx-auto block w-3/4 focus:ring-gray-300 focus:border-gray-300 rounded-md"
                                          _placeholder "Your email address" ]
                              ]
                              div [ _class "flex items-center justify-center" ] [
                                  button [ _type "submit"
                                           _class
                                               "inline-flex items-center justify-center px-5 py-3 border border-transparent text-base font-medium rounded-md text-white bg-red-600 opacity-100 hover:bg-red-700 shadow-sm" ] [
                                      str "Join"
                                  ]
                              ]
                          ]
                      ]
                  ]
              ]
          ] ]

    let private document domain =
        html [ _lang "en"
               _class "m-0 p-0 w-full h-full bg-cover bg-composition8" ] [
            head
                []
                ([ Page.titled "DDD-CQRS-ES"
                   FavIcon.links
                   Meta.tags
                   PlausibleAnalytics.scripts domain
                   Tailwind.stylesheets
                   Cascading.stylesheet ((nameof Home).ToLowerInvariant()) ]
                 |> List.concat)
            body [ _class "m-0 p-0 min-w-320 w-full h-full" ] bodyContent
        ]

    let file domain =
        ((nameof Home).ToLowerInvariant() + ".html", document domain)
