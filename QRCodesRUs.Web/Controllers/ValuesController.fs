namespace QRCodesRUs.Web.Controllers

open System
open System.Threading
open System.Collections.Generic
open System.Linq
open System.Net
open System.Net.Http
open System.Web.Http
open System.Net.Http.Formatting
open System.Web.Mvc
open System.Web.Http.Results
open System.Net.Http.Headers
open FSharpx
open QRCodesRUs.Web
open QRCodesRUs.Web.Model
open QRCodesRUs.Web.ViewModels
open QRCodesRUs.CodeGeneration

type ImageResult(imageStream: IO.Stream, ?contentType: MediaTypeHeaderValue) =

    let actualContentType = contentType |> Option.getOrElse (new MediaTypeHeaderValue("image/png"))

    let generateImageResponse() =
        async {
            let message = new HttpResponseMessage(HttpStatusCode.OK)

            imageStream.Seek(0L, IO.SeekOrigin.Begin) |> ignore
            message.Content <- new StreamContent(imageStream)

            message.Content.Headers.ContentType <- actualContentType

            return message
        }

    interface IHttpActionResult with
        member x.ExecuteAsync(cancellationToken: CancellationToken) : Tasks.Task<HttpResponseMessage> = 
            generateImageResponse() |> Async.StartAsTask


/// Retrieves images.
[<RoutePrefix("codes")>]
type CodesController(repository: QrCodeRepository) =
    inherit ApiController()
    
    member x.Get ([<ModelBinder(typeof<QrCodeIdModelBinder>)>]id: QrCodeId) : IHttpActionResult =
        repository.GetById id 
     |> Async.map (function
        | Some(imageStream) -> 
            new ImageResult(imageStream) :> IHttpActionResult
        | None         -> 
            new NotFoundResult(x) :> _)
     |> Async.RunSynchronously