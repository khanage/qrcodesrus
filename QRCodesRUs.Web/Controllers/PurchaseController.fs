namespace QRCodesRUs.Web

open System
open FSharpx
open System.Web.Mvc
open QRCodesRUs.Data
open QRCodesRUs.Web.ViewModels
open Microsoft.AspNet.Identity


type PurchaseController(productRepository: ProductRepository, orderService: OrderService) =
    inherit Controller()

    member x.Index(productId: int, qrCodeId: Guid) =
        let product = productRepository.ProductById productId
        x.View(new PurchaseViewModel(product.Value, qrCodeId, 0.0m))
    
    [<HttpPost>]
    member x.Index(purchaseModel: PurchaseViewModel) =
        if x.ModelState.IsValid 
        then
            let product = productRepository.ProductById purchaseModel.ProductId |> Option.get
            let address = purchaseModel.Address
            let total   = product.Price

            let orderId = match purchaseModel.AsPurchase(x.User.Identity.GetUserId(), total, address) |> orderService.PlaceOrder with
                          | Success id -> id :> obj
                          | Failure message -> raise(new Exception(message))
            
            x.RedirectToAction("Purchased", Mvc.asRouteValues ["orderId", orderId]) :> ActionResult

        else x.View purchaseModel :> _
            
            
    member x.Purchased(orderId: Guid) =
        let orderDetails = orderService.LoadOrderDetails orderId
        x.View(new OrderDetailsViewModel(orderDetails))
