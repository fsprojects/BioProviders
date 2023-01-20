namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Generation Context State Types.
// --------------------------------------------------------------------------------------

module Context =

    // ----------------------------------------------------------------------------------
    // Base Name Type.
    // ----------------------------------------------------------------------------------
       
    /// The underlying Name type. Used to determine whether a string follows a regex 
    /// pattern supported by a Type Provider.
    type Name = 
    | PlainName of string
    | RegexName of string

        /// Creates a Name type. If a string is empty, or its last character is '*', the
        /// string is a RegexName. Otherwise, the string is a PlainName.
        static member Create (name:string) = 
            match name with
            | _ when name.Length = 0 -> RegexName ("*")
            | _ when name.[name.Length - 1] = '*' -> RegexName name
            | _ -> PlainName name

        /// Converts a Name type to a string.
        override this.ToString() = 
            match this with
            | PlainName name -> name
            | RegexName name -> name


    // ----------------------------------------------------------------------------------
    // Database Name Type.
    // ----------------------------------------------------------------------------------

    /// Typed representation of an NCBI Database. NCBI contains two main genome databases
    /// GenBank and RefSeq. 
    type DatabaseName = 
    | GenBank
    | RefSeq

        /// Determines the NCBI FTP server path to the appropriate database.
        member this.GetPath() = 
            match this with
            | GenBank -> "/genomes/all/GCA"
            | RefSeq -> "/genomes/all/GCF"


    // ----------------------------------------------------------------------------------
    // Species Types.
    // ----------------------------------------------------------------------------------

    /// Typed representation of the Species name.
    type SpeciesName = 
    | SpeciesPlainName of string
    | SpeciesRegexName of string

        /// Creates a Species Name type. Returns SpeciesRegexName if the species name 
        /// follows a regex format. Otherwise, returns SpeciesPlainName.
        static member Create (species:string) = 
            match Name.Create species with
            | PlainName name -> SpeciesPlainName name
            | RegexName name -> SpeciesRegexName name

        /// Converts a Species Name type to a string. For regex names, the final '*'
        /// character is replaced by '.*' to follow correct regex formatting.
        override this.ToString() = 
            match this with
            | SpeciesPlainName name -> name
            | SpeciesRegexName name -> name.Substring(0, name.Length - 1) + ".*"


    // ----------------------------------------------------------------------------------
    // Accession Types.
    // ----------------------------------------------------------------------------------

    /// Typed representation of the Accession name.
    and AccessionName = 
    | AccessionPlainName of string
    | AccessionRegexName of string

        /// Creates an Accession Name type. Returns AccessionRegexName if the species
        /// name follows a regex format. Otherwise, returns AccessionPlainName.
        static member Create (assembly:string) = 
            match Name.Create assembly with
            | PlainName name -> AccessionPlainName name
            | RegexName name -> AccessionRegexName name

        /// Converts an Accession Name type to a string. For regex names, the final '*'
        /// character is replaced by '.*' to follow correct regex formatting.
        override this.ToString() = 
            match this with
            | AccessionPlainName name -> name
            | AccessionRegexName name -> name.Substring(0, name.Length - 1) + ".*"


    // --------------------------------------------------------------------------------------
    // Generation Context.
    // --------------------------------------------------------------------------------------

    /// The context for type generation.
    type Context = 
        { DatabaseName: DatabaseName
          SpeciesName: SpeciesName
          Accession: AccessionName }

        /// Parses a species and accession string and returns the corresponding Species and
        /// Accession types.
        static member Parse (species:string) (accession:string) =
            let speciesName = species.ToString() |> (fun s -> s.Trim().ToLower())
            let accessionName = accession.ToString() |> (fun s -> s.Trim().ToLower())

            SpeciesName.Create speciesName, AccessionName.Create accessionName
