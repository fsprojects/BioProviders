namespace BioProviders

open System.Collections.Generic

// --------------------------------------------------------------------------------------
// Genomic GenBank Flat File Sequence Representation.
// --------------------------------------------------------------------------------------

type IGenBankGenomicSequence = 
    inherit seq<byte>
    abstract Length : int64 with get
    abstract Item : int64 -> byte with get
    abstract GetComplementedSequence : unit -> IGenBankGenomicSequence
    abstract GetReversedSequence : unit -> IGenBankGenomicSequence
    abstract GetReverseComplementedSequence : unit -> IGenBankGenomicSequence
    abstract GetSubSequence : int64 -> int64 -> IGenBankGenomicSequence
    abstract ToString : unit -> string


type GenBankGenomicSequence (sequence:Bio.ISequence) = 
    
    // Standardise sequence (capitalise items).
    let _seq = sequence.GetComplementedSequence().GetComplementedSequence()
    
    // Implement GenBank sequence interface.
    interface IGenBankGenomicSequence with
        member __.Length = _seq.Count
        member __.Item with get index = _seq.Item(index)
        member __.GetComplementedSequence() = new GenBankGenomicSequence(_seq.GetComplementedSequence()) :> IGenBankGenomicSequence
        member __.GetReversedSequence() = new GenBankGenomicSequence(_seq.GetReversedSequence()) :> IGenBankGenomicSequence
        member __.GetReverseComplementedSequence() = new GenBankGenomicSequence(_seq.GetReverseComplementedSequence()) :> IGenBankGenomicSequence
        member __.GetSubSequence start length = new GenBankGenomicSequence(_seq.GetSubSequence(start, length)) :> IGenBankGenomicSequence
        member __.GetEnumerator(): IEnumerator<byte> = _seq.GetEnumerator()
        member __.GetEnumerator(): System.Collections.IEnumerator = _seq.GetEnumerator() :> System.Collections.IEnumerator
        override __.ToString() = _seq.ToString()
