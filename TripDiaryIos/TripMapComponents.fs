module TripMapComponents
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open TripDiaryLibrary
open VL
open MonoTouch.MapKit
open MonoTouch.CoreLocation 

type NoteAnnotation(lat,lng,note) =
    inherit MKAnnotation()
    let mutable mCoordinate = new CLLocationCoordinate2D(lat,lng) 
    override this.Coordinate with get() = mCoordinate and set v = mCoordinate <- v
    override this.Title with get() = localize "activetrip_noteannotation_title" 
    member this.Note with get() = note

type PhotoAnnotation(lat,lng,thumbnailName,photoName) =
    inherit MKAnnotation()
    let mutable mCoordinate = new CLLocationCoordinate2D(lat,lng)  
    override this.Coordinate with get() = mCoordinate and set v = mCoordinate <- v
    override this.Title with get() = localize "activetrip_photoannotation_title"     
    member this.PhotoName with get() = photoName
    member this.ThumbnailName with get() = thumbnailName
    
type MapDelegate(trackLocation:bool) =
    inherit MKMapViewDelegate() 
    let mutable updatedFirstTime = true
    let noteAnnotationId = "noteAnnotationId"
    let photoAnnotationId = "photoAnnotationId"


    let noteAnnotationClicked = Event<string>()
    let photoAnnotationClicked = Event<string>()

    let userLocationUpdated = Event<_>()
    member this.UserLocationUpdated = userLocationUpdated.Publish
    member this.NoteAnnotationClicked = noteAnnotationClicked.Publish
    member this.PhotoAnnotationClicked = photoAnnotationClicked.Publish

    override this.DidUpdateUserLocation(mapView,userLocation) =
        userLocationUpdated.Trigger(userLocation)

        if updatedFirstTime && trackLocation then   
            userLocation.Title <- ""          
            mapView.CenterCoordinate <- userLocation.Coordinate
            let region = MKCoordinateRegion.FromDistance(userLocation.Coordinate,300.,300.)
            mapView.Region <- region
            updatedFirstTime <- false

    override this.GetViewForAnnotation(mapview,annotation) =
        match annotation with
        | :? MKUserLocation -> null 
        | :? NoteAnnotation ->        
            let noteAnnotation = annotation :?> NoteAnnotation         
            let view =  match mapview.DequeueReusableAnnotation(noteAnnotationId) with
                        | null -> new MKPinAnnotationView(annotation, noteAnnotationId) :> MKAnnotationView
                        | existingView -> existingView   
            view.CanShowCallout <- true   

            let button = UIButton.FromType(UIButtonType.DetailDisclosure)
            button.TouchUpInside.Add (fun _ -> 
                noteAnnotationClicked.Trigger(noteAnnotation.Note)
            )
            view.RightCalloutAccessoryView <- button
                     
            view
        | :? PhotoAnnotation ->
            let photoAnnotation = annotation :?> PhotoAnnotation
            let view =  match mapview.DequeueReusableAnnotation(photoAnnotationId) with
                        | null -> new MKAnnotationView(annotation, photoAnnotationId)
                        | existingView -> existingView  
            let imageView = new UIImageView(UIImage.FromFile(fileInPersonalFolder photoAnnotation.ThumbnailName))
            imageView.Layer.CornerRadius <- 5.f
            imageView.Layer.BorderWidth <- 2.f
            imageView.Layer.BorderColor <- Colors.photoAnnotationBorder.CGColor
            imageView.Layer.MasksToBounds <- true
            view.AddSubview(imageView) 
            view.CenterOffset <- new PointF(-15.f,-15.f)
            let button = UIButton.FromType(UIButtonType.DetailDisclosure)
            button.TouchUpInside.Add (fun _ -> 
                photoAnnotationClicked.Trigger(photoAnnotation.PhotoName)
            )
            view.RightCalloutAccessoryView <- button
            
            view.CanShowCallout <- true            
            view
    
        | _ -> null