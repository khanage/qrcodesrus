namespace QRCodesRUs.Web    

open System
open QRCodesRUs.Data
open QRCodesRUs.Web.Model
open QRCodesRUs.Web.ViewModels
open System.Web.Mvc
open FSharpx
open Microsoft.Practices.ServiceLocation
open System.Web.Routing

type QrCodeIdHttpModelBinder() =

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

    interface IModelBinderProvider with
        member x.GetBinder(t: Type) =
            if typeof<QrCodeId>.IsAssignableFrom(t) 
            then x :> IModelBinder 
            else null

type PurchaseViewModelBinder() =
    inherit DefaultModelBinder()
    
    override x.CreateModel(controllerContext: ControllerContext, bindingContext: ModelBindingContext, modelType: Type) =
        let repo = ServiceLocator.Current.GetInstance(typeof<IProductRepository>) :?> IProductRepository

        option {
            let shippingCosts = 0.0m
            let rawProductId = bindingContext.ValueProvider.GetValue "productId" 
            let rawQrCode = bindingContext.ValueProvider.GetValue "qrCodeId"
        

            let! mproductId = rawProductId |> Option.ofNull |> Option.whenEmpty (fun() -> raise(new Exception(sprintf "%A was null" rawProductId)))
            let! mqrId = rawQrCode |> Option.ofNull |> Option.whenEmpty (fun () -> raise(new Exception(sprintf "%A was null" rawQrCode)))

            let! productId = mproductId.AttemptedValue |> Option.fromTryPattern Int32.TryParse
                             |> Option.whenEmpty (fun () -> raise(new Exception(sprintf "Couldn't parse %A" mproductId.AttemptedValue)))
            let! qrCode = mqrId.AttemptedValue |> Option.fromTryPattern Guid.TryParse    
                            |> Option.whenEmpty (fun () -> raise(new Exception(sprintf "Couldn't parse %A" mqrId.AttemptedValue)))
            let! product = repo.ProductById productId
                            |> Option.whenEmpty (fun () -> raise(new Exception(sprintf "Couldn't load product %i" productId)))


            return new PurchaseViewModel(product, qrCode, shippingCosts) :> obj
        } |> Option.getOrElseF (fun () -> raise(new Exception(sprintf "Failed to bind model")))

    interface IModelBinderProvider with
        member x.GetBinder(t: Type) =
            if typeof<PurchaseViewModel>.IsAssignableFrom(t) 
            then x :> IModelBinder 
            else null

