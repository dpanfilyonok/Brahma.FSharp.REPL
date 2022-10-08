﻿namespace Model

open System.IO
open FSharp.Compiler.CodeAnalysis
open Brahma.FSharp.OpenCL.Translator
open Brahma.FSharp.OpenCL.Printer
open FSharp.Quotations

// TODO можно подумать над обработкой и тестированием ошибок еще (3 типа)
type TranslationProvider() =
    let checker = FSharpChecker.Create()

    let moduleName = "Command"

    let kernelVarName = "command"

    let translator = FSQuotationToOpenCLTranslator.CreateDefault()

    let openclTranslate (expr: Expr) =
        translator.Translate expr
        |> fst
        |> AST.print

    member this.Translate(code: string) =
        let fn = Path.GetTempFileName()
        let fn2 = Path.ChangeExtension(fn, ".fsx")
        let fn3 = Path.ChangeExtension(fn, ".dll")

        File.WriteAllText(fn2, code)

        let errors, exitCode, dynAssembly =
            checker.CompileToDynamicAssembly(
                [|
                    "-o"; fn3
                    "-a"; fn2
                    "-r"; @"C:\Users\anticnvm\.nuget\packages\brahma.fsharp\2.0.1\lib\net5.0\Brahma.FSharp.OpenCL.Core.dll"
                    "-r"; @"C:\Users\anticnvm\.nuget\packages\brahma.fsharp.opencl.shared\2.0.1\lib\net5.0\Brahma.FSharp.OpenCL.Shared.dll"
                |],
                execute = None
            )
            |> Async.RunSynchronously

        let assembly =
            match errors, exitCode with
            | [| |], 0 -> dynAssembly.Value
            | _ -> failwithf $"\nExitCode: %i{exitCode}\n%A{errors}"

        // тут нуллы возвращаются, а не исключение кидается
        let command =
            let type' = assembly.GetType(moduleName)
            let pInfo = type'.GetProperty(kernelVarName)
            pInfo.GetValue(null) |> unbox<Expr>

        openclTranslate command