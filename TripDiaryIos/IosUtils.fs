[<AutoOpen>]
module IosUtils

open MonoTouch.Foundation
open MonoTouch.UIKit


///create center constraint
let center view superView attribute = 
            NSLayoutConstraint.Create(
                view,
                attribute,
                NSLayoutRelation.Equal,
                superView,
                attribute,
                1.f,
                0.f) 

///create horizontal center constraint
let centerX view superView = center view superView NSLayoutAttribute.CenterX

///create vertical center constraint               
let centerY view superView = center view superView NSLayoutAttribute.CenterY

let topLayoutGuide (controller:UIViewController) view =    
    NSLayoutConstraint.Create(
        controller.TopLayoutGuide,
        NSLayoutAttribute.Bottom,
        NSLayoutRelation.Equal,
        view,
        NSLayoutAttribute.Top,
        1.f,
        0.f
    )                    
let bottomLayoutGuide (controller:UIViewController) view =    
    NSLayoutConstraint.Create(
        controller.BottomLayoutGuide,
        NSLayoutAttribute.Top,
        NSLayoutRelation.Equal,
        view,
        NSLayoutAttribute.Bottom,
        1.f,
        0.f
    )  
