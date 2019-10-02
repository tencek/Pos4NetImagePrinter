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
         | Some label -> 
            printer.PrintNormal(PrinterStation.Receipt, "\027|4C" + label + "\x1B|1lF")
         | _ -> ()

         let width = 
            match options.width with
            | Full -> printer.RecLineWidth
            | AsIs -> PosPrinter.PrinterBitmapAsIs
            | Pixels pixels -> pixels

         let bitmapFilePath = 
            match options.imageConversion with
            | NoConversion ->
                options.imageFilePath
            | ToBmp8bit ->
                let bmpFilePath = Path.ChangeExtension(options.imageFilePath, "bmp")
                ImageConversion.convertToBpm options.imageFilePath bmpFilePath
                bmpFilePath

         try
            printer.PrintBitmap(PrinterStation.Receipt, bitmapFilePath, width, PosPrinter.PrinterBitmapCenter)
         with
            ex ->
               errorHandler ex

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
