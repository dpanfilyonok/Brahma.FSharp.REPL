namespace Server.Logic

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open Brahma.FSharp.OpenCL.Translator
open Brahma.FSharp.OpenCL.Printer
open FSharp.Quotations

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

        let getFullPath relativePath =
            Path.Combine [|
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                relativePath
            |]

        let errors, exitCode, dynAssembly =
            checker.CompileToDynamicAssembly(
                [|
                    "-o"; fn3
                    "-a"; fn2
                    "-r"; getFullPath <| Path.Combine [| ".nuget"; "package"; "brahma.fsharp"; "2.0.1"; "lib"; "net5.0"; "Brahma.FSharp.OpenCL.Core.dll" |]
                    "-r"; getFullPath <| Path.Combine [| ".nuget"; "package"; "brahma.fsharp.opencl.shared"; "2.0.1"; "lib"; "net5.0"; "Brahma.FSharp.OpenCL.Shared.dll" |]
                |],
                execute = None
            )
            |> Async.RunSynchronously

        let assembly =
            match errors, exitCode with
            | [| |], 0 -> dynAssembly.Value
            | _ -> failwithf $"\nExitCode: %i{exitCode}\n%A{errors}"

        // TODO throw exception
        let command =
            let type' = assembly.GetType(moduleName)
            let pInfo = type'.GetProperty(kernelVarName)
            pInfo.GetValue(null) |> unbox<Expr>

        openclTranslate command
