namespace QRCodesRUs.Web.ViewModels

open System
open System.Collections.Generic
open System.Web.Mvc
open QRCodesRUs.Data
open FSharpx

type ReminderViewModel(user: ApplicationUser) =
    
    let defaultReminder = new PasswordReminder(3, "month")
    let reminder = user.Reminder() |> Option.getOrElse defaultReminder

    let dateUnit = reminder.DateUnit
    let numericValue = reminder.NumericValue

    new() = ReminderViewModel(new ApplicationUser())
    
    member private x.ProjectSelectListItem (v: KeyValuePair<string,string>) =
        let actualValue = v.Key
        let displayText = v.Value
        let isSelected = reminder.DateUnit.Equals(v.Key, StringComparison.OrdinalIgnoreCase)

        new SelectListItem(Value = actualValue, Text = displayText, Selected = isSelected)
    
    member val DateUnit = dateUnit with get, set
    member val NumericValue = numericValue with get, set
    
    member x.DisplayValueForUnit 
        with get() = 
            if(x.NumericValue = 1)
            then x.DateUnit
            else x.DateUnit + "s"

    member x.HasReminder with get() = user.HasReminder()
    member x.UnitOptions with get() = reminder.AllValues |> Seq.map x.ProjectSelectListItem

    member x.AsPasswordReminder() =
        new PasswordReminder(x.NumericValue, x.DateUnit)