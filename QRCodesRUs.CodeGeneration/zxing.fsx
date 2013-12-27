open System
open System.IO
open System.Drawing.Imaging

#I @"D:\Dropbox\masters\508\QRCodesRUs\QRCodesRUs.CodeGeneration\bin\Debug"

#r "zxing.dll"
#r "zxing.presentation.dll"
#r "System.Drawing.dll"
#r "System.Numerics.dll"

#load "QrGenerator.fs"
open QRCodesRUs.CodeGeneration

let writeTo (path: string) (text: string)
    = use file = File.OpenWrite(path)
      QrGenerator.createCodeForText text |> QrGenerator.writeBitmapToFile file