module Domain

open System
open SQLite

[<AllowNullLiteral>]
type Trip(id,name,createdAt,stoppedAt:DateTime option) =
    [<PrimaryKey>]
    member val Id = id with get,set
    member val Name = name with get,set
    member val StartLatitude = 0. with get,set
    member val StartLongitude = 0. with get,set
    
    member val CreatedAt = createdAt with get,set
    member val StoppedAt = stoppedAt |> optionToNullable with get,set
    [<Ignore>]
    member this.StoppedAtOption with get() = this.StoppedAt |> optionOfNullable and set v = this.StoppedAt <- optionToNullable v
    new() = Trip(Guid.Empty,String.Empty,DateTime.MinValue,None)
     

[<AllowNullLiteral>]
type Note(id,tripId,text,lat,lng,createdAt) =
    [<PrimaryKey>]
    member val Id = id with get,set
    member val TripId = tripId with get,set
    member val Text = text with get,set
    member val Latitude = lat with get,set
    member val Longitude = lng with get,set
    member val CreatedAt = createdAt with get,set
    new() = Note(Guid.Empty,Guid.Empty,"",0.,0.,DateTime.MinValue)    

 


 

     