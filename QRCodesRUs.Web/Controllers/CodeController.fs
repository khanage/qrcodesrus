﻿namespace QRCodesRUs.Web.Controllers

open System
open System.Web.Mvc
open System.Web.Routing
open FSharpx
open FSharpx.Option
open QRCodesRUs.Web.Model
open QRCodesRUs.Web.ViewModels
open QRCodesRUs.CodeGeneration

type CodeController(repository: QrCodeRepository) =
    inherit Controller()
    
    let redirectToItemPage (vm: CodeIndexViewModel) =
        let width, height = PageSizeData.dimensionsForSize vm.PageType |> Option.get

        let id = repository.CreateNew vm.UserCode width height
        let routeDictionary = new RouteValueDictionary(new Map<_,_> [|"id", id :> obj|])

        RedirectToRouteResult("GetQRCodeById", routeDictionary) :> ActionResult

    member x.Index () = x.View(CodeIndexViewModel())

    [<HttpPost>]
    member x.Index(vm: CodeIndexViewModel) =
        if not x.ModelState.IsValid 
        then x.View(vm) :> ActionResult
        else redirectToItemPage vm
            

    member x.ItemById(id: QrCodeId) =
        repository.GetById id 
     |> Async.map (function
        | Some(result) -> new FileStreamResult(result, "image/png")             :> ActionResult
        | None         -> new HttpStatusCodeResult(Net.HttpStatusCode.NotFound) :> ActionResult)
     |> Async.StartAsTask