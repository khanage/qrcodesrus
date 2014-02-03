namespace QRCodesRUs.CodeGeneration

open System
open System.IO
open System.Threading.Tasks
open System.Collections.Generic
open FSharpx
open QRCodesRUs.Core
open QRCodesRUs.Web.Model

type QrCodeRepository =
    abstract member GetById: QrCodeId -> Stream Option Async
    abstract member CreateNew: text: string -> width: int -> height: int -> QrCodeId

type InMemoryQrCodeRepository(creator: QrCodeCreator) =
    let dict = new Dictionary<QrCodeId,Task<Stream>>()
    let mutable firstId : Option<QrCodeId> = None

    interface QrCodeRepository with
        member x.GetById id = 
            match id :> obj with
            | null -> (x :> QrCodeRepository).GetById (firstId.Value)
            | _ -> 
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
            let id = new QrCodeId(Guid.NewGuid())
            let asyncCreation = creator.CreateCodeForText text width height 

            firstId <- Some id

            dict.[id] <- Async.StartAsTask <| asyncCreation
            
            id