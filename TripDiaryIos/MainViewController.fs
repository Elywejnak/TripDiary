
namespace TripDiaryIos
open System
open System.IO
open MonoTouch.UIKit
open System.Drawing
open MonoTouch.Foundation
open TripDiaryLibrary
open VL
open TripDiaryLibrary

type MainViewController() = 
    inherit UIViewController()

    let dbPath = [| Environment.GetFolderPath(Environment.SpecialFolder.Personal); "tripdiary.db" |] |> Path.Combine
    let database = new Database(dbPath)
    do database.CreateTablesIfNotExists [typeof<Domain.Trip>]
    let tripDataAccess = TripDataAccess(database) 


    override this.ViewDidLoad() = 
        base.ViewDidLoad()  
        this.View.BackgroundColor <- UIColor.White      


         
        //this.PresentViewController(new UINavigationController(activeController),true, (fun _ -> printfn "returned"))

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated)
        let activeController = match tripDataAccess.GetLastTrip() with
                               | Some t -> new ActiveTripController () :> UIViewController
                               | None -> new MenuPageController (tripDataAccess) :> UIViewController
         
        this.NavigationController.PushViewController(activeController, true)
         
    override this.ViewDidAppear(animated) =
        base.ViewDidAppear(animated)            
 
        