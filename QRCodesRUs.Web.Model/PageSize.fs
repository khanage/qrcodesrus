namespace QRCodesRUs.Web.Model

type PageSize 
    = A3 = 3 
    | A4 = 4

[<AutoOpen>]
module PageSizePatterns =
    type PageDimension = { width : int; height : int}
    
    let (|PageDimensions|_|) (p: PageSize) = 
        match p with
        | PageSize.A3 -> Some({width = 297; height = 420})
        | PageSize.A4 -> Some({width = 210; height = 297})
        | _  -> None

module PageSizeData =
    open System
    open System.Linq

    let names = Enum.GetNames(typeof<PageSize>).Cast<string>()
    let values = Enum.GetValues(typeof<PageSize>).Cast<int>()

    let valuesAndNames = values |> Seq.zip <| names
    
    let dimensionsForSize (p: PageSize) 
        = match p with
            | PageDimensions { width = width; height = height } -> Some(width, height)
            | _ -> None
