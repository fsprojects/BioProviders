namespace BioProviders.RunTime

open System.IO
open BioProviders.Common
open System.Text.RegularExpressions

// --------------------------------------------------------------------------------------
// GenBank Species Type Representation.
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
