namespace TripDiaryIos
open System
open MonoTouch.UIKit
open System.Drawing
open VL
open TripDiaryLibrary

type NewTripController(dataAccess:TripDataAccess) as this = 
    inherit UIViewController()

    let lblTripName = Controls.label "newtrip_lbl_name"
    let tvTripName = Controls.textview()
    let btnContinue = Controls.button "newtrip_btn_continue" (fun _ -> 
        let tripName = tvTripName.Text.Trim()
        if not (String.IsNullOrEmpty(tripName)) then           
             
            match dataAccess.CreateTrip(tvTripName.Text) with
            | true ->
                this.NavigationController.PopToRootViewController(true) |> ignore
    )

    do
        this.Title <- localize "newtrip_title"

        tvTripName.Ended.Add (fun _ -> 
            
            ()
        )


    override this.ViewDidLoad() =
        base.ViewDidLoad()
        this.View.BackgroundColor <- UIColor.White

        this.Add(lblTripName)
        this.Add(tvTripName)
        this.Add(btnContinue)

        let constraints = [
            H [ !- 40. ; !@ lblTripName ; !- 40.]
            H [ !- 40. ; !@ tvTripName ; !- 40.]     

            V [ !- 80. ; !@ lblTripName ; !- 10. ; !@ tvTripName @@ [!!= 30.] ; !- 20. ; !@ btnContinue ] 


        ]  
        VL.packageInto this.View constraints |> ignore
        this.View.AddConstraint(centerX btnContinue this.View)
        

    override this.ViewDidAppear(animated)=
        base.ViewDidAppear(animated)