namespace QRCodesRUs.Web.Controllers

open System
open FSharpx
open QRCodesRUs.Data
open QRCodesRUs.Web
open QRCodesRUs.Web.Model
open QRCodesRUs.CodeGeneration
open System.Web.Http
open System.Web.Http.ModelBinding
open System.Web.Http.Results

type QrCodeController(repository: QrCodeRepository) =
    inherit ApiController()
    
    member x.Get ([<ModelBinder(typeof<QrCodeIdHttpModelBinder>)>]id: QrCodeId) : IHttpActionResult =
        repository.GetById id 
     |> Async.map (function
        | Some(imageStream) -> 
            new ImageResult(imageStream) :> IHttpActionResult
        | None         -> 
            new NotFoundResult(x) :> _)
     |> Async.RunSynchronously