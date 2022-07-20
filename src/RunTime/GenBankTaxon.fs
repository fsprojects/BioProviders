﻿namespace BioProviders

open BioProviders.Common
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// GenBank Taxon Representation.
// --------------------------------------------------------------------------------------

type IGenBankTaxon = 
    abstract GetSpecies : string -> IGenBankSpecies list


type GenBankTaxon(context:Context) =

    interface IGenBankTaxon with
        member __.GetSpecies (speciesPattern:string) = 
            let database = context.DatabaseName
            CacheAccess.getSpeciesCollection database speciesPattern
            |> List.map (fun speciesInfo -> 
                                let (speciesID, speciesName, assemblyLookupPath) = speciesInfo
                                new GenBankSpecies(database, speciesID, speciesName, assemblyLookupPath))