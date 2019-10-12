namespace Pos4NetImagePrinter.CommandLineOptions

type Printer = 
   | Default
   | ByName of string

type Path = string

type Width =
   | AsIs
   | Full
   | Pixels of int

type Label = 
   | NoLabel
   | FileName
   | Custom of string

type CutOption = CutAfter | NoCut

type ImageConversion = NoConversion | ToBmp8bit

type CommandLineOptions = {
   printer: Printer
   imageFilePath: Path
   width: Width
   label: Label
   cut: CutOption
   imageConversion: ImageConversion
   }

