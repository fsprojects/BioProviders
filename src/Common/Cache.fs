namespace BioProviders.Common

open System.Text.RegularExpressions
open System.IO
open FluentFTP

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

    let getLookupCharacter (name:string) =
        match name.Chars(0) with
        | c when System.Char.IsLetter(c) -> c
        | _ -> '#'

    let getSpeciesLookupFilePath (speciesName:string) =
        let character = getLookupCharacter speciesName
        __SOURCE_DIRECTORY__ + @$"..\..\..\data\genbank-species-{character}.txt.gz"

    let getAssemblyLookupFilePath (speciesName:string) =
        let character = getLookupCharacter speciesName
        __SOURCE_DIRECTORY__ + @$"..\..\..\data\genbank-assemblies-{character}.txt.gz"

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
  
// --------------------------------------------------------------------------------------
// Cache Implementation.
// --------------------------------------------------------------------------------------

open CacheHelpers

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

    let private getSpeciesID (speciesName:string) = 
        let speciesLookupFile = getSpeciesLookupFilePath speciesName

        match CacheHelpers.loadFile speciesLookupFile with
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
                            if info.[1] <> speciesName then
                                checkLine()
                            else info.[0]
                        else invalidOp "The species could not be found. Check the species name is correct."
                    checkLine())

    let getAssemblyPath (speciesName:string) (accession:string) = 
        let speciesID = getSpeciesID speciesName
        let assemblyLookupFile = getAssemblyLookupFilePath speciesName

        match CacheHelpers.loadFile assemblyLookupFile with
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
                            if info.[0] <> speciesID || info.[1] <> accession then
                                checkLine()
                            else info.[2]
                        else invalidOp "The assembly could not be found. Check the accession is correct."
                    checkLine())

    let getAssemblies (speciesName:string) (accessionPattern:string) = 
        let speciesID = getSpeciesID speciesName
        let assemblyLookupFile = getAssemblyLookupFilePath speciesName

        match CacheHelpers.loadFile assemblyLookupFile with
        | None -> invalidOp "Could not load assembly lookup file."
        | Some data -> 
            data :> Stream
            |> (fun stream -> new Compression.GZipStream(stream, Compression.CompressionMode.Decompress)) 
            |> (fun gzipStream -> 
                    use stream = new StreamReader(gzipStream)
                    let rec checkLine (assemblies:(string*string) list) =
                        if not(stream.EndOfStream && assemblies.Length = 0) then
                            let line = stream.ReadLine()
                            let info = line.Split(',')
                            if info.[0] <> speciesID || not(Regex.IsMatch(info.[1], accessionPattern)) then
                                if assemblies.Length = 0 then checkLine assemblies
                                else assemblies
                            else assemblies @ [(info.[1], info.[2])] |> checkLine 
                        else invalidOp "No assemblies matching the accession pattern could be found. Check the accession pattern is correct."
                    checkLine [])
