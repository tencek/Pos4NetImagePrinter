﻿cd $PSScriptRoot
$files =  Get-ChildItem bmpsuite-2.5\g
foreach ($file in $files) 
{
   ..\Release\net472\publish\Pos4NetImagePrinter.exe /printer TM-T88VMU /path $file.FullName /label $file.Name /Width AsIs /NoCut
}