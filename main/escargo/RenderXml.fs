namespace Escargo

open Giraffe.ViewEngine

module RenderXml =
    let xmlDocument (document: XmlNode) : string =
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
        + RenderView.AsString.xmlNode document
