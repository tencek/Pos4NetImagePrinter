module Pos4NetImagePrinter.PosPrinter

open Microsoft.PointOfService

let private getPrinterDeviceInfoByName deviceName = 
   let posExplorer = PosExplorer()
   let posPrinterInfoMaybe = posExplorer.GetDevice("PosPrinter", deviceName)
   if isNull posPrinterInfoMaybe then
       seq { for d in posExplorer.GetDevices("PosPrinter") -> d }
       |> Seq.tryFind (fun d -> d.ServiceObjectName = deviceName)
   else
       Some posPrinterInfoMaybe

let private getDefaultPrinterDeviceInfo () =
   let posExplorer = PosExplorer()
   seq { for d in posExplorer.GetDevices("PosPrinter") -> d }
   |> Seq.tryFind (fun _d -> true)

let private setUpDevice deviceName =
    getPrinterDeviceInfoByName deviceName
    |> Option.bind (fun posPrinterInfo ->
        try
            let device = PosExplorer().CreateInstance(posPrinterInfo)
            let posPrinter = device :?> PosPrinter
            posPrinter.Open()
            Some posPrinter
        with ex ->
            printfn "Problem: %A" ex
            None)

let rec private ensure func =
    match func() with
    | Some v ->
        v
    | None ->
        System.Threading.Thread.Sleep 500
        printfn "Problem getting a device"
        ensure func
        
let getReady logicalDeviceName =
    let posPrinter = ensure (fun () -> setUpDevice logicalDeviceName)
    posPrinter.Claim(-1)
    posPrinter.DeviceEnabled <- true
    posPrinter

    