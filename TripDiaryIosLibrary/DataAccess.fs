namespace TripDiaryLibrary

open Domain
open SQLite
open System
open System.Linq

type TripDataAccess(db:Database) = 
        
    member this.GetLastTrip() =          
        db.Query<Trip> "select * from trip where stoppedat is null" |> Seq.tryPick Some 

    member this.CreateTrip name = 
        let trip = new Trip(Guid.NewGuid(),name,DateTime.UtcNow,None)
        db.Insert trip = 1
        