namespace BioProviders.Common

open System.IO
open FluentFTP

// --------------------------------------------------------------------------------------
// Cache Interface.
// --------------------------------------------------------------------------------------

type ICache =
    abstract LoadFile : string -> Stream
    abstract SaveFile : string -> FtpStatus
    abstract Purge : unit -> unit

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

    let saveCacheFile (path:string) = 
        let cachePath = getCacheFilePath(path)
        FTP.downloadGenBankFlatFile(cachePath, path)

    let clearCache () = ()
  
// --------------------------------------------------------------------------------------
// Cache Implementation.
// --------------------------------------------------------------------------------------

open CacheHelpers

type Cache () = 
    interface ICache with 
        member _.SaveFile (path:string) =  saveCacheFile path

        member _.Purge () = clearCache ()

        member this.LoadFile (path:string) =
            match loadCacheFile (path) with
            | Some (data) -> data :> Stream
            | None -> 
                match (this :> ICache).SaveFile (path) with
                | FtpStatus.Success -> (this :> ICache).LoadFile (path)
                | _ -> failwithf "Unable to load or save the path %s." path
