namespace QRCodesRUs.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open QRCodesRUs.Web
open Microsoft.AspNet.Identity
open QRCodesRUs.Web.ViewModels

open System.Security.Principal

[<Authorize>]
type ReminderController(userService: UserService) =
    inherit Controller()

    member x.Index () = 
        let currentUser = x.User.Identity.GetUserId() |> userService.LoadUser
        ReminderViewModel currentUser |> x.View

    [<HttpPost>]
    member x.Index (reminder: ReminderViewModel) =
        let currentUser = reminder.AsPasswordReminder() |> userService.CreateReminderFor <| x.User.Identity.GetUserId() 
        let viewModel = new ReminderViewModel(currentUser)

        x.View(viewModel)

    member x.RemoveReminder() =
        x.User.Identity.GetUserId() |> userService.RemoveReminder |> ignore
        x.RedirectToAction("Index")