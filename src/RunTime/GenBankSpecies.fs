namespace BioProviders

open BioProviders.Common
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// GenBank Species Representation.
// --------------------------------------------------------------------------------------

type IGenBankSpecies = 
    abstract SpeciesName : string
    abstract SpeciesID : string
    abstract AssemblyLookupPath : string
    abstract GetAssemblies : string -> IGenBankAssembly list


type GenBankSpecies(database:DatabaseName, speciesID:string, speciesName:string, assemblyLookupPath:string) =

    interface IGenBankSpecies with
        member __.SpeciesName = speciesName
        member __.SpeciesID = speciesID
        member __.AssemblyLookupPath = assemblyLookupPath
        member this.GetAssemblies (accessionPattern:string) = 
            let species = this :> IGenBankSpecies
            CacheAccess.getAssemblies (database) (species.AssemblyLookupPath) (species.SpeciesID) (accessionPattern)
            |> List.map (fun assemblyInfo -> 
                                let (accession, assemblyName, assemblyPath) = assemblyInfo
                                new GenBankAssembly(accession, assemblyName, assemblyPath))

    new (context:Context) = 
        let database = context.DatabaseName
        let species = context.SpeciesName
        let (speciesID, speciesName, assemblyLookupPath) = CacheAccess.getSpecies database species

        GenBankSpecies(database, speciesID, speciesName, assemblyLookupPath)
