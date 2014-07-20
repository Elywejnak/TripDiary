namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open TripDiaryLibrary
open VL
open MonoTouch.MapKit
open MonoTouch.CoreLocation

type MapDelegate() =
    inherit MKMapViewDelegate() 
    let mutable updatedFirstTime = true

    let userLocationUpdated = Event<_>()
    member this.UserLocationUpdated = userLocationUpdated.Publish
    override this.DidUpdateUserLocation(mapView,userLocation) =
        userLocationUpdated.Trigger(userLocation)
        mapView.CenterCoordinate <- userLocation.Coordinate

        if updatedFirstTime then 
            let region = MKCoordinateRegion.FromDistance(userLocation.Coordinate,300.,300.)
            mapView.Region <- region
            updatedFirstTime <- false
       

type ActiveTripController(dataAccess:DataAccess,trip:Trip) as this = 
    inherit UIViewController()

    let mapDelegate = new MapDelegate()
    let mutable lat,lng = 0.,0.
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
            this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        this.NavigationController.PushViewController(noteWriterController, true)
    
    let tripNameLabel = Controls.label trip.Name
    do 
        tripNameLabel.TextAlignment <- UITextAlignment.Center
        tripNameLabel.Font <- Fonts.tripTitle

    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this      

        //Navigation controls       
        let leftButtonBarItem = new UIBarButtonItem(localize "activetrip_btn_canceltrip", UIBarButtonItemStyle.Plain, cancelTripClicked)
        this.NavigationItem.SetLeftBarButtonItem(leftButtonBarItem, true)
                         
        let btnTakePhoto = Controls.barButtonItemWithImage "addphoto" takePhotoClicked
        let btnAddNote = Controls.barButtonItemWithImage "addnote" addNoteClicked 
        this.NavigationItem.SetRightBarButtonItems([|btnAddNote;btnTakePhoto|],true)

        //Content controls         
        this.Add tripNameLabel
        this.View.AddConstraint(topLayoutGuide this -10.f tripNameLabel)      
        VL.packageInto this.View [ H [ !- 0. ; !@ tripNameLabel ; !- 0.] ] |> ignore

        let map = new MKMapView(UIScreen.MainScreen.Bounds)
        map.ShowsUserLocation <- true
        map.ZoomEnabled <- true

         

        map.Delegate <- mapDelegate

        this.Add(map) 
               

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)