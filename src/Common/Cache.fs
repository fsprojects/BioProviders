namespace BioProviders.Common

open System.Reflection
open System.Text.RegularExpressions
open System.IO
open FluentFTP
open BioProviders.Common.Context

// --------------------------------------------------------------------------------------
// Cache Interface.
// --------------------------------------------------------------------------------------

type private ICache =
    abstract LoadDirectory : string -> Stream
    abstract LoadFile : string -> Stream
    abstract SaveDirectory : string -> FtpStatus
    abstract SaveFile : string -> FtpStatus
    abstract Purge : unit -> unit


// --------------------------------------------------------------------------------------
// Cache Helpers.
// --------------------------------------------------------------------------------------

module private CacheHelpers = 
    
    module General =

        let getCacheFilePath (path:string) = 
            let cacheLocation = Path.Combine(Path.GetTempPath(), "BioProviders")
            let cacheFileName = path.Replace("/", " ").Trim().Replace(" ", "-")
            Path.Combine(cacheLocation, cacheFileName)

        let loadFile (path:string) = 
            if File.Exists(path) then Some(File.OpenRead(path))
            else None

        let loadCacheFile (path:string) =
            let cachePath = getCacheFilePath(path)
            loadFile cachePath

        let saveCacheFile (path:string) = 
            let cachePath = getCacheFilePath(path)
            FTP.downloadGenBankFlatFile(cachePath, path)

        let saveCacheDirectory (path:string) =
            let cachePath = getCacheFilePath(path)
            FTP.downloadGenBankDirectory(cachePath, path)

        let clearCache () = ()


    module GenBank =
        
        let private getLookupCharacter (name:string) =
            match name.Chars(0) with
            | c when System.Char.IsLetter(c) -> c
            | _ -> '#'

        let private getContentPath (fileName:string) =
            let assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Path.Combine(assemblyDirectory, "data", fileName)

        let private getSpeciesLookupPath (speciesName:string) =
            let character = getLookupCharacter speciesName
            getContentPath $"genbank-species-{character}.txt.gz"

        let private getAssemblyLookupPath (speciesName:string) =
            let character = getLookupCharacter speciesName
            getContentPath $"genbank-assemblies-{character}.txt.gz"

        let private getSpeciesID (speciesName:string) = 
            let speciesLookupFile = getSpeciesLookupPath speciesName

            match General.loadFile speciesLookupFile with
            | None -> invalidOp "Could not load species lookup file."
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
                                    checkLine()
                                else info.[0]
                            else invalidOp "The species could not be found. Check the species name is correct."
                        checkLine())

        let parseAssemblyLine (database:DatabaseName) (assemblyLine:string) =
            let assemblyInfo = assemblyLine.Split(',')
            let accession = assemblyInfo.[1]
            let assemblyPath = $"{database.GetPath()}/{assemblyInfo.[2]}"
            let assemblyName = assemblyPath.Split('/') |> (fun parts -> parts.[parts.Length - 1])

            (accession, assemblyName, assemblyPath)

        let getAssembly (database:DatabaseName) (species:SpeciesName) (accession:Accession) = 
            let speciesID = getSpeciesID (species.ToString())
            let assemblyLookupFile = getAssemblyLookupPath (species.ToString())

            match General.loadFile assemblyLookupFile with
            | None -> invalidOp "Could not load assembly lookup file."
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
                                    checkLine()
                                else parseAssemblyLine database line
                            else invalidOp "The assembly could not be found. Check that the accession is correct."
                        checkLine())

        let getAssemblies (database:DatabaseName) (assemblyLookupPath:string) (speciesID:string) (accessionPattern:string) = 
            match General.loadFile assemblyLookupPath with
            | None -> invalidOp "Could not load assembly lookup file."
            | Some data -> 
                data :> Stream
                |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress)) 
                |> (fun gzipStream -> 
                        use stream = new StreamReader(gzipStream)
                        let rec checkLine (assemblies:(string*string*string) list) =
                            if not(stream.EndOfStream && assemblies.Length = 0) then
                                let line = stream.ReadLine()
                                let info = line.Split(',')
                                if info.[0] <> speciesID || not(Regex.IsMatch(info.[1].ToLower(), accessionPattern)) then
                                    if assemblies.Length = 0 then checkLine assemblies
                                    else assemblies
                                else assemblies @ [ parseAssemblyLine database line ] |> checkLine 
                            else invalidOp "No assemblies matching the accession pattern could be found. Check the accession pattern is correct."
                        checkLine [])

        let getSpecies (species:SpeciesName) = 
            let speciesName = species.ToString()
            let speciesID = getSpeciesID (speciesName)
            let assemblyLookupFile = getAssemblyLookupPath (speciesName)
            
            (speciesID, speciesName, assemblyLookupFile)

        let getSpeciesCollection (speciesPattern:string) =
            let speciesLookupPath = getSpeciesLookupPath speciesPattern
            let assemblyLookupPath = getAssemblyLookupPath speciesPattern

            match General.loadFile speciesLookupPath with
            | None -> invalidOp "Could not load assembly lookup file."
            | Some data -> 
                data :> Stream
                |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress)) 
                |> (fun gzipStream -> 
                        use stream = new StreamReader(gzipStream)
                        let rec checkLine (species:(string*string*string) list) =
                            if not(stream.EndOfStream && species.Length = 0) then
                                let line = stream.ReadLine()
                                let info = line.Split(',')
                                if not(Regex.IsMatch(info.[1].ToLower(), speciesPattern)) then
                                    if species.Length = 0 then checkLine species
                                    else species
                                else 
                                    let speciesID = info.[0]
                                    let speciesName = info.[1]
                                    species @ [ (speciesID ,speciesName , assemblyLookupPath) ] |> checkLine 
                            else invalidOp "No species matching the pattern could be found. Check the species pattern is correct."
                        checkLine [])

  
// --------------------------------------------------------------------------------------
// Cache Implementation.
// --------------------------------------------------------------------------------------

open CacheHelpers.General

type private Cache () = 
    interface ICache with 
        member __.SaveFile (path:string) = saveCacheFile path

        member __.SaveDirectory (path:string) = saveCacheDirectory path

        member __.Purge () = clearCache ()

        member this.LoadFile (path:string) =
            match loadCacheFile (path) with
            | Some data -> data :> Stream
            | None -> 
                match (this :> ICache).SaveFile (path) with
                | FtpStatus.Success -> (this :> ICache).LoadFile (path)
                | _ -> failwithf "Unable to load or save the file %s." path

        member this.LoadDirectory (path:string) = 
            match loadCacheFile (path) with
            | Some data -> data :> Stream
            | None ->
                match (this :> ICache).SaveDirectory (path) with
                | FtpStatus.Success -> (this :> ICache).LoadDirectory (path)
                | _ -> failwithf "Unable to load or save the directory %s." path


// --------------------------------------------------------------------------------------
// Cache Access.
// --------------------------------------------------------------------------------------

module CacheAccess =

    let loadFile (path:string) = 
        (new Cache() :> ICache).LoadFile path

    let getAssembly (database:DatabaseName) (species:SpeciesName) (accession:Accession) = 
        match database with
        | RefSeq _ -> failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.GenBank.getAssembly database species accession

    let getAssemblies (database:DatabaseName) (assemblyLookupPath:string) (speciesID:string) (accessionPattern:string) =
        match database with
        | RefSeq _ -> failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.GenBank.getAssemblies database assemblyLookupPath speciesID accessionPattern

    let getSpecies (database:DatabaseName) (species:SpeciesName) = 
        match database with
        | RefSeq _ -> failwith "RefSeq is not currently supported."
        | GenBank _ -> CacheHelpers.GenBank.getSpecies species        

    let getSpeciesCollection (database:DatabaseName) (speciesPattern:string) = 
        match database, speciesPattern with
        | RefSeq _, _ -> failwith "RefSeq is not currently supported."
        | GenBank _, ".*" -> failwith "A species pattern is required."
        | GenBank _, _ -> CacheHelpers.GenBank.getSpeciesCollection speciesPattern
