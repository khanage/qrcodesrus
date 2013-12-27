namespace QRCodesRUs.Web.Model

type Thing = abstract member Info : unit -> string

type Implementation() =
    interface Thing with member x.Info() = "Hello, world"
