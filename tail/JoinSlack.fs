namespace Tail

module JoinSlack =
    let handler =
        // if not an email address
        //   BAD REQUEST
        // if known email address (verify using storage)
        //   OK
        // if not expected captcha reponse then
        //   BAD REQUEST
        // if link expired then
        //   store email address
        //   OK
        // if link not expired then
        //   Open link in AngleSharp
        //   Fill in email address
        //   Submit
        //   If page 