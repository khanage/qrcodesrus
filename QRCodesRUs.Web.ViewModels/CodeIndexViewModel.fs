namespace QRCodesRUs.Web.ViewModels

type CodeIndexViewModel() = 
    member val UserCode = "" with get, set

type CodeCreateNewCodeViewModel(filePath: string) =
    member x.PathToCode with get() = filePath