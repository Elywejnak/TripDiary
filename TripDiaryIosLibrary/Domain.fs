module Domain

open System
open SQLite

[<AllowNullLiteral>]
type Trip() =
    [<PrimaryKey>]
    member val Id = Guid.Empty with get,set
    member val Name = "" with get,set
