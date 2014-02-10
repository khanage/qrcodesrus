namespace QRCodesRUs.CodeGeneration

open System.IO

type QrCodeCreator =
    abstract member CreateCodeForText: string -> int -> int -> Async<Stream>

type QrCodeCreatorImplementation() =
    interface QrCodeCreator with
        member x.CreateCodeForText code width height =
            async {
                let stream = new MemoryStream() :> Stream

                let! bitmap = QrGenerator.createCodeForText code { height = height; width = width}

                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png)
                stream.Seek(0L, SeekOrigin.Begin) |> ignore
            
                return stream       
            }

