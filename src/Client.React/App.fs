module Client.React

open Browser.Dom
open Browser.Types
open Fetch
open Thoth.Fetch
open Feliz

[<ReactComponent>]
let Translator() =
    let message, setMessage = React.useState ""

    Html.div [
        Html.div [
            prop.className "home-container"
            prop.children [
                Html.div [
                    prop.className "inner-container"
                    prop.children [
                        Html.textarea [
                            prop.id "codeInputTextarea"
                            prop.classes [ "home-textarea"; "textarea" ]
                            prop.defaultValue "module Command\nopen Brahma.FSharp\n\nlet command = "
                            prop.placeholder "Type your kernel code here"
                            prop.spellcheck false
                            prop.custom ("data-lt-active", false)
                        ]
                        Html.button [
                            prop.classes [ "home-button"; "button" ]
                            prop.text "GO"
                            prop.onClick <| fun _ ->
                                promise {
                                    let textArea = document.getElementById("codeInputTextarea") :?> HTMLTextAreaElement
                                    let code = textArea.value

                                    let! message =
                                        Fetch.post(
                                            "api/Translate",
                                            code,
                                            headers = [
                                                HttpRequestHeaders.Accept "application/json"
                                                HttpRequestHeaders.Custom ("Access-Control-Allow-Origin", "*")
                                            ],
                                            properties = [
                                                RequestProperties.Mode RequestMode.Nocors
                                            ]
                                        )

                                    setMessage message
                                    return ()
                                }
                                |> ignore
                        ]
                        Html.span [
                            prop.classes [ "home-text"; "textarea" ]
                            prop.text message
                            prop.style [
                                style.whitespace.preline
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let App() =
    React.fragment [
        Translator()
    ]

ReactDOM.render(App(), document.getElementById "root")
