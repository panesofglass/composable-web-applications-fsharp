namespace Rich.Data

module Queries =
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

    type Crimes = ODataService<ServiceUri = "https://api.datamarket.azure.com/data.gov/Crimes">
    let comparePropertyCrimeRates() =
        let crime = new Crimes.ServiceTypes.datagovCrimesContainer()
        crime.Credentials <- NetworkCredential (Util.ADM_USER_ID, Util.ADM_ACCOUNT_ID)
        query {
            for m in crime.CityCrime do 
            where (m.City = "Redmond" || m.City = "Kirkland" || m.City = "Bellevue" || m.City = "Seattle")
            where (m.State = "Washington")
            where (m.Year = 2008)
            sortBy (m.City)
        }
        |> Seq.map (fun c -> (c.City, (float c.Burglary + float c.PropertyCrime) / float c.Population * 100.0))
        |> Seq.sortBy snd
        |> Seq.toList

