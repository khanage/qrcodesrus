namespace QRCodesRUs.Web.ViewModels

type CodeCreateNewCodeViewModel(filePath: string) =
    member x.PathToCode with get() = filePath