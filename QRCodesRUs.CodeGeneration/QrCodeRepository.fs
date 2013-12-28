namespace QRCodesRUs.CodeGeneration

open System
open System.IO
open System.Collections.Generic
open FSharpx

type QrCodeId = Guid

type QrCodeRepository =
    abstract member GetById: QrCodeId -> Stream Async
    abstract member CreateNew: string -> int -> int -> QrCodeId

type InMemoryQrCodeRepository(creator: QrCodeCreator) =
    let dict = new Dictionary<QrCodeId,IO.Stream>()

    interface QrCodeRepository with
        member x.GetById id = Async.returnM <| dict.[id]

        member x.CreateNew text width height =
            let id = Guid.NewGuid()

            let stream = creator.CreateCodeForText text width height 
                      |> Async.RunSynchronously

            dict.[id] <- stream
            id