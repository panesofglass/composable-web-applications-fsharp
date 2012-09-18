// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "System.Data.Services.Client"
#r "FSharp.Data.TypeProviders"
#r @"M:\Code\composable-web-applications-fsharp\packages\FSharpx.TypeProviders.1.6.30\lib\45\Fsharpx.TypeProviders.dll"
open FSharpx

(**
 * HttpClient vs. Type Providers
 *)

#r @"M:\Code\composable-web-applications-fsharp\lib\WorldBank.TypeProvider.dll"
#load "FSharpChartEx.fsx"
open MSDN.FSharp.Charting
open System.Drawing

// -----------------------------------------------------------------

// Get average university enrollment for North American countries
let avgEnrollment =
    [ for c in WorldBank.Regions.``North America`` do
        yield! c.``School enrollment, tertiary (% gross)`` ]
    |> Seq.groupBy fst
    |> Seq.map (fun (y, v) -> y, Seq.averageBy snd v)
    |> Array.ofSeq
    |> Array.sortBy fst

// Create list of other countries that we want to plot
let countries = 
  [ WorldBank.Countries.``United States``, Color.DarkRed
    WorldBank.Countries.Canada, Color.DarkGreen ]

// Compare countries with EU average
FSharpChart.Combine
  [ yield FSharpChart.NiceLine(avgEnrollment, "NA", Color.Blue)
    for country, clr in countries do
      let data = country.``School enrollment, tertiary (% gross)``
      yield FSharpChart.NiceLine(data, unbox country, clr) ]
  |> FSharpChart.NiceChart







(**
 * Accessing structured data documents
 *)

type InlinedJson = StructuredJSON<Schema = """{
    "authors": [
        { "name": "Steffen", "age": 29 },
        { "name": "Tomas" },
        { "name": "Ryan", "age": 33 }
    ]
}""">

let json = InlinedJson().Root
let authors = json.GetAuthors()
for author in authors do
    match author.Age with
    | Some age -> printfn "%s is %d years old" author.Name age
    | None -> printfn "%s is an author" author.Name







(**
 * Exploring Netflix data
 *)

open System.Net
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.NullableOperators

type Catalog = ODataService<"http://odata.netflix.com/Catalog/">
let context = Catalog.GetDataContext()

let getAllRockyTitles() =
    query {
        for title in context.Titles do
        where (title.Name.Contains "Rocky")
        select title
    }
    |> Seq.toList

let getClassicBMovies() =
    query {
        for genre in context.Genres do
        where (genre.Name = "Sci-Fi & Fantasy")
        for title in genre.Titles do
        where (title.ReleaseYear ?<= 1959)
        where (title.ReleaseYear ?>= 1934)
        sortByNullable title.AverageRating
        select title
    }
    |> Seq.toList






(**
 * Accessing a SQL database
 *)
#r "System.Data.Linq"
#r "System.Data.Entity"
#load "Data.fs"

Rich.Data.Queries.findAlbum "Best"
