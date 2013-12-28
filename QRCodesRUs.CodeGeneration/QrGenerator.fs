namespace QRCodesRUs.CodeGeneration

type Dimensions = { height : int; width: int }

// This is just a wrapper around ZXing
module internal QrGenerator =
    open System
    open System.IO
    open System.Drawing
    open System.Drawing.Imaging

    open ZXing
    open ZXing.Common

    let createCodeForText (text: string) ({ height = height; width = width})
        = async {
            let writer = 
              let w = new BarcodeWriter()
              w.Format <- BarcodeFormat.QR_CODE
              w.Options <- let o = new EncodingOptions()
                           o.Height <- height
                           o.Width <- width
                           o
              w

            return writer.Write text
          }
