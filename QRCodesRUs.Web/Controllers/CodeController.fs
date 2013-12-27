namespace QRCodesRUs.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open QRCodesRUs.Web.ViewModels
open QRCodesRUs.CodeGeneration

type CodeController() =
    inherit Controller()

    member this.Index () = this.View()

    [<HttpPost>]
    member this.CreateNewCode(vm: CodeIndexViewModel)
        = let path = System.IO.Path.GetTempFileName()
          let file = System.IO.File.OpenWrite path

          let bitmap = QrGenerator.createCodeForText vm.UserCode

          QrGenerator.writeBitmapToFile file bitmap

          file.Close()

          new FileStreamResult(System.IO.File.OpenRead path, "image/png")