namespace Pos4NetImagePrinter.CommandLineOptions

type Printer = 
   | Default
   | ByName of string

type Path = string

type Width =
   | AsIs
   | Full
   | Pixels of int

type Labels = string list

type CutOption = CutAfter | NoCut

type ImageConversion = NoConversion | ToBmp8Bit | ToBmp16Bit | ToBmp24Bit | ToBmp32Bit

type CommandLineOptions = {
   printer: Printer
   imageFilePath: Path
   width: Width
   labels: Labels
   cut: CutOption
   imageConversion: ImageConversion
   }

