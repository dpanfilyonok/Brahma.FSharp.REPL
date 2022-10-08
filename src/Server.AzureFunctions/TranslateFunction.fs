namespace Server.AzureFunctions

open System
open System.IO
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open Model

module Translate =
    let translator = TranslationProvider()

    [<AllowNullLiteral>]
    type CodeContainer() =
        member val Code = "" with get, set

    [<FunctionName("Translate")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest)
        (log: ILogger)
        =
        async {
            log.LogInformation($"F# HTTP trigger function processed a request.\n{req.Body}")

            use stream = new StreamReader(req.Body)
            let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask
            let data = JsonConvert.DeserializeObject<CodeContainer>(reqBody)

            let responseMessage = translator.Translate(data.Code)

            return OkObjectResult(responseMessage) :> IActionResult
        }
        |> Async.StartAsTask
