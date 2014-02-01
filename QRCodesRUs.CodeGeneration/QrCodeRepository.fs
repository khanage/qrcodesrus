namespace QRCodesRUs.CodeGeneration

open System
open System.IO
open System.Threading.Tasks
open System.Collections.Generic
open FSharpx
open QRCodesRUs.Core

type QrCodeId = Guid

type QrCodeRepository =
    abstract member GetById: QrCodeId -> Stream Option Async
    abstract member CreateNew: text: string -> width: int -> height: int -> QrCodeId

type InMemoryQrCodeRepository(creator: QrCodeCreator) =
    let dict = new Dictionary<QrCodeId,Task<Stream>>()

    interface QrCodeRepository with
        member x.GetById id = 
            async {
                match dict.TryGetValue(id) with
                | true, taskStream -> 
                    let! stream = Async.AwaitTask taskStream

                    stream.Seek(0L, SeekOrigin.Begin) |> ignore

                    let clone = new MemoryStream()
                    
                    do! stream.CopyToAsync(clone) |> Task.toAsync_
                     
                    clone.Seek(0L, SeekOrigin.Begin) |> ignore
                        
                    return Some(clone :> Stream)
                | otherwise        -> 
                    return None
            }

        member x.CreateNew text width height =
            let id = Guid.NewGuid()
            let asyncCreation = creator.CreateCodeForText text width height 

            dict.[id] <- Async.StartAsTask <| asyncCreation
            
            id