namespace QRCodesRUs.Web.Model

open System.Web.Routing
open FSharpx
        
[<AutoOpen>]
module Monads =
    let option = Option.MaybeBuilder()

module Option = 
    let ofNull =
        function
        | null -> None
        | a -> Some a
    let whenEmpty (f: unit -> unit) (ma: 'a option): 'a option =
        match ma with
        | None -> f()
        | _ -> ()
        ma

module Mvc =
    let asRouteValues (kvps: (string * obj) seq): RouteValueDictionary =
        new RouteValueDictionary(new Map<_,_>(kvps))