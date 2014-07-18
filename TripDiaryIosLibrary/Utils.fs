[<AutoOpen>]
module Utils
open System

let toOption = function | null -> None | v -> Some v
let toNull = function | None -> null | Some v -> v


let optionToNullable = function | None -> System.Nullable() | Some v -> System.Nullable(v)
let optionOfNullable (item:System.Nullable<'a>) = if item.HasValue then Some item.Value else None