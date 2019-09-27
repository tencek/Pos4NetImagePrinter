module LineDisplayProcessor

open Microsoft.PointOfService

let setUpDevice logicalDeviceName =
    let posExplorer = PosExplorer()
    let lineDisplayInfoMaybe = posExplorer.GetDevice("LineDisplay", logicalDeviceName)
    if isNull lineDisplayInfoMaybe then
        seq { for d in posExplorer.GetDevices("LineDisplay") -> d }
        |> Seq.tryFind (fun d -> d.ServiceObjectName = logicalDeviceName)
    else
        Some lineDisplayInfoMaybe
    |> Option.bind (fun lineDisplayInfo ->
        try
            let device = posExplorer.CreateInstance(lineDisplayInfo)
            let lineDisplay = device :?> LineDisplay
            lineDisplay.Open()
            Some lineDisplay
        with ex ->
            printfn "Problem: %A" ex
            None)
let rec ensure func =
    match func() with
    | Some v ->
        v
    | None ->
        System.Threading.Thread.Sleep 500
        printfn "Problem getting a line display"
        ensure func
        
let getReady logicalDeviceName codePage =
    let lineDisplay = ensure (fun () -> setUpDevice logicalDeviceName)
    lineDisplay.Claim(-1)
    lineDisplay.DeviceEnabled <- true
    if lineDisplay.CharacterSetList |> Array.contains codePage then
        printfn "Setting character set: %i" codePage
        lineDisplay.CharacterSet <- codePage
        printfn "New current character set: %i" lineDisplay.CharacterSet
    else
        printfn "LineDisplay doesn't support code page: %i" codePage
    if lineDisplay.CapMapCharacterSet then
        printfn "LineDisplay supports CapMapCharacterSet, using it"
        lineDisplay.MapCharacterSet <- true
    else
        printfn "LineDisplay doesn't support CapMapCharacterSet, we don't have support for converting ourselves, yet"
    lineDisplay.CursorUpdate <- true
    lineDisplay

let testString (lineDisplay: LineDisplay) value =
    lineDisplay.CursorColumn <- 0
    lineDisplay.DisplayTextAt(0, 0, value)
    let c =lineDisplay.CursorColumn
    lineDisplay.DisplayText("|")
    let c2 =lineDisplay.CursorColumn
    (c, c2)
    