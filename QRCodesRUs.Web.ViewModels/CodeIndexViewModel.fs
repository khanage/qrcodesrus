namespace QRCodesRUs.Web.ViewModels

open System
open System.ComponentModel.DataAnnotations
open System.Web.Mvc
open QRCodesRUs.Web.Model


type CodeIndexViewModel() =
    let listItemsForPageSizes 
        = PageSizeData.valuesAndNames 
       |> Seq.map (fun (value, name) -> 
                   let sli = new SelectListItem()
              
                   sli.Value <- value.ToString()
                   sli.Text <- name

                   sli.Selected <- name.Equals("A4", StringComparison.OrdinalIgnoreCase)

                   sli)

    [<Required(ErrorMessage = "The password is required")>]
    member val UserCode = "" with get, set
    member val PageType = PageSize.A4 with get, set

    member x.AllPageSizes with get() = listItemsForPageSizes