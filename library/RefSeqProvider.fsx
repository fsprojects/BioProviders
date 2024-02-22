(**
[![Script](../img/badge-script.svg)](https://fsprojects.github.io/BioProviders//library/RefSeqProvider.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](https://fsprojects.github.io/BioProviders//library/RefSeqProvider.ipynb)

# RefSeq Type Provider

This article describes how to use the RefSeq Type Provider to remotely access genomic data stored in the
[RefSeq](https://www.ncbi.nlm.nih.gov/genbank/) database. This Type Provider collects and parses the genomic data
for a specified organism and generates a static type containing its metadata and sequence.

The RefSeq Type Provider uses [.NET Bio](https://github.com/dotnetbio/bio) to parse the RefSeq data files
and [BioFSharp](https://github.com/CSBiology/BioFSharp) to provide utilities for manipulating genomic sequences.

## Loading BioProviders Package

To load the RefSeq Type Provider, a script can use the NuGet syntax to reference the BioProviders package, shown below.

You can optionally include the BioFSharp package. While it's not required to use the basic BioProviders functions, it can be used to explore the metadata of the provided types, as shown in a later example.

*)
#r "nuget: BioProviders"
#r "nuget: BioFSharp"
(**
If creating an F# library or application, BioProviders can be added as a package reference. You can use your IDE for this, or use the `dotnet add package BioProviders` command in your project folder from the command line.

BioProviders can then be used in your script or code by using an open command. Opening its dependencies should not be required. (BioFSharp is loaded for future examples.)

*)
open BioProviders
open BioFSharp
(**
## RefSeqProvider Example

The RefSeq Type Provider will be demonstrated for [this RefSeq assembly](https://www.ncbi.nlm.nih.gov/datasets/genome/GCF_001224225.1/)
of the **Staphylococcus borealis** species. To create a typed representation of the assembly, two pieces of information
must be given to the Type Provider:

* Species name

* RefSeq assembly accession

For this example, the species name is "Staphylococcus borealis" and the RefSeq assembly accession is "GCF_001224225.1".
To find this information:

* Visit [https://www.ncbi.nlm.nih.gov/datasets/](https://www.ncbi.nlm.nih.gov/datasets/)

* Search for the name of the species

* Select to view all genones of the species

You can then select the assembly's RefSeq (as well as GenBank) accession from the list that appears.

![Animation of findng a RefSeq assembly accession on NCBI.](../img/RefSeq_Info.gif).

Passing this information to the Type Provider generates the Assembly Type. The genomic data can then be extracted from the
Assembly Type by invoking the Genome method. This is demonstrated below.

*)
// Define species name and RefSeq assembly accession.
let [<Literal>] Species = "Staphylococcus borealis"
let [<Literal>] Accession = "GCF_001224225.1"

// Create RefSeq assembly type.
type Borealis = RefSeqProvider<Species, Accession>

// Extract statically-typed genome data.
let genome = Borealis.Genome()
(**
### Metadata

Each genome is accompanied by metadata describing the organism and sequence recorded in the assembly. This metadata can
be extracted using the Metadata field of the Genome Type created previously. The Metadata type is largely based on that
provided by [.NET Bio](http://dotnetbio.github.io/Help/html/319bf2e6-4fcf-9f93-586f-fc7ffcf04a83.htm), with modifications
made to be more idiomatic with F#.

Below is an example of how the raw metadata type can be retrieved and displayed:

*)
// Extract the metadata.
let metadata = genome.Metadata

// Display the metadata type.
printf "%A" metadata(* output: 
{ Locus = Some { Date = Some 4/27/2023 12:00:00 AM
                 DivisionCode = Some CON
                 MoleculeType = Some DNA
                 Name = Some "NZ_CUEE01000001"
                 SequenceLength = 563044
                 SequenceType = Some "bp"
                 Strand = None
                 StrandTopology = Some Linear }
  Definition =
   Some "Staphylococcus borealis strain 51-48, whole genome shotgun sequence."
  Accession = Some { Primary = Some "NZ_CUEE01000001"
                     Secondary = Some ["NZ_CUEE01000000"] }
  Version = Some { Accession = Some "NZ_CUEE01000001"
                   CompoundAccession = Some "NZ_CUEE01000001.1"
                   GiNumber = None
                   Version = Some "1" }
  DbLinks =
   Some
     [{ Numbers = Some [" PRJNA224116"]
        Type = Some BioProject }; { Numbers = Some [" SAMEA1035138"]
                                    Type = None };
      { Numbers = Some [" GCF_001224225.1"]
        Type = None }]
  DbSource = None
  Keywords = Some "WGS; RefSeq."
  Primary = None
  Source =
   Some
     { CommonName = Some "Staphylococcus borealis"
       Organism =
        Some
          { ClassLevels =
             Some
               "Bacteria; Bacillota; Bacilli; Bacillales; Staphylococcaceae; Staphylococcus."
            Genus = Some "Staphylococcus"
            Species = Some "borealis" } }
  References =
   Some
     [{ Authors = Some "Informatics,Pathogen."
        Consortiums = None
        Journal =
         Some
           "Submitted (10-MAR-2015) SC, Wellcome Trust Sanger Institute, CB10 1SA, United Kingdom"
        Location = None
        Medline = None
        Number = 1
        PubMed = None
        Remarks = None
        Title = Some "Direct Submission" }]
  Comments =
   Some
     ["REFSEQ INFORMATION: The reference sequence is identical to
CUEE01000001.1.
The annotation was added by the NCBI Prokaryotic Genome Annotation
Pipeline (PGAP). Information about PGAP can be found here:
https://www.ncbi.nlm.nih.gov/genome/annotation_prok/

##Genome-Annotation-Data-START##
Annotation Provider               :: NCBI RefSeq
Annotation Date                   :: 04/27/2023 01:28:26
Annotation Pipeline               :: NCBI Prokaryotic Genome
Annotation Pipeline (PGAP)
Annotation Method                 :: Best-placed reference protein
set; GeneMarkS-2+
Annotation Software revision      :: 6.5
Features Annotated                :: Gene; CDS; rRNA; tRNA; ncRNA
Genes (total)                     :: 2,650
CDSs (total)                      :: 2,584
Genes (coding)                    :: 2,507
CDSs (with protein)               :: 2,507
Genes (RNA)                       :: 66
rRNAs                             :: 2, 1, 1 (5S, 16S, 23S)
complete rRNAs                    :: 2, 1, 1 (5S, 16S, 23S)
tRNAs                             :: 58
ncRNAs                            :: 4
Pseudo Genes (total)              :: 77
CDSs (without protein)            :: 77
Pseudo Genes (ambiguous residues) :: 0 of 77
Pseudo Genes (frameshifted)       :: 29 of 77
Pseudo Genes (incomplete)         :: 49 of 77
Pseudo Genes (internal stop)      :: 29 of 77
Pseudo Genes (multiple problems)  :: 23 of 77
##Genome-Annotation-Data-END##"]
  Contig = Some "join(CUEE01000001.1:1..563044)"
  Segment = None
  Origin = None }*)
(**
The metadata type consists of many fields, though not all fields of the metadata exist for all assemblies. Therefore, they are provided as option types, on which a match expression can be used. Below are examples of accessing fields from the example assembly.
✅ Example - Accessing a field that is provided.

*)
// Print definition if exists.
match metadata.Definition with
| Some definition -> printf "%s" definition
| None -> printf "No definition provided."(* output: 
Staphylococcus borealis strain 51-48, whole genome shotgun sequence.*)
(**
❌ Example - Accessing a field that is not provided.

*)
// Print database source if exists.
match metadata.DbSource with
| Some dbsource -> printf "%s" dbsource
| None -> printf "No database source provided."(* output: 
No database source provided.*)
(**
### Sequence

The genomic sequence for the organism can be extracted using the Sequence field of the Genome Type created previously.
This field provides a BioFSharp [BioSeq](https://csbiology.github.io/BioFSharp/reference/biofsharp-bioseq.html) containing
a series of [Nucleotides](https://csbiology.github.io/BioFSharp//reference/biofsharp-nucleotides-nucleotide.html). More
can be read about BioFSharp containers [here](https://csbiology.github.io/BioFSharp//BioCollections.html).

An example of accessing and manipulating the RefSeqProvider genomic sequence using BioFSharp is provided below:

*)
// Extract the BioFSharp BioSeq.
let sequence = genome.Sequence

// Display the sequence type.
printf "%A" sequence(* output: 
seq [C; A; G; G; ...]*)
// Take the complement, then transcribe and translate the coding strand.
sequence
|> BioSeq.complement
|> BioSeq.transcribeCodingStrand
|> BioSeq.translate 0(* output: 
seq [Val; Leu; Val; Ter; ...]*)
(**
## Wildcard Operators

Wildcard operators are supported in both the Species and Accession provided to the RefSeqProvider. By using asterisks "*"
at the end of a Species or Accession name, species or accessions starting with the provided pattern will be matched.

For example, we can get all **Staphylococcus** species starting with the letter 'c' and assembly accesions starting with
'GCF_01':

*)
// Define species name and RefSeq assembly accession using wildcards.
let [<Literal>] SpeciesPattern = "Staphylococcus c*"
let [<Literal>] AccessionPattern = "GCF_01*"

// Create RefSeq type containing all species matching the species pattern.
type SpeciesCollection = RefSeqProvider<SpeciesPattern, AccessionPattern>

// Select the species types.
type Capitis = SpeciesCollection.``Staphylococcus capitis``
type Cohnii = SpeciesCollection.``Staphylococcus cohnii``

// Select assemblies.
type Assembly1 = Capitis.``GCF_012926605.1``
type Assembly2 = Capitis.``GCF_012926635.1``
type Assembly3 = Cohnii.``GCF_013602215.1``
type Assembly4 = Cohnii.``GCF_013602265.1``

// Extract statically-typed genome data.
let data = Assembly1.Genome()

// Show the assembly's definition.
match data.Metadata.Definition with
| Some definition -> printf "%s" definition
| None -> printf "No definition provided."(* output: 
Staphylococcus capitis strain 18-857 NODE_1, whole genome shotgun sequence.*)
(**
The Accession parameter can also be omitted from the RefSeqProvider. In this case, all assemblies for the given species will
be matched. For example:

*)
// Define species name.
let [<Literal>] SpeciesName = "Staphylococcus lugdunensis"

// Create RefSeq type containing all assemblies for the species.
type Assemblies = RefSeqProvider<SpeciesName>

// Select assemblies.
type Assembly = Assemblies.``GCF_001546615.1``

// Show the assembly's primary accession.
match (Assembly.Genome()).Metadata.Accession with
| Some accession -> match accession.Primary with
                    | Some primary -> printf "%s" primary
                    | None -> printf "No primary accession provided."
| None -> printf "No accession provided."(* output: 
NZ_KQ957361*)

