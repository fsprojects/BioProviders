namespace BioProviders.Common

open FluentFTP

// --------------------------------------------------------------------------------------
// FTP Access for Type Providers.
// --------------------------------------------------------------------------------------

[<RequireQualifiedAccess>]
module FTP = 

    /// <summary>
    /// Creates and uses a connection with the NCBI FTP server.
    /// </summary>
    /// <param name="callback">The callback to be used on the connection.</param>
    let internal useNCBIConnection (callback) = 
        let serverBaseLocation = "ftp://ftp.ncbi.nlm.nih.gov" 
        use client = new FtpClient(serverBaseLocation)
        client.Connect()
        callback client

    /// <summary>
    /// Downloads a file from the NCBI FTP server to the local file system.
    /// </summary>
    /// <param name="localPath">The local path to which the file is downloaded.</param>
    /// <param name="remotePath">The NCBI path to the file being downloaded.</param>
    let downloadNCBIFile (localPath:string, remotePath:string) = 
        let downloadFile (connection: FtpClient) = 
            connection.DownloadFile(localPath, remotePath)
        useNCBIConnection downloadFile
