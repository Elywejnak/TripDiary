namespace TripDiaryIos

open System
open MonoTouch.UIKit
open System.Drawing
open Domain
open VL
open MonoTouch.Foundation

type PhotoDisplayController(photoName) as this = 
    inherit UIViewController()
 

    do base.Title <- localize "photodisplay_title"

    let image = new UIImageView()
    do
        image.Image <- UIImage.FromFile(fileInPersonalFolder photoName)
        image.ContentMode <- UIViewContentMode.ScaleAspectFit             
    let backClick sender eventArgs = this.NavigationController.PopViewControllerAnimated(true) |> ignore
         

    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this true
        this.NavigationController.NavigationBar.Hidden <- false
        this.NavigationItem.SetLeftBarButtonItem(Controls.barButtinItemWithText (localize "photodisplay_back") backClick, true)
                 
        this.Add(image)

        let constraints = [ 
            H [ !- 0. ; !@ image ; !- 0. ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(topLayoutGuide this -0.f image)
        this.View.AddConstraint(bottomLayoutGuide this 0.f image)
    
    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)
