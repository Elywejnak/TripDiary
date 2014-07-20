
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
    let database = new Database(dbPath)
    do database.CreateTablesIfNotExists [typeof<Trip>;typeof<Note>]
    let tripDataAccess = DataAccess(database) 


    let newTripController = new NewTripController(tripDataAccess)             
    let newTripButton = Controls.button "menu_btn_new" (fun _ -> 
        this.NavigationController.PushViewController(newTripController, true)
    )

    override this.ViewDidLoad() = 
        base.ViewDidLoad()
        Colors.styleController this
        this.Add(newTripButton)

        let constraints = [
            V [ !- 100. ; !@ newTripButton ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(centerX newTripButton this.View)    


         
        //this.PresentViewController(new UINavigationController(activeController),true, (fun _ -> printfn "returned"))

    override this.ViewWillAppear(animated) =
        this.NavigationController.NavigationBarHidden <- true
        base.ViewWillAppear(animated) 

        match tripDataAccess.GetLastTrip() with
        | Some trip -> 
            let activeTripController = new ActiveTripController (tripDataAccess, trip) :> UIViewController
            this.NavigationController.PresentViewController(new UINavigationController(activeTripController), true, null)
        | None -> ()
    
    override this.ViewWillDisappear(animated)=
        this.NavigationController.NavigationBarHidden <- false
        base.ViewWillDisappear(animated)         
    
    override this.ViewDidAppear(animated) =
        base.ViewDidAppear(animated)            
 
        