namespace BioProviders.RunTime

open System.IO
open System.Collections.Generic
open Bio.IO.GenBank
open Bio.Algorithms.StringSearch
open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Flat File Type Representation.
// --------------------------------------------------------------------------------------

type GenBankFlatFile (path:string) =
    let cache = new Cache()
    
    let sequence = 
        (cache :> ICache).LoadFile(path)
        |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
        |> (new GenBankParser()).Parse
        |> Seq.cast<Bio.ISequence>
        |> Seq.head
    
    member __.Metadata with get() = new GenBankMetadata(sequence)
    member __.Sequence with get() = new GenBankSequence(sequence)


// --------------------------------------------------------------------------------------
// GenBank Flat File MetaData Type Representation.
// --------------------------------------------------------------------------------------

and GenBankSequence (sequence:Bio.ISequence) = 

    // Standardise sequence (capitalise items).
    let _seq = sequence.GetComplementedSequence().GetComplementedSequence()

    // Add functions and members provided by DotNet Bio.
    member __.Count = _seq.Count
    member __.Item with get index = _seq.Item(index)
    member __.GetComplementedSequence() = new GenBankSequence(_seq.GetComplementedSequence())
    member __.GetReversedSequence() = new GenBankSequence(_seq.GetReversedSequence())
    member __.GetReverseComplementedSequence() = new GenBankSequence(_seq.GetReverseComplementedSequence())
    member __.GetSubSequence start length = new GenBankSequence(_seq.GetSubSequence(start, length))
    member __.IndexOfNonGap startPos = _seq.IndexOfNonGap startPos
    member __.IndexOfNonGap() = _seq.IndexOfNonGap()
    member __.LastIndexOfNonGap endPos = _seq.LastIndexOfNonGap endPos
    member __.LastIndexOfNonGap() = _seq.LastIndexOfNonGap()
    member __.GetEnumerator() = _seq.GetEnumerator()
    override __.ToString() = _seq.ToString()

    // Search sequence for a pattern.
    member __.Search(searchPattern:string) = 
        let searcher = new BoyerMoore()
        searcher.FindMatch(_seq, searchPattern) |> Seq.toList

    member __.Search(searchPatterns:List<string>) = 
        let searcher = new BoyerMoore()
        let patterns = ResizeArray<string> searchPatterns
        searcher.FindMatch(_seq, patterns)
        |> Seq.map (fun pair -> new KeyValuePair<string, int list>(pair.Key, pair.Value |> Seq.toList))
        |> Seq.map (|KeyValue|) 
        |> Map.ofSeq

    

// --------------------------------------------------------------------------------------
// GenBank Flat File Sequence Type Representation.
// --------------------------------------------------------------------------------------

and GenBankMetadata (sequence:Bio.ISequence) = 
    
    let metadata = ( sequence.Metadata.Item("GenBank") :?> Bio.IO.GenBank.GenBankMetadata )
    
    member __.BaseCount with get() = metadata.BaseCount
    member __.Comments with get() = metadata.Comments |> Seq.toList
    member __.Contig with get() = metadata.Contig
    member __.DbSource with get() = metadata.DbSource
    member __.Definition with get() = metadata.Definition
    member __.Keywords with get() = metadata.Keywords
    member __.Origin with get() = metadata.Origin
    member __.Primary with get() = metadata.Primary
    member __.Source with get() = new GenBankMetadataSource(metadata.Source)
    member __.Version with get() = new GenBankMetadataVersion(metadata.Version)
    member __.Accession with get() = new GenBankMetadataAccession(metadata.Accession)
    member __.Project with get() = new GenBankMetadataProject(metadata.Project)
    member __.References with get() = new GenBankMetadataReferences(metadata.References)
    member __.Segment with get() = new GenBankMetadataSegment(metadata.Segment)
    member __.Locus with get() = new GenBankMetadataLocus(metadata.Locus)
    member __.DbLinks with get() = new GenBankMetadataDbLink(metadata.DbLinks)
    member __.Features with get() = new GenBankMetadataFeatures(metadata.Features)


// References
and GenBankMetadataReferences (references:IList<CitationReference>) =
    member __.Count with get() = references.Count
    member __.Item with get index = new GenBankMetadataCitationReference(references.Item(index))

and GenBankMetadataCitationReference (citation:CitationReference) =
    member __.Authors = citation.Authors
    member __.Consortiums = citation.Consortiums
    member __.Journal = citation.Journal
    member __.Location = citation.Location
    member __.Medline = citation.Medline
    member __.Number = citation.Number
    member __.PubMed = citation.PubMed
    member __.Remarks = citation.Remarks
    member __.Title = citation.Title


// Version
and GenBankMetadataVersion (version:GenBankVersion) =
    member __.Accession with get() = version.Accession
    member __.CompoundAccession with get() = version.CompoundAccession
    member __.GiNumber with get() = version.GiNumber
    member __.Version with get() = version.Version


// Accession
and GenBankMetadataAccession(accession:GenBankAccession) = 
    member __.Primary with get() = accession.Primary
    member __.Secondary with get() = accession.Secondary


// Project
and GenBankMetadataProject(project:ProjectIdentifier) =
    member __.Name with get() = project.Name
    member __.Numbers with get() = project.Numbers |> Seq.toList


// Source
and GenBankMetadataSource(source:SequenceSource) =
    member __.CommonName with get() = source.CommonName
    member __.Organism with get() = new GenBankMetadataOrganismInfo(source.Organism)

and GenBankMetadataOrganismInfo(organism:OrganismInfo) =
    member __.ClassLevels with get() = organism.ClassLevels
    member __.Genus with get() = organism.Genus
    member __.Species with get() = organism.Species


// Database Link
and GenBankMetadataDbLink(database:IList<CrossReferenceLink>) =
    member __.Count with get() = database.Count
    member __.Item with get index = new GenBankMetadataCrossReferenceLink(database.Item(index))

and GenBankMetadataCrossReferenceLink(crossReference:CrossReferenceLink) = 
    member __.Numbers with get() = crossReference.Numbers |> Seq.toList
    member __.CrossReferenceType with get() = 
        match crossReference.Type with 
        | CrossReferenceType.None -> GenBankMetadataCrossReferenceType.None
        | CrossReferenceType.Project -> GenBankMetadataCrossReferenceType.Project
        | CrossReferenceType.TraceAssemblyArchive -> GenBankMetadataCrossReferenceType.TraceAssemblyArchive
        | CrossReferenceType.BioProject -> GenBankMetadataCrossReferenceType.BioProject
        | _ -> invalidArg "CrossReferenceType" "An invalid CrossReferenceType has been encountered."

and GenBankMetadataCrossReferenceType = 
| None
| Project
| TraceAssemblyArchive
| BioProject


// Segment
and GenBankMetadataSegment(segment:SequenceSegment) =
    member __.Count with get() = segment.Count
    member __.Current with get() = segment.Current


// Locus
and GenBankMetadataLocus(locus:GenBankLocusInfo) = 
    member __.Date with get() = locus.Date
    member __.DivisionCode with get() = 
        match locus.DivisionCode with
        | SequenceDivisionCode.None -> GenBankMetadataDivisionCode.None
        | SequenceDivisionCode.PRI -> GenBankMetadataDivisionCode.PRI
        | SequenceDivisionCode.ROD -> GenBankMetadataDivisionCode.ROD
        | SequenceDivisionCode.MAM -> GenBankMetadataDivisionCode.MAM
        | SequenceDivisionCode.VRT -> GenBankMetadataDivisionCode.VRT
        | SequenceDivisionCode.INV -> GenBankMetadataDivisionCode.INV
        | SequenceDivisionCode.PLN -> GenBankMetadataDivisionCode.PLN
        | SequenceDivisionCode.BCT -> GenBankMetadataDivisionCode.BCT
        | SequenceDivisionCode.VRL -> GenBankMetadataDivisionCode.VRL
        | SequenceDivisionCode.PHG -> GenBankMetadataDivisionCode.PHG
        | SequenceDivisionCode.SYN -> GenBankMetadataDivisionCode.SYN
        | SequenceDivisionCode.UNA -> GenBankMetadataDivisionCode.UNA
        | SequenceDivisionCode.EST -> GenBankMetadataDivisionCode.EST
        | SequenceDivisionCode.PAT -> GenBankMetadataDivisionCode.PAT
        | SequenceDivisionCode.STS -> GenBankMetadataDivisionCode.STS
        | SequenceDivisionCode.GSS -> GenBankMetadataDivisionCode.GSS
        | SequenceDivisionCode.HTG -> GenBankMetadataDivisionCode.HTG
        | SequenceDivisionCode.HTC -> GenBankMetadataDivisionCode.HTC
        | SequenceDivisionCode.ENV -> GenBankMetadataDivisionCode.ENV
        | SequenceDivisionCode.CON -> GenBankMetadataDivisionCode.CON
        | _ -> invalidArg "DivisionCode" "An invalid DivisionCode has been encountered."
    member __.MoleculeType with get() = 
        match locus.MoleculeType with
        | MoleculeType.Invalid -> GenBankMetadataMoleculeType.Invalid
        | MoleculeType.NA -> GenBankMetadataMoleculeType.NA
        | MoleculeType.DNA -> GenBankMetadataMoleculeType.DNA
        | MoleculeType.RNA -> GenBankMetadataMoleculeType.RNA
        | MoleculeType.tRNA -> GenBankMetadataMoleculeType.TRNA
        | MoleculeType.rRNA -> GenBankMetadataMoleculeType.RRNA
        | MoleculeType.mRNA -> GenBankMetadataMoleculeType.MRNA
        | MoleculeType.uRNA -> GenBankMetadataMoleculeType.URNA
        | MoleculeType.snRNA -> GenBankMetadataMoleculeType.SnRNA
        | MoleculeType.snoRNA -> GenBankMetadataMoleculeType.SnoRNA
        | MoleculeType.Protein -> GenBankMetadataMoleculeType.Protein
        | _ -> invalidArg "MoleculeType" "An invalid MoleculeType has been encountered."
    member __.Name with get() = locus.Name
    member __.SequenceLength with get() = locus.SequenceLength
    member __.SequenceType with get() = locus.SequenceType
    member __.Strand with get() = 
        match locus.Strand with
        | SequenceStrandType.None -> GenBankMetadataSequenceStrandType.None
        | SequenceStrandType.Single -> GenBankMetadataSequenceStrandType.Single
        | SequenceStrandType.Double -> GenBankMetadataSequenceStrandType.Double
        | SequenceStrandType.Mixed -> GenBankMetadataSequenceStrandType.Mixed
        | _ -> invalidArg "Strand" "An invalid Strand has been encountered."
    member __.StrandTopology with get() = 
        match locus.StrandTopology with
        | SequenceStrandTopology.None -> GenBankMetadataSequenceStrandTopology.None
        | SequenceStrandTopology.Linear -> GenBankMetadataSequenceStrandTopology.Linear
        | SequenceStrandTopology.Circular -> GenBankMetadataSequenceStrandTopology.Circular
        | _ -> invalidArg "StrandTopology" "An invalid StrandTopology has been encountered."

and GenBankMetadataDivisionCode = 
| None
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

and GenBankMetadataMoleculeType = 
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

and GenBankMetadataSequenceStrandType = 
| None
| Single
| Double
| Mixed

and GenBankMetadataSequenceStrandTopology = 
| None
| Linear
| Circular


// Features
and GenBankMetadataFeatures(features:SequenceFeatures) = 
    member __.All with get() = features.All
    member __.Attenuators with get() = features.Attenuators
    member __.CAATSignals with get() = features.CAATSignals
    member __.CodingSequences with get() = features.CodingSequences
    member __.DisplacementLoops with get() = features.DisplacementLoops
    member __.Enhancers with get() = features.Enhancers
    member __.Exons with get() = features.Exons
    member __.FivePrimeUTRs with get() = features.FivePrimeUTRs
    member __.GCSignals with get() = features.GCSignals
    member __.Genes with get() = features.Genes
    member __.InterveningDNAs with get() = features.InterveningDNAs
    member __.Introns with get() = features.Introns
    member __.LongTerminalRepeats with get() = features.LongTerminalRepeats
    member __.MaturePeptides with get() = features.MaturePeptides
    member __.MessengerRNAs with get() = features.MessengerRNAs
    member __.Minus10Signals with get() = features.Minus10Signals
    member __.Minus35Signals with get() = features.Minus35Signals
    member __.MiscBindings with get() = features.MiscBindings
    member __.MiscDifferences with get() = features.MiscDifferences
    member __.MiscFeatures with get() = features.MiscFeatures
    member __.MiscRecombinations with get() = features.MiscRecombinations
    member __.MiscRNAs with get() = features.MiscRNAs
    member __.MiscSignals with get() = features.MiscSignals
    member __.MiscStructures with get() = features.MiscStructures
    member __.ModifiedBases with get() = features.ModifiedBases
    member __.NonCodingRNAs with get() = features.NonCodingRNAs
    member __.OperonRegions with get() = features.OperonRegions
    member __.PolyASignals with get() = features.PolyASignals
    member __.PolyASites with get() = features.PolyASites
    member __.PrecursorRNAs with get() = features.PrecursorRNAs
    member __.Promoters with get() = features.Promoters
    member __.ProteinBindingSites with get() = features.ProteinBindingSites
    member __.RepeatRegions with get() = features.RepeatRegions
    member __.ReplicationOrigins with get() = features.ReplicationOrigins
    member __.RibosomalRNAs with get() = features.RibosomalRNAs
    member __.RibosomeBindingSites with get() = features.RibosomeBindingSites
    member __.SignalPeptides with get() = features.SignalPeptides
    member __.StemLoops with get() = features.StemLoops
    member __.TATASignals with get() = features.TATASignals
    member __.Terminators with get() = features.Terminators
    member __.ThreePrimeUTRs with get() = features.ThreePrimeUTRs
    member __.TransferMessengerRNAs with get() = features.TransferMessengerRNAs
    member __.TransferRNAs with get() = features.TransferRNAs
    member __.TransitPeptides with get() = features.TransitPeptides
    member __.UnsureSequenceRegions with get() = features.UnsureSequenceRegions
    member __.Variations with get() = features.Variations

