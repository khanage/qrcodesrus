namespace QRCodesRUs.Web.ViewModels

open System
open System.ComponentModel.DataAnnotations
open System.Web.Mvc
open QRCodesRUs.Web.Model


type CodeIndexViewModel() =
    [<Required(ErrorMessage = "The password is required")>]
    member val UserCode = "" with get, set

type QrCodeDisplayViewModel(id: QrCodeId) =
    member val Id = id with get, set