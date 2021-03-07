namespace Shell

open Giraffe.ViewEngine

module FavIcon =
    let links =
        [ link [ _rel "apple-touch-icon"
                 _sizes "180x180"
                 _href "images/apple-touch-icon.png" ]
          link [ _rel "icon"
                 _type "image/png"
                 _sizes "32x32"
                 _href "images/favicon-32x32.png" ]
          link [ _rel "icon"
                 _type "image/png"
                 _sizes "16x16"
                 _href "images/favicon-16x16.png" ]
          link [ _rel "manifest"
                 _href "site.webmanifest" ]
          link [ _rel "mask-icon"
                 _href "images/safari-pinned-tab.svg"
                 _color "#dcd2ae" ]
          meta [ _name "msapplication-TileColor"
                 _content "#dcd2ae" ]
          meta [ _name "msapplication-config"
                 _content "browserconfig.xml" ]
          meta [ _name "theme-color"
                 _content "#dcd2ae" ] ]
