namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing

type NewTripController() = 
    inherit UIViewController()


    override this.ViewDidLoad() =
        base.ViewDidLoad()

        this.View.BackgroundColor <- UIColor.White

        let btn = UIButton.FromType(UIButtonType.System)
        btn.Frame <- RectangleF(0.f,0.f,200.f,200.f)
        btn.SetTitle("Tlačítko", UIControlState.Normal) 
        this.Add(btn)


    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)