#I "../../bin/Debug/netstandard2.0"
#r "FSharp.Data.dll"

open BioProviders

// --------------------------------------------------------------------------------------
// Creating a type for a single Assembly.   
// --------------------------------------------------------------------------------------

let [<Literal>] taxon = "bacteria"
let [<Literal>] species = "Staphylococcus_borealis"
let [<Literal>] assembly = "GCA_003042555.1_ASM304255v1"

type Assembly = GenBankProvider<taxon, species, assembly>

let gbff = Assembly.GenBankFlatFile()


// --------------------------------------------------------------------------------------
// Creating a type for multiple Assemblies.  
// --------------------------------------------------------------------------------------

// Match all assemblies starting with GCA_013...
let [<Literal>] assemblyWildcard = "GCA_013*"

type Assemblies = GenBankProvider<taxon, species, assemblyWildcard>
type AssemblyType = Assemblies.``GCA_013345165.1_ASM1334516v1``

let genbankFlatFile = AssemblyType.GenBankFlatFile()

