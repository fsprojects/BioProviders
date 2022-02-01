namespace BioProviders.RunTime

open System.IO
open Bio.IO.GenBank
open BioProviders.Common

// --------------------------------------------------------------------------------------
// Base RunTime Types Genomic GenBank Flat File.
// --------------------------------------------------------------------------------------

type GenomicGBFF (path:string) = 

    let cache = new Cache()

    let sequence = 
        (cache :> ICache).LoadFile(path)
        |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
        |> (new GenBankParser()).Parse
        |> Seq.cast<Bio.ISequence>
        |> Seq.head

    member _.Metadata with get() = new GenomicGBFFMetadata(sequence)
    
and GenomicGBFFMetadata (sequence:Bio.ISequence) = 
    
    let metadata = ( sequence.Metadata.Item("GenBank") :?> GenBankMetadata )
    
    member _.Accession with get() = metadata.Accession
    member _.BaseCount with get() = metadata.BaseCount
    member _.Comments with get() = metadata.Comments
    member _.Contig with get() = metadata.Contig
    member _.DbLinks with get() = metadata.DbLinks
    member _.DbSource with get() = metadata.DbSource
    member _.Definition with get() = metadata.Definition
    member _.Features with get() = metadata.Features
    member _.Keywords with get() = metadata.Keywords
    member _.Locus with get() = metadata.Locus
    member _.Origin with get() = metadata.Origin
    member _.Primary with get() = metadata.Primary
    member _.Project with get() = metadata.Project
    member _.References with get() = metadata.References
    member _.Segment with get() = metadata.Segment
    member _.Source with get() = metadata.Source
    member _.Version with get() = metadata.Version
