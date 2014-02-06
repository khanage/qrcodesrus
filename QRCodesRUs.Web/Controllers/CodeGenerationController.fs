namespace QRCodesRUs.Web.Controllers

open System
open System.Web.Mvc
open System.Web.Routing
open FSharpx
open FSharpx.Option
open QRCodesRUs.Web.Model
open QRCodesRUs.Web.ViewModels
open QRCodesRUs.CodeGeneration
open QRCodesRUs.Web.Model
open QRCodesRUs.Data
open QRCodesRUs.Web

type CodeGenerationController(repository: QrCodeRepository, productRepository: IProductRepository) =
    inherit Controller()
    
    let redirectToItemPage (vm: CodeIndexViewModel) =
        let width, height = PageSizeData.dimensionsForSize PageSize.Mid |> Option.get
        let id = repository.CreateNew vm.UserCode width height

        RedirectToRouteResult("GetQRCodeById", Mvc.asRouteValues [|"id", id.Id :> obj|]) :> ActionResult

    member x.Index () = x.View(CodeIndexViewModel())

    [<HttpPost>]
    member x.Index(vm: CodeIndexViewModel) =
        if not x.ModelState.IsValid 
        then x.View(vm) :> ActionResult
        else redirectToItemPage vm
            

    member x.ItemById(id: QrCodeId) =
        let data = productRepository.AllProducts()
        x.View(new QrCodeDisplayViewModel(id, data)) :> ActionResult