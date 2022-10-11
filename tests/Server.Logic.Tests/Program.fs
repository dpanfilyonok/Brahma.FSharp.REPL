open Expecto

[<Tests>]
let allTests =
    testList "All tests" [
        testList "Smoke tests" ModelTests.tests
    ]
    |> testSequenced


[<EntryPoint>]
let main argv =
    allTests
    |> runTestsWithCLIArgs [] argv
