(**
---
category: Type Providers
categoryindex: 1
index: 1
---
*)

(**

[![Script](../images/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../images/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# GenBank Type Provider

This article describes how to use the GenBank Type Provider to remotely access genomic data stored in the 
[GenBank](https://www.ncbi.nlm.nih.gov/genbank/) database. This Type Provider collects and parses the genomic data
for a specified organism and generates a static type containing its metadata and sequence. 

The GenBank Type Provider uses [.NET Bio](https://github.com/dotnetbio/bio) to parse the GenBank data files
and [BioFSharp](https://github.com/CSBiology/BioFSharp) to provide utilities for manipulating genomic sequences.

<br />
## Loading BioProviders Package

To load the GenBank Type Provider the BioProviders and BioFSharp packages must be referenced and opened:
*)

#r "nuget: BioProviders"

open BioProviders
open BioFSharp

(**
<br />
## GenBankProvider Example

The GenBank Type Provider will be demonstrated for [this GenBank assembly](https://www.ncbi.nlm.nih.gov/nuccore/CP012411) 
of the *Candidatus Carsonella ruddii* species. To create a typed representation of the assembly, two pieces of information
must be given to the Type Provider:

* Species name
* GenBank assembly accession

For this example, the species name is "Candidatus Carsonella ruddii" and the GenBank assembly accession is "GCA_001274515.1".
To find this information:

* Visit https://www.ncbi.nlm.nih.gov/
* Select "Assembly" from the dropdown next to the searchbar
* Search for the name of the species
* Find the organism from the list of search results

The species name and GenBank assembly accession can then be found under the corresponding search result. 

<div class="container-fluid" style="margin:15px 0px 15px 0px;">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10" id="anim-holder">
            <a id="lnk" href="../images/GenBank_Info.gif"><img id="anim" src="../images/GenBank_Info.gif" /></a>
        </div>
        <div class="span1"></div>
    </div>
</div>

Passing this information to the Type Provider generates the Assembly Type. The genomic data can then be extracted from the 
Assembly Type by invoking the Genome method. This is demonstrated below.
*)

// Define species name and GenBank assembly accession.
let [<Literal>] Species = "Candidatus Carsonella ruddii"
let [<Literal>] Accession = "GCA_001274515.1"

// Create GenBank assembly type.
type Ruddii = GenBankProvider<Species, Accession>

// Extract statically-typed genome data.
let genome = Ruddii.Genome()

(**
<br />
<br />
### Metadata

Each genome is accompanied by metadata describing the organism and sequence recorded in the assembly. This metadata can
be extracted using the Metadata field of the Genome Type created previously. The Metadata type is largely based on that
provided by [.NET Bio](http://dotnetbio.github.io/Help/html/319bf2e6-4fcf-9f93-586f-fc7ffcf04a83.htm), with modifications
made to be more idiomatic with F#.

An example of accessing and manipulating the GenBankProvider metadata is provided below:

*)

// Extract the metadata.
let metadata = genome.Metadata

// Display the metadata type.
metadata

(*** include-it ***)

(** 
  <br />
  ✅ Example - Accessing a field that is provided. 
*)

// Print definition if exists.
match metadata.Definition with
| Some definition -> printf "%s" definition
| None -> printf "No definition provided."

(*** include-output ***)

(** 
  <br /> 
  ❌ Example - Accessing a field that is not provided. 
*)

// Print definition if exists.
match metadata.DbSource with
| Some dbsource -> printf "%s" dbsource
| None -> printf "No database source provided."

(*** include-output ***)

(**
<br />
<br />
### Sequence

The genomic sequence for the organism can be extracted using the Sequence field of the Genome Type created previously.
This field provides a BioFSharp [BioSeq](https://csbiology.github.io/BioFSharp/reference/biofsharp-bioseq.html) containing
a series of [Nucleotides](https://csbiology.github.io/BioFSharp//reference/biofsharp-nucleotides-nucleotide.html). More
can be read about BioFSharp containers [here](https://csbiology.github.io/BioFSharp//BioCollections.html). 

An example of accessing and manipulating the GenBankProvider genomic sequence using BioFSharp is provided below:
*)

// Extract the BioFSharp BioSeq.
let sequence = genome.Sequence

// Display the sequence type.
sequence

(*** include-it ***)

// Take the complement, then transcribe and translate the coding strand.
sequence
|> BioSeq.complement
|> BioSeq.transcribeCodingStrand
|> BioSeq.translate 0

(*** include-it ***)


(**
<br />
## Wildcard Operators

Wildcard operators are supported in both the Species and Accession provided to the GenBankProvider. By using asterisks "\*"
at the end of a Species or Accession name, species or accessions starting with the provided pattern will be matched. 

For example, we can get all *Staphylococcus* species starting with the letter 'c' and assembly accesions starting with
'GCA_01':
*)

// Define species name and GenBank assembly accession using wildcards.
let [<Literal>] SpeciesPattern = "Staphylococcus c*"
let [<Literal>] AccessionPattern = "GCA_01*"

// Create GenBank type containing all species matching the species pattern.
type SpeciesCollection = GenBankProvider<SpeciesPattern, AccessionPattern>

// Select the species types.
type Capitis = SpeciesCollection.``Staphylococcus capitis``
type Cohnii = SpeciesCollection.``Staphylococcus cohnii``

// Select assemblies.
type Assembly1 = Capitis.``GCA_012926605.1``
type Assembly2 = Capitis.``GCA_015645205.1``
type Assembly3 = Cohnii.``GCA_013349225.1``
type Assembly4 = Cohnii.``GCA_014884245.1``

// Extract statically-typed genome data.
let data = Assembly1.Genome()

(**
The Accession parameter can also be omitted from the GenBankProvider. In this case, all assemblies for the given species will
be matched. For example:
*)

// Define species name.
let [<Literal>] SpeciesName = "Staphylococcus lugdunensis"

// Create GenBank type containing all assemblies for the species.
type Assemblies = GenBankProvider<SpeciesName>

// Select assemblies.
type Assembly = Assemblies.``GCA_001546615.1``
