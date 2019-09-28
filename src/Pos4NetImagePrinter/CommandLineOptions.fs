module Pos4NetImagePrinter.CommandLineOptions

open System
open System.Text.RegularExpressions

type Path = string

type Printer = 
   | Default
   | ByName of string

type Width =
   | AsIs
   | Full
   | Size of int

type CommandLineOptions = {
   printer: Printer
   imageFilePath: Path
   width: Width
   label: string option
   }

let (|IgonoreCase|_|) pattern input =
   let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase)
   if m.Success then Some ()
   else None

let (|Int|_|) str =
  match System.Int32.TryParse(str) with
  | (true,int) -> Some(int)
  | _ -> None

let rec private parseCommandLineArgsRec args optionsSoFar = 
   match args with 
   | [] -> 
       optionsSoFar  

   | (IgonoreCase "/printer")::restArgs -> 
       match restArgs with
       | name::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with printer = ByName name } 
       | [] -> 
           raise <| new ArgumentNullException("printer", "No printer specified")

   | (IgonoreCase "/path")::restArgs -> 
       match restArgs with
       | path::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with imageFilePath = path } 
       | [] -> 
           raise <| new ArgumentNullException("path", "No image file path specified")

   | (IgonoreCase "/width")::restArgs -> 
      match restArgs with
      | (IgonoreCase "Full")::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = Full }
      | (IgonoreCase "AsIs")::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = AsIs }
      | (Int size)::restArgs ->
         parseCommandLineArgsRec restArgs { optionsSoFar with width = Size size }
      | arg::_restArgs ->
         raise <| new ArgumentOutOfRangeException("width", arg, "Invalid argument given for /width param. Use Full. AsIs or a number")
      | [] ->
           raise <| new ArgumentNullException("width", "No image width specified")

   | (IgonoreCase "/label")::restArgs -> 
       match restArgs with
       | label::restArgs -> 
           parseCommandLineArgsRec restArgs { optionsSoFar with label = Some label } 
       | [] -> 
           raise <| new ArgumentNullException("printer", "No printer specified")

   | _unknownArg::restArgs -> 
       parseCommandLineArgsRec restArgs optionsSoFar

let parseCommandLineArgs args = 
   let defaultOptions = {
      printer = Default
      imageFilePath = ""
      width = Full
      label = None
      }
   parseCommandLineArgsRec args defaultOptions 
