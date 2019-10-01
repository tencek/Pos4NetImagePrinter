module Pos4NetImagePrinter.ImageConversion

open SixLabors.ImageSharp
open SixLabors.ImageSharp.Formats.Bmp
open System

let convertToBpm (inputFilePath:string) (outputFilePath:string)=
   let bmpEncoder = new BmpEncoder()
   bmpEncoder.BitsPerPixel <- Nullable<BmpBitsPerPixel>(BmpBitsPerPixel.Pixel8)
   Image.Load(inputFilePath).Save(outputFilePath, bmpEncoder)
