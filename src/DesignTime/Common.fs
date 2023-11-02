namespace BioProviders.Common

open System.Reflection
open System.Text.RegularExpressions
open System.IO
open FluentFTP

open FSharp.Data


// --------------------------------------------------------------------------------------
// Project Helpers.
// --------------------------------------------------------------------------------------

module Helpers =

    /// Parses an optional string. Returns the None option type if the provided string
    /// is null or empty. Returns the Some option type containing the provided string
    /// otherwise.
    let parseOptionString (str: string) =
        match System.String.IsNullOrEmpty(str) with
        | true -> None
        | _ -> Some str

    /// Parses an optional list. Returns the None option type if the provided list
    /// is empty. Returns the Some option type containing the provided list otherwise.
    let parseOptionList (lst: 'a list) =
        match lst.Length with
        | 0 -> None
        | _ -> Some lst

    /// Parses an optional date. Returns the None option type if the provided date
    /// is the default DateTime object (i.e., 1/01/0001 12:00:00 AM). Returns the Some
    /// option type containing the provided date otherwise.
    let parseOptionDate (date: System.DateTime) =
        match date = new System.DateTime() with
        | true -> None
        | _ -> Some date


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
        static member Create(name: string) =
            match name with
            | _ when name.Length = 0 -> RegexName("*")
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

        // Returns the name of the database as a string.
        override this.ToString() =
            match this with
            | GenBank -> "GenBank"
            | RefSeq -> "RefSeq"


    // ----------------------------------------------------------------------------------
    // Species Types.
    // ----------------------------------------------------------------------------------

    /// Typed representation of the Species name.
    type SpeciesName =
        | SpeciesPlainName of string
        | SpeciesRegexName of string

        /// Creates a Species Name type. Returns SpeciesRegexName if the species name
        /// follows a regex format. Otherwise, returns SpeciesPlainName.
        static member Create(species: string) =
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
        static member Create(assembly: string) =
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
        static member Parse (species: string) (accession: string) =
            let speciesName = species.ToString() |> (fun s -> s.Trim().ToLower())
            let accessionName = accession.ToString() |> (fun s -> s.Trim().ToLower())

            SpeciesName.Create speciesName, AccessionName.Create accessionName


        /// Creates the context type given a Database, Species, and Accession.
        static member Create (database: DatabaseName) (species: SpeciesName) (accession: AccessionName) =
            { DatabaseName = database
              SpeciesName = species
              Accession = accession }


// --------------------------------------------------------------------------------------
// FTP Access for Type Providers.
// --------------------------------------------------------------------------------------

[<RequireQualifiedAccess>]
module FTP =

    /// Creates and uses a connection with the NCBI FTP server.
    let internal useNCBIConnection (callback) =
        let serverBaseLocation = "ftp://ftp.ncbi.nlm.nih.gov"
        use client = new FtpClient(serverBaseLocation)
        client.Connect()
        callback client

    // Checks if a file exists and if so, whether it is older than the remote
    // file.
    // - If a file doesn't exist, or is older: return to overwrite existing
    //   file.
    // - Otherwise: return to resume existing file (in case it wasn't
    //   downloaded fully before).
    let isNewerFile (localPath: string) (remotePath: string) (connection: FtpClient) =
        if (not (File.Exists(localPath))) then
            FtpLocalExists.Overwrite
        else
            match File.GetLastWriteTime(localPath) > connection.GetModifiedTime(remotePath) with
            | true -> FtpLocalExists.Append
            | _ -> FtpLocalExists.Overwrite

    /// Downloads a file from the NCBI FTP server to the local file system.
    let downloadNCBIFile (localPath: string, remotePath: string) =
        let downloadFile (connection: FtpClient) =

            // Check for changed file as well as verification.
            connection.DownloadFile(
                localPath,
                remotePath,
                (isNewerFile localPath remotePath connection),
                FtpVerify.Retry
            )

        useNCBIConnection downloadFile


// --------------------------------------------------------------------------------------
// Cache Interface.
// --------------------------------------------------------------------------------------

open Context

type private ICache =
    abstract LoadFile: string -> Stream
    abstract SaveFile: string -> FtpStatus
    abstract Purge: unit -> unit
    abstract PurgeOld: float -> unit


// --------------------------------------------------------------------------------------
// Cache Helpers.
// --------------------------------------------------------------------------------------

module private CacheHelpers =

    let private getCacheFilePath (path: string) =
        let cacheLocation = Path.Combine(Path.GetTempPath(), "BioProviders")
        let cacheFileName = path.Replace("/", " ").Trim().Replace(" ", "-")
        Path.Combine(cacheLocation, cacheFileName)

    let private loadFile (path: string) =
        if File.Exists(path) then
            Some(File.OpenRead(path))
        else
            None

    let internal loadCacheFile (path: string) =
        let cachePath = getCacheFilePath (path)
        loadFile cachePath

    let internal saveCacheFile (path: string) =
        let cachePath = getCacheFilePath (path)
        FTP.downloadNCBIFile (cachePath, path)

    let internal clearCache () =
        let cacheLocation = Path.Combine(Path.GetTempPath(), "BioProviders")

        if Directory.Exists cacheLocation then
            let cacheFiles = Directory.GetFiles cacheLocation
            Seq.iter (fun file -> File.Delete(file)) cacheFiles

    let internal clearCacheOld (days: float) =
        let cutOffDate = System.DateTime.Now.AddDays(-days)
        let cacheLocation = Path.Combine(Path.GetTempPath(), "BioProviders")

        if Directory.Exists cacheLocation then
            let cacheFiles = Directory.GetFiles cacheLocation

            Seq.iter
                (fun file ->
                    match File.GetLastAccessTime(file) < cutOffDate with
                    | true -> File.Delete(file)
                    | _ -> ())
                cacheFiles

    // Used to download an assembly list from a remote server.
    let private saveAssemblyList (path: string) =
        let cachePath = getCacheFilePath (path)

        try
            // At the moment, use the base URL for the raw .gz files in the
            // BioProviders repository.
            let url =
                sprintf "https://github.com/fsprojects/BioProviders/raw/main/build/data/%s" path

            let data = Http.Request(url).Body

            match data with
            | Binary bytes ->
                // Create the BioProviders directory if it doesn't exist yet.
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "BioProviders"))
                |> ignore

                File.WriteAllBytes(cachePath, bytes)
                true
            | _ ->
                failwith (
                    sprintf "Could not download remote file %s to %s - did not recieve binary content." path cachePath
                )

                false
        with ex ->
            failwith (
                sprintf
                    "Could not download remote file %s to %s, because of the following exception: %s"
                    path
                    cachePath
                    ex.Message
            )

            false

    // Used to load a data file referring to the location of assemblies on
    // GenBank's FTP server. If the file does not exist in the cache
    // location, attempts to download it from the FTP server (with the
    // above function).
    let private loadAssemblyList (path: string) =

        let fullPath = getCacheFilePath path

        // Read the existing file if the data file has already been
        // downloaded.
        if File.Exists(fullPath) then
            Some(File.OpenRead(fullPath))
        else
            match saveAssemblyList path with
            | false -> None
            | _ -> Some(File.OpenRead(fullPath))

    let private getLookupCharacter (name: string) =
        match name.Chars(0) with
        | c when System.Char.IsLetter(c) -> c
        | _ -> '#'

    let private getSpeciesLookupPath (database: DatabaseName) (speciesName: string) =
        let character = getLookupCharacter speciesName
        $"{(database.ToString()).ToLower()}-species-{character}.txt.gz"

    let private getAssemblyLookupPath (database: DatabaseName) (speciesName: string) =
        let character = getLookupCharacter speciesName
        $"{(database.ToString()).ToLower()}-assemblies-{character}.txt.gz"

    let private parseAssemblyLine (database: DatabaseName) (assemblyLine: string) =
        let assemblyInfo = assemblyLine.Split(',')
        let accession = assemblyInfo.[1]
        let assemblyPath = $"{database.GetPath()}/{assemblyInfo.[2]}"

        let assemblyName =
            assemblyPath.Split('/') |> (fun parts -> parts.[parts.Length - 1])

        (accession, assemblyName, assemblyPath)

    let private getSpeciesID (database: DatabaseName) (speciesName: string) =
        let speciesLookupFile = getSpeciesLookupPath database speciesName

        match loadAssemblyList speciesLookupFile with
        | None -> invalidOp $"Could not load {database.ToString()} species lookup file."
        | Some data ->
            data :> Stream
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (fun gzipStream ->
                use stream = new StreamReader(gzipStream)

                let rec checkLine () =
                    if not stream.EndOfStream then
                        let line = stream.ReadLine()
                        let info = line.Split(',')

                        if info.[1].ToLower() <> speciesName then
                            checkLine ()
                        else
                            info.[0]
                    else
                        invalidOp
                            $"The species could not be found. Check the species name is correct and it is a valid {database.ToString()} species."

                checkLine ())

    let getAssembly (database: DatabaseName) (species: SpeciesName) (accession: AccessionName) =
        let speciesID = getSpeciesID database (species.ToString())
        let assemblyLookupFile = getAssemblyLookupPath database (species.ToString())

        match loadAssemblyList assemblyLookupFile with
        | None -> invalidOp $"Could not load {database.ToString()} assembly lookup file."
        | Some data ->
            data :> Stream
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (fun gzipStream ->
                use stream = new StreamReader(gzipStream)

                let rec checkLine () =
                    if not stream.EndOfStream then
                        let line = stream.ReadLine()
                        let info = line.Split(',')

                        if info.[0] <> speciesID || info.[1].ToLower() <> (accession.ToString()) then
                            checkLine ()
                        else
                            parseAssemblyLine database line
                    else
                        invalidOp
                            $"The assembly could not be found. Check that the accession is correct and it is a valid {database.ToString()} accession."

                checkLine ())

    let getAssemblyCollection
        (database: DatabaseName)
        (assemblyLookupPath: string)
        (speciesID: string)
        (accessionPattern: string)
        =
        match loadAssemblyList assemblyLookupPath with
        | None -> invalidOp $"Could not load {database.ToString()} assembly lookup file."
        | Some data ->
            data :> Stream
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (fun gzipStream ->
                use stream = new StreamReader(gzipStream)

                let rec checkLine (assemblies: (string * string * string) list) =
                    if not (stream.EndOfStream && assemblies.Length = 0) then
                        let line = stream.ReadLine()
                        let info = line.Split(',')

                        if
                            info.[0] <> speciesID
                            || not (Regex.IsMatch(info.[1].ToLower(), accessionPattern))
                        then
                            if assemblies.Length = 0 then
                                checkLine assemblies
                            else
                                assemblies
                        else
                            assemblies @ [ parseAssemblyLine database line ] |> checkLine
                    else
                        invalidOp
                            "No assemblies matching the accession pattern could be found. Check the accession pattern is correct."

                checkLine [])

    let getSpecies (database: DatabaseName) (species: SpeciesName) =
        let speciesName = species.ToString()
        let speciesID = getSpeciesID database (speciesName)
        let assemblyLookupFile = getAssemblyLookupPath database (speciesName)

        (speciesID, speciesName, assemblyLookupFile)

    let getSpeciesCollection (database: DatabaseName) (speciesPattern: string) =
        let speciesLookupPath = getSpeciesLookupPath database speciesPattern
        let assemblyLookupPath = getAssemblyLookupPath database speciesPattern

        match loadAssemblyList speciesLookupPath with
        | None -> invalidOp $"Could not load {database.ToString()} assembly lookup file."
        | Some data ->
            data :> Stream
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress))
            |> (fun gzipStream ->
                use stream = new StreamReader(gzipStream)

                let rec checkLine (species: (string * string * string) list) =
                    if not (stream.EndOfStream && species.Length = 0) then
                        let line = stream.ReadLine()
                        let info = line.Split(',')

                        if not (Regex.IsMatch(info.[1].ToLower(), speciesPattern)) then
                            if species.Length = 0 then checkLine species else species
                        else
                            let speciesID = info.[0]
                            let speciesName = info.[1]
                            species @ [ (speciesID, speciesName, assemblyLookupPath) ] |> checkLine
                    else
                        invalidOp
                            "No species matching the pattern could be found. Check the species pattern is correct."

                checkLine [])


// --------------------------------------------------------------------------------------
// Cache Implementation.
// --------------------------------------------------------------------------------------

//open CacheHelpers.General
open CacheHelpers

type private Cache() =
    interface ICache with
        member __.SaveFile(path: string) = saveCacheFile path

        member __.Purge() = clearCache ()

        member __.PurgeOld(days) = clearCacheOld days

        member this.LoadFile(path: string) =
            match loadCacheFile (path) with
            | Some data -> data :> Stream
            | None ->
                match (this :> ICache).SaveFile(path) with
                | FtpStatus.Success -> (this :> ICache).LoadFile(path)
                | _ -> failwithf "Unable to load or save the file %s." path


// --------------------------------------------------------------------------------------
// Cache Access.
// --------------------------------------------------------------------------------------

module CacheAccess =

    let loadFile (path: string) = (new Cache() :> ICache).LoadFile path

    let getAssembly (database: DatabaseName) (species: SpeciesName) (accession: AccessionName) =
        match database with
        | RefSeq _ -> CacheHelpers.getAssembly database species accession //CacheHelpers.RefSeq.getAssembly database species accession //failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.getAssembly database species accession //CacheHelpers.GenBank.getAssembly database species accession

    let getAssemblies
        (database: DatabaseName)
        (assemblyLookupPath: string)
        (speciesID: string)
        (accessionPattern: string)
        =
        match database with
        | RefSeq _ -> CacheHelpers.getAssemblyCollection database assemblyLookupPath speciesID accessionPattern //CacheHelpers.RefSeq.getAssemblies database assemblyLookupPath speciesID accessionPattern //failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.getAssemblyCollection database assemblyLookupPath speciesID accessionPattern //CacheHelpers.GenBank.getAssemblies database assemblyLookupPath speciesID accessionPattern

    let getSpecies (database: DatabaseName) (species: SpeciesName) =
        match database with
        | RefSeq _ -> CacheHelpers.getSpecies database species //CacheHelpers.RefSeq.getSpecies species //failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.getSpecies database species //CacheHelpers.GenBank.getSpecies species

    let getSpeciesCollection (database: DatabaseName) (speciesPattern: string) =
        match database, speciesPattern with
        | RefSeq _, ".*" -> failwith "A species pattern is required."
        | RefSeq _, _ -> CacheHelpers.getSpeciesCollection database speciesPattern //CacheHelpers.RefSeq.getSpeciesCollection speciesPattern //failwith "RefSeq is not currently supported."
        | GenBank _, ".*" -> failwith "A species pattern is required."
        | GenBank _, _ -> CacheHelpers.getSpeciesCollection database speciesPattern //CacheHelpers.GenBank.getSpeciesCollection speciesPattern

    let deleteOldFiles = (new Cache() :> ICache).PurgeOld(90)
