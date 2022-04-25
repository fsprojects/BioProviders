namespace BioProviders.RunTime

open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
open Bio.IO.GenBank
open Bio.Algorithms.StringSearch
open BioProviders.Common


module BaseTypes = 

    // ----------------------------------------------------------------------------------
    // GenBank Flat File Sequence Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankSequence = 
        inherit seq<byte>
        abstract Count : int64 with get
        abstract Item : int64 -> byte with get
        abstract GetComplementedSequence : unit -> IGenBankSequence
        abstract GetReversedSequence : unit -> IGenBankSequence
        abstract GetReverseComplementedSequence : unit -> IGenBankSequence
        abstract GetSubSequence : int64 -> int64 -> IGenBankSequence
        abstract IndexOfNonGap : unit -> int64
        abstract IndexOfNonGap : int64 -> int64
        abstract LastIndexOfNonGap : unit -> int64
        abstract LastIndexOfNonGap : int64 -> int64
        abstract ToString : unit -> string
        abstract Search : string -> int list
        abstract Search : string list -> Map<string, int list>


    type GenBankSequence (sequence:Bio.ISequence) = 
        
            // Standardise sequence (capitalise items).
            let _seq = sequence.GetComplementedSequence().GetComplementedSequence()
        
            // Implement GenBank sequence interface.
            interface IGenBankSequence with
                member __.Count = _seq.Count
                member __.Item with get index = _seq.Item(index)
                member __.GetComplementedSequence() = new GenBankSequence(_seq.GetComplementedSequence()) :> IGenBankSequence
                member __.GetReversedSequence() = new GenBankSequence(_seq.GetReversedSequence()) :> IGenBankSequence
                member __.GetReverseComplementedSequence() = new GenBankSequence(_seq.GetReverseComplementedSequence()) :> IGenBankSequence
                member __.GetSubSequence start length = new GenBankSequence(_seq.GetSubSequence(start, length)) :> IGenBankSequence
                member __.IndexOfNonGap startPos = _seq.IndexOfNonGap startPos
                member __.IndexOfNonGap() = _seq.IndexOfNonGap()
                member __.LastIndexOfNonGap endPos = _seq.LastIndexOfNonGap endPos
                member __.LastIndexOfNonGap() = _seq.LastIndexOfNonGap()
                member __.GetEnumerator(): IEnumerator<byte> = _seq.GetEnumerator()
                member __.GetEnumerator(): System.Collections.IEnumerator = _seq.GetEnumerator() :> System.Collections.IEnumerator
                override __.ToString() = _seq.ToString()

                member __.Search(searchPattern:string) = 
                    let searcher = new BoyerMoore()
                    searcher.FindMatch(_seq, searchPattern) |> Seq.toList

                member __.Search(searchPatterns:string list) = 
                    let searcher = new BoyerMoore()
                    let patterns = ResizeArray<string> searchPatterns
                    searcher.FindMatch(_seq, patterns)
                    |> Seq.map (fun pair -> new KeyValuePair<string, int list>(pair.Key, pair.Value |> Seq.toList))
                    |> Seq.map (|KeyValue|) 
                    |> Map.ofSeq


    // ----------------------------------------------------------------------------------
    // GenBank Flat File Metadata Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankMetadata =
        abstract BaseCount : string with get
        abstract Comments : string list with get
        abstract Contig : string with get
        abstract DbSource : string with get
        abstract Definition : string with get
        abstract Keywords : string with get
        abstract Origin : string with get
        abstract Primary : string with get
        abstract Source : IGenBankSource with get
        abstract Version : IGenBankVersion with get
        abstract Accession : IGenBankAccession with get
        abstract References : IGenBankReferences with get
        abstract Segment : IGenBankSegment with get
        abstract Locus : IGenBankLocus with get
        abstract DbLinks : IGenBankDbLinks with get
        

    and GenBankMetadata (sequence:Bio.ISequence) = 
        
        let metadata = ( sequence.Metadata.Item("GenBank") :?> Bio.IO.GenBank.GenBankMetadata )
        
        interface IGenBankMetadata with
            member __.BaseCount with get() = metadata.BaseCount
            member __.Comments with get() = metadata.Comments |> Seq.toList
            member __.Contig with get() = metadata.Contig
            member __.DbSource with get() = metadata.DbSource
            member __.Definition with get() = metadata.Definition
            member __.Keywords with get() = metadata.Keywords
            member __.Origin with get() = metadata.Origin
            member __.Primary with get() = metadata.Primary
            member __.Source with get() = new GenBankSource(metadata.Source) :> IGenBankSource
            member __.Version with get() = new GenBankVersion(metadata.Version) :> IGenBankVersion
            member __.Accession with get() = new GenBankAccession(metadata.Accession) :> IGenBankAccession
            member __.References with get() = new GenBankReferences(metadata.References) :> IGenBankReferences
            member __.Segment with get() = new GenBankSegment(metadata.Segment) :> IGenBankSegment
            member __.Locus with get() = new GenBankLocus(metadata.Locus) :> IGenBankLocus
            member __.DbLinks with get() = new GenBankDbLinks(metadata.DbLinks) :> IGenBankDbLinks
    
    
    // References
    and IGenBankReferences = 
        inherit seq<IGenBankCitationReference>
        abstract Count : int with get
        abstract Item : int -> IGenBankCitationReference with get
        abstract ToString : unit -> string

    and GenBankReferences (references:IList<Bio.IO.GenBank.CitationReference>) =
        interface IGenBankReferences with
            member __.GetEnumerator(): IEnumerator<IGenBankCitationReference> = 
                (references |> Seq.map (fun x -> (new GenBankCitationReference(x)) :> IGenBankCitationReference) ).GetEnumerator()
            member x.GetEnumerator(): System.Collections.IEnumerator = 
                (references |> Seq.map (fun x -> (new GenBankCitationReference(x)) :> IGenBankCitationReference) ).GetEnumerator() :> System.Collections.IEnumerator
            member __.Count with get() = references.Count
            member __.Item with get index = new GenBankCitationReference(references.Item(index)) :> IGenBankCitationReference
            member __.ToString () = references.ToString()

    and IGenBankCitationReference = 
        abstract Authors : string with get
        abstract Consortiums : string with get
        abstract Journal : string with get
        abstract Location : string with get
        abstract Medline : string with get
        abstract Number : int
        abstract PubMed : string with get
        abstract Remarks : string with get
        abstract Title : string with get
        abstract ToString : unit -> string
    
    and GenBankCitationReference (citation:Bio.IO.GenBank.CitationReference) =
        interface IGenBankCitationReference with
            member __.Authors = citation.Authors
            member __.Consortiums = citation.Consortiums
            member __.Journal = citation.Journal
            member __.Location = citation.Location
            member __.Medline = citation.Medline
            member __.Number = citation.Number
            member __.PubMed = citation.PubMed
            member __.Remarks = citation.Remarks
            member __.Title = citation.Title
            member __.ToString () = citation.ToString()
    
    
    // Version
    and IGenBankVersion =
        abstract Accession : string with get
        abstract CompoundAccession : string with get
        abstract GiNumber : string with get
        abstract Version : string with get
        abstract ToString : unit -> string

    and GenBankVersion (version:Bio.IO.GenBank.GenBankVersion) =
        interface IGenBankVersion with
            member __.Accession with get() = version.Accession
            member __.CompoundAccession with get() = version.CompoundAccession
            member __.GiNumber with get() = version.GiNumber
            member __.Version with get() = version.Version
            member __.ToString () = version.ToString()
    
    
    // Accession
    and IGenBankAccession =
        abstract Primary : string with get
        abstract Secondary : string list with get
        abstract ToString : unit -> string
        
    and GenBankAccession(accession:Bio.IO.GenBank.GenBankAccession) = 
        interface IGenBankAccession with
            member __.Primary with get() = accession.Primary
            member __.Secondary with get() = accession.Secondary |> Seq.toList
            member __.ToString () = accession.ToString()
    

    // Source
    and IGenBankSource = 
        abstract CommonName : string with get
        abstract Organism : IGenBankOrganismInfo with get
        abstract ToString : unit -> string

    and GenBankSource(source:Bio.IO.GenBank.SequenceSource) =
        interface IGenBankSource with
            member __.CommonName with get() = source.CommonName
            member __.Organism with get() = new GenBankOrganismInfo(source.Organism) :> IGenBankOrganismInfo
            member __.ToString () = source.ToString()

    and IGenBankOrganismInfo = 
        abstract ClassLevels : string with get
        abstract Genus : string with get
        abstract Species : string with get
        abstract ToString : unit -> string
    
    and GenBankOrganismInfo(organism:Bio.IO.GenBank.OrganismInfo) =
        interface IGenBankOrganismInfo with
            member __.ClassLevels with get() = organism.ClassLevels
            member __.Genus with get() = organism.Genus
            member __.Species with get() = organism.Species
            member __.ToString () = organism.ToString()

    
    // Database Link
    and IGenBankDbLinks = 
        inherit seq<IGenBankCrossReferenceLink>
        abstract Count : int with get
        abstract Item : int -> GenBankCrossReferenceLink with get
        abstract ToString : unit -> string

    and GenBankDbLinks(database:IList<Bio.IO.GenBank.CrossReferenceLink>) =
        interface IGenBankDbLinks with
            member this.GetEnumerator(): IEnumerator<IGenBankCrossReferenceLink> = 
                (database |> Seq.map (fun x -> (new GenBankCrossReferenceLink(x)) :> IGenBankCrossReferenceLink) ).GetEnumerator()
            member this.GetEnumerator(): System.Collections.IEnumerator = 
                (database |> Seq.map (fun x -> (new GenBankCrossReferenceLink(x)) :> IGenBankCrossReferenceLink) ).GetEnumerator() :> System.Collections.IEnumerator
            member __.Count with get() = database.Count
            member __.Item with get index = new GenBankCrossReferenceLink(database.Item(index))
            member __.ToString () = database.ToString()

    and IGenBankCrossReferenceLink = 
        abstract Numbers : string list with get
        abstract Type : GenBankCrossReferenceType with get
        abstract ToString : unit -> string
    
    and GenBankCrossReferenceLink(crossReference:Bio.IO.GenBank.CrossReferenceLink) = 
        interface IGenBankCrossReferenceLink with
            member __.Numbers with get() = crossReference.Numbers |> Seq.toList
            member __.Type with get() = 
                match crossReference.Type with 
                | CrossReferenceType.None -> GenBankCrossReferenceType.None
                | CrossReferenceType.Project -> GenBankCrossReferenceType.Project
                | CrossReferenceType.TraceAssemblyArchive -> GenBankCrossReferenceType.TraceAssemblyArchive
                | CrossReferenceType.BioProject -> GenBankCrossReferenceType.BioProject
                | _ -> invalidArg "CrossReferenceType" "An invalid CrossReferenceType has been encountered."
            member __.ToString () = crossReference.ToString()
    
    and GenBankCrossReferenceType = 
    | None
    | Project
    | TraceAssemblyArchive
    | BioProject
    
    
    // Segment
    and IGenBankSegment = 
        abstract Count : int with get
        abstract Current : int with get
        abstract ToString : unit -> string

    and GenBankSegment(segment:Bio.IO.GenBank.SequenceSegment) =
        interface IGenBankSegment with
            member __.Count with get() = segment.Count
            member __.Current with get() = segment.Current
            member __.ToString () = segment.ToString()
    
    
    // Locus
    and IGenBankLocus = 
        abstract Date : System.DateTime with get
        abstract DivisionCode : GenBankDivisionCode with get
        abstract MoleculeType : GenBankMoleculeType with get
        abstract Name : string with get
        abstract SequenceLength : int with get
        abstract SequenceType : string with get
        abstract Strand : GenBankSequenceStrandType with get
        abstract StrandTopology : GenBankSequenceStrandTopology with get
        abstract ToString : unit -> string

    and GenBankLocus(locus:Bio.IO.GenBank.GenBankLocusInfo) = 
        interface IGenBankLocus with
            member __.Date with get() = locus.Date
            member __.DivisionCode with get() = 
                match locus.DivisionCode with
                | SequenceDivisionCode.None -> GenBankDivisionCode.None
                | SequenceDivisionCode.PRI -> GenBankDivisionCode.PRI
                | SequenceDivisionCode.ROD -> GenBankDivisionCode.ROD
                | SequenceDivisionCode.MAM -> GenBankDivisionCode.MAM
                | SequenceDivisionCode.VRT -> GenBankDivisionCode.VRT
                | SequenceDivisionCode.INV -> GenBankDivisionCode.INV
                | SequenceDivisionCode.PLN -> GenBankDivisionCode.PLN
                | SequenceDivisionCode.BCT -> GenBankDivisionCode.BCT
                | SequenceDivisionCode.VRL -> GenBankDivisionCode.VRL
                | SequenceDivisionCode.PHG -> GenBankDivisionCode.PHG
                | SequenceDivisionCode.SYN -> GenBankDivisionCode.SYN
                | SequenceDivisionCode.UNA -> GenBankDivisionCode.UNA
                | SequenceDivisionCode.EST -> GenBankDivisionCode.EST
                | SequenceDivisionCode.PAT -> GenBankDivisionCode.PAT
                | SequenceDivisionCode.STS -> GenBankDivisionCode.STS
                | SequenceDivisionCode.GSS -> GenBankDivisionCode.GSS
                | SequenceDivisionCode.HTG -> GenBankDivisionCode.HTG
                | SequenceDivisionCode.HTC -> GenBankDivisionCode.HTC
                | SequenceDivisionCode.ENV -> GenBankDivisionCode.ENV
                | SequenceDivisionCode.CON -> GenBankDivisionCode.CON
                | _ -> invalidArg "DivisionCode" "An invalid DivisionCode has been encountered."
            member __.MoleculeType with get() = 
                match locus.MoleculeType with
                | MoleculeType.Invalid -> GenBankMoleculeType.Invalid
                | MoleculeType.NA -> GenBankMoleculeType.NA
                | MoleculeType.DNA -> GenBankMoleculeType.DNA
                | MoleculeType.RNA -> GenBankMoleculeType.RNA
                | MoleculeType.tRNA -> GenBankMoleculeType.TRNA
                | MoleculeType.rRNA -> GenBankMoleculeType.RRNA
                | MoleculeType.mRNA -> GenBankMoleculeType.MRNA
                | MoleculeType.uRNA -> GenBankMoleculeType.URNA
                | MoleculeType.snRNA -> GenBankMoleculeType.SnRNA
                | MoleculeType.snoRNA -> GenBankMoleculeType.SnoRNA
                | MoleculeType.Protein -> GenBankMoleculeType.Protein
                | _ -> invalidArg "MoleculeType" "An invalid MoleculeType has been encountered."
            member __.Name with get() = locus.Name
            member __.SequenceLength with get() = locus.SequenceLength
            member __.SequenceType with get() = locus.SequenceType
            member __.Strand with get() = 
                match locus.Strand with
                | SequenceStrandType.None -> GenBankSequenceStrandType.None
                | SequenceStrandType.Single -> GenBankSequenceStrandType.Single
                | SequenceStrandType.Double -> GenBankSequenceStrandType.Double
                | SequenceStrandType.Mixed -> GenBankSequenceStrandType.Mixed
                | _ -> invalidArg "Strand" "An invalid Strand has been encountered."
            member __.StrandTopology with get() = 
                match locus.StrandTopology with
                | SequenceStrandTopology.None -> GenBankSequenceStrandTopology.None
                | SequenceStrandTopology.Linear -> GenBankSequenceStrandTopology.Linear
                | SequenceStrandTopology.Circular -> GenBankSequenceStrandTopology.Circular
                | _ -> invalidArg "StrandTopology" "An invalid StrandTopology has been encountered."
            member __.ToString () = locus.ToString()
    
    and GenBankDivisionCode = 
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
    
    and GenBankMoleculeType = 
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
    
    and GenBankSequenceStrandType = 
    | None
    | Single
    | Double
    | Mixed
    
    and GenBankSequenceStrandTopology = 
    | None
    | Linear
    | Circular


    // ----------------------------------------------------------------------------------
    // GenBank Flat File Representation.
    // ----------------------------------------------------------------------------------

    type GenBankFlatFile (path:string) =

        let cache = new Cache()
        
        // Create DotNet Bio ISequence for the GenBank Flat File.
        let sequence = 
            (cache :> ICache).LoadFile(path)
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head

        member __.Sequence = new GenBankSequence(sequence)
        member __.Metadata = new GenBankMetadata(sequence)


    // ----------------------------------------------------------------------------------
    // GenBank Assembly Representation.
    // ----------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------
    // GenBank Species Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankSpecies = 
        abstract Assemblies : string list

    type GenBankSpecies = 

        private { AssemblyList: string list }

        interface IGenBankSpecies with
            member x.Assemblies = x.AssemblyList

        static member Create (path:string) (pattern:string) = 
            let cache = new Cache()

            let regex = match pattern with
                        | _ when pattern.Length = 0 || pattern.[pattern.Length - 1] <> '*' -> failwith ""
                        | _ -> pattern.Substring(0, pattern.Length - 1) + ".*"

            let assemblies = (cache :> ICache).LoadDirectory(path)
                             |> fun stream -> 
                                    ( seq { use sr = new StreamReader (stream)
                                            while not sr.EndOfStream do
                                            yield sr.ReadLine () } ) 
                             |> Seq.filter (fun assembly -> 
                                                match assembly with
                                                | _ when Regex.IsMatch(assembly, regex) -> true
                                                | _ -> false)
                             |> Seq.toList

            { AssemblyList = assemblies } :> IGenBankSpecies
