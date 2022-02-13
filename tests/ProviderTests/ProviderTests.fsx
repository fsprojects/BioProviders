#I "../../bin/Debug/netstandard2.0"
#r "Bio.Core.dll"
#r "FSharp.Data.dll"

open BioProviders

//-------------------------------------------------------------//
//  Creating a type for a single Assembly.                     //
//-------------------------------------------------------------//

let [<Literal>] taxon = "bacteria"
let [<Literal>] species = "Staphylococcus_borealis"
let [<Literal>] assembly = "GCA_003042555.1_ASM304255v1"

type Assembly304255Type = GenBankProvider<taxon, species, assembly>

let genome304255 = Assembly304255Type.GenomicGBFF()

//-------------------------------------------------------------//
//  Creating a type for multiple Assemblies.                   //
//-------------------------------------------------------------//

let [<Literal>] assemblyWildcard = "GCA*"

type SpeciesType = GenBankProvider<taxon, species, assemblyWildcard>

type Assembly358083Type = SpeciesType.``GCA_003580835.1_ASM358083v1``

let genome358083 = Assembly358083Type.GenomicGBFF()

