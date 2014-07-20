
namespace TripDiaryIos
open System
open System.IO
open MonoTouch.UIKit
open System.Drawing
open MonoTouch.Foundation
open TripDiaryLibrary
open VL
open TripDiaryLibrary
open Domain

type MainViewController() as this = 
    inherit UIViewController()

    let dbPath = [| Environment.GetFolderPath(Environment.SpecialFolder.Personal); "tripdiary.db" |] |> Path.Combine

    let dataAccess = DataAccess(dbPath) 
    let newTripController = new NewTripController(dataAccess)  
           
               
    let newTripButton = Controls.button "menu_btn_new" (fun _ -> 
        this.NavigationController.PushViewController(newTripController, true)
    ) 
    
    let tvLogo = Controls.label "TripDiary"
    do  tvLogo.Font <- Fonts.logo
        tvLogo.TextAlignment <- UITextAlignment.Center
        tvLogo.TextColor <- Colors.logo

    override this.ViewDidLoad() = 
        base.ViewDidLoad()
        Colors.styleController this
        this.Add(tvLogo)
        this.Add(newTripButton)

        let constraints = [      
            H [ !- 10. ; !@ tvLogo ; !- 10.]      
            V [ !@ tvLogo ; !- 30. ; !@ newTripButton ]
        ]  
        constraints |> List.iter (VL.addConstraints this.View) 
        this.View.AddConstraint(topLayoutGuide this -10.f tvLogo)  
        this.View.AddConstraint(centerX tvLogo this.View)

        this.View.AddConstraint(centerX newTripButton this.View)  

    override this.ViewWillAppear(animated) =
        this.NavigationController.NavigationBarHidden <- true
        base.ViewWillAppear(animated) 

        match dataAccess.GetLastTrip() with
        | Some trip -> 
            let activeTripController = new ActiveTripController (dataAccess, trip) :> UIViewController
            this.NavigationController.PresentViewController(new UINavigationController(activeTripController), true, null)
        | None -> ()
    
    override this.ViewWillDisappear(animated)=
        this.NavigationController.NavigationBarHidden <- false
        base.ViewWillDisappear(animated)         
    
    override this.ViewDidAppear(animated) =
        base.ViewDidAppear(animated)            
 
        