namespace Pos4NetImagePrinter.CommandLineOptions

type Printer = 
   | Default
   | ByName of string

type Path = string

type Width =
   | AsIs
   | Full
   | Pixels of int

type Label = string

type CutOption = CutAfter | NoCut

type ImageConversion = NoConversion | ToBmp8bit

type CommandLineOptions = {
   printer: Printer
   imageFilePath: Path
   width: Width
   label: Label option
   cut: CutOption
   imageConversion: ImageConversion
   }

