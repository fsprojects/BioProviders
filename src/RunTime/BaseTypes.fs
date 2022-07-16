namespace BioProviders.RunTime

open System.IO
open System.Collections.Generic
open Bio.IO.GenBank
open BioProviders.Common


module BaseTypes = 

    // ----------------------------------------------------------------------------------
    // GenBank Flat File Sequence Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankGenomeSequence = 
        inherit seq<byte>
        abstract Count : int64 with get
        abstract Item : int64 -> byte with get
        abstract GetComplementedSequence : unit -> IGenBankGenomeSequence
        abstract GetReversedSequence : unit -> IGenBankGenomeSequence
        abstract GetReverseComplementedSequence : unit -> IGenBankGenomeSequence
        abstract GetSubSequence : int64 -> int64 -> IGenBankGenomeSequence
        abstract ToString : unit -> string


    type GenBankGenomeSequence (sequence:Bio.ISequence) = 
        
            // Standardise sequence (capitalise items).
            let _seq = sequence.GetComplementedSequence().GetComplementedSequence()
        
            // Implement GenBank sequence interface.
            interface IGenBankGenomeSequence with
                member __.Count = _seq.Count
                member __.Item with get index = _seq.Item(index)
                member __.GetComplementedSequence() = new GenBankGenomeSequence(_seq.GetComplementedSequence()) :> IGenBankGenomeSequence
                member __.GetReversedSequence() = new GenBankGenomeSequence(_seq.GetReversedSequence()) :> IGenBankGenomeSequence
                member __.GetReverseComplementedSequence() = new GenBankGenomeSequence(_seq.GetReverseComplementedSequence()) :> IGenBankGenomeSequence
                member __.GetSubSequence start length = new GenBankGenomeSequence(_seq.GetSubSequence(start, length)) :> IGenBankGenomeSequence
                member __.GetEnumerator(): IEnumerator<byte> = _seq.GetEnumerator()
                member __.GetEnumerator(): System.Collections.IEnumerator = _seq.GetEnumerator() :> System.Collections.IEnumerator
                override __.ToString() = _seq.ToString()


    // ----------------------------------------------------------------------------------
    // GenBank Flat File Metadata Representation.
    // ----------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------
    // GenBank Flat File Representation.
    // ----------------------------------------------------------------------------------

    type GenBankGenome (path:string) =

        // Create DotNet Bio ISequence for the GenBank Flat File.
        let sequence = 
            CacheAccess.loadFile path
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head

        member __.Sequence = new GenBankGenomeSequence(sequence)


    // ----------------------------------------------------------------------------------
    // GenBank Assembly Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankAssembly = 
        abstract Accession : string
        abstract AssemblyPath : string
        abstract GenBankFlatFilePath : string


    type GenBankAssembly =

        private { Accession: string
                  AssemblyPath: string
                  GenBankFlatFilePath: string }

        interface IGenBankAssembly with
            member x.Accession = x.Accession
            member x.AssemblyPath = x.AssemblyPath
            member x.GenBankFlatFilePath = x.GenBankFlatFilePath

        static member CreateAssemblyPath (databasePath:string) (assemblyPath:string) = 
            $"/{databasePath}/{assemblyPath}"

        static member CreateGenBankFlatFilePath (assemblyPath:string) = 
            assemblyPath.Split('/')
            |> (fun parts -> parts.[parts.Length - 1])
            |> (fun identifier -> $"{assemblyPath}/{identifier}_genomic.gbff.gz")

        static member Create (databasePath:string) (speciesName:string) (accession:string) =  
            let assemblyPath = CacheAccess.getAssemblyPath speciesName accession
                               |> GenBankAssembly.CreateAssemblyPath databasePath
            let genbankFlatFilePath = GenBankAssembly.CreateGenBankFlatFilePath assemblyPath

            { Accession = accession 
              AssemblyPath = assemblyPath 
              GenBankFlatFilePath = genbankFlatFilePath } :> IGenBankAssembly


    // ----------------------------------------------------------------------------------
    // GenBank Species Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankSpecies = 
        abstract SpeciesName : string
        abstract Assemblies : IGenBankAssembly list


    type GenBankSpecies = 
        
        private { SpeciesName: string
                  Assemblies: IGenBankAssembly list }

        interface IGenBankSpecies with
            member x.SpeciesName = x.SpeciesName
            member x.Assemblies = x.Assemblies
        
        static member Create (databasePath:string) (speciesName:string) (accessionPattern:string) = 
            let createAssembly (accession:string, assemblyPath:string) =
                let assemblyPath = GenBankAssembly.CreateAssemblyPath databasePath assemblyPath
                let genbankFlatFilePath = GenBankAssembly.CreateGenBankFlatFilePath assemblyPath
                { Accession = accession
                  AssemblyPath = assemblyPath
                  GenBankFlatFilePath = genbankFlatFilePath } :> IGenBankAssembly

            let pattern = match accessionPattern with
                          | _ when accessionPattern.Length = 0 || accessionPattern.[accessionPattern.Length - 1] <> '*' -> failwith ""
                          | _ -> accessionPattern.Substring(0, accessionPattern.Length - 1) + ".*"

            let assemblies = CacheAccess.getAssemblies speciesName pattern
                             |> List.map (fun assembly -> createAssembly assembly)
            
            { SpeciesName = speciesName 
              Assemblies = assemblies } :> IGenBankSpecies


    // ----------------------------------------------------------------------------------
    // GenBank Species Collection Representation.
    // ----------------------------------------------------------------------------------