namespace BioProviders

open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Assembly Representation.
// --------------------------------------------------------------------------------------

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
