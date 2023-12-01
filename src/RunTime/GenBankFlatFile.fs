namespace BioProviders

open System.IO
open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Flat File Representation.
// --------------------------------------------------------------------------------------

module GenBankFlatFile =

    /// <summary>
    /// GenBank Flat File representation.
    ///     <para>It consists of two members:</para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Metadata - The metadata for the current sequece, as a GenBankMetadata type.</description>
    ///         </item>
    ///         <item>
    ///             <description>Sequence - The sequence itself as a BioFSharp BioSeq type.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    type GenBankFlatFile =
        { Metadata: Metadata.Metadata
          Sequence: BioFSharp.BioSeq.BioSeq<BioFSharp.Nucleotides.Nucleotide> }

    /// <summary>
    /// Basic constructor for GenBankFlatFile type. Creates a representation based upon an NCBI assembly file.
    /// </summary>
    /// <param name="path">The path to the assembly file. This will usually reside in the cache folder.</param>
    let createGenBankFlatFile (path: string) =

        // Delete files that are too old.
        // Ideally, we'd have this in a different place, rather than accessed
        // any time we want to create a new flat file.
        CacheAccess.deleteOldFiles

        // Create DotNet Bio ISequence for the GenBank Flat File.
        let sequence =
            CacheAccess.loadFile path
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new Bio.IO.GenBank.GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head

        let metadata = sequence.Metadata.Item("GenBank") :?> Bio.IO.GenBank.GenBankMetadata

        // Added by Samuel Smith n7581769.
        // Change the last access date for the requested file.
        if (File.Exists(path)) then
            File.SetLastAccessTime(path, System.DateTime.Now)

        // Create GenBank Flat File Type.
        { Metadata = Metadata.createMetadata metadata
          Sequence = Sequence.createSequence sequence }
