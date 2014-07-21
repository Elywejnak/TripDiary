
namespace TripDiaryIos
open System
open System.IO
open System.Drawing
open System.Collections.Generic
open MonoTouch.UIKit
open MonoTouch.Foundation
open TripDiaryLibrary
open VL
open Domain

type TableSource(data:List<Trip>) =
    inherit UITableViewSource()
    let cellIdentifier = "TripCell"
    let onRowSelected = new Event<Trip>()
    member this.OnRowSelected with get() = onRowSelected.Publish
    override this.RowsInSection(tableview, section) = data.Count

    override this.GetCell(tableview, index) =
        let cell =  match tableview.DequeueReusableCell(cellIdentifier) with
                    | null -> new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier)
                    | cell -> cell
        cell.TextLabel.Text <- data.[index.Row].Name
        cell.TextLabel.TextAlignment <- UITextAlignment.Center
        cell

    override this.RowSelected(tableview, index) =
        tableview.DeselectRow(index, true)
        onRowSelected.Trigger(data.[index.Row])

        
        

type MainViewController() as this = 
    inherit UIViewController()

    let dbPath = [| Environment.GetFolderPath(Environment.SpecialFolder.Personal); "tripdiary.db" |] |> Path.Combine

    let dataAccess = DataAccess(dbPath) 
               
    let newTripButton = Controls.button "menu_btn_new" (fun _ -> 
        let newTripController = new NewTripController(dataAccess)  
        this.NavigationController.PushViewController(newTripController, true)
    ) 
    
    let tvLogo = Controls.label "TripDiary"
    do  tvLogo.Font <- Fonts.logo
        tvLogo.TextAlignment <- UITextAlignment.Center
        tvLogo.TextColor <- Colors.logo


     
    let lvTrips = new UITableView()
    let reloadTrips() =
        let trips = new List<Trip>(dataAccess.GetTrips())
        let tripsSource = new TableSource(trips)
        tripsSource.OnRowSelected.Add(fun t ->
            let tripDisplayController = new TripDisplayController(dataAccess,t)
            this.NavigationController.PushViewController(tripDisplayController, true)
            ()
        )
        lvTrips.Source <- tripsSource 
        lvTrips.ReloadData()

        

    override this.ViewDidLoad() = 
        base.ViewDidLoad()
        Colors.styleController this false


        this.Add(tvLogo)
        this.Add(newTripButton)
        this.Add(lvTrips)

        let constraints = [      
            H [ !- 10. ; !@ tvLogo ; !- 10.]      
            V [ !@ tvLogo ; !- 30. ; !@ newTripButton ]

            V [ !@ newTripButton ; !- 30. ; !@ lvTrips ; !- 10. ]
            H [ !- 10. ; !@ lvTrips ; !- 10.]
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
        | None -> reloadTrips()
    
    override this.ViewWillDisappear(animated)=
        this.NavigationController.NavigationBarHidden <- false
        base.ViewWillDisappear(animated)         
    
    override this.ViewDidAppear(animated) =
        base.ViewDidAppear(animated)            
 
        