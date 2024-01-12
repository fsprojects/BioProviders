# F# BioProviders

[![Made with F#](https://img.shields.io/badge/Made%20with-FSharp-rgb(184,69,252).svg)](https://fsharp.org/) [![NuGet Status](https://img.shields.io/nuget/v/BioProviders.svg)](https://www.nuget.org/packages/BioProviders/) [![GitHub contributors](https://img.shields.io/github/contributors/AlexKenna/BioProviders.svg)](https://github.com/AlexKenna/BioProviders/graphs/contributors)

The F# BioProviders simplify programmatic access to bioinformatics data.

This library provides strongly-typed access to over 240 million genomic sequences through a set of Type Providers, including the GenBankProvider and RefSeqProvider. For more information, see the detailed [documentation](https://fsprojects.github.io/BioProviders/).

The BioProviders work by parsing genomic data files using the [.NET Bio](https://github.com/dotnetbio/bio) library, which are then represented using types from the [BioFSharp](https://github.com/CSBiology/BioFSharp) library.

## Example

Below, a simple example of finding the complement of the genomic sequence of a Staphylococcus lugdunensis assembly is provided.

```fsharp
#r "nuget: BioProviders"

open BioProviders
open BioFSharp

let [<Literal>] Species = "Staphylococcus lugdunensis"
let [<Literal>] Accession = "GCA_001546615.1"

let genome = GenBankProvider<Species, Accession>.Genome()

genome.Sequence |> BioSeq.complement
```

The above code produces the result:

```fsharp
BioSeq.BioSeq<Nucleotides.Nucleotide> = seq [C; T; A; C; ...]

```

## Building
[![Build Status](https://github.com/AlexKenna/BioProviders/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AlexKenna/BioProviders/actions)

To build the BioProviders package, perform the following steps:

* Install the .NET SDK specified in the global.json file
* `build.sh -t Build` or `build.cmd -t Build`

## Creating data files

BioProviders uses a set of data files generated from assembly lists from the NCBI FTP server for species and assembly lookup.

- To generate these files, run ```dotnet fsi DataFileGenerator.fsx```, to save the files to ```build\data```.
	- Approximately 1 GB is required due to the download size. They are deleted on process completion; use the argument ```-keepDownloads``` to keep them.
	- To save the files in the type provider's cache folder, use the argument ```-saveToCache```.
- By default, the package downloads files from this repository to ```AppData\Local\Temp\BioProviders```. To change this for your own version, change the URL in the file ```remote.txt``` in ```src\DesignTime```.

## Formatting

The BioProviders package code is formatted using [fantomas](https://fsprojects.github.io/fantomas/).

* To format the code, run `build.sh -t Format` or `build.cmd -t Format`
* To check formatting, run `build.sh -t CheckFormat` or `build.cmd -t CheckFormat`

## License

BioProviders is covered by the MIT license.

The package also uses:
- [BioFSharp](https://github.com/CSBiology/BioFSharp) - MIT license
- [.NET Bio](https://github.com/dotnetbio/bio) - Apache-2.0 license
- [FluentFTP](https://github.com/robinrodricks/FluentFTP) - MIT license
- [FSharp.Data](https://github.com/fsprojects/FSharp.Data/) - Apache-2.0 license

## Maintainers

Current maintainers are [Alex Kenna](https://github.com/AlexKenna), [Samuel Smith](https://github.com/n7581769) and [James Hogan](https://github.com/jamesmhogan).
