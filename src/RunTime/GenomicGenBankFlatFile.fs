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



// --------------------------------------------------------------------------------------
// GenBank Flat File Representation.
// --------------------------------------------------------------------------------------

type IGenomicGenBankFlatFile = 
    abstract Sequence : IGenBankGenomicSequence

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
