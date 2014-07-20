namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open VL
open TripDiaryLibrary

type NewTripController(dataAccess:DataAccess) as this = 
    inherit UIViewController()

    let lblTripName = Controls.label "newtrip_lbl_name"
    let tvTripName = Controls.textview()
    do  tvTripName.BecomeFirstResponder() |> ignore


    let btnContinue = Controls.button "newtrip_btn_continue" (fun _ -> 
        let tripName = tvTripName.Text.Trim()
        if not (String.IsNullOrEmpty(tripName)) then      
            
            match dataAccess.CreateTrip tvTripName.Text with
            | true -> this.NavigationController.PopViewControllerAnimated (true) |> ignore
            | false -> printfn "trip was not inserted"
    )

    do
        this.Title <- localize "newtrip_title"



    override this.ViewDidLoad() =
        base.ViewDidLoad()
        Colors.styleController this true

        this.Add(lblTripName)
        this.Add(tvTripName)
        this.Add(btnContinue)

        let constraints = [
            H [ !- 10. ; !@ lblTripName ; !- 10.]
            H [ !- 10. ; !@ tvTripName ; !- 10.]
            V [ !- 80. ; !@ lblTripName ; !- 10. ; !@ tvTripName @@ [!!= 30.] ; !- 20. ; !@ btnContinue ]
        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(centerX btnContinue this.View)
        

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)
 