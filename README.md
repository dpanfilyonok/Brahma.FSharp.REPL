# Brahma.FSharp REPL

[![Build and Deploy Pages](https://github.com/dpanfilyonok/Brahma.FSharp.REPL/actions/workflows/build-and-deploy-pages.yml/badge.svg?branch=main)](https://github.com/dpanfilyonok/Brahma.FSharp.REPL/actions/workflows/build-and-deploy-pages.yml)

[**Brahma.Sharp**](https://github.com/YaccConstructor/Brahma.FSharp) is a library that provides the ability to use the OpenCL framework in F# programs. 
It comes with a translator from F# to OpenCL C programming language.


With it this **REPL** allows you to translate F# code into OpenCL code _online_.

## Tech Stack

### Client.React
- [**Fable**](https://github.com/fable-compiler/Fable) as F# to JavaScript compiler
- [**Feliz**](https://github.com/Zaid-Ajaj/Feliz) as React API provider in Fable

### Server.AzureFunctions
- **Azure Functions** as serverless backend service provider

### Server.Giraffe
- [**Giraffe**](https://github.com/giraffe-fsharp/Giraffe) as web framework based on ASP.NET Core

### Server.Logic
- Brahma.FSharp
- FSharp.Compiler.Service

### Other
- **Docker** as development environment
- **Github Actions** as CI/CD tool
- **Github Pages** as hosting service
- [**Expecto**](https://github.com/haf/expecto) as testing framework for business logic

## Running The Application Locally

### Manual Environment Setup
* .NET 6 SDK
* Node.js 16
* [Azure Static Web Apps CLI](https://github.com/azure/static-web-apps-cli)
* [Azure Function Core Tools](https://github.com/Azure/azure-functions-core-tools)

Once the repo is created from the terminal run:

```bash
$> dotnet tool restore
$> dotnet paket install
$> npm install
$> npm install -g @azure/static-web-apps-cli azure-functions-core-tools@4
```

With all dependencies installed, you can launch the apps, which will require three terminals:

1. Terminal 1: `npm start`
1. Terminal 2: `cd api && func start`
1. Terminal 3: `swa start http://localhost:3000 --api-location http://localhost:7071`

Then you can navigate to `http://localhost:4280` to access the emulator.

### Docker Setup 

## License
This project licensed under MIT License. License text can be found in the [license file](https://github.com/dpanfilyonok/Brahma.FSharp.REPL/blob/main/LICENSE.md).
