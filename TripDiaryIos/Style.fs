[<AutoOpen>]
module Style
open MonoTouch.UIKit

module Colors =

    let background = UIColor.White
    let navigationBarBarTint = UIColor.FromRGB(0,122,255)
    let navigationBarTint = UIColor.White
    let navigationBarTitleColor = UIColor.White
    let tripTitle = UIColor.FromRGB(0,122,255)
    let buttonText = UIColor.White
    let buttonBackground = UIColor.FromRGB(0,122,255)
    let logo = UIColor.Black

    let styleController (controller:UIViewController) useWhiteStatusBar =        
        if useWhiteStatusBar then controller.NavigationController.NavigationBar.BarStyle <- UIBarStyle.Black
        controller.View.BackgroundColor <- background 
        controller.NavigationController.NavigationBar.BarTintColor <- navigationBarBarTint 
        controller.NavigationController.NavigationBar.TintColor <- navigationBarTint

        let textAttributes =  controller.NavigationController.NavigationBar.GetTitleTextAttributes()
        textAttributes.TextColor <- navigationBarTitleColor
        controller.NavigationController.NavigationBar.SetTitleTextAttributes(textAttributes)

module Fonts =

    let tripTitle = UIFont.BoldSystemFontOfSize(26.f)
    let button = UIFont.BoldSystemFontOfSize(26.f)
    let label = UIFont.SystemFontOfSize(26.f)
    let logo = UIFont.BoldSystemFontOfSize(60.f)