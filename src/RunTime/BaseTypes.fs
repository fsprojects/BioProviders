namespace BioProviders.RunTime

open System.IO
open Bio.IO.GenBank
open BioProviders.Common
 open System.Text.RegularExpressions

// --------------------------------------------------------------------------------------
// Base RunTime Types Genomic GenBank Flat File.
// --------------------------------------------------------------------------------------


type Species (path:string, assemblyPattern:string) = 
    
    let cache = new Cache()

    let getAssemblyNames (stream:Stream) = seq {
        use sr = new StreamReader (stream)
        while not sr.EndOfStream do
            yield sr.ReadLine ()
    }

    let filterAssemblies (pattern:string) (assemblies) = 
        let regex = match pattern with
                    | _ when pattern.Length = 0 -> failwith ""
                    | _ when pattern.[pattern.Length - 1] <> '*' -> failwith ""
                    | _ -> pattern.Substring(0, pattern.Length - 1) + ".*"

        assemblies
        |> List.filter (fun assembly -> 
                            match assembly with
                            | _ when Regex.IsMatch(assembly, regex) -> true
                            | _ -> false)

    let assemblies = 
        (cache :> ICache).LoadDirectory(path)
        |> getAssemblyNames
        |> Seq.toList
        |> filterAssemblies assemblyPattern

    member __.Assemblies with get() = assemblies

type GenomicGBFF (path:string) = 

    let cache = new Cache()

    let sequence = 
        (cache :> ICache).LoadFile(path)
        |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
        |> (new GenBankParser()).Parse
        |> Seq.cast<Bio.ISequence>
        |> Seq.head

    member __.Metadata with get() = new GenomicGBFFMetadata(sequence)
    
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
