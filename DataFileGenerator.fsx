#r "nuget: CsvHelper"
#r "nuget: FluentFTP, 34.0.1"

open CsvHelper
open System.IO
open CsvHelper.Configuration
open System.Globalization
open System.IO.Compression
open FluentFTP

// ------ Record types used for reading and writing files ------
// Rows for the original GenBank TSV file.
type GenBankRow = {
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

// Character array.
let characters = Seq.concat [['#']; ['a' .. 'z']]

// Base URL for GenBank files on the FTP server. Used to delete the correct
// number of characters from the FTP path.
let genBankURL = "https://ftp.ncbi.nlm.nih.gov/genomes/all/GCA/"

// ------ Functions for generating and writing data files ------
// Function for matching the first character of a species name.
// Characters that are not letters are treated as a '#'.
let getLookupCharacter (name: string) =
    match name.Chars(0) with
    | c when System.Char.IsLetter(c) -> System.Char.ToLower(c)
    | _ -> '#'

// Generate a list of distinct species with a unique species ID number for
// each, starting with the specified character.
let getSpeciesList (filteredList : GenBankRow list) (count : int) =
    // Get a distinct list of species names.
    // Also sorts it into alphabetical order.
    let distinctList = List.sort (List.distinct (List.map (fun row -> row.organism_name) filteredList))
    // Return a list of SpeciesRows.
    List.mapi (fun i name -> { species_id = (i + count).ToString() ; species_name = name }) distinctList

// Generate a list of assemblies belonging to the species of a specified
// character, with the correct ID number for their species.
let getAssemblyList (filteredList : GenBankRow list) (speciesList : SpeciesRow list) =
    // Function for finding a species name match for a certain row.
    let findNameMatch row = List.tryFind (fun species -> species.species_name.Equals(row.organism_name)) speciesList
    // Filter the CSV rows by those that have one of the organism names in the
    // supplied list, and that have a FTP path that isn't "na".
    let listWithPaths = List.filter (fun (row : GenBankRow) -> not (row.ftp_path.Equals("na"))) filteredList
    // Function for sorting a list of AssemblyRows. It should be in the order
    // of species IDs, and then the accessions if the IDs are the same.
    let sortAssemblies (assembly1 : AssemblyRow) (assembly2: AssemblyRow) =
        match assembly1.species_id.CompareTo(assembly2.species_id) with
        | 0 -> assembly1.assembly_accession.CompareTo(assembly2.assembly_accession)
        | result -> result
    // Return a (sorted) list of AssemblyRows.
    List.sortWith sortAssemblies (List.map (fun row -> { species_id = ((findNameMatch row).Value.species_id) ; assembly_accession = row.assembly_accession ; ftp_path = row.ftp_path.[(String.length genBankURL)..] } ) listWithPaths)

// Compresses a written text file using GZip compression, writes it to a new
// file and deletes the original.
let compressFile (filename : string) =
    let originalFile = File.OpenRead(filename)
    let gZipFile = (new FileInfo(filename + ".gz")).Create()
    let gZipStream = new GZipStream(gZipFile, CompressionMode.Compress)
    originalFile.CopyTo(gZipStream)
    gZipStream.Flush()
    gZipStream.Close()
    originalFile.Close()
    File.Delete(filename)

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

let downloadedFilePath = (Path.Combine(Path.GetTempPath(), "BioProviders_Build", "downloaded_list.txt"))

// ------ Main operations ------

printfn "------------ Starting operations to generate GenBank data file lists for BioProviders. ------------"

printfn "------ Downloading GenBank summary file to %s...... ------" downloadedFilePath

let status = downloadNCBIFile (downloadedFilePath, "/genomes/genbank/assembly_summary_genbank.txt")

match status with
    | FtpStatus.Failed -> failwith "------ Failed to download file from NCBI FTP server. ------"
    | FtpStatus.Skipped -> printfn "------ File already downloaded. ------"
    | _ -> printfn "------ File downloaded successfully. ------"

printfn "------ Loading in GenBank assembly summary TSV... ------"

// Load in the GenBank file.
(*let reader = new StreamReader("D:\\Users\\Samuel Smith_3\\Documents\\RA\\Downloads\\GenBank FTP\\assembly_summary_genbank_25-09-2023.txt")*)
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
let records = Seq.toList (csv.GetRecords<GenBankRow>())

// Show how many records were loaded.
printfn "Loaded %i records." (List.length records)
printfn "------ TSV loaded successfully. ------"

// Generate a list of species and assembies for the given characater, and write
// them to a file. An integer acculmulator is used to ensure unique numerical
// IDs for all distinct species.
let generateLists (fullList : GenBankRow list) (acc : int) (character : char) =
    // Filter the full list of assemblies for only those that have an organism
    // name matching the current character.
    let filteredList = List.filter (fun row -> (getLookupCharacter row.organism_name).Equals(character)) fullList

    // Generate the lists of species and assemblies for the given character.
    let speciesList = (getSpeciesList filteredList acc)
    let assemblyList = (getAssemblyList filteredList speciesList)

    // Generate the filenames for the species and assembly files.
    let speciesFilename = $"./build/data/genbank-species-{character}.txt"
    let assemblyFilename = $"./build/data/genbank-assemblies-{character}.txt"

    // Write the species entries to a file.
    let speciesWriter = new StreamWriter(speciesFilename)
    let speciesCsv = new CsvWriter(speciesWriter, CultureInfo.InvariantCulture)
    speciesCsv.WriteRecords(speciesList)
    speciesWriter.Flush()
    speciesWriter.Close()
    compressFile(speciesFilename)

    // Write the assembly entries to a file.
    let assemblyWriter = new StreamWriter(assemblyFilename)
    let assemblyCsv = new CsvWriter(assemblyWriter, CultureInfo.InvariantCulture)
    assemblyCsv.WriteRecords(assemblyList)
    assemblyWriter.Flush()
    assemblyWriter.Close()
    compressFile(assemblyFilename)

    // Add the number of new species to the acculmulator, to start at the
    // correct number for the next character.
    acc + List.length speciesList

printfn "------ Generating new lists from loaded GenBank assembly list... ------"
printfn "------ Successfully generated lists for %i species. ------" (Seq.fold (generateLists records) 0 characters)
printfn "------------ All operations completed. ------------"