namespace TripDiaryIos

open System
open MonoTouch.UIKit
open System.Drawing
open MonoTouch.Foundation
open TripDiaryLibrary
open VL

type MenuPageController(tripDataAccess:TripDataAccess) as this = 
    inherit UIViewController()

    let newTripController = new NewTripController()
             
    let newTripButton = Controls.button "menu_btn_new" (fun _ -> 
        this.NavigationController.PushViewController(newTripController, true)
    )

    override this.ViewDidLoad() =
        base.ViewDidLoad()         
        this.View.BackgroundColor <- UIColor.White
        this.NavigationController.NavigationBar.TintColor <- UIColor.White

        this.Add(newTripButton)

        let constraints = [
            V [ !- 100. ; !@ newTripButton ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(centerX newTripButton this.View)

    override this.DidReceiveMemoryWarning() = 
        base.DidReceiveMemoryWarning()

    override this.ViewWillAppear(animated)=
        this.NavigationController.NavigationBarHidden <- true
        base.ViewWillAppear(animated)

    override this.ViewWillDisappear(animated)=
        this.NavigationController.NavigationBarHidden <- false
        base.ViewWillDisappear(animated)

    override this.ViewDidAppear(animated)= 
        base.ViewDidAppear(animated)