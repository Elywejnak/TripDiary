namespace TripDiaryLibrary

open Domain
open SQLite
open System.Linq

type TripDataAccess(db:Database) = 
        
    member this.GetLastTrip() =  db.Query<Trip> "select * from trip" |> Seq.tryPick Some 