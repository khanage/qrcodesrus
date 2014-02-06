namespace QRCodesRUs.Web.ViewModels

open System
open QRCodesRUs.Data
open System.ComponentModel.DataAnnotations

type PurchaseViewModel(product: Product, qrCodeId: Guid, shippingCost: decimal) =

    new() = PurchaseViewModel(new Product(), Guid.NewGuid(), 0.0m)

    member val ProductId = product.ProductID with get, set
    member val QrCodeId = qrCodeId with get, set
    member val ProductName = product.Name with get, set
    member val ProductImage = product.ImageName with get, set
    member val Price = product.Price with get, set

    member x.IsShippingFree with get() = shippingCost = 0.0m

    [<Required>]
    [<RegularExpression("^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13})$", ErrorMessage = "Invalid credit card")>]
    member val CreditCardNumber = "" with get, set
    
    [<Required>]
    [<StringLength(80, MinimumLength = 2)>]
    member val CardholderName = "" with get, set
    
    [<Required>]
    [<Range(1,12)>]
    member val ExpiryMonth = new Nullable<int>() with get, set
    
    [<Required>]
    [<Range(14,20)>]
    member val ExpiryYear = new Nullable<int>() with get, set