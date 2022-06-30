# BioProviders

[![Made with F#](https://img.shields.io/badge/Made%20with-FSharp-rgb(184,69,252).svg)](https://fsharp.org/) [![NuGet Status](https://img.shields.io/nuget/v/BioProviders.svg)](https://www.nuget.org/packages/BioProviders/) [![GitHub contributors](https://img.shields.io/github/contributors/AlexKenna/BioProviders.svg)](https://github.com/AlexKenna/BioProviders/graphs/contributors)

The F# BioProviders simplify programmatic access to genomic datasets.

This library provides strongly typed access to genomic sequences and data through a set of TypeProviders including the GenBankProvider. The ability to interact with the genomic sequences and data is provided through [.NET Bio](https://github.com/dotnetbio/bio) with goals to later incorporate [BioFSharp](https://github.com/CSBiology/BioFSharp) functionality.

## Build
[![Build Status](https://github.com/AlexKenna/BioProviders/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AlexKenna/BioProviders/actions) 

Install .Net SDK 3.0.100 or higher

Windows:
1. Run `dotnet tool restore`
2. Run `dotnet paket install`
3. Run `dotnet build ".\BioProviders.sln"`

## Documentation
More information can be found in the [documentation](https://github.com/AlexKenna/BioProviders/tree/main/docs).
