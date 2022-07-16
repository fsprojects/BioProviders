namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes

// --------------------------------------------------------------------------------------
// Design-Time Context State Types.
// --------------------------------------------------------------------------------------

module Context =

    /// <summary>
    /// The context for type generation.
    /// </summary>
    type Context = 
        { ProvidedType: ProvidedTypeDefinition
          DatabaseName: DatabaseName
          SpeciesName: SpeciesName
          Accession: Accession }

        static member Parse (species:string) (assembly:string) =
            let speciesName = species.ToString() |> (fun s -> s.Trim().ToLower())
            let assemblyName = assembly.ToString() |> (fun s -> s.Trim())

            match speciesName, assemblyName with
            | "", _ when assemblyName <> "" -> invalidArg "Species" "Species must not be empty if an Assembly is provided."
            | _ -> (SpeciesName.Create speciesName, Accession.Create assemblyName)

        static member Create (providedType) (database) (species) (assembly) = 
            { ProvidedType = providedType
              DatabaseName = database
              SpeciesName = species
              Accession = assembly }

        
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

        member this.GetPath() = 
            match this with
            | GenBank -> "genomes/all/GCA"
            | RefSeq -> "genomes/all/GCF"


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
    /// Typed representation of the Assembly.
    /// </summary>
    and Accession = 
    | AccessionPlainName of string
    | AccessionRegexName of string

        static member Create (assembly:string) = 
            match Name.Create assembly with
            | PlainName name -> AccessionPlainName name
            | RegexName name -> AccessionRegexName name

        override this.ToString() = 
            match this with
            | AccessionPlainName name -> name
            | AccessionRegexName name -> name
