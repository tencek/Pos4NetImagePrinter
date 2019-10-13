module Pos4NetImagePrinter.Pos4NetImagePrinter

open Microsoft.PointOfService
open Pos4NetImagePrinter.CommandLineOptions
open System.IO

let logFunc str =
   printfn "%s" str

let errorHandler error =
   logFunc <| "Error!"
   logFunc <| error.ToString()
   System.Console.Beep()

let deviceErrorHandler (eventArgs:DeviceErrorEventArgs) =
   errorHandler eventArgs
   System.Console.Beep()


[<EntryPoint>]
let main args =
   try
      let options = 
          List.ofArray args
          |> CommandLineOptions.Parser.parseCommandLineArgs

      let printer = 
         match options.printer with
         | Default -> PosPrinter.getDefaultDevice ()
         | ByName name -> PosPrinter.getDeviceByName name

      printer.ErrorEvent.Add errorHandler
      printer.MapMode <- MapMode.Dots

      if printer.CapRecBitmap then

         match options.label with
         | Custom label -> Some label
         | FileName -> Some <| Path.GetFileNameWithoutExtension(options.imageFilePath)
         | NoLabel -> None
         |> function
         |Some label ->
            printer.PrintNormal(PrinterStation.Receipt, "\027|4C" + label + "\x1B|1lF")
         | None ->
            ()

         let width = 
            match options.width with
            | Full -> printer.RecLineWidth
            | AsIs -> PosPrinter.PrinterBitmapAsIs
            | Pixels pixels -> pixels

         let tempFilePath = 
            match options.imageConversion with
            | NoConversion ->
                let tempFilePath = 
                   Path.GetTempFileName()
                   |> (fun path -> Path.ChangeExtension(path, Path.GetExtension(options.imageFilePath)))
                logFunc <| sprintf "Copying %s to %s" options.imageFilePath tempFilePath
                File.Copy(options.imageFilePath, tempFilePath)
                tempFilePath
            | toBmp ->
               let bmpBitsPerPixel =
                  match toBmp with
                  | ToBmp8Bit -> ImageConversion.Pixel8
                  | ToBmp16Bit -> ImageConversion.Pixel16
                  | ToBmp24Bit -> ImageConversion.Pixel24
                  | _default -> ImageConversion.Pixel32
               let tempBmpFilePath = 
                  Path.GetTempFileName()
                  |> (fun path -> Path.ChangeExtension(path, "bmp"))
               logFunc <| sprintf "Converting %s to %s (%A)" options.imageFilePath tempBmpFilePath bmpBitsPerPixel
               ImageConversion.convertToBpm options.imageFilePath tempBmpFilePath bmpBitsPerPixel
               tempBmpFilePath

         try
            printer.PrintBitmap(PrinterStation.Receipt, tempFilePath, width, PosPrinter.PrinterBitmapCenter)
         with
            ex ->
               errorHandler ex
         
         logFunc <| sprintf "Deleting %s" tempFilePath
         File.Delete(tempFilePath)

         if options.cut = CutAfter && printer.CapRecPaperCut then
            printer.CutPaper(PosPrinter.PrinterCutPaperFullCut)

         printer.Release()
      else
         failwithf "Printer %A does not support bitmap printing!" options.printer
      0
   with 
      ex ->
         errorHandler ex
         -1
