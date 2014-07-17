[<AutoOpen>]
module Utils

let toOption = function | null -> None | v -> Some v
