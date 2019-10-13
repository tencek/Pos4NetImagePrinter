module Pos4NetImagePrinter.ImageConversion

open SixLabors.ImageSharp
open SixLabors.ImageSharp.Formats.Bmp
open System

type BmpBitsPerPixel = Pixel8 | Pixel16 | Pixel24 | Pixel32

let convertToBpm (inputFilePath:string) (outputFilePath:string) (bitsPerPixel:BmpBitsPerPixel)=
   let bmpEncoder = new BmpEncoder()
   let imageSharpBitsPerPixel =
      match bitsPerPixel with
      | Pixel8 -> Formats.Bmp.BmpBitsPerPixel.Pixel8
      | Pixel16 -> Formats.Bmp.BmpBitsPerPixel.Pixel16
      | Pixel24 -> Formats.Bmp.BmpBitsPerPixel.Pixel24
      | Pixel32 -> Formats.Bmp.BmpBitsPerPixel.Pixel32
   bmpEncoder.BitsPerPixel <- Nullable<Formats.Bmp.BmpBitsPerPixel>(imageSharpBitsPerPixel)
   Image.Load(inputFilePath).Save(outputFilePath, bmpEncoder)
