open System
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Web.Http
open Frank
open Rich.Data

let hello request = async {
    return OK ignore <| Str "Hello, world!"
}

let echo (request: HttpRequestMessage) = async {
    let! content = request.Content.AsyncReadAsString()
    return OK
        <| ``Content-Type`` "text/plain"
        <| Str content
}

let helloResource = route "/" (get hello <|> post echo)

let formatters = [| new JsonMediaTypeFormatter() :> MediaTypeFormatter
                    new XmlMediaTypeFormatter() :> MediaTypeFormatter |]

let searchAlbums request = async {
    let term = getParam request "term"
    return Queries.findAlbum term
}

let albumsResource = route "/{term}" <| get (runConneg formatters searchAlbums)

let app = merge [ helloResource; albumsResource ]

[<EntryPoint>]
let main argv = 
    use config = new HttpConfiguration()
    config.Register app
    use server = new HttpServer(config)
    use client = new HttpClient(server)
    async {
        let! hello = Async.AwaitTask <| client.GetStringAsync("http://example.org/")
        printfn "Got %A" hello

        let! echo = Async.AwaitTask <| client.PostAsJsonAsync("http://example.org/", "test=true")
        let! echoResult = echo.Content.AsyncReadAsString()
        printfn "Echoed %s" echoResult

        let request = new HttpRequestMessage(HttpMethod.Get, "http://example.org/Best")
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        let! albumsResponse = Async.AwaitTask <| client.SendAsync(request)
        let! albumsResult = albumsResponse.Content.AsyncReadAsString()
        printfn "Albums matching \"Best\":\r\n%A" albumsResult
    }
    |> Async.RunSynchronously

    Console.WriteLine("Press Enter to close")
    Console.ReadLine() |> ignore
    0 // return an integer exit code
