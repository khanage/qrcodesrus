namespace QRCodesRUs.Web

open System.Security.Principal
open Microsoft.AspNet.Identity
open QRCodesRUs.Data

type UserService(userManager: UserManager<ApplicationUser>) =
    member x.LoadUser (userId: string) =
        userManager.FindById userId

    member x.RemoveReminder (userId: string) =
        let user = userManager.FindById userId
        user.RemoveReminder()
        userManager.Update user |> ignore
        user

    member x.CreateReminderFor (reminder: PasswordReminder) (userId: string)  =
        let user = userManager.FindById userId
        user.SetReminder reminder
        userManager.Update user |> ignore
        user

