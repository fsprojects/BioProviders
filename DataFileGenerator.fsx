#r "nuget: CsvHelper"
#r "nuget: FluentFTP, 34.0.1"
#r "nuget: Goblinfactory.Konsole"

open CsvHelper
open System.IO
open CsvHelper.Configuration
open System.Globalization
open System.IO.Compression
open FluentFTP
open Konsole

// ------ Record types used for reading and writing files ------
// Rows for the original GenBank TSV file.
type FileRow = {
    assembly_accession : string
    bioproject : string
    biosample : string
    wgs_master : string
    refseq_category : string
    taxid : string
    species_taxid : string
    organism_name : string
    infraspecific_name : string
    isolate : string
    version_status : string
    assembly_level : string
    release_type : string
    genome_rep : string
    seq_rel_date : string
    asm_name : string
    asm_submitter : string
    gbrs_paired_asm : string
    paired_asm_comp : string
    ftp_path : string
    excluded_from_refseq : string
    relation_to_type_material : string
    asm_not_live_date : string
    assembly_type : string
    group : string
    genome_size : string
    genome_size_ungapped : string
    gc_percent : string
    replicon_count : string
    scaffold_count : string
    contig_count : string
    annotation_provider : string
    annotation_name : string
    annotation_date : string
    total_gene_count : string
    protein_coding_gene_count : string
    non_coding_gene_count : string
    pubmed_id : string
}

// Rows for the generated assembly TSV file.
type AssemblyRow = {
    species_id : string
    assembly_accession : string
    ftp_path : string
}

// Rows for the generated species TSV file.
type SpeciesRow = {
    species_id : string
    species_name : string
}

/// Typed representation of an NCBI Database. NCBI contains two main genome databases
/// GenBank and RefSeq.
type DatabaseName =
    | GenBank
    | RefSeq

    // Returns the base path of the files of each database. Used to remove the
    // necessary characters from the URLs in the original assembly list when
    // creating the new lists.
    member this.GetBasePath() =
        match this with
        | GenBank -> "https://ftp.ncbi.nlm.nih.gov/genomes/all/GCA/"
        | RefSeq -> "https://ftp.ncbi.nlm.nih.gov/genomes/all/GCF/"

    // Returns the location of the assembly file on the FTP server for the
    // database. Does not include the host path.
    member this.GetAssemblyFilePath() =
        match this with
        | GenBank -> "/genomes/genbank/assembly_summary_genbank.txt"
        | RefSeq -> "/genomes/refseq/assembly_summary_refseq.txt"

    // Returns the name of the database as a string.
    member this.GetName() =
        match this with
        | GenBank -> "GenBank"
        | RefSeq -> "RefSeq"

    // Returns the filename of the assembly file.
    member this.GetFilename() =
        match this with
        | GenBank -> "assembly_summary_genbank.txt"
        | RefSeq -> "assembly_summary_refseq.txt"

// Character array.
let characters = Seq.concat [['#']; ['a' .. 'z']]

// ------ Functions for generating and writing data files ------
// Function for matching the first character of a species name.
// Characters that are not letters are treated as a '#'.
let getLookupCharacter (name: string) =
    match name.Chars(0) with
    | c when System.Char.IsLetter(c) -> System.Char.ToLower(c)
    | _ -> '#'

// Generate a list of distinct species with a unique species ID number for
// each, starting with the specified character.
// Species rows have two properties:
// - The species ID; and
// - The species name.
let getSpeciesList (filteredList : FileRow list) (count : int) =

    // Get a distinct list of species names.
    // Also sorts it into alphabetical order.
    let distinctList = List.sort (List.distinct (List.map (fun row -> row.organism_name) filteredList))

    // Return a list of SpeciesRows.
    List.mapi (fun i name -> { species_id = (i + count).ToString() ; species_name = name }) distinctList

// Generate a list of assemblies belonging to the species of a specified
// character, with the correct ID number for their species.
// Assembly rows have three properties:
// - The species ID;
// - The assembly's accession ID; and
// - A path for the FTP location on NCBI's servers. This path does not include
//   the base path as the type provider adds that itself.
let getAssemblyList (database : DatabaseName) (filteredList : FileRow list) (speciesList : SpeciesRow list) =

    // Function for finding a species name match for a certain row.
    let findNameMatch row = List.tryFind (fun species -> species.species_name.Equals(row.organism_name)) speciesList

    // Filter the CSV rows by those that have one of the organism names in the
    // supplied list, and that have a FTP path that isn't "na".
    let listWithPaths = List.filter (fun (row : FileRow) -> not (row.ftp_path.Equals("na"))) filteredList

    // Function for sorting a list of AssemblyRows. It should be in the order
    // of species IDs, and then the accessions if the IDs are the same.
    let sortAssemblies (assembly1 : AssemblyRow) (assembly2: AssemblyRow) =
        match assembly1.species_id.CompareTo(assembly2.species_id) with
        | 0 -> assembly1.assembly_accession.CompareTo(assembly2.assembly_accession)
        | result -> result

    // Return a (sorted) list of AssemblyRows.
    List.sortWith sortAssemblies (List.map (fun row -> { species_id = ((findNameMatch row).Value.species_id) ; assembly_accession = row.assembly_accession ; ftp_path = row.ftp_path.[(String.length (database.GetBasePath()))..] } ) listWithPaths)

// Compresses a written text file using GZip compression, writes it to a new
// file and deletes the original.
let compressFile (filename : string) =

    // Open the original file.
    let originalFile = File.OpenRead(filename)

    // Create a stream for a new GZip file.
    let gZipFile = (new FileInfo(filename + ".gz")).Create()
    let gZipStream = new GZipStream(gZipFile, CompressionMode.Compress)

    // Send the original file to the GZip stream to create the compressed
    // version.
    originalFile.CopyTo(gZipStream)

    // Ensure everything is written to the file before closing it.
    gZipStream.Flush()
    gZipStream.Close()

    // Close the original file and delete it.
    originalFile.Close()
    File.Delete(filename)

// Function for writing a list of species or assemblies to a CSV under GZip
// compression.
let writeFile (filename : string) list =

    // Write the records to the file as comma separated values.
    let writer = new StreamWriter(filename)
    let csv = new CsvWriter(writer, CultureInfo.InvariantCulture)
    csv.WriteRecords(list)

    // Ensure everything is written to the file before closing it.
    writer.Flush()
    writer.Close()

    // Call the function to compress the file.
    compressFile(filename)

// ------ FTP functions ------

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
// - Otherwise: try to resume existing file (in case it wasn't
//   downloaded fully before).
let isNewerFile (localPath: string) (remotePath: string) (connection: FtpClient) =
    if (not (File.Exists(localPath))) then
        FtpLocalExists.Overwrite
    else
        match File.GetLastWriteTime(localPath) > connection.GetModifiedTime(remotePath) with
        | true ->
            printfn "Previously downloaded file is the most current version. Will continue download if required."
            FtpLocalExists.Append
        | _ ->
            printfn "Remote file is newer than previously downloaded file. Will redownload."
            FtpLocalExists.Overwrite

/// Downloads a file from the NCBI FTP server to the local file system.
let downloadNCBIFile (localPath: string, remotePath: string) =
    let downloadFile (connection: FtpClient) =

        // Check if there's a newer file first.
        let operation = isNewerFile localPath remotePath connection

        // Controls the progress bar for downloads.
        let progressBar = new ProgressBar(100)
        let progress = new System.Action<FtpProgress>(fun x ->
            match x.Progress with
            | 100.0 -> progressBar.Refresh(100, "Complete.")
            | _ -> progressBar.Refresh(int x.Progress, "Downloading...")
        )

        // Check for changed file as well as verification.
        connection.DownloadFile(
            localPath,
            remotePath,
            operation,
            FtpVerify.Retry,
            progress
        )

    useNCBIConnection downloadFile

// Creates the path for saving a downloaed NCBI file.
let createDownloadPath (database : DatabaseName) =
    (Path.Combine(Path.GetTempPath(), "BioProviders_Build", (database.GetFilename())))

// ------ Parsing operations ------

// Download the corresponding assembly file from the GenBank FTP server and
// parse it into a set of records.
let getFtpList (database : DatabaseName) =
    let downloadedFilePath = createDownloadPath database
    printfn "%s summary file will be downloaded to %s." (database.GetName()) downloadedFilePath

    // Attempt to download the file, and then check the status of the download.
    let status = downloadNCBIFile (downloadedFilePath, (database.GetAssemblyFilePath()))
    match status with
        | FtpStatus.Failed -> failwith "Failed to download file from NCBI FTP server."
        | FtpStatus.Skipped -> printfn "File already downloaded."
        | _ -> printfn "File downloaded successfully."

    printfn "Loading in %s assembly summary TSV..." (database.GetName())

    // Load in the GenBank file.
    let reader = new StreamReader(downloadedFilePath)

    // A function to skip lines that start with ##, to ignore the comment.
    let skipFunction (args : ShouldSkipRecordArgs) =
        args.Row[0].StartsWith("##")

    // Configuration for the CSV reader. It:
    // - Chooses tab as the delimiter;
    // - Sets the mode to no escape to ignore quotes;
    // - Uses the above function to skip comment lines; and
    // - Clear the # symbol on any headers.
    let config = new CsvConfiguration(CultureInfo.InvariantCulture)
    config.Delimiter <- "\t"
    config.Mode <- CsvMode.NoEscape
    config.ShouldSkipRecord <- new ShouldSkipRecord(skipFunction)
    config.PrepareHeaderForMatch <- fun args -> args.Header.TrimStart('#')

    // Create a CSV reader object and get all records in the loaded file.
    let csv = new CsvReader(reader, config)
    let records = Seq.toList (csv.GetRecords<FileRow>())

    // Show how many records were loaded.
    printfn "%s TSV loaded successfully with a total of %i records." (database.GetName()) (List.length records)

    // Close the file and return the records.
    reader.Close()
    records


// Generate a list of species and assembies for the given characater, and write
// them to a file. An integer acculmulator is used to ensure unique numerical
// IDs for all distinct species.
let generateDatabaseLists location (database : DatabaseName) (fullList : FileRow list) (progressBar : ProgressBar) (acc : int) (character : char) =

    // Update the progress bar.
    progressBar.Refresh((Seq.findIndex ((=) character) characters), $"Processing \"{character}\" species")

    // Filter the full list of assemblies for only those that have an organism
    // name matching the current character.
    let filteredList = List.filter (fun row -> (getLookupCharacter row.organism_name).Equals(character)) fullList

    // Generate the lists of species and assemblies for the given character.
    let speciesList = (getSpeciesList filteredList acc)
    let assemblyList = (getAssemblyList database filteredList speciesList)

    // Ensure that the destination folder exists.
    Directory.CreateDirectory(location) |> ignore

    // Generate the filenames for the species and assembly files.
    let speciesFilename = $"{location}{(database.GetName().ToLower())}-species-{character}.txt"
    let assemblyFilename = $"{location}{(database.GetName().ToLower())}-assemblies-{character}.txt"

    // Write the species entries to a file.
    writeFile speciesFilename speciesList

    // Write the assembly entries to a file.
    writeFile assemblyFilename assemblyList

    // Add the number of new species to the acculmulator, to start at the
    // correct number for the next character.
    acc + List.length speciesList

// Attempts to delete a downloaded NCBI file.
let tryDelete database =
    let filename = createDownloadPath database

    // Attempt to delete the file.
    try
        File.Delete(filename)
        printfn "Deleted downloaded file."
    with
    | :? IOException as ex ->
        printfn "Could not delete downloaded file because of exception \"%s\". %s will need to be deleted manually."  ex.Message filename


// Generates the lists for the specified database.
let generateLists clearCache location (database : DatabaseName) =
    printfn "------ Creating lists for %s... ------" (database.GetName())
    try
        // Generate the list of records from a downloaded TSV.
        let records = getFtpList database
        printfn "Generating new lists from loaded %s assembly list..." (database.GetName())

        // Set up a progress bar to show the progress of parsing records.
        let progressBar = new ProgressBar(Seq.length characters)

        // Parse the records to extract the correct properties and save
        // them as new compressed lists.
        printfn "Generated lists for %i species." (Seq.fold (generateDatabaseLists location database records progressBar) 0 characters)
        progressBar.Refresh(Seq.length characters, "All species complete.")
        printfn "------ %s operations successful. ------" (database.GetName())
        if (clearCache) then tryDelete database
    with
    | _ as ex ->
        printfn "Encountered exception \"%s\" while trying to generate lists." ex.Message
        printfn "------ %s operations failed. ------" (database.GetName())

        // If the operation failed, it might not be to do with the download, so
        // the user might want to keep the file for another attempt.
        if (clearCache) then
            printfn "Should the downloaded file be deleted? (y/n)"
            let userKey = System.Console.ReadKey().KeyChar
            match userKey with
                | 'y' | 'Y' -> tryDelete database
                | 'n' | 'N' -> ()
                | _ -> printfn "Defaulted to \"n\"."

// ------ Main program ------
let args = fsi.CommandLineArgs |> Array.tail

// Check if the script was executed with a "-saveToTemp" argument.
// This saves the files to the "BioProviders" folder in AppData\Local that the
// type provider uses.
let targetFolder =
    match Seq.tryFind (fun arg -> arg.Equals("-saveToCache")) args with
    | Some value -> Path.Combine(Path.GetTempPath(), "BioProviders\\")
    | None -> "./build/data/"

// Check if the script was executed with a "-keepDownloads" argument.
// This keeps the files that were downloaded to generate the lists in
// a "BioProviders_Build" folder in AppData\Local.
let clearCache =
    match Seq.tryFind (fun arg -> arg.Equals("-keepDownloads")) args with
    | Some value -> false
    | None -> true

// Begin the process of generating lists.
printfn "------------ Starting operations to generate GenBank and RefSeq data file lists for BioProviders. ------------"
printfn "Save location is %s" targetFolder
generateLists clearCache targetFolder GenBank
generateLists clearCache targetFolder RefSeq
printfn "------------ All operations completed. ------------"