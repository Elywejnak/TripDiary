namespace TripDiaryLibrary

open Domain
open SQLite
open System
open System.Linq

type DataAccess(dbPath:string) as this = 


    let createConnection() = new SQLiteConnection(dbPath)

    do this.CreateTablesIfNotExists [typeof<Trip>;typeof<Note>]

    member this.CreateTablesIfNotExists (tableTypes:Type seq) =
        let connection = createConnection()
        tableTypes |> Seq.iter (fun tableType -> connection.CreateTable(tableType) |> ignore) 
        
    member this.GetLastTrip() =          
        createConnection().Query<Trip> "select * from trip where stoppedat is null" |> Seq.tryPick Some 

    member this.CreateTrip name  = 
        let trip = new Trip(Guid.NewGuid(),name,DateTime.UtcNow,None)
        createConnection().Insert trip = 1

    member this.UpdateTrip trip = 
        createConnection().Update trip = 1


    member this.SaveNote tripId text lat lng=
        let note = new Note(Guid.NewGuid(),tripId,text,lat,lng,DateTime.UtcNow)
        createConnection().Insert note = 1
        