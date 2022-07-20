#I "../../bin/Debug/lib/netstandard2.0"
#r "BioProviders.dll"

open BioProviders

// Generate the Assembly Type
let [<Literal>] Species = "Staphylococcus lugdunensis"
let [<Literal>] Accession = "gca_001546615.1"

type AssemblyType = GenBankProvider<Species, Accession>


// Use the Assembly Type
let gbff = AssemblyType.Genome()

gbff.Sequence.GetSubSequence 0 20
|> fun x -> x.ToString()
|> printfn "The first 20 bases for the sequence are: %s"
