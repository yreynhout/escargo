namespace Helix.Pages

open System.IO

module Resources =
  type private Marker() = class end
  let private readEmbeddedResource name =
    let qualifiedResourceName = sprintf "%s.%s" typeof<Marker>.Namespace name
    use stream = typeof<Marker>.Assembly.GetManifestResourceStream(qualifiedResourceName)
    use reader = new StreamReader(stream)
    reader.ReadToEnd()

  let slack_active_link_page = readEmbeddedResource "slack_active_link.html"
  let slack_not_active_link_page = readEmbeddedResource "slack_not_active_link.html"