namespace BioProviders

// --------------------------------------------------------------------------------------
// GenBank Flat File Sequence Representation.
// --------------------------------------------------------------------------------------

module Sequence =

    let createSequence (sequence: Bio.ISequence) =
        BioFSharp.BioSeq.ofNucleotideString (sequence.ToString())
