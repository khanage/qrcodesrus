namespace QRCodesRUs.Data

open FSharpx
open System
open Microsoft.AspNet.Identity.EntityFramework
open System.Collections.Generic
open System.Data.Entity

type PasswordReminder(numericValue: int, dateUnit: string) =    
    static member private PossibleValues : IDictionary<string,string> = 
        new Map<_,_> (
            [|
                "month", "months"
                "day", "days"
            |] ) :> _

    member val NumericValue = numericValue with get, set
    member val DateUnit = dateUnit with get, set
    member val AllValues: IDictionary<string,string> = PasswordReminder.PossibleValues with get

type ApplicationUser() =
    inherit IdentityUser()
    
    member val ReminderValue = new Nullable<int>(0) with get, set
    member val DateUnit: string = null with get, set
    
    member x.HasReminder() = x.ReminderValue.HasValue && not <| System.String.IsNullOrEmpty x.DateUnit

    member x.Reminder() =
        if x.HasReminder()
        then Some (new PasswordReminder(x.ReminderValue.Value, x.DateUnit))
        else None

    member x.SetReminder (reminder: PasswordReminder) =
        x.ReminderValue <- new Nullable<_>(reminder.NumericValue)
        x.DateUnit <- reminder.DateUnit

    member x.RemoveReminder() = 
        x.ReminderValue <- new Nullable<int>()
        x.DateUnit <- null

type ApplicationDbContext() =
    inherit IdentityDbContext<ApplicationUser>("AuthenticationContext")

    do Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>())

    do ()