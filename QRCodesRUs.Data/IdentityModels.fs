namespace QRCodesRUs.Data

open FSharpx
open Microsoft.AspNet.Identity.EntityFramework
open System.Collections.Generic

type PasswordReminder() =
    member val Value: int = 0 with get, set
    member val Unit = "month" with get, set

    member x.FormatUnitForDisplay() =
        if(x.Value = 1)
        then x.Unit
        else x.Unit + "s"
    
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
    val mutable private reminder : PasswordReminder option
    member x.Reminder with get() = x.reminder |> Option.getOrElse (new PasswordReminder()) 
                       and set v = x.reminder <- Some v

    member x.HasReminder() = 
        match x.reminder with
        | None -> false
        | _ -> true

type ApplicationDbContext() =
    inherit IdentityDbContext<ApplicationUser>("DefaultConnection")
    do ()