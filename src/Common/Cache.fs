namespace BioProviders.Common

open System.IO
open FluentFTP

// --------------------------------------------------------------------------------------
// Cache Interface.
// --------------------------------------------------------------------------------------

type ICache =
    abstract LoadDirectory : string -> Stream
    abstract LoadFile : string -> Stream
    abstract SaveDirectory : string -> FtpStatus
    abstract SaveFile : string -> FtpStatus
    abstract Purge : unit -> unit
    abstract Update : string -> unit

// --------------------------------------------------------------------------------------
// Cache Helpers.
// --------------------------------------------------------------------------------------

module CacheHelpers = 

    let getCacheFilePath (path:string) = 
        let cacheLocation = Path.Combine(Path.GetTempPath(), "BioProviders")
        let cacheFileName = path.Replace("/", " ").Trim().Replace(" ", "-")
    
        Path.Combine(cacheLocation, cacheFileName)

    let loadCacheFile (path:string) =
        let cachePath = getCacheFilePath(path)
        if File.Exists(cachePath) then Some(File.OpenRead(cachePath))
        else None

    let loadCacheDirectory (path:string) =
        let cachePath = getCacheFilePath(path)
        if File.Exists(cachePath) then Some(File.OpenRead(cachePath))
        else None

    let saveCacheFile (path:string) = 
        let cachePath = getCacheFilePath(path)
        FTP.downloadGenBankFlatFile(cachePath, path)

    let saveCacheDirectory (path:string) =
        let cachePath = getCacheFilePath(path)
        FTP.downloadGenBankDirectory(cachePath, path)

    let clearCache () = ()

    let updateCache (path:string) = ()
  
// --------------------------------------------------------------------------------------
// Cache Implementation.
// --------------------------------------------------------------------------------------

open CacheHelpers

type Cache () = 
    interface ICache with 
        member __.SaveFile (path:string) = saveCacheFile path

        member __.SaveDirectory (path:string) = saveCacheDirectory path

        member __.Purge () = clearCache ()

        member __.Update (server:string) = updateCache server

        member this.LoadFile (path:string) =
            match loadCacheFile (path) with
            | Some (data) -> data :> Stream
            | None -> 
                match (this :> ICache).SaveFile (path) with
                | FtpStatus.Success -> (this :> ICache).LoadFile (path)
                | _ -> failwithf "Unable to load or save the file %s." path

        member this.LoadDirectory (path:string) = 
            match loadCacheDirectory (path) with
            | Some (data) -> data :> Stream
            | None ->
                match (this :> ICache).SaveDirectory (path) with
                | FtpStatus.Success -> (this :> ICache).LoadDirectory (path)
                | _ -> failwithf "Unable to load or save the directory %s." path
