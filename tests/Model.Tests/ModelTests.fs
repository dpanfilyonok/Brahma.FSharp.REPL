module ModelTests

open Expecto
open Model

type TestData =
    {
        TestName: string
        Code: string
        Expected: string
    }

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
