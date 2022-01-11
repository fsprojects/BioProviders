#I "../../bin/Debug/netstandard2.0"
#r "FSharp.Data.BioProviders.dll"

open GenBank

type AssemblyType = AssemblyProvider<"bacteria", "Staphylococcus_borealis", "GCA_003042555.1_ASM304255v1">

let genome = AssemblyType.GenomicGBFF()

genome.Metadata.Accession
