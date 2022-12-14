namespace BioProviders

open System.IO
open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Flat File Metadata Representation.
// --------------------------------------------------------------------------------------



// --------------------------------------------------------------------------------------
// GenBank Flat File Representation.
// --------------------------------------------------------------------------------------

type GenomicGenBankFlatFile (path:string) =

    // Create DotNet Bio ISequence for the GenBank Flat File.
    let sequence = 
        CacheAccess.loadFile path
        |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
        |> (new Bio.IO.GenBank.GenBankParser()).Parse
        |> Seq.cast<Bio.ISequence>
        |> Seq.head

    // Add type members.
    member __.Sequence = new GenBankGenomicSequence(sequence)
