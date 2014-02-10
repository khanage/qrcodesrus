namespace QRCodesRUs.Web.ViewModels

open System
open System.ComponentModel.DataAnnotations
open System.Web.Mvc
open QRCodesRUs.Web.Model
open System.Web.Routing
open QRCodesRUs.Data

type CodeIndexViewModel() =
    [<Required(ErrorMessage = "The password is required")>]
    member val UserCode = "" with get, set

type QrCodeDisplayViewModel(id: QrCodeId, data: Product seq) =
    member val Id = id with get, set
    member val Products = data with get, set