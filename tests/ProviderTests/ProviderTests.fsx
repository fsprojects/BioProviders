#I "../../bin/Debug/lib/netstandard2.0"
#r "BioProviders.dll"

open BioProviders

// Generate the Assembly Type
let [<Literal>] Taxon = "bacteria"
let [<Literal>] Species = "Staphylococcus_lugdunensis"
let [<Literal>] Assembly = "GCA_000185485.1_ASM18548v1"

type AssemblyType = GenBankProvider<Taxon, Species, Assembly>


// Use the Assembly Type
let gbff = AssemblyType.Genome()

gbff.Sequence.GetSubSequence 0 20
|> fun x -> x.ToString()
|> printfn "The first 20 bases for the sequence are: %s"
