namespace QRCodesRUs.Web.Model

open System

type CardDetails(number: string, name: string, expiry: DateTime) =
    member x.Cardnumber with get() = number
    member x.CardholderName with get() = name
    member x.Expiry with get() = expiry

type PurchaseRequest(userId: string, cardDetails: CardDetails, total: decimal, productId: int, addressToShip: string) =
    member x.UserId with get() = userId
    member x.CardDetails with get() = cardDetails
    member x.Total with get() = total
    member x.AddressToShip with get() = addressToShip
    member x.ProductId with get() = productId

