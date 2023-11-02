// --------------------------------------------------------------------------------------
// FAKE Build Script.
// --------------------------------------------------------------------------------------

#r "paket: groupref build //"

#if !FAKE
#load ".fake/build.fsx/intellisense.fsx"
#r "netstandard"
#endif

open System
open System.IO
open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet
open Fake.DotNet.Testing
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Tools.Git

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let (!!) includes =
    (!!includes).SetBaseDirectory __SOURCE_DIRECTORY__


// --------------------------------------------------------------------------------------
// Project Information.
// --------------------------------------------------------------------------------------

let project = "BioProviders"
let authors = "Alex Kenna;Samuel Smith;fsprojects contributors"
let summary = "F# library for accessing and manipulating bioinformatic datasets."

let description =
    """
    The BioProviders library provides tools and functionality to simplify accessing and 
    manipulating bioinformatic data. This library includes:

    * GenBankProvider — Type Provider for type-safe access to the genomic sequences and
                        their metadata for over 500,000 species in the GenBank database. 
    * RefSeqProvider — Type Provider for type-safe access to the genomic sequences and
                       their metadata for over 140,000 species in the RefSeq database.
    """

let tags = "F# fsharp data typeprovider bioinformatics genbank refseq"

let gitOwner = "fsprojects"
let gitHome = "https://github.com/" + gitOwner
let gitName = "BioProviders"

let packageProjectUrl = "https://fsprojects.github.io/BioProviders/"
let repositoryType = "git"
let repositoryUrl = "https://github.com/fsprojects/BioProviders"
let license = "MIT"

// Read release notes & version info from RELEASE_NOTES.md
let release = ReleaseNotes.load "RELEASE_NOTES.md"

let isCI = Environment.GetEnvironmentVariable("CI") <> null


// --------------------------------------------------------------------------------------
// Generate Assembly Information.
// --------------------------------------------------------------------------------------

Target.create "AssemblyInfo" (fun _ ->
    for file in !! "src/AssemblyInfo*.fs" do
        let replace (oldValue: string) newValue (str: string) = str.Replace(oldValue, newValue)

        let title =
            Path.GetFileNameWithoutExtension file |> replace "AssemblyInfo" "BioProviders"

        let versionSuffix = ".0"
        let version = release.AssemblyVersion + versionSuffix

        AssemblyInfoFile.createFSharp
            file
            [ AssemblyInfo.Title title
              AssemblyInfo.Product project
              AssemblyInfo.Description summary
              AssemblyInfo.Version version
              AssemblyInfo.FileVersion version ])


// --------------------------------------------------------------------------------------
// Clean Build Results.
// --------------------------------------------------------------------------------------

Target.create "Clean" (fun _ ->
    seq {
        yield! !! "**/bin"
        yield! !! "**/obj"
        yield! !! "**/temp"
    }
    |> Shell.cleanDirs)

Target.create "CleanDocs" (fun _ -> Shell.cleanDirs [ "output" ])


// --------------------------------------------------------------------------------------
// Build Library & Test Projects
// --------------------------------------------------------------------------------------

Target.create "Build" (fun _ ->
    "BioProviders.sln"
    |> DotNet.build (fun o ->
        { o with
            Configuration = DotNet.BuildConfiguration.Release })

    "BioProviders.TestsAndDocs.sln"
    |> DotNet.build (fun o ->
        { o with
            Configuration = DotNet.BuildConfiguration.Release }))

Target.create "RunTests" (fun _ ->
    let setParams (o: DotNet.TestOptions) =
        { o with
            Configuration = DotNet.BuildConfiguration.Release
            Logger = if isCI then Some "GitHubActions" else None }

    "BioProviders.sln" |> DotNet.test setParams

    "BioProviders.TestsAndDocs.sln" |> DotNet.test setParams)


// --------------------------------------------------------------------------------------
// Build Packages.
// --------------------------------------------------------------------------------------

Target.create "Pack" (fun _ ->
    // Format the release notes
    let releaseNotes = release.Notes |> String.concat "\n"

    let properties =
        [ ("Version", release.NugetVersion)
          ("Authors", authors)
          ("PackageProjectUrl", packageProjectUrl)
          ("PackageTags", tags)
          ("RepositoryType", repositoryType)
          ("RepositoryUrl", repositoryUrl)
          ("PackageLicenseExpression", license)
          ("PackageReleaseNotes", releaseNotes)
          ("Summary", summary)
          ("PackageDescription", description) ]

    DotNet.pack
        (fun p ->
            { p with
                Configuration = DotNet.BuildConfiguration.Release
                OutputPath = Some "bin"
                MSBuildParams =
                    { p.MSBuildParams with
                        Properties = properties } })
        "BioProviders.sln")


// --------------------------------------------------------------------------------------
// Generate Documentation.
// --------------------------------------------------------------------------------------

Target.create "GenerateDocs" (fun _ ->
    Shell.cleanDir ".fsdocs"

    let result =
        DotNet.exec
            id
            "fsdocs"
            ("build --properties Configuration=Release --eval --clean --parameters fsdocs-package-version "
             + release.NugetVersion)

    if not result.OK then
        printfn "Errors while generating docs: %A" result.Messages
        failwith "Failed to generate docs")


// --------------------------------------------------------------------------------------
// Help.
// --------------------------------------------------------------------------------------

Target.create "Help" (fun _ ->
    printfn ""
    printfn "  Please specify the target by calling 'build -t <Target>'"
    printfn ""
    printfn "  Targets for building:"
    printfn "  * Build"
    printfn "  * RunTests"
    printfn "  * GenerateDocs"
    printfn "  * Pack (creates package only, doesn't publish)"
    printfn "  * All (calls previous 4)"
    printfn "")

let sourceFiles =
    !! "src/**/*.fs" ++ "src/**/*.fsi" ++ "build.fsx"
    -- "src/**/obj/**/*.fs"
    -- "src/AssemblyInfo*.fs"

Target.create "Format" (fun _ ->
    let result =
        sourceFiles
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> DotNet.exec id "fantomas"

    if not result.OK then
        printfn "Errors while formatting all files: %A" result.Messages)

Target.create "CheckFormat" (fun _ ->
    let result =
        sourceFiles
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> sprintf "%s --check"
        |> DotNet.exec id "fantomas"

    if result.ExitCode = 0 then
        Trace.log "No files need formatting"
    elif result.ExitCode = 99 then
        failwith "Some files need formatting, run `dotnet fake build -t Format` to format them"
    else
        Trace.logf "Errors while formatting: %A" result.Errors
        failwith "Unknown errors while formatting")

Target.create "All" ignore

"Clean" ==> "AssemblyInfo" ==> "CheckFormat" ==> "Build"

"Build" ==> "CleanDocs" ==> "GenerateDocs" ==> "All"

"Build" ==> "Pack" ==> "All"
"Build" ==> "All"
"Build" ==> "RunTests" ==> "All"

Target.runOrDefaultWithArguments "Help"
