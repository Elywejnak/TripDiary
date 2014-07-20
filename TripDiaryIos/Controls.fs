module Controls

open MonoTouch.UIKit
open MonoTouch.Foundation

let button localizationKey clickHandler = 
    let btn = UIButton.FromType(UIButtonType.System) 
    btn.SetTitle(localize localizationKey, UIControlState.Normal) 
    btn.TouchUpInside.Add clickHandler
    btn.Layer.BorderColor <- UIColor.Blue.CGColor
    btn.Layer.BorderWidth <- 1.f
    btn.ContentEdgeInsets <- new UIEdgeInsets(5.f,5.f,5.f,5.f) 
    btn

let label localizationKey = 
    let lbl = new UILabel()
    lbl.Text <- localize localizationKey 
    lbl        

let textview() =
    let tv = new UITextView()
    tv.Layer.BorderColor <- UIColor.Blue.CGColor
    tv.Layer.BorderWidth <- 1.f
    tv

 
let barButtonItemWithImage imageName (clickHandler:obj->System.EventArgs->unit) =
    let image = UIImage.FromBundle(imageName)
    let barButtonItem = new UIBarButtonItem(image,UIBarButtonItemStyle.Plain, clickHandler)
    barButtonItem

let barButtinItemWithText localizationKey (clickHandler:obj->System.EventArgs->unit) =
    new UIBarButtonItem(localize localizationKey, UIBarButtonItemStyle.Plain, clickHandler)