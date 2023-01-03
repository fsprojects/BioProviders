namespace BioProviders

open BioProviders.Common
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// GenBank Structure Type Representation.
// --------------------------------------------------------------------------------------

/// <summary>
/// GenBank types, functions, and utilities.
/// </summary>
module GenBank =

    /// <summary>
    /// Creates a type for the GenBank assembly identified by the provided accession.
    /// </summary>
    let createAssembly (context:Context) =

        let (accession, assemblyName, assemblyPath) = 
            (context.DatabaseName, context.Species.SpeciesName, context.Accession.AccessionName)
            |||> CacheAccess.getAssembly
        
        { DatabaseName = context.DatabaseName
          Species = context.Species
          Accession = 
            { AccessionName = AccessionPlainName accession 
              AssemblyName = Some assemblyName
              AssemblyPath = Some assemblyPath
              GenBankFlatFilePath = Some $"{assemblyPath}/{assemblyName}_genomic.gbff.gz" } }


    /// <summary>
    /// Creates types for all GenBank assemblies matching the provided accession pattern.
    /// </summary>
    let rec createAssemblies (context:Context) =

        // Extract information.
        let database = context.DatabaseName
        let species = context.Species.SpeciesName
        let accession = context.Accession.AccessionName.ToString()

        // Create assemblies, making sure LookupPath and SpeciesID are defined.
        match context.Species.LookupPath, context.Species.SpeciesID with
        | Some lookup, Some speciesID -> 
            CacheAccess.getAssemblies (context.DatabaseName) (lookup) (speciesID) (accession)
            |> List.map (fun assemblyInfo -> 
                                let (accession, assemblyName, assemblyPath) = assemblyInfo
                                { DatabaseName = context.DatabaseName
                                  Species = context.Species
                                  Accession = 
                                    { AccessionName = AccessionPlainName accession 
                                      AssemblyName = Some assemblyName
                                      AssemblyPath = Some assemblyPath
                                      GenBankFlatFilePath = Some $"{assemblyPath}/{assemblyName}_genomic.gbff.gz" } })
        | _ -> let (speciesID, speciesName, assemblyLookupPath) = CacheAccess.getSpecies database species
               { DatabaseName = DatabaseName.GenBank
                 Species = 
                   { SpeciesName = SpeciesPlainName speciesName
                     SpeciesID = Some speciesID 
                     LookupPath = Some assemblyLookupPath }
                 Accession = context.Accession } 
               |> createAssemblies

    
    /// <summary>
    /// Creates types for all GenBank species matching the provided species pattern.
    /// </summary>
    let createSpecies (context:Context) =
        (context.DatabaseName, context.Species.SpeciesName.ToString())
        ||> CacheAccess.getSpeciesCollection 
        |> List.map (fun speciesInfo -> 
                            let (speciesID, speciesName, assemblyLookupPath) = speciesInfo
                            { DatabaseName = DatabaseName.GenBank
                              Species = 
                                { SpeciesName = SpeciesPlainName speciesName
                                  SpeciesID = Some speciesID 
                                  LookupPath = Some assemblyLookupPath }
                              Accession = context.Accession } )
