module Pos4NetImagePrinter.PosPrinter

open Microsoft.PointOfService
open FSharp.Collections

let private getPrinterDeviceInfoByName printerName = 
   PosExplorer().GetDevice("PosPrinter", printerName)
   |> Option.ofObj
   |> function
      | Some deviceInfo ->
         deviceInfo
      | None ->
         PosExplorer().GetDevices("PosPrinter")
         |> Seq.cast<DeviceInfo>
         |> Seq.tryFind (fun d -> d.ServiceObjectName = printerName)
         |> function
            | Some printerInfo -> 
               printerInfo
            | None ->
               failwithf "Printer device %s not found!" printerName

let private getDefaultPrinterDeviceInfo () =
   PosExplorer().GetDevices("PosPrinter")
   |> Seq.cast
   |> Seq.tryHead
   |> function
      | Some deviceInfo ->
         deviceInfo
      | None ->
         failwith "No printer device found!"

let private setUpDevice posPrinterInfo =
   let device = PosExplorer().CreateInstance(posPrinterInfo)
   let posPrinter = device :?> PosPrinter
   posPrinter.Open()
   posPrinter
        
let getDeviceByName logicalDeviceName =
    let posPrinter = getPrinterDeviceInfoByName logicalDeviceName |> setUpDevice
    posPrinter.Claim(-1)
    posPrinter.DeviceEnabled <- true
    posPrinter

let getDefaultDevice () =
   let posPrinter = getDefaultPrinterDeviceInfo () |> setUpDevice
   posPrinter.Claim(-1)
   posPrinter.DeviceEnabled <- true
   posPrinter
