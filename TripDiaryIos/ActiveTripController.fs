namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open TripDiaryLibrary
open VL
open MonoTouch.MapKit
open MonoTouch.CoreLocation

type NoteAnnotation(coordinate:CLLocationCoordinate2D, title) =
    inherit MKAnnotation()
    let mutable mCoordinate = coordinate 
    override this.Coordinate with get() = mCoordinate and set v = mCoordinate <- v
    override this.Title with get() = title 

    
    

type MapDelegate() =
    inherit MKMapViewDelegate() 
    let mutable updatedFirstTime = true
    let noteAnnotationId = "noteAnnotationId"


    let userLocationUpdated = Event<_>()
    member this.UserLocationUpdated = userLocationUpdated.Publish
    override this.DidUpdateUserLocation(mapView,userLocation) =
        userLocationUpdated.Trigger(userLocation)

        if updatedFirstTime then 
            mapView.CenterCoordinate <- userLocation.Coordinate
            let region = MKCoordinateRegion.FromDistance(userLocation.Coordinate,300.,300.)
            mapView.Region <- region
            updatedFirstTime <- false

    override this.GetViewForAnnotation(mapview,annotation) =
        match annotation with
        | :? MKUserLocation -> null
        | :? NoteAnnotation ->
                let view =  match mapview.DequeueReusableAnnotation(noteAnnotationId) with
                            | null -> new MKPinAnnotationView(annotation, noteAnnotationId) :> MKAnnotationView
                            | existingView -> existingView  
                view.TintColor <- UIColor.Blue   
                view.CanShowCallout <- true            
                view
        | _ -> null

   
        
       

type ActiveTripController(dataAccess:DataAccess,trip:Trip) as this = 
    inherit UIViewController()

    //actual location
    let mutable lat,lng = 0.,0.

    let mapDelegate = new MapDelegate()
    do 
        mapDelegate.UserLocationUpdated.Add(fun location ->            
            lat <- location.Coordinate.Latitude
            lng <- location.Coordinate.Longitude
            //update first trip position
            if trip.StartLatitude = 0.0 && trip.StartLongitude = 0.0 then
                trip.StartLatitude <- location.Coordinate.Latitude
                trip.StartLongitude <- location.Coordinate.Longitude
                dataAccess.UpdateTrip trip |> printfn "Trip update status: %b"
        )

    let map = new MKMapView(UIScreen.MainScreen.Bounds)
    do 
        map.ShowsUserLocation <- true
        map.ZoomEnabled <- true
        map.Delegate <- mapDelegate




    //let mutable trip = trip
    let btnTakePhoto = Controls.button "activetrip_btn_takephoto" (fun _ -> ())
    let cancelTripClicked sender eventArgs = printfn "activetrip_btn_canceltrip"

    let takePhotoClicked sender eventArgs = 
        printfn "ActiveTripController.takePhotoClicked"        
        try
            let imagePickerController = new UIImagePickerController()
            imagePickerController.SourceType <- UIImagePickerControllerSourceType.Camera
            imagePickerController.MediaTypes <- UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera)
            imagePickerController.ShowsCameraControls <- true
            imagePickerController.FinishedPickingImage.Add (fun x ->
                printfn "ActiveTripController.ImagePicker.FinishedPickingImage"
                imagePickerController.DismissViewController(true, null)                         
            )
            imagePickerController.Canceled.Add (fun x ->
                printfn "ActiveTripController.ImagePicker.Canceled"   
                imagePickerController.DismissViewController(true, null)         
            )
            this.NavigationController.PresentViewController(imagePickerController, true, null)
        with ex -> 
            ()
 
    let addNoteClicked sender eventArgs = 
        printfn "addNoteClicked"
        //location when note writing started
        let noteLat,noteLng = lat,lng
        let noteWriterController = new NoteWriterController()
        noteWriterController.Canceled.Add (fun _ ->
            printfn "ActiveTripController.NoteWriter.Canceled"  
            this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        noteWriterController.Finished.Add (fun note ->
            printfn "ActiveTripController.NoteWriter.Finished with note=`%s`" note    
            dataAccess.SaveNote trip.Id note noteLat noteLng |> printfn "Note saving status: %b"    

            //show note point on map      
            let annotation = new NoteAnnotation(new CLLocationCoordinate2D(noteLat,noteLng), "Note: " + note)
            map.AddAnnotation(annotation)

            this.NavigationController.PopViewControllerAnimated(true) |> ignore

        )
        this.NavigationController.PushViewController(noteWriterController, true)
    
    let tripNameLabel = Controls.label trip.Name
    do 
        tripNameLabel.TextAlignment <- UITextAlignment.Center
        tripNameLabel.TextColor <- Colors.tripTitle
        tripNameLabel.Font <- Fonts.tripTitle

    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this true    

        //Navigation controls       
        let leftButtonBarItem = new UIBarButtonItem(localize "activetrip_btn_canceltrip", UIBarButtonItemStyle.Plain, cancelTripClicked)
        leftButtonBarItem.Clicked.Add (fun e -> 
            trip.StoppedAt <- System.Nullable(DateTime.UtcNow)
            dataAccess.UpdateTrip(trip) |> printfn "Trip update status: %b"
            this.DismissViewController(true, null)
            //this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        this.NavigationItem.SetLeftBarButtonItem(leftButtonBarItem, true)
                         
        let btnTakePhoto = Controls.barButtonItemWithImage "addphoto" takePhotoClicked
        let btnAddNote = Controls.barButtonItemWithImage "addnote" addNoteClicked 
        this.NavigationItem.SetRightBarButtonItems([|btnAddNote;btnTakePhoto|],true)

        //Content controls           
        this.View <- map          
        VL.packageInto this.View [ H [ !- 0. ; !@ tripNameLabel ; !- 0.] ] |> ignore
        this.View.AddConstraint(topLayoutGuide this -10.f tripNameLabel)
                 

         
               

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)