﻿[<AutoOpen>]
module Localization 
let en = Map.ofList [
            "menu_btn_new", "Create new trip"
            "newtrip_title", "Creating new trip"
            "newtrip_lbl_name", "Name your trip"
            "newtrip_btn_continue", "Continue"
            "activetrip_btn_takephoto", "Take photo"
            "activetrip_btn_canceltrip", "Cancel trip"

            "writenote_title", "Write your note"
        ]  
         
let localize (key:string):string = match Map.tryFind key en with | Some v -> v | None -> key