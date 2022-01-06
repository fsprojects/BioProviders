// --------------------------------------------------------------------------------------
// Design-Time Context State Types.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes

module Context =

    type DatabaseName = 
        | RefSeq
        | GenBank
        with override this.ToString () =  
                match this with
                | RefSeq -> "/genomes/refseq"
                | GenBank -> "/genomes/genbank"
        
    type TaxonName = TaxonName of string
        with override this.ToString () = 
                let getValue (TaxonName t) = t
                this |> getValue 
        
    type SpeciesName = SpeciesName of string
        with override this.ToString () = 
                let getValue (SpeciesName t) = t
                this |> getValue 
    
    type AssemblyGroupName = AssemblyGroupName of string
        with override this.ToString () = 
                let getValue (AssemblyGroupName t) = t
                this |> getValue 
        
    type AssemblyName = AssemblyName of string
        with override this.ToString () = 
                let getValue (AssemblyName t) = t
                this |> getValue 
    
        
    type AssemblyContext = 
        { ProvidedType: ProvidedTypeDefinition
          DatabaseName: DatabaseName
          TaxonName: TaxonName
          SpeciesName: SpeciesName
          AssemblyGroupName: AssemblyGroupName
          AssemblyName: AssemblyName }

        static member Parse taxon species assembly = 
            let taxonName = taxon.ToString() |> (fun s -> s.Trim())
            let speciesName = species.ToString() |> (fun s -> s.Trim())
            let assemblyName = assembly.ToString() |> (fun s -> s.Trim())

            match taxonName, speciesName, assemblyName with
            | "", _, _ -> invalidArg "Taxon" "Taxon cannot be empty."
            | _, "", _ -> invalidArg "Species" "Species cannot be empty."
            | _, _, "" -> invalidArg "Assembly" "Assembly cannot be empty."
            | _ -> (TaxonName taxonName, SpeciesName speciesName, AssemblyName assemblyName)
    
        static member Create providedType database taxon species assembly = 
            { ProvidedType = providedType
              DatabaseName = database
              TaxonName = taxon
              SpeciesName = species
              AssemblyGroupName = AssemblyGroupName ( "all_assembly_versions" )
              AssemblyName = assembly }

        member this.GetURL () = 
            String.concat "/" [ 
                this.DatabaseName.ToString();
                this.TaxonName.ToString();
                this.SpeciesName.ToString();
                this.AssemblyGroupName.ToString();
                this.AssemblyName.ToString()
            ]
