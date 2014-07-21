namespace TripDiaryIos

open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open VL
open MonoTouch.Foundation

type NoteDisplayController(note) as this = 
    inherit UIViewController() 

    do base.Title <- localize "notedisplay_title"

    let textview = Controls.textview()
    do
        textview.Text <- note
        textview.Editable <- false 
        textview.Layer.BorderWidth <- 0.f
        textview.Font <- UIFont.SystemFontOfSize(18.f)
        this.AutomaticallyAdjustsScrollViewInsets <- false
             
    let backClick sender eventArgs = this.NavigationController.PopViewControllerAnimated(true) |> ignore
         

    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this true
        this.NavigationController.NavigationBar.Hidden <- false
        this.NavigationItem.SetLeftBarButtonItem(Controls.barButtinItemWithText (localize "notedisplay_back") backClick, true)
                 
        this.Add(textview)

        let constraints = [ 
            H [ !- 20. ; !@ textview ; !- 20. ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(topLayoutGuide this 0.f textview)
        this.View.AddConstraint(bottomLayoutGuide this 0.f textview)
    
    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)
