﻿namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open VL
open MonoTouch.Foundation

type NoteWriterController() as this = 
    inherit UIViewController()

    let canceled = new Event<_>()
    let finished = new Event<string>()

    do base.Title <- localize "notewriter_title"

    let textview = Controls.textview()
    do
        textview.Editable <- true
        textview.BecomeFirstResponder() |> ignore
        textview.Layer.BorderWidth <- 0.f
        textview.Font <- UIFont.SystemFontOfSize(18.f)
        this.AutomaticallyAdjustsScrollViewInsets <- false

    let addClick sender eventArgs = finished.Trigger(textview.Text)        
    let cancelClick sender eventArgs = canceled.Trigger()

//    let onKeyboardNotification notification = ()
//    let registerForKeyboardNotifications() =
//        NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, onKeyboardNotification) |> ignore
//        NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, onKeyboardNotification) |> ignore
                

    member this.Canceled = canceled.Publish
    member this.Finished = finished.Publish


    override this.ViewDidLoad() =
        base.ViewDidLoad()
        this.NavigationController.NavigationBar.Hidden <- false
        this.View.BackgroundColor <- UIColor.White  

        this.NavigationItem.SetRightBarButtonItem(Controls.barButtinItemWithText (localize "notewriter_add") addClick, true)
        this.NavigationItem.SetLeftBarButtonItem(Controls.barButtinItemWithText (localize "notewriter_cancel") cancelClick, true)

         
        this.Add(textview)

        let constraints = [ 
            H [ !- 20. ; !@ textview ; !- 20. ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(topLayoutGuide this textview)
        this.View.AddConstraint(bottomLayoutGuide this textview)
    
    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)