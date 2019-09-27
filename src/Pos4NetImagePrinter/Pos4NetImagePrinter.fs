open System
open System.Text

#if NETCOREAPP3_0
let uintToString (value: uint32) =
    Rune(ch).ToString()
let validValues (codePage: int) =
    let cp = Encoding.GetEncoding(codePage)
    let runes =
        seq { for u in UInt32.MinValue .. UInt32.MaxValue do if Rune.IsValid(u) then yield Rune(u) }
    let isCp r =
        let backRune =
            r.ToString()
            |> cp.GetBytes
            |> cp.GetString
            |> fun s -> Rune.GetRuneAt(s, 0)
        r = backRune
    seq { for r in runes do if isCp r then yield r }
    |> Seq.map (fun r -> 
        r.Value
        |> uint32
        )
#else
let inline isBmp (value: uint32) =
    value <= 0xFFFFu
let uintToString (value: uint32) =
    if isBmp value then
        (char value).ToString()
    else
        let highSurrogateCodePoint = char ((value + ((0xD800u - 0x40u) <<< 10)) >>> 10)
        let lowSurrogateCodePoint = char ((value &&& 0x3FFu) + 0xDC00u)
        sprintf "%c%c" highSurrogateCodePoint lowSurrogateCodePoint

let validValues (codePage: int) =
    let cp = Encoding.GetEncoding(codePage)
    let inline isValidUnicodeScalar (value: uint32) =
        ((value - 0x110000u) ^^^ 0xD800u) >= 0xFFEF0800u
    let validValues =
        seq { for u in UInt32.MinValue .. UInt32.MaxValue do if isValidUnicodeScalar u then yield u }
    let isCp v =
        let backRune =
            let isHighSurrogateCodePoint (value: uint32) =
                ((value - 0xD800u) <= (0xDBFFu - 0xD800u))

            uintToString v
            |> cp.GetBytes
            |> cp.GetString
            |> fun s -> 
                if (isHighSurrogateCodePoint (uint32 s.[0])) then
                    ((uint32 s.[0]) <<< 10) + (uint32 s.[1]) - ((0xD800u <<< 10) + 0xDC00u - (1u <<< 16))
                else
                    (uint32 s.[0])
        v = backRune
    seq { for v in validValues do if isCp v then yield v }
#endif



[<EntryPoint>]
let main argv =
    printfn "Hello!"
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
    
    let chs = validValues 950

    let lineDisplay = LineDisplayProcessor.getReady "RSLD.EASTASIA.COM1" 950  //"59772000"

    for ch in chs do
        let r = uintToString ch
        let tenChars = sprintf "%s%s%s%s%s%s%s%s%s%s" r r r r r r r r r r
        let position, pos2 = LineDisplayProcessor.testString lineDisplay tenChars
        if position <> 20 || pos2 <> 21 then
            printfn "%i %i 12345678901234567890" position pos2
            printfn "%i %i %s : %d %04X" position pos2 tenChars ch ch

    lineDisplay.Release()

    0 // return an integer exit code
