[<AutoOpen>]
module IosUtils

open MonoTouch.Foundation

let localizeWithComment key comment = NSBundle.MainBundle.LocalizedString(key,comment)
let localize key = localizeWithComment key null


