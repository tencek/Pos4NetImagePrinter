module Pos4NetImagePrinter.Pos4NetImagePrinter

open Microsoft.PointOfService
open Pos4NetImagePrinter.CommandLineOptions

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
            printer.PrintNormal(PrinterStation.Receipt, "\027|4C\027|rvC" + label)
         | _ -> ()

         let width = 
            match options.width with
            | Full -> printer.RecLineWidth
            | AsIs -> PosPrinter.PrinterBitmapAsIs
            | Pixels pixels -> pixels

         printer.PrintBitmap(PrinterStation.Receipt, options.imageFilePath, width, PosPrinter.PrinterBitmapCenter)
         
         if printer.CapRecPaperCut then
            printer.CutPaper(PosPrinter.PrinterCutPaperFullCut)

         printer.Release()
      else
         failwithf "Printer %A does not support bitmap printing!" options.printer
      0
   with 
      ex ->
         errorHandler ex
         -1
