// --------------------------------------------------------------------------------------
// Base RunTime Types for Type Provider Erasure.
// --------------------------------------------------------------------------------------

namespace BioProviders.RunTime

open System.IO
open Bio.IO
open BioProviders.Common

module BaseTypes = 

    type Assembly () =

        static member LoadGBFF (path:string) =
            let cache = new Cache()
            new GenBankFlatFile (cache, path)

    and GenBankFlatFile (cache:Cache, path:string) = 

        let sequence = 
            (cache :> ICache).LoadFile(path)
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new GenBank.GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head

        member _.Metadata = new GenBankMetadata(sequence)

    and GenBankMetadata (seq:Bio.ISequence) = 

        let metadata = ( seq.Metadata.Item("GenBank") :?> GenBankMetadata )
            
        member _.Accession = metadata.Accession

        member _.BaseCount = metadata.BaseCount

        member _.Comments = metadata.Comments

        member _.Contig = metadata.Contig

        member _.DbLinks = metadata.DbLinks

        member _.DbSource = metadata.DbSource

        member _.Definition = metadata.Definition

        member _.Features = metadata.Features

        member _.Keywords = metadata.Keywords

        member _.Locus = metadata.Locus

        member _.Origin = metadata.Origin

        member _.Primary = metadata.Primary

        member _.Project = metadata.Project

        member _.References = metadata.References

        member _.Segment = metadata.Segment

        member _.Source = metadata.Source

        member _.Version = metadata.Version
