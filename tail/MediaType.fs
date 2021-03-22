namespace Tail

open Microsoft.Extensions.Primitives
open Microsoft.Net.Http.Headers

module MediaType =
  let matches (actual: StringSegment) (expected: StringSegment) =
    let parsed =
      ref (MediaTypeHeaderValue(StringSegment("*/*")))

    MediaTypeHeaderValue.TryParse(actual, parsed) && parsed.Value.MediaType = expected