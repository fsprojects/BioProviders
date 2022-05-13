#I "../../bin/Debug/netstandard2.0"
#r "BioProviders.dll"

open BioProviders

// --------------------------------------------------------------------------------------
// Creating a type for a single Assembly.   
// --------------------------------------------------------------------------------------

let [<Literal>] taxon = "bacteria"
let [<Literal>] species = "Staphylococcus_lugdunensis"
let [<Literal>] assembly = "GCA_000025085.1_ASM2508v1"

type Assembly = GenBankProvider<taxon, species, assembly>

let gbff = Assembly.Genome()


// --------------------------------------------------------------------------------------
// Creating a type for multiple Assemblies.  
// --------------------------------------------------------------------------------------

// Match all assemblies starting with GCA_013...
let [<Literal>] assemblyWildcard = "GCA_000*"

type Assemblies = GenBankProvider<taxon, species, assemblyWildcard>
type AssemblyType = Assemblies.``GCA_000025085.1_ASM2508v1``

let genbankFlatFile = AssemblyType.Genome()

