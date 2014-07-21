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
 
  



   
        
type ImagePickerDelegate(dataAccess:DataAccess, tripId, lat, lng, map:MKMapView) =
    inherit UIImagePickerControllerDelegate()

    override this.FinishedPickingImage(picker, image, editingInfo) =
        printfn "ActiveTripController.ImagePicker.FinishedPickingImage"
        let imageName = Guid.NewGuid().ToString() + ".jpg"
        let scaledImage = image.Scale(new SizeF(image.Size.Width /2.f,image.Size.Height/2.f))
        match saveImage imageName scaledImage with
        | (true, _) -> dataAccess.SavePhoto tripId imageName lat lng |> printfn "Photo saving status: %b"
        | (false, err) -> printfn "Error saving image %s because of %s" imageName err.LocalizedDescription

        let thumbnail = scaledImage.Scale(new SizeF(45.f,45.f), 1.f)
        let thumbnailName = "thumb_" + imageName
        match saveImage thumbnailName thumbnail with
        | (true, _) -> ()
        | (false, err) -> printfn "Error saving image thumbnail %s because of %s" thumbnailName err.LocalizedDescription

        let photoAnnotation = new PhotoAnnotation(lat,lng, thumbnailName, imageName)
        map.AddAnnotation(photoAnnotation)
        picker.DismissViewController(true, null) 



    override this.Canceled(picker) = picker.DismissViewController(true, null)   
    

type ActiveTripController(dataAccess:DataAccess,trip:Trip) as this = 
    inherit UIViewController()

    //actual location
    let mutable lat,lng = 0.,0.

    let mapDelegate = new MapDelegate(true)
    do  mapDelegate.UserLocationUpdated.Add(fun location ->            
            lat <- location.Coordinate.Latitude
            lng <- location.Coordinate.Longitude
            //update first trip position
            if trip.StartLatitude = 0.0 && trip.StartLongitude = 0.0 then
                trip.StartLatitude <- location.Coordinate.Latitude
                trip.StartLongitude <- location.Coordinate.Longitude
                dataAccess.UpdateTrip trip |> printfn "Trip update status: %b"
        )
        mapDelegate.NoteAnnotationClicked.Add (fun note ->
            this.NavigationController.PushViewController(new NoteDisplayController(note), true)            
        )
        mapDelegate.PhotoAnnotationClicked.Add (fun photoName ->
            this.NavigationController.PushViewController(new PhotoDisplayController(photoName),true)
        )

    let map = new MKMapView(UIScreen.MainScreen.Bounds)
    do  map.ShowsUserLocation <- true
        map.ZoomEnabled <- true
        map.Delegate <- mapDelegate


    let notes = dataAccess.GetNotes (trip.Id)
    do  notes |> Seq.iter (fun n -> new NoteAnnotation(n.Latitude,n.Longitude,n.Text) |> map.AddAnnotation )

    let photos = dataAccess.GetPhotos (trip.Id)
    do  photos |> Seq.iter (fun p -> new PhotoAnnotation(p.Latitude, p.Longitude,"thumb_" + p.Name, p.Name) |> map.AddAnnotation )
    




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
            imagePickerController.Delegate <- new ImagePickerDelegate(dataAccess,trip.Id, lat, lng, map)
              
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
            let annotation = new NoteAnnotation(noteLat, noteLng, note)
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

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated) 
            
    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)

    override this.DidReceiveMemoryWarning() =
        base.DidReceiveMemoryWarning()
