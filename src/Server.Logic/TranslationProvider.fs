namespace Server.Logic

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open Brahma.FSharp.OpenCL.Translator
open Brahma.FSharp.OpenCL.Printer
open FSharp.Quotations
open System.Runtime.InteropServices
open Microsoft.FSharp.Core

type TranslationProvider() =
    let checker = FSharpChecker.Create()

    let moduleName = "Command"

    let kernelVarName = "command"

    let translator = FSQuotationToOpenCLTranslator.CreateDefault()

    // TODO check exception
    let isDocker =
        Environment.GetEnvironmentVariable("IS_DOCKER")
        |> Option.ofObj
        |> Option.defaultValue "false"
        |> Convert.ToBoolean

    let openclTranslate (expr: Expr) =
        translator.Translate expr
        |> fst
        |> AST.print

    member this.Translate(code: string) =
        let fn = Path.GetTempFileName()
        let fn2 = Path.ChangeExtension(fn, ".fsx")
        let fn3 = Path.ChangeExtension(fn, ".dll")

        File.WriteAllText(fn2, code)

        let references =
            [|
                if not isDocker then
                    let getPathFromRoot paths =
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            Path.Combine(paths)
                        )

                    "-r"; getPathFromRoot [| ".nuget"; "packages"; "brahma.fsharp"; "2.0.1"; "lib"; "net5.0"; "Brahma.FSharp.OpenCL.Core.dll" |]
                    "-r"; getPathFromRoot [| ".nuget"; "packages"; "brahma.fsharp.opencl.shared"; "2.0.1"; "lib"; "net5.0"; "Brahma.FSharp.OpenCL.Shared.dll" |]

                else
                    let dlls =
                        RuntimeEnvironment.GetRuntimeDirectory()
                        |> DirectoryInfo
                        |> fun di -> di.EnumerateFiles()
                        |> Seq.map (fun fi -> fi.FullName)
                        |> Seq.filter (fun names -> names.EndsWith(".dll"))
                        |> Seq.map (sprintf "-r:%s")

                    let monoPath = @"/usr/lib/mono/4.5/"

                    "-I"; monoPath
                    "-I"; Path.Combine [| RuntimeEnvironment.GetRuntimeDirectory(); |]

                    "-r"; Path.Combine [| monoPath; "System.Runtime.Remoting.dll" |]
                    "-r"; Path.Combine [| monoPath; "System.Runtime.Serialization.Formatters.Soap.dll" |]
                    "-r"; Path.Combine [| monoPath; "System.Web.Services.dll" |]
                    "-r"; Path.Combine [| monoPath; "System.Windows.Forms.dll" |]
                    "-r"; @"./Brahma.FSharp.OpenCL.Core.dll"
                    "-r"; @"./Brahma.FSharp.OpenCL.Shared.dll"
                    yield! dlls
            |]

        let errors, exitCode, dynAssembly =
            checker.CompileToDynamicAssembly(
                [|
                    "--noframework"
                    "-o"; fn3
                    "-a"; fn2
                    yield! references
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
