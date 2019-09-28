namespace Pos4NetImagePrinter.CommandLineOptions

type Path = string

type Printer = 
   | Default
   | ByName of string

type Width =
   | AsIs
   | Full
   | Pixels of int

type CommandLineOptions = {
   printer: Printer
   imageFilePath: Path
   width: Width
   label: string option
   }

