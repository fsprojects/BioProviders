// --------------------------------------------------------------------------------------
// FTP Access for Type Providers.
// --------------------------------------------------------------------------------------

namespace BioProviders.Common

open FluentFTP

[<RequireQualifiedAccess>]
module FTP = 

    /// <summary>
    /// Creates and uses a connection with the FTP GenBank server.
    /// </summary>
    /// <param name="callback">The callback to be used on the connection.</param>
    let internal useConnection (callback) = 
        let serverBaseLocation = "ftp://ftp.ncbi.nlm.nih.gov" 
        use client = new FtpClient(serverBaseLocation)
        client.Connect()
        callback client

    /// <summary>
    /// Downloads a GZIP GenBank File to the local file system.
    /// </summary>
    /// <param name="localPath">The path to which the GenBank Flat File is being downloaded.</param>
    /// <param name="remotePath">The path to the GenBank Flat File being downloaded.</param>
    let downloadGenBankFlatFile (localPath:string, remotePath:string) = 
        let downloadFile (connection: FtpClient) = 
            connection.DownloadFile(localPath, remotePath)
        useConnection downloadFile
