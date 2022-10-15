open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Server.Giraffe
open Giraffe
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

let builder = WebApplication.CreateBuilder()

builder.Services
    .AddCors()
    .AddGiraffe()
|> ignore

builder.Logging
    .AddConsole()
    .AddDebug()
|> ignore

let app = builder.Build()

app
    .UseGiraffeErrorHandler(Handlers.errorHandler)
    .UseHttpsRedirection()
    .UseCors(fun corsBuilder ->
        corsBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
        |> ignore
    )
    .UseStaticFiles()
    .UseGiraffe(Handlers.webApp)

app.Run()

type Program() = class end
