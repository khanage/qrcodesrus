namespace QRCodesRUs.Web.Controllers

open System.Web.Mvc
open Microsoft.AspNet.Identity
open QRCodesRUs.WebHacks.Controllers

[<Authorize>]
type AccountController() =
    inherit HackedAccountController()
    // This is this way because it was  too much effort to port the code gened c# controller
    