namespace QRCodesRUs.Web

open System
open FSharpx
open System.Web.Mvc
open QRCodesRUs.Data
open QRCodesRUs.Web.ViewModels


type PurchaseController(productRepository: IProductRepository) =
    inherit Controller()

    member x.Index(productId: int, qrCodeId: Guid) =
        let product = productRepository.ProductById productId
        x.View(new PurchaseViewModel(product.Value, qrCodeId, 0.0m))
    
    [<HttpPost>]
    member x.Index(purchaseModel: PurchaseViewModel) =
        if not x.ModelState.IsValid then x.View purchaseModel :> ActionResult
        else 
            
            x.RedirectToAction("Purchased", Mvc.asRouteValues ["orderId", Guid.NewGuid() :> obj]) :> ActionResult
            
    member x.Purchased() =
        x.View()
