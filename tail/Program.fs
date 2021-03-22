namespace Tail

open System.IO
open Microsoft.AspNetCore.Hosting


module Program =
#if LAMBDA_ONLY
  // type LambdaEntryPoint() =
  //     inherit Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction()

  //     override _.Init(builder : IWebHostBuilder) =
  //         let contentRoot = Directory.GetCurrentDirectory()

  //         builder
  //             .UseContentRoot(contentRoot)
  //             .Configure(Action<IApplicationBuilder> configureApp)
  //             .ConfigureServices(configureServices)
  //             |> ignore
#endif

  [<EntryPoint>]
  let main args = 0
