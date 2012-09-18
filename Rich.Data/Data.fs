namespace Rich.Data

module Models =
    type Album =
        { Title: string
          Artist: string
          Genre: string
          Price: decimal }

module Queries =
    open System.Data.Linq
    open System.Data.Entity
    open Microsoft.FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open Microsoft.FSharp.Linq.NullableOperators
    open Models

    type internal db = SqlEntityConnection<ConnectionString="Server=.\sqlexpress;Initial Catalog=mvcmusicstore;Integrated Security=SSPI;MultipleActiveResultSets=true", Pluralize = true>
    let internal context = db.GetDataContext()

    let findAlbum term =
        query {
            for album in context.Albums do
            where (album.Title.Contains term)
            sortBy album.Genre.Name
            thenBy album.Title
            select
                { Title = album.Title
                  Artist = album.Artist.Name
                  Genre = album.Genre.Name
                  Price = album.Price }
        }
        |> Seq.toList
