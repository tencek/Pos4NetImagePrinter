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
let main argv =

    let printer = PosPrinter.getReady "Microsoft PosPrinter Simulator"
    
    printer.ErrorEvent.Add errorHandler

    //if printer.CapRecBitmap then
    try
       printer.PrintNormal(PrinterStation.Receipt, "Ejchuchu")
       printer.PrintBitmap(PrinterStation.Receipt, @"C:\Users\Manžel\Pictures\SmileFace.jpg", 100, PosPrinter.PrinterBitmapCenter)
    with 
      ex ->
         errorHandler ex
    //else
      //()

    System.Threading.Thread.Sleep 1000

    printer.Release()

    0 // return an integer exit code
