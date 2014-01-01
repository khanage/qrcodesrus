namespace QRCodesRUs.CodeGeneration

open System
open System.IO
open System.Threading.Tasks
open System.Collections.Generic
open FSharpx

type QrCodeId = Guid

type QrCodeRepository =
    abstract member GetById: QrCodeId -> Stream Option Async
    abstract member CreateNew: string -> int -> int -> QrCodeId

type InMemoryQrCodeRepository(creator: QrCodeCreator) =
    let dict = new Dictionary<QrCodeId,Task<Stream>>()

    interface QrCodeRepository with
        member x.GetById id = 
            async {
                match dict.TryGetValue(id) with
                | (true, taskStream) -> 
                    let! s = Async.AwaitTask taskStream
                    return Some(s)
                | otherwise          -> 
                    return None
            }

        member x.CreateNew text width height =
            let id = Guid.NewGuid()
            let asyncCreation = creator.CreateCodeForText text width height 

            dict.[id] <- Async.StartAsTask <| asyncCreation
            
            id