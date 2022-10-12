namespace Server.AzureFunctions

open System.IO
open System.Web.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open Server.Logic

module Translate =
    let translator = TranslationProvider()

    [<FunctionName("Translate")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest)
        (log: ILogger)
        =
        async {
            log.LogInformation($"F# HTTP trigger function processed a request.\n{req.Body}")

            use stream = new StreamReader(req.Body)
            let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask
            let data = JsonConvert.DeserializeObject<string>(reqBody)

            try
                let responseMessage = translator.Translate(data)
                return OkObjectResult(responseMessage) :> IActionResult
            with
            | e ->
                return ExceptionResult(e, true) :> IActionResult
        }
        |> Async.StartAsTask
