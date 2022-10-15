module ServerGiraffeTests

open System.Net.Http
open Expecto
open Newtonsoft.Json
open Server.Giraffe
open Microsoft.AspNetCore.Mvc.Testing

type TestData =
    {
        TestName: string
        Code: string
        Expected: string
    }

let createTestApp() =
    new WebApplicationFactory<Program.Program>()
    |> fun app -> app.Server

type HttpClient with
    member this.Put (path: string) (payload: obj) =
        let json = JsonConvert.SerializeObject payload

        use content =
            new StringContent(json, System.Text.Encoding.UTF8, "application/json")

        this.PutAsync(path, content) |> Async.AwaitTask

    member this.Post(path: string, payload: obj) =
        let json = JsonConvert.SerializeObject payload

        use content =
            new StringContent(json, System.Text.Encoding.UTF8, "application/json")

        this.PostAsync(path, content) |> Async.AwaitTask

//    member this.Get<'a>(path: string) =
//        this.GetAsync(path)
//        |> Async.AwaitTask
//        |> Async.Bind
//            (fun resp ->
//                resp.Content.ReadAsStringAsync()
//                |> Async.AwaitTask
//                |> Async.map JsonConvert.DeserializeObject<'a>)

    member this.GetString(path: string) =
        this.GetStringAsync(path) |> Async.AwaitTask

//let test =
//    testTask "" {
//        let api = createTestApp().CreateClient()
//        let! response = api.Post("/api/Translate", box "sss")
//        Assert.Equal("Hello world", response)
//    }

let testData = [
    {
        TestName = "Smoke test"
        Code = """
module Command
open Brahma.FSharp

let command =
    <@
        fun (range: Range1D) (buf: ClArray<int>) ->
            buf.[0] <- 1
    @>
"""
        Expected = """
__kernel void brahmaKernel (__global int * buf)
{buf [0] = 1 ;}
""".Trim()
    }

    {
        TestName = "Quotation injection test"
        Code = """
module Command
open Brahma.FSharp

let myF = <@ fun x y -> x - y @>

let command =
    <@ fun (range: Range1D) (buf: int clarray) ->
        buf.[0] <- (%myF) 2 3
        buf.[1] <- (%myF) 4 5
    @>
"""
        Expected = """
int ItemUnitFunc ()
{return (2 - 3) ;}
int ItemUnitFunc2 ()
{return (4 - 5) ;}
__kernel void brahmaKernel (__global int * buf)
{buf [0] = ItemUnitFunc () ;
 buf [1] = ItemUnitFunc2 () ;}
""".Trim()
    }

    {
        TestName = "User defined type test"
        Code = """
module Command
open Brahma.FSharp

[<Struct>]
type StructOfIntInt64 =
    val mutable X: int
    val mutable Y: int64
    new(x, y) = { X = x; Y = y }

let command =
    <@
        fun (range: Range1D) (buf:  ClArray<StructOfIntInt64>) ->
            if range.GlobalID0 = 0 then
                let b = buf.[0]
                buf.[0] <- buf.[1]
                buf.[1] <- b
    @>
"""
        Expected = """
typedef struct struct0 {int X ;
                        long Y ;} struct0 ;
__kernel void brahmaKernel (__global struct0 * buf)
{if ((get_global_id (0) == 0))
 {struct0 b = buf [0] ;
  buf [0] = buf [1] ;
  buf [1] = b ;} ;}
""".Trim()
    }
]

let translator = TranslationProvider()

let tests =
    testData
    |> List.map (fun data ->
        testCase data.TestName <| fun () ->
            let actual = translator.Translate(data.Code)
            "Translated code should be correct"
            |> Expect.equal actual data.Expected
    )
