namespace Website

open System
open System.Text
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open Rich.Data

module Search =
    [<Rpc>]
    let queryAlbums term =
        let sb = new StringBuilder()
        Queries.findAlbum term
        |> List.map (fun album -> "<li>" + album.Title + "</li>")
        |> List.reduce (+)

    [<JavaScript>]
    let searchUi () =
        let output = UL []
        let input = Input [Attr.Type "text"]
        input
        |> OnKeyPress (fun _ _ ->
            output.Html <- queryAlbums input.Value)
        Div [Attr.Class "search"] -< [
            H3 [Text "Album Search"]
            P [
                Text "Term:"
                input :> IPagelet
            ]
            P [Text "Results:"]
            P [output]
        ]

type AlbumSearch() =
    inherit Web.Control()
    [<JavaScript>]
    override this.Body = Search.searchUi () :> _
