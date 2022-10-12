open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Server.Giraffe

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()

    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .Configure(Action<IApplicationBuilder> Config.configureApp)
                .ConfigureServices(Config.configureServices)
                .ConfigureLogging(Config.configureLogging)
            |> ignore
        )
        .Build()
        .Run()
    0
