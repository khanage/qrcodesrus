namespace QRCodesRUs.Web

open System
open FSharpx
open System.Web.Mvc
open QRCodesRUs.Data
open QRCodesRUs.Web.ViewModels

type PurchaseController() =
    inherit Controller()

    member x.Index(productId: int, qrCodeId: Guid) =
        let product = ProductRepository.ProductById(productId) 
        x.View(new PurchaseViewModel(product.Value, qrCodeId, 0.0m))
    
    [<HttpPost>]
    member x.Index(purchaseModel: PurchaseViewModel) =
        if not x.ModelState.IsValid then x.View(purchaseModel) :> ActionResult
        else x.RedirectToAction("Purchased") :> ActionResult
            
    member x.Purchased() =
        x.View()
