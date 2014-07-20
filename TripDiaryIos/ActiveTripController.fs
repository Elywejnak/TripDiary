﻿namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open TripDiaryLibrary

type ActiveTripController(tripDataAccess:DataAccess,trip:Trip) as this = 
    inherit UIViewController()

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
        let noteWriterController = new NoteWriterController()
        noteWriterController.Canceled.Add (fun _ ->
            printfn "ActiveTripController.NoteWriter.Canceled"  
            this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        noteWriterController.Finished.Add (fun note ->
            printfn "ActiveTripController.NoteWriter.Finished with note=`%s`" note    
            tripDataAccess.SaveNote trip.Id note |> printfn "Note saving status:%b"          
            this.NavigationController.PopViewControllerAnimated(true) |> ignore
        )
        this.NavigationController.PushViewController(noteWriterController, true)

    
    override this.ViewDidLoad() =
        base.ViewDidLoad()
        this.View.BackgroundColor <- UIColor.White      
               
        let leftButtonBarItem = new UIBarButtonItem(localize "activetrip_btn_canceltrip", UIBarButtonItemStyle.Plain, cancelTripClicked)
        this.NavigationItem.SetLeftBarButtonItem(leftButtonBarItem, true)
                 
        let btnTakePhoto = Controls.barButtonItemWithImage "addphoto" takePhotoClicked
        let btnAddNote = Controls.barButtonItemWithImage "addnote" addNoteClicked 

        this.NavigationItem.SetRightBarButtonItems([|btnAddNote;btnTakePhoto|],true)

               

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)