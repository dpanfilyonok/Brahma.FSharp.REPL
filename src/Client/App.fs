module App

open Browser.Dom
//open Fetch
open Thoth.Fetch
open Feliz

[<ReactComponent>]
let Counter() =
    let count, setCount = React.useState 0

    Html.div [
        Html.h1 count
        Html.button [
            prop.text "Increment"
            prop.onClick <| fun _ ->
                setCount (count + 1)
        ]
    ]

[<ReactComponent>]
let Message() =
    let message, setMessage = React.useState ""

    let code = """
module Command
open Brahma.FSharp

let command =
    <@
        fun (range: Range1D) (buf: ClArray<int>) ->
            buf.[0] <- 1
    @>
"""

    Html.div [
        Html.button [
            prop.text "Get a message from the API"
            prop.onClick <| fun _ ->
                promise {
                    let! message = Fetch.post ("/api/Translate/", code)
                    setMessage message
                    return ()
                }
                |> ignore
        ]

        if message = "" then
            Html.none
        else
            Html.p message
    ]

[<ReactComponent>]
let App () =
    React.fragment [
        Counter()
        Message()
    ]

ReactDOM.render (App(), document.getElementById "root")
