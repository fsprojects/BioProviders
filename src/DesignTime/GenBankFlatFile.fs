namespace BioProviders.RunTime

open System.IO
open Bio.IO.GenBank
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
    
    member __.Metadata with get() = new GenomicGBFFMetadata(sequence)
    member __.Sequence with get() = new GenomicGBFFSequence(sequence)


// --------------------------------------------------------------------------------------
// GenBank Flat File MetaData Type Representation.
// --------------------------------------------------------------------------------------

and GenomicGBFFSequence (sequence:Bio.ISequence) = 

    // Standardise sequence (capitalise items).
    let _seq = sequence.GetComplementedSequence().GetComplementedSequence()

    member __.Alphabet = _seq.Alphabet
    member __.Count = _seq.Count
    member __.Item with get index = _seq.Item(index)
    member __.GetComplementedSequence() = new GenomicGBFFSequence(_seq.GetComplementedSequence())
    member __.GetReversedSequence() = new GenomicGBFFSequence(_seq.GetReversedSequence())
    member __.GetReverseComplementedSequence() = new GenomicGBFFSequence(_seq.GetReverseComplementedSequence())
    member __.GetSubSequence start length = new GenomicGBFFSequence(_seq.GetSubSequence(start, length))
    member __.IndexOfNonGap startPos = _seq.IndexOfNonGap startPos
    member __.IndexOfNonGap() = _seq.IndexOfNonGap()
    member __.LastIndexOfNonGap endPos = _seq.LastIndexOfNonGap endPos
    member __.LastIndexOfNonGap() = _seq.LastIndexOfNonGap()
    member __.GetEnumerator() = _seq.GetEnumerator()
    override __.ToString() = _seq.ToString()


// --------------------------------------------------------------------------------------
// GenBank Flat File Sequence Type Representation.
// --------------------------------------------------------------------------------------

and GenomicGBFFMetadata (sequence:Bio.ISequence) = 
    
    let metadata = ( sequence.Metadata.Item("GenBank") :?> GenBankMetadata )
    
    member __.Accession with get() = metadata.Accession
    member __.BaseCount with get() = metadata.BaseCount
    member __.Comments with get() = metadata.Comments
    member __.Contig with get() = metadata.Contig
    member __.DbLinks with get() = metadata.DbLinks
    member __.DbSource with get() = metadata.DbSource
    member __.Definition with get() = metadata.Definition
    member __.Features with get() = metadata.Features
    member __.Keywords with get() = metadata.Keywords
    member __.Locus with get() = metadata.Locus
    member __.Origin with get() = metadata.Origin
    member __.Primary with get() = metadata.Primary
    member __.Project with get() = metadata.Project
    member __.References with get() = metadata.References
    member __.Segment with get() = metadata.Segment
    member __.Source with get() = metadata.Source
    member __.Version with get() = metadata.Version
