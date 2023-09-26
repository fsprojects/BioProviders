namespace BioProviders

open BioProviders.Common
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// GenBank Species Representation.
// --------------------------------------------------------------------------------------

type GenBankSpecies(database: DatabaseName, speciesID: string, speciesName: string, assemblyLookupPath: string) =

    member species.SpeciesName = speciesName
    member species.SpeciesID = speciesID
    member species.AssemblyLookupPath = assemblyLookupPath

    member species.GetAssemblies(accessionPattern: string) =
        CacheAccess.getAssemblies (database) (species.AssemblyLookupPath) (species.SpeciesID) (accessionPattern)
        |> List.map (fun assemblyInfo ->
            let (accession, assemblyName, assemblyPath) = assemblyInfo
            new GenBankAssembly(accession, assemblyName, assemblyPath))

    new(context: Context) =
        let database = context.DatabaseName
        let species = context.SpeciesName

        let (speciesID, speciesName, assemblyLookupPath) =
            CacheAccess.getSpecies database species

        GenBankSpecies(database, speciesID, speciesName, assemblyLookupPath)
