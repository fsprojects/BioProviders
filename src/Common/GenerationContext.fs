namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Generation Context State Types.
// --------------------------------------------------------------------------------------

module Context =

    // ----------------------------------------------------------------------------------
    // Base Name Type.
    // ----------------------------------------------------------------------------------
       
    /// <summary>
    /// The base name type. Responsible for determining if a string is regex in nature.
    /// </summary>
    type Name = 
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


    // ----------------------------------------------------------------------------------
    // Database Name Type.
    // ----------------------------------------------------------------------------------

    /// <summary>
    /// Typed representation of the Database.
    /// </summary>
    type DatabaseName = 
    | GenBank
    | RefSeq

        member this.GetPath() = 
            match this with
            | GenBank -> "/genomes/all/GCA"
            | RefSeq -> "/genomes/all/GCF"


    // ----------------------------------------------------------------------------------
    // Species Types.
    // ----------------------------------------------------------------------------------

    /// <summary>
    /// Typed representation of the Species name.
    /// </summary>
    type SpeciesName = 
    | SpeciesPlainName of string
    | SpeciesRegexName of string

        static member Create (species:string) = 
            match Name.Create species with
            | PlainName name -> SpeciesPlainName name
            | RegexName name -> SpeciesRegexName name

        override this.ToString() = 
            match this with
            | SpeciesPlainName name -> name
            | SpeciesRegexName name -> name.Substring(0, name.Length - 1) + ".*"


    /// <summary>
    /// Typed representation of the Species context.
    /// </summary>
    type Species =
        { SpeciesName : SpeciesName 
          SpeciesID : string option 
          LookupPath : string option }


    // ----------------------------------------------------------------------------------
    // Accession Types.
    // ----------------------------------------------------------------------------------

    /// <summary>
    /// Typed representation of the Accession name.
    /// </summary>
    type AccessionName = 
    | AccessionPlainName of string
    | AccessionRegexName of string

        static member Create (assembly:string) = 
            match Name.Create assembly with
            | PlainName name -> AccessionPlainName name
            | RegexName name -> AccessionRegexName name

        override this.ToString() = 
            match this with
            | AccessionPlainName name -> name
            | AccessionRegexName name -> name.Substring(0, name.Length - 1) + ".*"


    /// <summary>
    /// Typed representation of the Species context.
    /// </summary>
    type Accession =
        { AccessionName : AccessionName 
          AssemblyName : string option 
          AssemblyPath : string option
          GenBankFlatFilePath : string option }


    // --------------------------------------------------------------------------------------
    // Generation Context.
    // --------------------------------------------------------------------------------------

    /// <summary>
    /// The context for type generation.
    /// </summary>
    type Context = 
        { DatabaseName: DatabaseName
          Species: Species
          Accession: Accession }

        static member Parse (species:string) (assembly:string) =
            let speciesName = species.ToString() |> (fun s -> s.Trim().ToLower())
            let accessionName = assembly.ToString() |> (fun s -> s.Trim().ToLower())

            match speciesName, accessionName with
            | "", _ when accessionName <> "" -> invalidArg "Species" "Species must not be empty if an Assembly is provided."
            | _ -> (SpeciesName.Create speciesName, AccessionName.Create accessionName)

        static member CreateDefault (database:DatabaseName) (species:SpeciesName) (accessionName:AccessionName) = 
            { DatabaseName = database
              Species = 
                { SpeciesName = species
                  SpeciesID = None 
                  LookupPath = None }
              Accession = 
                { AccessionName = accessionName 
                  AssemblyName = None
                  AssemblyPath = None
                  GenBankFlatFilePath = None } }
