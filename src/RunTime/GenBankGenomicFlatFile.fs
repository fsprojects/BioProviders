namespace BioProviders

open System.IO
open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Flat File Representation.
// --------------------------------------------------------------------------------------

module GenBankFlatFile =

    /// <summary>
    /// GenBank Flat File representation.
    /// </summary>
    type GenBankFlatFile =
        { Metadata : Metadata.Metadata
          Sequence : int}

    /// <summary>
    /// Basic constructor for GenBankFlatFile type.
    /// </summary>
    let createGenBankFlatFile (path:string) = 
        
        // Create DotNet Bio ISequence for the GenBank Flat File.
        let sequence = 
            CacheAccess.loadFile path
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new Bio.IO.GenBank.GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head
        let metadata = sequence.Metadata.Item("GenBank") :?> Bio.IO.GenBank.GenBankMetadata

        // Create GenBank Flat File Type.
        { Metadata = Metadata.createMetadata metadata
          Sequence = Sequence.createSequence sequence }
