namespace TripDiaryLibrary
open System
open SQLite

type Database(dbPath:string) =
    
    let createConnection() = new SQLiteConnection(dbPath)
    member this.CreateTablesIfNotExists (tableTypes:Type seq) =
        let connection = createConnection()
        tableTypes |> Seq.iter (fun tableType -> connection.CreateTable(tableType) |> ignore)    

    member this.Query<'a when 'a:(new : unit -> 'a)> sql = createConnection().Query<'a>(sql)     

    member this.Insert<'a> item =
        createConnection().Insert(item)
        