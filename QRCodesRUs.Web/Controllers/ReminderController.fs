namespace QRCodesRUs.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open Microsoft.AspNet.Identity
open Microsoft.AspNet.Identity.EntityFramework
open QRCodesRUs.WebHacks.Models
open QRCodesRUs.Web.ViewModels
open FSharpx
open QRCodesRUs.Web.Model
open QRCodesRUs.Data

open System.Security.Principal

module Option =
    let ofNull<'a when 'a : null> (a: 'a) =
        match a with 
        | null -> None
        | _ -> Some a

[<Authorize>]
type ReminderController() =
    inherit Controller()

    let usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()))

    let reminderOfCurrentUserOrDefault(user: IPrincipal) =
        let currentUser = user.Identity.GetUserId() |> usermanager.FindById 
        new ReminderViewModel(currentUser.Reminder |> Option.ofNull)

    member x.Index () = 
        let thing = reminderOfCurrentUserOrDefault x.User
        x.View thing

    [<HttpPost>]
    member x.Index (reminder: ReminderViewModel) =
        let currentUser = x.User.Identity.GetUserId() |> usermanager.FindById 
        currentUser.Reminder <- reminder.AsPasswordReminder()
        let updateResult = usermanager.Update currentUser
        x.View(reminder)