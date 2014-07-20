namespace TripDiaryLibrary

open Domain
open SQLite
open System
open System.Linq

type DataAccess(db:Database) = 
        
    member this.GetLastTrip() =          
        db.Query<Trip> "select * from trip where stoppedat is null" |> Seq.tryPick Some 

    member this.CreateTrip name = 
        let trip = new Trip(Guid.NewGuid(),name,DateTime.UtcNow,None)
        db.Insert trip = 1


    member this.SaveNote tripId text =
        let note = new Note(Guid.NewGuid(),tripId,text,DateTime.UtcNow)
        db.Insert note = 1
        