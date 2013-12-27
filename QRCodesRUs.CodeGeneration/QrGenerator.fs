namespace QRCodesRUs.CodeGeneration

// This is just a wrapper around ZXing
module QrGenerator =
    open System
    open System.IO
    open System.Drawing
    open System.Drawing.Imaging

    open ZXing
    open ZXing.Common

    let createCodeForText (text: string) 
        = let writer = 
              let w = new BarcodeWriter()
              w.Format <- BarcodeFormat.QR_CODE
              w.Options <- let o = new EncodingOptions()
                           o.Height <- 300
                           o.Width <- 300
                           o
              w
          writer.Write text

    let writeBitmapToFile (file: Stream) (image: Bitmap) = image.Save(file, ImageFormat.Png)
