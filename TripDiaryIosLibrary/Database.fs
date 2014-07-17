namespace TripDiaryLibrary
open System
open SQLite

type Database(dbPath:string) =
    
    member this.CreateTablesIfNotExists (tableTypes:Type seq) =
        let connection = new SQLiteConnection(dbPath)
        tableTypes |> Seq.iter (fun tableType -> connection.CreateTable(tableType) |> ignore)    

    member this.Query<'a when 'a:(new : unit -> 'a)> sql = (new SQLiteConnection(dbPath)).Query<'a>(sql)     
