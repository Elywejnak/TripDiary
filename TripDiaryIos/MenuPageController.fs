namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open MonoTouch.Foundation
open TripDiaryLibrary

type MenuPageController(tripDataAccess:TripDataAccess) as this = 
    inherit UIViewController()

    let newTripController = new NewTripController()

    let button localizeKey clickHandler = 
        let btn = UIButton.FromType(UIButtonType.System) 
        btn.SetTitle(localize localizeKey, UIControlState.Normal) 
        btn.TouchUpInside.Add clickHandler
        btn
            
    let lastTrip = tripDataAccess.GetLastTrip()
    do
        match lastTrip with
        | Some t -> printfn "some %A" t
        | None -> printfn "none"
    //let startOrStopTrip = match lastTrip     
            
  
    override this.ViewDidLoad() =
        base.ViewDidLoad()         
        this.View.BackgroundColor <- UIColor.White


    override this.ViewWillAppear(animated)=
        this.NavigationController.NavigationBarHidden <- true
        base.ViewWillAppear(animated)

    override this.ViewWillDisappear(animated)=
        this.NavigationController.NavigationBarHidden <- false
        base.ViewWillDisappear(animated)