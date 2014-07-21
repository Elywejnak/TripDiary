namespace TripDiaryLibrary

open Domain
open SQLite
open System
open System.Linq

type DataAccess(dbPath:string) as this = 


    let createConnection() = new SQLiteConnection(dbPath)

    do this.CreateTablesIfNotExists [typeof<Trip>;typeof<Note>;typeof<Photo>]

    member this.CreateTablesIfNotExists (tableTypes:Type seq) =
        let connection = createConnection()
        tableTypes |> Seq.iter (fun tableType -> connection.CreateTable(tableType) |> ignore) 
        
    member this.GetLastTrip() =          
        createConnection().Query<Trip> "select * from trip where stoppedat is null" |> Seq.tryPick Some 

    member this.GetTrips() =
        createConnection().Query<Trip> "select * from trip order by stoppedat desc"


    member this.CreateTrip name  = 
        let trip = new Trip(Guid.NewGuid().ToString("N"),name,DateTime.UtcNow,None)
        createConnection().Insert trip = 1

    member this.UpdateTrip trip = 
        createConnection().Update trip = 1


    member this.SaveNote tripId text lat lng=
        let note = new Note(Guid.NewGuid().ToString("N"),tripId,text,lat,lng,DateTime.UtcNow)
        createConnection().Insert note = 1

    member this.GetNotes tripId =
        let q = sprintf "select * from note where tripId = '%s'" tripId
        createConnection().Query<Note> q

    member this.SavePhoto tripId name lat lng=
        let photo = new Photo(Guid.NewGuid().ToString("N"),tripId,name,lat,lng,DateTime.UtcNow)
        createConnection().Insert photo = 1

    member this.GetPhotos tripId =
        let q = (sprintf "select * from photo where tripId = '%s'" tripId) 
        createConnection().Query<Photo> q
        