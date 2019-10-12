module Pos4NetImagePrinter.CommandLineOptions.Parser

open System
open System.Text.RegularExpressions

let private (|IgnoreCase|_|) pattern input =
   let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase)
   if m.Success then Some ()
   else None

let private (|Int|_|) str =
  match System.Int32.TryParse(str) with
  | (true,int) -> Some(int)
  | _ -> None

let rec private parseCommandLineArgsRec args optionsSoFar = 
   match args with 
   | [] -> 
       optionsSoFar  

   | (IgnoreCase "/printer")::restArgs -> 
       match restArgs with
       | name::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with printer = ByName name } 
       | [] -> 
           raise <| new ArgumentNullException("printer", "No printer specified")

   | (IgnoreCase "/path")::restArgs -> 
       match restArgs with
       | path::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with imageFilePath = path } 
       | [] -> 
           raise <| new ArgumentNullException("path", "No image file path specified")

   | (IgnoreCase "/width")::restArgs -> 
      match restArgs with
      | (IgnoreCase "Full")::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = Full }
      | (IgnoreCase "AsIs")::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = AsIs }
      | (Int size)::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = Pixels size }
      | arg::_restArgs ->
         raise <| new ArgumentOutOfRangeException("width", arg, "Invalid argument given for /width param. Use Full. AsIs or a number")
      | [] ->
           raise <| new ArgumentNullException("width", "No image width specified")

   | (IgnoreCase "/label")::restArgs -> 
       match restArgs with
       | label::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with label = Custom label } 
       | [] -> 
           raise <| new ArgumentNullException("printer", "No printer specified")

   | (IgnoreCase "/NoCut")::restArgs ->
      parseCommandLineArgsRec restArgs { optionsSoFar with cut = NoCut }

    | (IgnoreCase "/NoConversion")::restArgs ->
       parseCommandLineArgsRec restArgs { optionsSoFar with imageConversion = NoConversion }

   | _unknownArg::restArgs -> 
       parseCommandLineArgsRec restArgs optionsSoFar

let parseCommandLineArgs args = 
   let defaultOptions = {
      printer = Default
      imageFilePath = ""
      width = Full
      label = NoLabel
      cut = CutAfter
      imageConversion = ToBmp8bit
      }
   parseCommandLineArgsRec args defaultOptions 

