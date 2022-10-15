namespace Server.Giraffe

open System
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Primitives
open Newtonsoft.Json
open Giraffe
open Server.Logic

module Handlers =
    let apiHandler =
        let translator = TranslationProvider()

        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                use stream = new StreamReader(ctx.Request.Body)
                let! reqBody = stream.ReadToEndAsync()
                let data = JsonConvert.DeserializeObject<string>(reqBody)

                let responseMessage = translator.Translate(data)
                return! json responseMessage next ctx
            }

    let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            POST >=> route "/api/Translate" >=> apiHandler
            setStatusCode 404 >=> text "Not Found"
        ]

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
