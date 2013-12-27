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

    let createTempImageFor (code: string) =
        async {
            let path = System.IO.Path.GetTempFileName()
            
            use file = System.IO.File.OpenWrite path

            let! bitmap = QrGenerator.createCodeForText code

            do! QrGenerator.writeBitmapToFile file bitmap    
            
            return path          
        }

    member this.Index () = this.View()

    [<HttpPost>]
    member this.CreateNewCode(vm: CodeIndexViewModel) =
        async {
            let! path = createTempImageFor vm.UserCode            
            return new FileStreamResult(System.IO.File.OpenRead path, "image/png")
        } |> Async.StartAsTask