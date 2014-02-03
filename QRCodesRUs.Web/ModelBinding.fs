namespace QRCodesRUs.Web    

open System
open QRCodesRUs.Web.Model
open System.Web.Mvc
open FSharpx

module Option = 
    let ofNull =
        function
        | null -> None
        | a -> Some a

[<AutoOpen>]
module Monads =
    let option = Option.MaybeBuilder()

type QrCodeIdModelBinder() = 
    inherit DefaultModelBinder()

    let option = Option.MaybeBuilder()
        
    member x.BindInBase(controllerContext: ControllerContext, bindingContext: ModelBindingContext) =
        base.BindModel(controllerContext, bindingContext)

    override x.BindModel (controllerContext, bindingContext) =
        let callBase() = x.BindInBase(controllerContext, bindingContext)

        option {
            let! idValue = bindingContext.ValueProvider.GetValue "id" 
                        |> Option.ofNull

            let! guid = idValue.RawValue.ToString() 
                        |> Option.fromTryPattern Guid.TryParse

            return new QrCodeId(guid) :> obj
        } 
     |> Option.getOrElseF callBase


    interface System.Web.Http.ModelBinding.IModelBinder with
        member x.BindModel(actionContext, bindingContext) =
            
            if typeof<QrCodeId>.IsAssignableFrom(bindingContext.ModelType) 
            then 
                option {
                    let! idValue = bindingContext.ValueProvider.GetValue "id" |> Option.ofNull
                    let! guid = idValue.RawValue.ToString()
                             |> Option.fromTryPattern Guid.TryParse

                    bindingContext.Model <- new QrCodeId(guid)

                    return true
                } |> Option.getOrDefault
            else false

    interface IModelBinderProvider with
        member x.GetBinder(t: Type) =
            if typeof<QrCodeId>.IsAssignableFrom(t) 
            then x :> IModelBinder 
            else null

