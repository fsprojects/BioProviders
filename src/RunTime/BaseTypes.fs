namespace BioProviders.RunTime

open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
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

        let cache = new Cache()
        
        // Create DotNet Bio ISequence for the GenBank Flat File.
        let sequence = 
            (cache :> ICache).LoadFile(path)
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (new GenBankParser()).Parse
            |> Seq.cast<Bio.ISequence>
            |> Seq.head

        member __.Sequence = new GenBankGenomeSequence(sequence)


    // ----------------------------------------------------------------------------------
    // GenBank Assembly Representation.
    // ----------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------
    // GenBank Species Representation.
    // ----------------------------------------------------------------------------------

    type IGenBankSpecies = 
        abstract Assemblies : string list

    type GenBankSpecies = 

        private { AssemblyList: string list }

        interface IGenBankSpecies with
            member x.Assemblies = x.AssemblyList

        static member Create (path:string) (pattern:string) = 
            let cache = new Cache()

            let regex = match pattern with
                        | _ when pattern.Length = 0 || pattern.[pattern.Length - 1] <> '*' -> failwith ""
                        | _ -> pattern.Substring(0, pattern.Length - 1) + ".*"

            let assemblies = (cache :> ICache).LoadDirectory(path)
                             |> fun stream -> 
                                    ( seq { use sr = new StreamReader (stream)
                                            while not sr.EndOfStream do
                                            yield sr.ReadLine () } ) 
                             |> Seq.filter (fun assembly -> 
                                                match assembly with
                                                | _ when Regex.IsMatch(assembly, regex) -> true
                                                | _ -> false)
                             |> Seq.toList

            { AssemblyList = assemblies } :> IGenBankSpecies
