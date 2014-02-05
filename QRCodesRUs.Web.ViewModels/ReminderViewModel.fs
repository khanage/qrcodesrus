namespace QRCodesRUs.Web.ViewModels

open System
open System.Collections.Generic
open System.Web.Mvc
open QRCodesRUs.Data
open FSharpx

type ReminderViewModel(maybeReminder: PasswordReminder option) =
    
    let defaultReminder = new PasswordReminder(Unit = "month", Value = 3)
    let reminder = maybeReminder |> Option.getOrElse defaultReminder

    let dateUnit = reminder.Unit
    let numericValue = reminder.Value

    new() = ReminderViewModel(None)
    
    member private x.ProjectSelectListItem (v: KeyValuePair<string,string>) =
        let displayText = v.Key
        let actualValue = v.Key
        let isSelected = reminder.Unit.Equals(v.Key, StringComparison.OrdinalIgnoreCase)

        new SelectListItem(Text = displayText, Value = actualValue, Selected = isSelected)
    
    member x.DisplayValueForUnit 
        with get() = 
            if(numericValue = 1)
            then dateUnit
            else dateUnit + "s"

    member val DateUnit = dateUnit with get, set
    member val NumericValue = numericValue with get, set

    member x.HasReminder with get() = maybeReminder |> Option.isSome
    member x.UnitOptions with get() = reminder.AllValues |> Seq.map x.ProjectSelectListItem

    member x.AsPasswordReminder() =
        new PasswordReminder(Unit = x.DateUnit, Value = x.NumericValue)