namespace Helix

open Expecto

module Program =

  [<EntryPoint>]
  let main args = runTestsWithCLIArgs [] args (testList "all" [ VerifyGoogleRecaptchaIntegration.tests; VerifySlackIntegration.tests ])