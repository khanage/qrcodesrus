namespace QRCodesRUs.Data

open FSharpx
open Microsoft.AspNet.Identity.EntityFramework
open System.Collections.Generic

[<AllowNullLiteral>]
type PasswordReminder() =
    member val Value: int = 0 with get, set
    member val Unit = "month" with get, set
    
    static member private PossibleValues : IDictionary<string,string> = 
        new Map<_,_> (
            [|
                "month", "months"
                "day", "days"
            |] ) :> _

    member val AllValues: IDictionary<string,string> = PasswordReminder.PossibleValues with get

type ApplicationUser() =
    inherit IdentityUser()

    [<DefaultValue>]
    val mutable private reminder : PasswordReminder
    member x.Reminder with get() = x.reminder
                       and set v = x.reminder <- v

    member x.HasReminder() = 
        match x.reminder with
        | null -> false
        | _ -> true

type ApplicationDbContext() =
    inherit IdentityDbContext<ApplicationUser>("DefaultConnection")
    do ()