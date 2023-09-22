namespace BioProviders

open BioProviders.Common
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// GenBank Assembly Representation.
// --------------------------------------------------------------------------------------

type GenBankAssembly(accession: string, assemblyName: string, assemblyPath: string) =

    member __.Accession = accession
    member __.AssemblyName = assemblyName
    member __.AssemblyPath = assemblyPath
    member __.GenBankFlatFilePath = $"{assemblyPath}/{assemblyName}_genomic.gbff.gz"

    new(context: Context) =
        let database = context.DatabaseName
        let species = context.SpeciesName
        let accession = context.Accession

        let (accessionNumber, assemblyName, assemblyPath) =
            CacheAccess.getAssembly database species accession

        GenBankAssembly(accessionNumber, assemblyName, assemblyPath)
