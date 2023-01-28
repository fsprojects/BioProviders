# F# BioProviders

[![Made with F#](https://img.shields.io/badge/Made%20with-FSharp-rgb(184,69,252).svg)](https://fsharp.org/) [![NuGet Status](https://img.shields.io/nuget/v/BioProviders.svg)](https://www.nuget.org/packages/BioProviders/) [![GitHub contributors](https://img.shields.io/github/contributors/AlexKenna/BioProviders.svg)](https://github.com/AlexKenna/BioProviders/graphs/contributors)

The F# BioProviders simplify programmatic access to bioinformatics data.

This library provides strongly-typed access to over 240 million genomic sequences through a set of Type Providers, including the GenBankProvider. For more information, see the detailed [documentation](https://fsprojects.github.io/BioProviders/).

The BioProvidrs work by parsing genomic data files using the [.NET Bio](https://github.com/dotnetbio/bio) library, which are then represented using types from the [BioFSharp](https://github.com/CSBiology/BioFSharp) library.

## Example

Below, a simple example of finding the complement of the genomic sequence of a Staphylococcus lugdunensis assembly is provided.

```fsharp
#r "nuget:BioProviders"
#r "nuget:BioFSharp"

open BioProviders
open BioFSharp

let [<Literal>] Species = "Staphylococcus lugdunensis"
let [<Literal>] Accession = "GCA_001546615.1"

let genome = GenBankProvider<Species, Accession>.Genome()

genome.Sequence |> BioSeq.complement
```

## Building
[![Build Status](https://github.com/AlexKenna/BioProviders/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AlexKenna/BioProviders/actions) 

To build the BioProviders package, perform the following steps:

* Install the .NET SDK specified in the global.json file
* `build.sh -t Build` or `build.cmd -t Build`

## Formatting

The BioProviders package code is formatted using [fantomas](https://fsprojects.github.io/fantomas/). 

* To format the code, run `build.sh -t Format` 
* To check formatting, run `build.sh -t CheckFormat` 

## License

BioProviders is covered by the MIT license.

The package also uses [BioFSharp](https://github.com/CSBiology/BioFSharp) and [.NET Bio](https://github.com/dotnetbio/bio), which use the MIT and Apache-2.0 licenses, respectively.
