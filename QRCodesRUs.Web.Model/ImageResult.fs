namespace QRCodesRUs.Web.Model

open System.IO
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Web.Http
open System.Threading
open System.Threading.Tasks
open FSharpx

type ImageResult(imageStream: Stream, ?contentType: MediaTypeHeaderValue) =

    let actualContentType = contentType |> Option.getOrElse (new MediaTypeHeaderValue("image/png"))

    let generateImageResponse() =
        async {
            let message = new HttpResponseMessage(HttpStatusCode.OK)

            imageStream.Seek(0L, SeekOrigin.Begin) |> ignore
            message.Content <- new StreamContent(imageStream)

            message.Content.Headers.ContentType <- actualContentType

            return message
        }

    interface IHttpActionResult with
        member x.ExecuteAsync(cancellationToken: CancellationToken) : Task<HttpResponseMessage> = 
            generateImageResponse() |> Async.StartAsTask


