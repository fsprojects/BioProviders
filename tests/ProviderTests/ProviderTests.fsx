#I "../../bin/Debug/lib/netstandard2.0"
#r "BioProviders.dll"
#r "nuget: BioFSharp"

open BioProviders
open BioFSharp

// Generate the Assembly Type
let [<Literal>] Species = "Staphylococcus lugdunensis"
let [<Literal>] Accession = "gca_001546615.1"

type AssemblyType = GenBankProvider<Species, Accession>


// Use the Assembly Type
let gbff = AssemblyType.Genome()

gbff.Sequence |> BioSeq.complement
