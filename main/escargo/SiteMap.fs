namespace Escargo

open System
open Giraffe.ViewEngine

module SiteMap =
    let private xmlns value = attr "xmlns" value

    let private xsi =
        attr "xmlns:xsi" "http://www.w3.org/2001/XMLSchema-instance"

    let private schemaLocation value = attr "xsi:schemaLocation" value
    let private urlset = tag "urlset"
    let private url = tag "url"
    let private loc = tag "loc"
    let private lastmod = tag "lastmod"

    let private document domain =
        let now =
            DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss+00:00")

        urlset [ xmlns "http://www.sitemaps.org/schemas/sitemap/0.9"
                 xsi
                 schemaLocation
                     "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd" ] [
            url [] [
                loc [] [
                    str ("https://" + domain + "/")
                ]
                lastmod [] [ str now ]
            ]
            url [] [
                loc [] [
                    str ("https://" + domain + "/home.html")
                ]
                lastmod [] [ str now ]
            ]
        ]

    let file domain = ("sitemap.xml", document domain)
