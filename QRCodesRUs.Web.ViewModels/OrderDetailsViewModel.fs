namespace QRCodesRUs.Web.ViewModels

open QRCodesRUs.Data

type OrderDetailsViewModel(order: Order) =
    member val OrderId = order.OrderID with get, set
    member val Address = order.Address with get, set
    member val DueTime = order.EstimatedDelivery with get, set

