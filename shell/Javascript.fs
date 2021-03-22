namespace Shell

open Giraffe.ViewEngine

module Javascript =
  let private content (endpoint: string) =
    $"""
function onSubmit(token) {{
  const slackIcon = document.getElementById('slack-icon');
  if(!slackIcon.classList.contains('animate-pulse'))
  {{
    slackIcon.classList.add('animate-pulse');
  }}

  fetch('{endpoint}', {{
    method: 'POST',
    headers: {{
      'content-type': 'application/json'
    }},
    body: {{
      email: document.getElementById('email').value,
      token: token
    }}
  }})
  .then(response => {{
    console.log(response);
    slackIcon.classList.remove('animate-pulse');
  }})
  .catch(err => {{
    console.log(err);
    slackIcon.classList.remove('animate-pulse');
  }})
}}"""

  let scripts endpoint =
    [ script [ _type "text/javascript" ] [
        rawText (content endpoint)
      ] ]
