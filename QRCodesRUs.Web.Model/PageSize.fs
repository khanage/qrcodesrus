namespace QRCodesRUs.Web.Model

type PageSize 
    = Large = 3 
    | Mid = 4

[<AutoOpen>]
module PageSizePatterns =
    type PageDimension = { width : int; height : int}
    
    let (|PageDimensions|_|) (p: PageSize) = 
        match p with
        | PageSize.Large -> Some({width = 840; height = 840})
        | PageSize.Mid -> Some({width = 420; height = 420})
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
