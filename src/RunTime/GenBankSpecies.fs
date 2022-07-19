namespace BioProviders

open BioProviders.Common

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
