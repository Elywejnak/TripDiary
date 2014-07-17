(*
#r "/Developer/MonoTouch/usr/lib/mono/2.1/monotouch"
*)
/// Visual Layout DSL
module VL
open MonoTouch.UIKit
open MonoTouch.Foundation

type Predicate = 
    | EQ  of float  //predicates - constants
    | GE  of float  // >=
    | LE  of float  // <=
    | EQv of string //predicates - refering to view
    | LEv of string // >= 
    | GEv of string // <=

type PrioritizedPredicate = 
    | P  of Predicate          //no priority
    | Pc of Predicate * float  //constant priority
    | Pm of Predicate * string //metric priority

type Spacing =
    | Sp             //standard space
    | M    of float  //spacing (padding) constraints - constants
    | Mge  of float  // >=
    | Mle  of float  // <=
    | Mv   of string //spacing (padding) constLd_Trnts - refering to view
    | Mvge of string // >=
    | Mvle of string // <=
    | Mm   of string //spacing (padding) constraints - refering to metric
    | Mmge of string // >=
    | Mmle of string // <=
    | Ms   of Spacing list

type VLView = MonoTouch.UIKit.UIView //can conditional compile for other platforms

type LayoutContent =
    | Vu   of VLView
    | Vuc  of VLView * Predicate list
    | Vucp of VLView * PrioritizedPredicate list
    | Mg   of Spacing

type Anchor = Ld | Tr | Ld_Tr | NA //leading, trailing, both, none

type Layout = 
    | HorzLayout of Anchor * LayoutContent list
    | VertLayout of Anchor * LayoutContent list

//convenience operators and shortcut functions
let inferAnchor l =
    match l, List.rev l with
    | []      , []          -> Anchor.Ld
    | Mg _ ::_, Mg _ :: _   -> Anchor.Ld_Tr
    | Mg _ ::_, _           -> Anchor.Ld
    | _       , Mg _ :: _   -> Anchor.Tr
    | _,_                   -> Anchor.NA

//horizontal and vertical layout
let H  l = HorzLayout(inferAnchor l, l)
let V  l = VertLayout(inferAnchor l, l)
 
//length constraints for views
let (!!=) = EQ
let (!!>) = GE
let (!!<) = LE
   
//spacing constraitns (between views)
let (!-)    c = Mg (M c)
let (!->=)  c = Mg (Mge c)
let (!-<=)  c = Mg (Mle c)
let sp        = Mg Sp

let (!@)  a = Vu(a)
let (@@) a b = match a with Vu v -> Vuc (v,b) | _ -> failwith "@@ operator only converts Vu to Vuc"


///DSL translator
module private VLGen =
    open System

    module List =
        let inline intersperse y xs =
            let rec loop i xs acc =
                match xs with
                | []      -> acc
                | x::[]   -> x::acc |> List.rev
                | x::rest -> loop i rest (i::x::acc)
            loop y xs []
                
    type Term = T of string | Ts of Term list

    let rec emitSpacing = function
        | Sp    -> T "-"
        | M f   -> sprintf "%.2f"   f |> T
        | Mge f -> sprintf ">=%.2f" f |> T
        | Mle f -> sprintf "<=%.2f" f |> T
        | Mv n   | Mm n   -> sprintf "=%s"  n |> T
        | Mvge n | Mmge n -> sprintf ">=%s" n |> T
        | Mvle n | Mmle n -> sprintf "<=%s" n |> T
        | Ms ps -> Ts (ps |> List.map emitSpacing |> List.intersperse (T ","))

    let emitPredicate = function
        | EQ f -> sprintf "==%.2f" f |> T
        | GE f -> sprintf ">=%.2f" f |> T
        | LE f -> sprintf "<=%.2f" f |> T
        | EQv n -> sprintf "==%s"  n |> T
        | GEv n -> sprintf ">=%s" n |> T
        | LEv n -> sprintf "<=%s" n |> T

    let emitPrioritizedPredicate = function
        | P  pred     -> emitPredicate pred
        | Pc (pred,f) -> Ts [emitPredicate pred; sprintf "@%.2f" f |> T]
        | Pm (pred,n) -> Ts [emitPredicate pred; sprintf "@%s" n  |> T]

    let inline emitPlist n ps f =
        Ts [
            T (sprintf "[%s(" n)
            Ts (ps |> List.map f |> List.intersperse (T ","))
            T ")]"
        ]

    let rec emitLayoutContent fViewName = function
        | Vu  v       -> sprintf "[%s]" (fViewName v) |> T
        | Vuc (v,ps)  -> emitPlist (fViewName v) ps emitPredicate
        | Vucp (v,ps) -> emitPlist (fViewName v) ps emitPrioritizedPredicate
        //spacing
        | Mg Sp    -> T "-"
        | Mg (M f) -> Ts [T "-"; emitSpacing (M f); T "-"]
        | Mg m     -> Ts [T "-"; T "("; emitSpacing m; T ")"; T "-"]

    let lopLeadingSpacing ls =
        let rec lopSpacing = function
            | Mg _ :: rest -> lopSpacing rest
            | xs -> xs
        lopSpacing ls
            
    let lopTrailingSpacing = List.rev >> lopLeadingSpacing >> List.rev
    let lopSpacing = lopLeadingSpacing >> lopTrailingSpacing

    let emitLayout fCntnEmitter = function
        | VertLayout (Ld_Tr,l) -> Ts [T "V:|" ; Ts (l |> List.map fCntnEmitter); T "|"]
        | HorzLayout (Ld_Tr,l) -> Ts [T "H:|" ; Ts (l |> List.map fCntnEmitter); T "|"]
        | VertLayout (Ld,   l) -> Ts [T "V:|" ; Ts (l |> lopTrailingSpacing |> List.map fCntnEmitter)]
        | HorzLayout (Ld,   l) -> Ts [T "H:|" ; Ts (l |> lopTrailingSpacing |> List.map fCntnEmitter)]
        | VertLayout (Tr,   l) -> Ts [T "V:" ; Ts (l |> lopLeadingSpacing |> List.map fCntnEmitter); T "|"]
        | HorzLayout (Tr,   l) -> Ts [T "H:" ; Ts (l |> lopLeadingSpacing |> List.map fCntnEmitter); T "|"]
        | VertLayout (NA,   l) -> Ts [T "V:" ; Ts (l |> lopSpacing |> List.map fCntnEmitter)]
        | HorzLayout (NA,   l) -> Ts [T "H:" ; Ts (l |> lopSpacing |> List.map fCntnEmitter)]
     
    let flatten terms =
        let rec loop ts = 
            seq {for t in ts do
                    match t with 
                    | T s   -> yield s
                    | Ts ts -> yield! loop ts}
        loop terms

    let genConstraintString fLayoutEmitter layout = 
        let terms = fLayoutEmitter layout
        let fterms = flatten [terms]
        String.Join("",fterms |> Seq.toArray)

    let embeddedView = function
        | Vu  (v)    | Vu (v)  
        | Vuc (v,_)  | Vuc (v,_)
        | Vucp (v,_) | Vucp (v,_) -> Some v
        | _ -> None

    let layoutContents = function VertLayout (_,l) | HorzLayout (_,l) -> l

    let layoutViews = 
            layoutContents  
            >> List.map embeddedView 
            >> List.choose (fun a->a) 

    let nameViews  = List.mapi (fun i v -> v, sprintf "V_%d" i)

    let namedLayoutViews = layoutViews >> nameViews


//API
let genConstraintString layout = 
    let namedViews = VLGen.namedLayoutViews layout
    let nameMap = System.Collections.Generic.Dictionary<VLView,string>()
    namedViews |> Seq.iter (fun (k,v) -> nameMap.Add(k,v))
    let fNamedView v = nameMap.[v]
    let fCntnEmitter = VLGen.emitLayoutContent fNamedView
    let fLayoutEmitter = VLGen.emitLayout fCntnEmitter
    let cnstrnt = VLGen.genConstraintString fLayoutEmitter layout
    namedViews |> Seq.map fst |> Seq.iter (fun v -> v.TranslatesAutoresizingMaskIntoConstraints <- false)
    let keys = namedViews |> Seq.map snd |> Seq.map box |> Seq.toArray
    let objs = namedViews |> Seq.map fst |> Seq.map box |> Seq.toArray
    let dict = NSDictionary.FromObjectsAndKeys(objs,keys)
    cnstrnt,dict

let genConstraintsWith layout (layoutOptions:NSLayoutFormatOptions) =
    let cnstrnt,dict = genConstraintString layout
    let nsarr = NSLayoutConstraint.FromVisualFormat(cnstrnt, layoutOptions, null, dict)
    nsarr

let noLayoutOption = enum<NSLayoutFormatOptions>(0)

let genConstraints layout = genConstraintsWith layout noLayoutOption

let addConstraintsWith (container:VLView) layout layoutOptions =
    container.AddConstraints (genConstraintsWith layout layoutOptions)

let addConstraints container layout = addConstraintsWith container layout noLayoutOption

let packageInto (v:VLView) layoutList =
    layoutList |> List.collect VLGen.layoutViews |> List.iter v.AddSubview
    layoutList |> List.iter (addConstraints v)
    v

let package list = 
    let view = new VLView()
    view.TranslatesAutoresizingMaskIntoConstraints <- false
    packageInto view list

let package1 layout = package [layout]