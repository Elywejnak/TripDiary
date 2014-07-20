[<AutoOpen>]
module Style
open MonoTouch.UIKit

module Colors =

    let background = UIColor.White
    let navigationBarBarTint = UIColor.FromRGB(0,122,255)
    let navigationBarTint = UIColor.White
    let navigationBarTitleColor = UIColor.White


    let styleController (controller:UIViewController) = 
        controller.View.BackgroundColor <- background 
        controller.NavigationController.NavigationBar.BarTintColor <- navigationBarBarTint 
        controller.NavigationController.NavigationBar.TintColor <- navigationBarTint

        let textAttributes =  controller.NavigationController.NavigationBar.GetTitleTextAttributes()
        textAttributes.TextColor <- navigationBarTitleColor
        controller.NavigationController.NavigationBar.SetTitleTextAttributes(textAttributes)

module Fonts =

    let tripTitle = UIFont.BoldSystemFontOfSize(20.f)