namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open TripDiaryLibrary
open VL
open MonoTouch.MapKit
open MonoTouch.CoreLocation
open TripMapComponents

type TripDisplayController(dataAccess:DataAccess,trip:Trip) as this = 
    inherit UIViewController()

    let mapDelegate = new MapDelegate(false)
    do  mapDelegate.NoteAnnotationClicked.Add (fun note ->
            this.NavigationController.PushViewController(new NoteDisplayController(note), true)            
        )
        mapDelegate.PhotoAnnotationClicked.Add (fun photoName ->
            this.NavigationController.PushViewController(new PhotoDisplayController(photoName),true)
        )

    let map = new MKMapView(UIScreen.MainScreen.Bounds)
    do  map.ZoomEnabled <- true
        map.Delegate <- mapDelegate
        //map.CenterCoordinate <- new CLLocationCoordinate2D(trip.StartLatitude, trip.StartLongitude)
        let region = MKCoordinateRegion.FromDistance(map.CenterCoordinate,300.,300.)
        map.Region <- region
        map.CenterCoordinate <- new CLLocationCoordinate2D(trip.StartLatitude, trip.StartLongitude)


    let notes = dataAccess.GetNotes (trip.Id)
    do  notes |> Seq.iter (fun n -> new NoteAnnotation(n.Latitude,n.Longitude,n.Text) |> map.AddAnnotation )

    let photos = dataAccess.GetPhotos (trip.Id)
    do  photos |> Seq.iter (fun p -> new PhotoAnnotation(p.Latitude, p.Longitude,"thumb_" + p.Name, p.Name) |> map.AddAnnotation )
        
    
    let tripNameLabel = Controls.label trip.Name
    do 
        tripNameLabel.TextAlignment <- UITextAlignment.Center
        tripNameLabel.TextColor <- Colors.tripTitle
        tripNameLabel.Font <- Fonts.tripTitle

    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this true    

        //Navigation controls       
        let leftButtonBarItem = new UIBarButtonItem(localize "tripdisplay_btn_back", UIBarButtonItemStyle.Plain, null)
        leftButtonBarItem.Clicked.Add (fun e ->             
            this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        this.NavigationItem.SetLeftBarButtonItem(leftButtonBarItem, true) 

        //Content controls           
        this.View <- map          
        VL.packageInto this.View [ H [ !- 0. ; !@ tripNameLabel ; !- 0.] ] |> ignore
        this.View.AddConstraint(topLayoutGuide this -10.f tripNameLabel)

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated) 
        
            
    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)

    override this.DidReceiveMemoryWarning() =
        base.DidReceiveMemoryWarning()
