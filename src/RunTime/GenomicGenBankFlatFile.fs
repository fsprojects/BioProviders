namespace BioProviders

open System.IO
open System.Collections.Generic
open Bio.IO.GenBank
open BioProviders.Common

// --------------------------------------------------------------------------------------
// Genomic GenBank Flat File Sequence Representation.
// --------------------------------------------------------------------------------------

type IGenBankGenomicSequence = 
    inherit seq<byte>
    abstract Count : int64 with get
    abstract Item : int64 -> byte with get
    abstract GetComplementedSequence : unit -> IGenBankGenomicSequence
    abstract GetReversedSequence : unit -> IGenBankGenomicSequence
    abstract GetReverseComplementedSequence : unit -> IGenBankGenomicSequence
    abstract GetSubSequence : int64 -> int64 -> IGenBankGenomicSequence
    abstract ToString : unit -> string


type GenBankGenomeSequence (sequence:Bio.ISequence) = 
    
    // Standardise sequence (capitalise items).
    let _seq = sequence.GetComplementedSequence().GetComplementedSequence()
    
    // Implement GenBank sequence interface.
    interface IGenBankGenomicSequence with
        member __.Count = _seq.Count
        member __.Item with get index = _seq.Item(index)
        member __.GetComplementedSequence() = new GenBankGenomeSequence(_seq.GetComplementedSequence()) :> IGenBankGenomicSequence
        member __.GetReversedSequence() = new GenBankGenomeSequence(_seq.GetReversedSequence()) :> IGenBankGenomicSequence
        member __.GetReverseComplementedSequence() = new GenBankGenomeSequence(_seq.GetReverseComplementedSequence()) :> IGenBankGenomicSequence
        member __.GetSubSequence start length = new GenBankGenomeSequence(_seq.GetSubSequence(start, length)) :> IGenBankGenomicSequence
        member __.GetEnumerator(): IEnumerator<byte> = _seq.GetEnumerator()
        member __.GetEnumerator(): System.Collections.IEnumerator = _seq.GetEnumerator() :> System.Collections.IEnumerator
        override __.ToString() = _seq.ToString()


// --------------------------------------------------------------------------------------
// GenBank Flat File Metadata Representation.
// --------------------------------------------------------------------------------------

// Accession
type IGenBankAccession =
    abstract Primary : string option with get
    abstract Secondary : string list with get
    

type GenBankAccession(accession:Bio.IO.GenBank.GenBankAccession) = 
    interface IGenBankAccession with
        member __.Primary with get() = Helpers.parseOptionString accession.Primary
        member __.Secondary with get() = accession.Secondary |> Seq.toList


// Database Link
type GenBankCrossReferenceType = 
| Project
| TraceAssemblyArchive
| BioProject


type IGenBankCrossReferenceLink = 
    abstract Numbers : string list with get
    abstract Type : GenBankCrossReferenceType option with get


type GenBankCrossReferenceLink(crossReference:Bio.IO.GenBank.CrossReferenceLink) = 
    interface IGenBankCrossReferenceLink with
        member __.Numbers with get() = crossReference.Numbers |> Seq.toList
        member __.Type with get() = 
            match crossReference.Type with 
            | CrossReferenceType.None -> None
            | CrossReferenceType.Project -> Some GenBankCrossReferenceType.Project
            | CrossReferenceType.TraceAssemblyArchive -> Some GenBankCrossReferenceType.TraceAssemblyArchive
            | CrossReferenceType.BioProject -> Some GenBankCrossReferenceType.BioProject
            | _ -> invalidArg "CrossReferenceType" "An invalid CrossReferenceType has been encountered."


type IGenBankDbLinks = 
    inherit seq<IGenBankCrossReferenceLink>
    abstract Count : int with get
    abstract Item : int -> GenBankCrossReferenceLink with get


type GenBankDbLinks(database:IList<Bio.IO.GenBank.CrossReferenceLink>) =
    interface IGenBankDbLinks with
        member this.GetEnumerator(): IEnumerator<IGenBankCrossReferenceLink> = 
            (database |> Seq.map (fun x -> (new GenBankCrossReferenceLink(x)) :> IGenBankCrossReferenceLink) ).GetEnumerator()
        member this.GetEnumerator(): System.Collections.IEnumerator = 
            (database |> Seq.map (fun x -> (new GenBankCrossReferenceLink(x)) :> IGenBankCrossReferenceLink) ).GetEnumerator() :> System.Collections.IEnumerator
        member __.Count with get() = database.Count
        member __.Item with get index = new GenBankCrossReferenceLink(database.Item(index))


// Locus
type GenBankDivisionCode = 
| PRI
| ROD
| MAM
| VRT
| INV
| PLN
| BCT
| VRL
| PHG
| SYN
| UNA
| EST
| PAT
| STS
| GSS
| HTG
| HTC
| ENV
| CON


type GenBankMoleculeType = 
| Invalid
| NA
| DNA
| RNA
| TRNA
| RRNA
| MRNA
| URNA
| SnRNA
| SnoRNA
| Protein


type GenBankSequenceStrandType = 
| Single
| Double
| Mixed


type GenBankSequenceStrandTopology = 
| Linear
| Circular


type IGenBankLocus = 
    abstract Date : System.DateTime option with get
    abstract DivisionCode : GenBankDivisionCode option with get
    abstract MoleculeType : GenBankMoleculeType option with get
    abstract Name : string option with get
    abstract SequenceLength : int with get
    abstract SequenceType : string option with get
    abstract Strand : GenBankSequenceStrandType option with get
    abstract StrandTopology : GenBankSequenceStrandTopology option with get


type GenBankLocus(locus:Bio.IO.GenBank.GenBankLocusInfo) = 
    interface IGenBankLocus with
        member __.Date with get() = 
            match locus.Date = System.DateTime() with 
            | true -> None 
            | false -> Some locus.Date
        member __.DivisionCode with get() = 
            match locus.DivisionCode with
            | SequenceDivisionCode.None -> None
            | SequenceDivisionCode.PRI -> Some GenBankDivisionCode.PRI
            | SequenceDivisionCode.ROD -> Some GenBankDivisionCode.ROD
            | SequenceDivisionCode.MAM -> Some GenBankDivisionCode.MAM
            | SequenceDivisionCode.VRT -> Some GenBankDivisionCode.VRT
            | SequenceDivisionCode.INV -> Some GenBankDivisionCode.INV
            | SequenceDivisionCode.PLN -> Some GenBankDivisionCode.PLN
            | SequenceDivisionCode.BCT -> Some GenBankDivisionCode.BCT
            | SequenceDivisionCode.VRL -> Some GenBankDivisionCode.VRL
            | SequenceDivisionCode.PHG -> Some GenBankDivisionCode.PHG
            | SequenceDivisionCode.SYN -> Some GenBankDivisionCode.SYN
            | SequenceDivisionCode.UNA -> Some GenBankDivisionCode.UNA
            | SequenceDivisionCode.EST -> Some GenBankDivisionCode.EST
            | SequenceDivisionCode.PAT -> Some GenBankDivisionCode.PAT
            | SequenceDivisionCode.STS -> Some GenBankDivisionCode.STS
            | SequenceDivisionCode.GSS -> Some GenBankDivisionCode.GSS
            | SequenceDivisionCode.HTG -> Some GenBankDivisionCode.HTG
            | SequenceDivisionCode.HTC -> Some GenBankDivisionCode.HTC
            | SequenceDivisionCode.ENV -> Some GenBankDivisionCode.ENV
            | SequenceDivisionCode.CON -> Some GenBankDivisionCode.CON
            | _ -> invalidArg "DivisionCode" "An invalid DivisionCode has been encountered."
        member __.MoleculeType with get() = 
            match locus.MoleculeType with
            | MoleculeType.Invalid -> None
            | MoleculeType.NA -> Some GenBankMoleculeType.NA
            | MoleculeType.DNA -> Some GenBankMoleculeType.DNA
            | MoleculeType.RNA -> Some GenBankMoleculeType.RNA
            | MoleculeType.tRNA -> Some GenBankMoleculeType.TRNA
            | MoleculeType.rRNA -> Some GenBankMoleculeType.RRNA
            | MoleculeType.mRNA -> Some GenBankMoleculeType.MRNA
            | MoleculeType.uRNA -> Some GenBankMoleculeType.URNA
            | MoleculeType.snRNA -> Some GenBankMoleculeType.SnRNA
            | MoleculeType.snoRNA -> Some GenBankMoleculeType.SnoRNA
            | MoleculeType.Protein -> Some GenBankMoleculeType.Protein
            | _ -> invalidArg "MoleculeType" "An invalid MoleculeType has been encountered."
        member __.Name with get() = Helpers.parseOptionString locus.Name
        member __.SequenceLength with get() = locus.SequenceLength
        member __.SequenceType with get() = Helpers.parseOptionString locus.SequenceType
        member __.Strand with get() = 
            match locus.Strand with
            | SequenceStrandType.None -> None
            | SequenceStrandType.Single -> Some GenBankSequenceStrandType.Single
            | SequenceStrandType.Double -> Some GenBankSequenceStrandType.Double
            | SequenceStrandType.Mixed -> Some GenBankSequenceStrandType.Mixed
            | _ -> invalidArg "Strand" "An invalid Strand has been encountered."
        member __.StrandTopology with get() = 
            match locus.StrandTopology with
            | SequenceStrandTopology.None -> None
            | SequenceStrandTopology.Linear -> Some GenBankSequenceStrandTopology.Linear
            | SequenceStrandTopology.Circular -> Some GenBankSequenceStrandTopology.Circular
            | _ -> invalidArg "StrandTopology" "An invalid StrandTopology has been encountered."


// References
type IGenBankCitationReference = 
    abstract Authors : string option with get
    abstract Consortiums : string option with get
    abstract Journal : string option with get
    abstract Location : string option with get
    abstract Medline : string option with get
    abstract Number : int with get
    abstract PubMed : string option with get
    abstract Remarks : string option with get
    abstract Title : string option with get


and GenBankCitationReference (citation:Bio.IO.GenBank.CitationReference) =
    interface IGenBankCitationReference with
        member __.Authors = Helpers.parseOptionString citation.Authors
        member __.Consortiums = Helpers.parseOptionString citation.Consortiums
        member __.Journal = Helpers.parseOptionString citation.Journal
        member __.Location = Helpers.parseOptionString citation.Location
        member __.Medline = Helpers.parseOptionString citation.Medline
        member __.Number = citation.Number
        member __.PubMed = Helpers.parseOptionString  citation.PubMed
        member __.Remarks = Helpers.parseOptionString citation.Remarks
        member __.Title = Helpers.parseOptionString citation.Title


type IGenBankReferences = 
    inherit seq<IGenBankCitationReference>
    abstract Count : int with get
    abstract Item : int -> IGenBankCitationReference with get


type GenBankReferences (references:IList<Bio.IO.GenBank.CitationReference>) =
    interface IGenBankReferences with
        member __.GetEnumerator(): IEnumerator<IGenBankCitationReference> = 
            (references |> Seq.map (fun x -> (new GenBankCitationReference(x)) :> IGenBankCitationReference) ).GetEnumerator()
        member x.GetEnumerator(): System.Collections.IEnumerator = 
            (references |> Seq.map (fun x -> (new GenBankCitationReference(x)) :> IGenBankCitationReference) ).GetEnumerator() :> System.Collections.IEnumerator
        member __.Count with get() = references.Count
        member __.Item with get index = new GenBankCitationReference(references.Item(index)) :> IGenBankCitationReference


// Segment
and IGenBankSegment = 
    abstract Count : int with get
    abstract Current : int with get


and GenBankSegment(segment:Bio.IO.GenBank.SequenceSegment) =
    interface IGenBankSegment with
        member __.Count with get() = segment.Count
        member __.Current with get() = segment.Current


// Source
type IGenBankOrganismInfo = 
    abstract ClassLevels : string option with get
    abstract Genus : string option with get
    abstract Species : string option with get


type GenBankOrganismInfo(organism:Bio.IO.GenBank.OrganismInfo) =
    interface IGenBankOrganismInfo with
        member __.ClassLevels with get() = Helpers.parseOptionString organism.ClassLevels
        member __.Genus with get() = Helpers.parseOptionString organism.Genus
        member __.Species with get() = Helpers.parseOptionString organism.Species


type IGenBankSource = 
    abstract CommonName : string option with get
    abstract Organism : IGenBankOrganismInfo with get


type GenBankSource(source:Bio.IO.GenBank.SequenceSource) =
    interface IGenBankSource with
        member __.CommonName with get() = Helpers.parseOptionString source.CommonName
        member __.Organism with get() = new GenBankOrganismInfo(source.Organism) :> IGenBankOrganismInfo


// Version
type IGenBankVersion = 
    abstract Accession : string option with get
    abstract CompoundAccession : string option with get
    abstract GiNumber : string option with get
    abstract Version : string option with get


type GenBankVersion (version:Bio.IO.GenBank.GenBankVersion) =
    interface IGenBankVersion with
        member __.Accession with get() =  Helpers.parseOptionString version.Accession
        member __.CompoundAccession with get() = Helpers.parseOptionString version.CompoundAccession
        member __.GiNumber with get() = Helpers.parseOptionString version.GiNumber
        member __.Version with get() = Helpers.parseOptionString version.Version


// Grouped Metadata
type IGenBankGenomicMetadata = 
    abstract Accession : IGenBankAccession with get
    abstract BaseCount : string option with get
    abstract Comments : string list with get
    abstract Contig : string option with get
    abstract DbLinks : IGenBankDbLinks with get
    abstract DbSource : string option with get
    abstract Definition : string option with get
    abstract Keywords : string option with get
    abstract Locus : IGenBankLocus with get
    abstract Origin : string option with get
    abstract Primary : string option with get
    abstract References : IGenBankReferences with get
    abstract Segment : IGenBankSegment with get
    abstract Source : IGenBankSource with get
    abstract Version : IGenBankVersion with get


type GenBankGenomeMetadata (sequence:Bio.ISequence) = 

    let metadata = ( sequence.Metadata.Item("GenBank") :?> Bio.IO.GenBank.GenBankMetadata )
    
    // Implement GenBank sequence interface.
    interface IGenBankGenomicMetadata with
        member __.Accession with get() = new GenBankAccession(metadata.Accession) :> IGenBankAccession
        member __.BaseCount with get() = Helpers.parseOptionString metadata.BaseCount
        member __.Comments with get() = metadata.Comments |> Seq.toList
        member __.Contig with get() = Helpers.parseOptionString metadata.Contig
        member __.DbLinks with get() = new GenBankDbLinks(metadata.DbLinks) :> IGenBankDbLinks
        member __.DbSource with get() = Helpers.parseOptionString metadata.DbSource
        member __.Definition with get() = Helpers.parseOptionString metadata.Definition
        member __.Keywords with get() = Helpers.parseOptionString metadata.Keywords
        member __.Locus with get() = new GenBankLocus(metadata.Locus) :> IGenBankLocus
        member __.Origin with get() = Helpers.parseOptionString metadata.Origin
        member __.Primary with get() = Helpers.parseOptionString metadata.Primary
        member __.References with get() = new GenBankReferences(metadata.References) :> IGenBankReferences
        member __.Segment with get() = new GenBankSegment(metadata.Segment) :> IGenBankSegment
        member __.Source with get() = new GenBankSource(metadata.Source) :> IGenBankSource
        member __.Version with get() = new GenBankVersion(metadata.Version) :> IGenBankVersion


// --------------------------------------------------------------------------------------
// GenBank Flat File Representation.
// --------------------------------------------------------------------------------------

type IGenomicGenBankFlatFile = 
    abstract Sequence : IGenBankGenomicSequence
    abstract Metadata : IGenBankGenomicMetadata

type GenomicGenBankFlatFile (path:string) =

    // Create DotNet Bio ISequence for the GenBank Flat File.
    let sequence = 
        CacheAccess.loadFile path
        |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
        |> (new GenBankParser()).Parse
        |> Seq.cast<Bio.ISequence>
        |> Seq.head

    interface IGenomicGenBankFlatFile with
        member __.Sequence = new GenBankGenomeSequence(sequence)
        member __.Metadata = new GenBankGenomeMetadata(sequence)
