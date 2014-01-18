namespace QRCodesRUs.Web.Controllers

open System.Web.Mvc
open Microsoft.AspNet.Identity
open QRCodesRUs.WebHacks.Controllers
open QRCodesRUs.WebHacks.Models

[<Authorize>]
type AccountController() =
    inherit HackedAccountController()
    