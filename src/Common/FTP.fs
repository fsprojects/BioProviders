namespace BioProviders.Common

open FluentFTP
// Added by Samuel Smith n7581769.
open System.IO

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

    // Added by Samuel Smith n7581769.
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

            // Samuel Smith n7581769.
            // Added check for changed file as well as verification.
            connection.DownloadFile(localPath, remotePath, (isNewerFile localPath remotePath connection), FtpVerify.Retry)

        useNCBIConnection downloadFile
