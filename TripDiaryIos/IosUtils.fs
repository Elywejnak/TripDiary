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

