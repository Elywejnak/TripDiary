namespace TripDiaryIos

open System
open System.IO

open MonoTouch.UIKit
open MonoTouch.Foundation
open TripDiaryLibrary

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    let window = new UIWindow (UIScreen.MainScreen.Bounds)

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching (app, options) =
        // If you have defined a root view controller, set it here:

        let dbPath = [| Environment.GetFolderPath(Environment.SpecialFolder.Personal); "tripdiary.db" |] |> Path.Combine
        let database = new Database(dbPath)
        database.CreateTablesIfNotExists [typeof<Domain.Trip>]

        let tripDataAccess = TripDataAccess(database) 

        window.RootViewController <- new UINavigationController(new MenuPageController (tripDataAccess))
        window.MakeKeyAndVisible ()
        true

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main (args, null, "AppDelegate")
        0

