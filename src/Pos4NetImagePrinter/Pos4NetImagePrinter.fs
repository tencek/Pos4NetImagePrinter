module Pos4NetImagePrinter.Pos4NetImagePrinter

open System
open System.Text
open Microsoft.PointOfService


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
          |> CommandLineOptions.parseCommandLineArgs

      let printer = PosPrinter.getReady "Microsoft PosPrinter Simulator"
      printer.ErrorEvent.Add errorHandler

      if printer.CapRecBitmap then
         printer.PrintNormal(PrinterStation.Receipt, "Ejchuchu")
         printer.PrintBitmap(PrinterStation.Receipt, @"C:\Users\Manžel\Pictures\SmileFace.jpg", 100, PosPrinter.PrinterBitmapCenter)
         System.Threading.Thread.Sleep 1000
         printer.Release()
      else
         failwithf "Printer %A does not support bitmap printing!" options.printer

   with 
      ex ->
         errorHandler ex


   0 // return an integer exit code
