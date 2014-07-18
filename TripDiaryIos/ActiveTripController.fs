namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain

type ActiveTripController(tripDataAccess,trip:Trip) = 
    inherit UIViewController()

    do base.Title <- trip.Name

    let btnTakePhoto = Controls.button "activetrip_btn_takephoto" (fun _ -> ())

    let cancelTripClicked sender eventArgs = printfn "activetrip_btn_canceltrip"
    member this.XXX x = ()
    override this.ViewDidLoad() =
        base.ViewDidLoad()
        this.View.BackgroundColor <- UIColor.White      
               
        let leftButtonBarItem = new UIBarButtonItem(localize "activetrip_btn_canceltrip", UIBarButtonItemStyle.Plain, cancelTripClicked)
        this.NavigationItem.SetLeftBarButtonItem(leftButtonBarItem, true)


               

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)