// --------------------------------------------------------------------------------------
// Design-Time Context State Types.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes

module Context =

    /// <summary>
    /// The context for type generation.
    /// </summary>
    type Context = 
    | PartialContext of PartialContext
    | CompleteContext of CompleteContext

        static member Parse (taxon:string) (species:string) (assembly:string) =
            let taxonName = taxon.ToString() |> (fun s -> s.Trim())
            let speciesName = species.ToString() |> (fun s -> s.Trim())
            let assemblyName = assembly.ToString() |> (fun s -> s.Trim())

            match taxonName, speciesName, assemblyName with
            | "", _, _ when assemblyName <> "" -> invalidArg "Taxon" "Taxon must not be empty if an Assembly is provided."
            | _, "", _ when assemblyName <> "" -> invalidArg "Species" "Species must not be empty if an Assembly is provided."
            | "", _, _ when speciesName <> "" -> invalidArg "Taxon" "Taxon must not be empty if a Species is provided."
            | _ -> (TaxonName.Create taxonName, SpeciesName.Create speciesName, AssemblyName.Create assemblyName)

        static member Create (providedType) (database) (taxon) (species) (assembly) = 
            match taxon, species, assembly with
            | TaxonPlainName _, SpeciesPlainName _, AssemblyPlainName _ ->
                CompleteContext ( CompleteContext.Create (providedType) 
                                                         (database) 
                                                         (taxon) 
                                                         (species) 
                                                         (assembly) )
            | _ -> PartialContext( PartialContext.Create (providedType) 
                                                         (database) 
                                                         (taxon) 
                                                         (species) 
                                                         (assembly) )


    /// <summary>
    /// A partially complete context - that is, a context containing one or more
    /// regex attributes.
    /// </summary>
    and PartialContext =
        { ProvidedType: ProvidedTypeDefinition
          DatabaseName: DatabaseName
          TaxonName: TaxonName
          SpeciesName: SpeciesName
          AssemblyGroupName: AssemblyGroupName
          AssemblyName: AssemblyName }

        static member Create (providedType) (database) (taxon) (species) (assembly) = 
            match taxon, species, assembly with
            | TaxonPlainName _, SpeciesPlainName _, AssemblyPlainName _ -> 
                failwith "Failed to create Partial Context. One or more of Taxon, Species, or Assembly were expected to be of Regex form."
            | _ -> { ProvidedType = providedType
                     DatabaseName = database
                     TaxonName = taxon
                     SpeciesName = species
                     AssemblyGroupName = AssemblyGroupName ( "all_assembly_versions" )
                     AssemblyName = assembly }  

        member private this.GetDatabasePath () = 
            "/" + this.DatabaseName.ToString()

        member private this.GetTaxonPath () = 
            String.concat "/" [
                this.GetDatabasePath ()
                this.TaxonName.ToString()
            ]

        member private this.GetSpeciesPath () = 
            String.concat "/" [
                this.GetTaxonPath ()
                this.SpeciesName.ToString()
                this.AssemblyGroupName.ToString()
            ]

        member this.GetCompletedPath () =
            match this.TaxonName, this.SpeciesName, this.AssemblyName with
            | TaxonRegexName _, _, _ -> this.GetDatabasePath ()
            | _, SpeciesRegexName _, _ -> this.GetTaxonPath ()
            | _, _, AssemblyRegexName _ -> this.GetSpeciesPath ()
            | _ -> failwith "Partial Context error. One or more of Taxon, Species, or Assembly were expected to be of Regex form."

    /// <summary>
    /// The complete context. All attributes are entirely determined, that is, there
    /// are no regex attributes.
    /// </summary>
    and CompleteContext = 
       { ProvidedType: ProvidedTypeDefinition
         DatabaseName: DatabaseName
         TaxonName: TaxonName
         SpeciesName: SpeciesName
         AssemblyGroupName: AssemblyGroupName
         AssemblyName: AssemblyName }

        static member Create (providedType) (database) (taxon) (species) (assembly) = 
            match taxon, species, assembly with
            | TaxonRegexName _, _, _ -> 
                invalidArg "Taxon" "Failed to create Complete Context. Taxon was expected to be plain."
            | _, SpeciesRegexName _, _ -> 
                invalidArg "Species" "Failed to create Complete Context. Species was expected to be plain."
            | _, _, AssemblyRegexName _ -> 
                invalidArg "Assembly" "Failed to create Complete Context. Assembly was expected to be plain"
            | TaxonPlainName _, SpeciesPlainName _, AssemblyPlainName _ -> 
                { ProvidedType = providedType
                  DatabaseName = database
                  TaxonName = taxon
                  SpeciesName = species
                  AssemblyGroupName = AssemblyGroupName ( "all_assembly_versions" )
                  AssemblyName = assembly }

        member this.GetAssemblyPath () = 
            "/" + String.concat "/" [
                this.DatabaseName.ToString()
                this.TaxonName.ToString()
                this.SpeciesName.ToString()
                this.AssemblyGroupName.ToString()
                this.AssemblyName.ToString()
            ]

        member this.GetGBFFPath () = 
            String.concat "/" [
                this.GetAssemblyPath ()
                this.AssemblyName.ToString() + "_genomic.gbff.gz"
            ]

        
    /// <summary>
    /// The base name type. Responsible for determining if a string is regex in nature.
    /// </summary>
    and Name = 
    | PlainName of string
    | RegexName of string

        static member Create (name:string) = 
            match name with
            | _ when name.Length = 0 -> RegexName ("*")
            | _ when name.[name.Length - 1] = '*' -> RegexName name
            | _ -> PlainName name

        override this.ToString() = 
            match this with
            | PlainName name -> name
            | RegexName name -> name


    /// <summary>
    /// Typed representation of the Database.
    /// </summary>
    and DatabaseName = 
    | GenBank
    | RefSeq

        override this.ToString() = 
            match this with
            | GenBank -> "genomes/genbank"
            | RefSeq -> "genomes/refseq"


    /// <summary>
    /// Typed representation of the Taxon.
    /// </summary>
    and TaxonName = 
    | TaxonPlainName of string
    | TaxonRegexName of string

        static member Create (taxon:string) = 
            match Name.Create taxon with
            | PlainName name -> TaxonPlainName name
            | RegexName name -> TaxonRegexName name

        override this.ToString() = 
            match this with
            | TaxonPlainName name -> name
            | TaxonRegexName name -> name


    /// <summary>
    /// Typed representation of the Species.
    /// </summary>
    and SpeciesName = 
    | SpeciesPlainName of string
    | SpeciesRegexName of string

        static member Create (species:string) = 
            match Name.Create species with
            | PlainName name -> SpeciesPlainName name
            | RegexName name -> SpeciesRegexName name

        override this.ToString() = 
            match this with
            | SpeciesPlainName name -> name
            | SpeciesRegexName name -> name


    /// <summary>
    /// Typed representation of the Assembly Group. Currently, only 
    /// all_available_assemblies is available.
    /// </summary>
    and AssemblyGroupName = AssemblyGroupName of string with

        static member Create () = 
            AssemblyGroupName "all_assembly_versions"

        override this.ToString() = 
            let getValue (AssemblyGroupName t) = t
            this |> getValue 


    /// <summary>
    /// Typed representation of the Assembly.
    /// </summary>
    and AssemblyName = 
    | AssemblyPlainName of string
    | AssemblyRegexName of string

        static member Create (assembly:string) = 
            match Name.Create assembly with
            | PlainName name -> AssemblyPlainName name
            | RegexName name -> AssemblyRegexName name

        override this.ToString() = 
            match this with
            | AssemblyPlainName name -> name
            | AssemblyRegexName name -> name
