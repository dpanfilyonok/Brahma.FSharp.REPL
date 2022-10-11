namespace Server.Giraffe

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

module Config =
    let configureCors (builder: CorsPolicyBuilder) =
        builder
            .WithOrigins(
                "http://localhost:5000",
                "https://localhost:5001"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
        |> ignore

    let configureApp (app: IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

        match env.IsDevelopment() with
        | true  ->
            app
                .UseDeveloperExceptionPage()
        | false ->
            app
                .UseGiraffeErrorHandler(Handlers.errorHandler)
                .UseHttpsRedirection()

        |> fun app ->
            app
                .UseCors(configureCors)
                .UseStaticFiles()
                .UseGiraffe(Handlers.webApp)

    let configureServices (services: IServiceCollection) =
        services
            .AddCors()
            .AddGiraffe()
        |> ignore

    let configureLogging (builder: ILoggingBuilder) =
        builder
            .AddConsole()
            .AddDebug()
        |> ignore
