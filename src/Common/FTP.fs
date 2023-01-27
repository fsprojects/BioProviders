namespace BioProviders.Common

open FluentFTP

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

    /// Downloads a file from the NCBI FTP server to the local file system.
    let downloadNCBIFile (localPath: string, remotePath: string) =
        let downloadFile (connection: FtpClient) =
            connection.DownloadFile(localPath, remotePath)

        useNCBIConnection downloadFile
