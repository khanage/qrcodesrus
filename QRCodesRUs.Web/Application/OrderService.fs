namespace QRCodesRUs.Web

open System
open QRCodesRUs.Data
open QRCodesRUs.Web.Model
open QRCodesRUs.Web.ViewModels

type OrderProcessing = Success of orderId: Guid | Failure of message: string 

type OrderService(orderRepository: OrderRepository, paymentService : PaymentService) =
    member x.PlaceOrder(purchaseRequest: PurchaseRequest) =
        if paymentService.ChargeAmmount purchaseRequest.CardDetails purchaseRequest.Total 
        then
            let id = Guid.NewGuid()
            let order = new Order(OrderID = id, ProductID = purchaseRequest.ProductId, UserId = purchaseRequest.UserId, Address = purchaseRequest.AddressToShip)
            orderRepository.SaveOrder order
            Success id
        else Failure "payment failed"

    member x.LoadOrderDetails(orderId: Guid) =
        new Order(OrderID = orderId)

