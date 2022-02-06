namespace BioProviders.Tests

open NUnit.Framework
open FsUnit
open BioProviders.DesignTime.Context
open BioProviders.Tests.Data


// --------------------------------------------------------------------------------------
// Context Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type Context() = 

    let mutable contexts = []

    let mutable providedType = FsCheck.Gen.sample 0 1 (generateProvidedType ()) |> Seq.head

    let mutable database = FsCheck.Gen.sample 0 1 (generateDatabase ()) |> Seq.head

    let mutable regexTaxonString = FsCheck.Gen.sample 0 1 (generateRegexTaxonString ()) |> Seq.head
    let mutable plainTaxonString = FsCheck.Gen.sample 0 1 (generatePlainTaxonString ()) |> Seq.head
    let mutable emptyTaxonString = FsCheck.Gen.sample 0 1 (generateEmptyString ()) |> Seq.head
    let mutable regexTaxon = FsCheck.Gen.sample 0 1 (generateRegexTaxon ()) |> Seq.head
    let mutable plainTaxon = FsCheck.Gen.sample 0 1 (generatePlainTaxon ()) |> Seq.head
    let mutable emptyTaxon = FsCheck.Gen.sample 0 1 (generateEmptyTaxon ()) |> Seq.head
    
    let mutable regexSpeciesString = FsCheck.Gen.sample 0 1 (generateRegexSpeciesString ()) |> Seq.head
    let mutable plainSpeciesString = FsCheck.Gen.sample 0 1 (generatePlainSpeciesString ()) |> Seq.head
    let mutable emptySpeciesString = FsCheck.Gen.sample 0 1 (generateEmptyString ()) |> Seq.head
    let mutable regexSpecies = FsCheck.Gen.sample 0 1 (generateRegexSpecies ()) |> Seq.head
    let mutable plainSpecies = FsCheck.Gen.sample 0 1 (generatePlainSpecies ()) |> Seq.head
    let mutable emptySpecies = FsCheck.Gen.sample 0 1 (generateEmptySpecies ()) |> Seq.head

    let mutable regexAssemblyString = FsCheck.Gen.sample 0 1 (generateRegexAssemblyString ()) |> Seq.head
    let mutable plainAssemblyString = FsCheck.Gen.sample 0 1 (generatePlainAssemblyString ()) |> Seq.head
    let mutable emptyAssemblyString = FsCheck.Gen.sample 0 1 (generateEmptyString ()) |> Seq.head
    let mutable regexAssembly = FsCheck.Gen.sample 0 1 (generateRegexAssembly ()) |> Seq.head
    let mutable plainAssembly = FsCheck.Gen.sample 0 1 (generatePlainAssembly ()) |> Seq.head
    let mutable emptyAssembly = FsCheck.Gen.sample 0 1 (generateEmptyAssembly ()) |> Seq.head
  
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 1

        contexts <- FsCheck.Gen.sample size 30 (generateContext ())

        providedType <- FsCheck.Gen.sample size count (generateProvidedType ()) |> Seq.head

        database <- FsCheck.Gen.sample size count (generateDatabase ()) |> Seq.head

        regexTaxonString <- FsCheck.Gen.sample size count (generateRegexTaxonString ()) |> Seq.head
        plainTaxonString <- FsCheck.Gen.sample size count (generatePlainTaxonString ()) |> Seq.head
        emptyTaxonString <- FsCheck.Gen.sample size count (generateEmptyString ()) |> Seq.head
        regexTaxon <- FsCheck.Gen.sample size count (generateRegexTaxon ()) |> Seq.head
        plainTaxon <- FsCheck.Gen.sample size count (generatePlainTaxon ()) |> Seq.head
        emptyTaxon <- FsCheck.Gen.sample size count (generateEmptyTaxon ()) |> Seq.head

        regexSpeciesString <- FsCheck.Gen.sample size count (generateRegexSpeciesString ()) |> Seq.head
        plainSpeciesString <- FsCheck.Gen.sample size count (generatePlainSpeciesString ()) |> Seq.head
        emptySpeciesString <- FsCheck.Gen.sample size count (generateEmptyString ()) |> Seq.head
        regexSpecies <- FsCheck.Gen.sample size count (generateRegexSpecies ()) |> Seq.head
        plainSpecies <- FsCheck.Gen.sample size count (generatePlainSpecies ()) |> Seq.head
        emptySpecies <- FsCheck.Gen.sample size count (generateEmptySpecies ()) |> Seq.head

        regexAssemblyString <- FsCheck.Gen.sample size count (generateRegexAssemblyString ()) |> Seq.head
        plainAssemblyString <- FsCheck.Gen.sample size count (generatePlainAssemblyString ()) |> Seq.head
        emptyAssemblyString <- FsCheck.Gen.sample size count (generateEmptyString ()) |> Seq.head
        regexAssembly <- FsCheck.Gen.sample size count (generateRegexAssembly ()) |> Seq.head
        plainAssembly <- FsCheck.Gen.sample size count (generatePlainAssembly ()) |> Seq.head
        emptyAssembly <- FsCheck.Gen.sample size count (generateEmptyAssembly ()) |> Seq.head

    [<Test>]
    member __.``Parse - Species cannot be empty if Assembly provided`` () = 
        (fun () -> (Context.Parse plainTaxonString emptySpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse plainTaxonString emptySpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse regexTaxonString emptySpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse regexTaxonString emptySpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

    [<Test>]
    member __.``Parse - Taxon cannot be empty if Assembly provided`` () = 
        (fun () -> (Context.Parse emptyTaxonString plainSpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptyTaxonString plainSpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptyTaxonString regexSpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptyTaxonString regexSpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptyTaxonString emptySpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

    [<Test>]
    member __.``Parse - Taxon cannot be empty if Species provided`` () = 
        (fun () -> (Context.Parse emptyTaxonString plainSpeciesString emptyAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptyTaxonString regexSpeciesString emptyAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

    [<Test>]
    member __.``Parse - Plain Taxon name`` () = 
        Context.Parse plainTaxonString plainSpeciesString plainAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Pass()
                     | (TaxonRegexName _, _, _) -> Assert.Fail())

        Context.Parse plainTaxonString regexSpeciesString plainAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Pass()
                     | (TaxonRegexName _, _, _) -> Assert.Fail())

        Context.Parse plainTaxonString plainSpeciesString regexAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Pass()
                     | (TaxonRegexName _, _, _) -> Assert.Fail())

        Context.Parse plainTaxonString regexSpeciesString regexAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Pass()
                     | (TaxonRegexName _, _, _) -> Assert.Fail())

    [<Test>]
    member __.``Parse - Regex Taxon name`` () = 
        Context.Parse regexTaxonString plainSpeciesString plainAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Fail()
                     | (TaxonRegexName _, _, _) -> Assert.Pass())

        Context.Parse regexTaxonString regexSpeciesString plainAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Fail()
                     | (TaxonRegexName _, _, _) -> Assert.Pass())

        Context.Parse regexTaxonString plainSpeciesString regexAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Fail()
                     | (TaxonRegexName _, _, _) -> Assert.Pass())

        Context.Parse regexTaxonString regexSpeciesString regexAssemblyString
        |> (function | (TaxonPlainName _, _, _) -> Assert.Fail()
                     | (TaxonRegexName _, _, _) -> Assert.Pass())

    [<Test>]
    member __.``Parse - Plain Species name`` () = 
        Context.Parse plainTaxonString plainSpeciesString plainAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Pass()
                     | (_, SpeciesRegexName _, _) -> Assert.Fail())

        Context.Parse regexTaxonString plainSpeciesString plainAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Pass()
                     | (_, SpeciesRegexName _, _) -> Assert.Fail())

        Context.Parse plainTaxonString plainSpeciesString regexAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Pass()
                     | (_, SpeciesRegexName _, _) -> Assert.Fail())

        Context.Parse regexTaxonString plainSpeciesString regexAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Pass()
                     | (_, SpeciesRegexName _, _) -> Assert.Fail())

    [<Test>]
    member __.``Parse - Regex Species name`` () = 
        Context.Parse plainTaxonString regexSpeciesString plainAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Fail()
                     | (_, SpeciesRegexName _, _) -> Assert.Pass())

        Context.Parse regexTaxonString regexSpeciesString plainAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Fail()
                     | (_, SpeciesRegexName _, _) -> Assert.Pass())

        Context.Parse plainTaxonString regexSpeciesString regexAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Fail()
                     | (_, SpeciesRegexName _, _) -> Assert.Pass())

        Context.Parse regexTaxonString regexSpeciesString regexAssemblyString
        |> (function | (_, SpeciesPlainName _, _) -> Assert.Fail()
                     | (_, SpeciesRegexName _, _) -> Assert.Pass())

    [<Test>]
    member __.``Parse - Plain Assembly name`` () = 
        Context.Parse plainTaxonString plainSpeciesString plainAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Pass()
                     | (_, _, AssemblyRegexName _) -> Assert.Fail())

        Context.Parse regexTaxonString plainSpeciesString plainAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Pass()
                     | (_, _, AssemblyRegexName _) -> Assert.Fail())

        Context.Parse plainTaxonString regexSpeciesString plainAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Pass()
                     | (_, _, AssemblyRegexName _) -> Assert.Fail())

        Context.Parse regexTaxonString regexSpeciesString plainAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Pass()
                     | (_, _, AssemblyRegexName _) -> Assert.Fail())

    [<Test>]
    member __.``Parse - Regex Assembly name`` () = 
        Context.Parse plainTaxonString plainSpeciesString regexAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Fail()
                     | (_, _, AssemblyRegexName _) -> Assert.Pass())

        Context.Parse regexTaxonString plainSpeciesString regexAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Fail()
                     | (_, _, AssemblyRegexName _) -> Assert.Pass())

        Context.Parse plainTaxonString regexSpeciesString regexAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Fail()
                     | (_, _, AssemblyRegexName _) -> Assert.Pass())

        Context.Parse regexTaxonString regexSpeciesString regexAssemblyString
        |> (function | (_, _, AssemblyPlainName _) -> Assert.Fail()
                     | (_, _, AssemblyRegexName _) -> Assert.Pass())

    [<Test>]
    member __.``Parse - Whitespace padding should be removed`` () = 

        let paddedPlainTaxon = " \n\r\t\f" + plainTaxonString + " \n\r\t\f"
        let paddedRegexTaxon = " \n\r\t\f" + regexTaxonString + " \n\r\t\f"
        let paddedPlainSpecies = " \n\r\t\f" + plainSpeciesString + " \n\r\t\f"
        let paddedRegexSpecies = " \n\r\t\f" + regexSpeciesString + " \n\r\t\f"
        let paddedPlainAssembly = " \n\r\t\f" + plainAssemblyString + " \n\r\t\f"
        let paddedRegexAssembly = " \n\r\t\f" + regexAssemblyString + " \n\r\t\f"

        Context.Parse paddedPlainTaxon paddedPlainSpecies paddedPlainAssembly
        |> should equal (TaxonPlainName plainTaxonString, 
                         SpeciesPlainName plainSpeciesString, 
                         AssemblyPlainName plainAssemblyString)

        Context.Parse paddedRegexTaxon paddedPlainSpecies paddedPlainAssembly
        |> should equal (TaxonRegexName regexTaxonString, 
                         SpeciesPlainName plainSpeciesString, 
                         AssemblyPlainName plainAssemblyString)

        Context.Parse paddedPlainTaxon paddedRegexSpecies paddedPlainAssembly
        |> should equal (TaxonPlainName plainTaxonString, 
                         SpeciesRegexName regexSpeciesString, 
                         AssemblyPlainName plainAssemblyString)

        Context.Parse paddedPlainTaxon paddedPlainSpecies paddedRegexAssembly
        |> should equal (TaxonPlainName plainTaxonString, 
                         SpeciesPlainName plainSpeciesString, 
                         AssemblyRegexName regexAssemblyString)

        Context.Parse paddedRegexTaxon paddedRegexSpecies paddedPlainAssembly
        |> should equal (TaxonRegexName regexTaxonString, 
                         SpeciesRegexName regexSpeciesString, 
                         AssemblyPlainName plainAssemblyString)

        Context.Parse paddedRegexTaxon paddedPlainSpecies paddedRegexAssembly
        |> should equal (TaxonRegexName regexTaxonString, 
                         SpeciesPlainName plainSpeciesString, 
                         AssemblyRegexName regexAssemblyString)

        Context.Parse paddedPlainTaxon paddedRegexSpecies paddedRegexAssembly
        |> should equal (TaxonPlainName plainTaxonString, 
                         SpeciesRegexName regexSpeciesString, 
                         AssemblyRegexName regexAssemblyString)

        Context.Parse paddedRegexTaxon paddedRegexSpecies paddedRegexAssembly
        |> should equal (TaxonRegexName regexTaxonString, 
                         SpeciesRegexName regexSpeciesString, 
                         AssemblyRegexName regexAssemblyString)

    [<Test>]
    member __.``Parse - Empty Assembly should be replaced by regex *`` () = 
        Context.Parse plainTaxonString plainSpeciesString emptyAssemblyString
        |> (function | (_, _, AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

        Context.Parse regexTaxonString plainSpeciesString emptyAssemblyString
        |> (function | (_, _, AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

        Context.Parse plainTaxonString regexSpeciesString emptyAssemblyString
        |> (function | (_, _, AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

        Context.Parse regexTaxonString regexSpeciesString emptyAssemblyString
        |> (function | (_, _, AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

    [<Test>]
    member __.``Parse - Empty Species should be replaced by regex * (Given Assembly also empty)`` () = 
        Context.Parse plainTaxonString emptySpeciesString emptyAssemblyString
        |> (function | (_, SpeciesRegexName "*", AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

        Context.Parse regexTaxonString emptySpeciesString emptyAssemblyString
        |> (function | (_, SpeciesRegexName "*", AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

    [<Test>]
    member __.``Parse - Empty Taxon should be replaced by regex * (Given Species and Assembly also empty)`` () = 
        Context.Parse emptyTaxonString emptySpeciesString emptyAssemblyString
        |> (function | (TaxonRegexName "*", SpeciesRegexName "*", AssemblyRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

    [<Test>]
    member __.``Create - AssemblyGroupName should always be all_assembly_versions`` () = 
        contexts
        |> Seq.iter (fun ctx -> 
                        ctx.AssemblyGroupName.ToString() |> should equal "all_assembly_versions")

    [<Test>]
    member __.``GetDatabasePath - Should always begin with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetDatabasePath())
        |> Seq.iter (fun path -> path |> should startWith "/")
    
    [<Test>]
    member __.``GetDatabasePath - Should never end with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetDatabasePath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetTaxonPath - Should always begin with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetTaxonPath())
        |> Seq.iter (fun path -> path |> should startWith "/")
    
    [<Test>]
    member __.``GetTaxonPath - Should never end with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetTaxonPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetSpeciesPath - Should always begin with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetSpeciesPath())
        |> Seq.iter (fun path -> path |> should startWith "/")
    
    [<Test>]
    member __.``GetSpeciesPath - Should never end with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetSpeciesPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetAssemblyPath - Should always begin with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetAssemblyPath())
        |> Seq.iter (fun path -> path |> should startWith "/")
    
    [<Test>]
    member __.``GetAssemblyPath - Should never end with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetAssemblyPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetGenomicGBFFPath - Should always begin with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetGenomicGBFFPath())
        |> Seq.iter (fun path -> path |> should startWith "/")
    
    [<Test>]
    member __.``GetGenomicGBFFPath - Should never end with /`` () =
        contexts
        |> List.map (fun ctx -> ctx.GetGenomicGBFFPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))


// --------------------------------------------------------------------------------------
// Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type Name() = 

    let mutable data = []
    
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count ( generatePlainName () )

    [<Test>]
    member __.``Create - Strings ending in * should return Regex Name`` () =
        data
        |> List.map (fun n -> n + "*")
        |> List.map (fun n -> Name.Create n)
        |> Seq.iter (fun n ->
                        match n with
                        | PlainName _ -> Assert.Fail()
                        | RegexName _ -> Assert.Pass())

    [<Test>]
    member __.``Create - Strings not ending in * should return Plain Name`` () =
        data
        |> List.map (fun n -> Name.Create n)
        |> Seq.iter (fun n ->
                        match n with
                        | PlainName _ -> Assert.Pass()
                        | RegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Strings containing * anywhere other than end should return Plain Name`` () =
        data
        |> List.map (fun n -> n.Insert(System.Random().Next(n.Length), "*"))
        |> List.map (fun n -> Name.Create n)
        |> Seq.iter (fun n ->
                        match n with
                        | PlainName _ -> Assert.Pass()
                        | RegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty string should return Regex Name of *`` () =
        [""; "\t"; " "; "\n"; "  \t \n  "]
        |> List.map (fun n -> Name.Create n)
        |> Seq.iter (fun n ->
                        match n with
                        | PlainName _ -> Assert.Fail()
                        | RegexName _ -> Assert.Pass())

    [<Test>]
    member __.``ToString - If non-empty string used to create Name, should return original string`` () =
        data
        |> List.map (fun n -> Name.Create n)
        |> List.map (fun n -> n.ToString())
        |> should equal data


// --------------------------------------------------------------------------------------
// Database Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type DatabaseName() = 

    [<Test>]
    member _.``ToString - RefSeq should return genomes/refseq`` () =
        let refseq = RefSeq
        refseq.ToString() |> should equal "genomes/refseq"

    [<Test>]
    member _.``ToString - GenBank should return genomes/genbank`` () =
        let genbank = GenBank
        genbank.ToString() |> should equal "genomes/genbank"


// --------------------------------------------------------------------------------------
// Taxon Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type TaxonName() = 

    let mutable data = []
    
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count ( generatePlainTaxonString () )

    [<Test>]
    member __.``Create - Strings ending in * should return Regex Taxon Name`` () =
        data
        |> List.map (fun t -> t + "*")
        |> List.map (fun t -> TaxonName.Create t)
        |> Seq.iter (fun t ->
                        match t with
                        | TaxonPlainName _ -> Assert.Fail()
                        | TaxonRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``Create - Strings not ending in * should return Plain Taxon Name`` () =
        data
        |> List.map (fun t -> TaxonName.Create t)
        |> Seq.iter (fun t ->
                        match t with
                        | TaxonPlainName _ -> Assert.Pass()
                        | TaxonRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Strings containing * anywhere other than end should return Plain Taxon Name`` () =
        data
        |> List.map (fun t -> t.Insert(System.Random().Next(t.Length), "*"))
        |> List.map (fun t -> TaxonName.Create t)
        |> Seq.iter (fun t ->
                        match t with
                        | TaxonPlainName _ -> Assert.Pass()
                        | TaxonRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty string should return Regex Taxon Name of *`` () =
        [""; "\t"; " "; "\n"; "  \t \n  "]
        |> List.map (fun t -> TaxonName.Create t)
        |> Seq.iter (fun t ->
                        match t with
                        | TaxonPlainName _ -> Assert.Fail()
                        | TaxonRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``ToString - Should always return string used to create the Taxon Name`` () =
        data
        |> List.map (fun t -> TaxonName.Create t)
        |> List.map (fun t -> t.ToString())
        |> should equal data


// --------------------------------------------------------------------------------------
// Species Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type SpeciesName() = 

    let mutable data = []
    
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count ( generatePlainSpeciesString () )

    [<Test>]
    member __.``Create - Strings ending in * should return Regex Species Name`` () =
        data
        |> List.map (fun s -> s + "*")
        |> List.map (fun s -> SpeciesName.Create s)
        |> Seq.iter (fun s ->
                        match s with
                        | SpeciesPlainName _ -> Assert.Fail()
                        | SpeciesRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``Create - Strings not ending in * should return Plain Species Name`` () =
        data
        |> List.map (fun s -> SpeciesName.Create s)
        |> Seq.iter (fun s ->
                        match s with
                        | SpeciesPlainName _ -> Assert.Pass()
                        | SpeciesRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Strings containing * anywhere other than end should return Plain Species Name`` () =
        data
        |> List.map (fun s -> s.Insert(System.Random().Next(s.Length), "*"))
        |> List.map (fun s -> SpeciesName.Create s)
        |> Seq.iter (fun s ->
                        match s with
                        | SpeciesPlainName _ -> Assert.Pass()
                        | SpeciesRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty string should return Regex Species Name of *`` () =
        [""; "\t"; " "; "\n"; "  \t \n  "]
        |> List.map (fun s -> SpeciesName.Create s)
        |> Seq.iter (fun s ->
                        match s with
                        | SpeciesPlainName _ -> Assert.Fail()
                        | SpeciesRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``ToString - Should always return string used to create the Species Name`` () =
        data
        |> List.map (fun s -> SpeciesName.Create s)
        |> List.map (fun s -> s.ToString())
        |> should equal data


// --------------------------------------------------------------------------------------
// Assembly Group Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type AssemblyGroupName() = 

    [<Test>]
    member __.``Create - Should always create the same AssemblyGroupName`` () =
        let asm1 = AssemblyGroupName.Create ()
        let asm2 = AssemblyGroupName.Create ()
        asm1 |> should equal asm2

    [<Test>]
    member __.``ToString - Should always be all_assembly_versions`` () =
        [1..20]
        |> List.map (fun _ -> AssemblyGroupName.Create ())
        |> List.map (fun a -> a.ToString())
        |> Seq.iter (fun a -> a |> should equal "all_assembly_versions")


// --------------------------------------------------------------------------------------
// Assembly Name Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type AssemblyName() = 

    let mutable data = []
    
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count ( generatePlainAssemblyString () )

    [<Test>]
    member __.``Create - Strings ending in * should return Regex Assembly Name`` () =
        data
        |> List.map (fun a -> a + "*")
        |> List.map (fun a -> AssemblyName.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AssemblyPlainName _ -> Assert.Fail()
                        | AssemblyRegexName _ -> Assert.Pass())
        
    [<Test>]
    member __.``Create - Strings not ending in * should return Plain Assembly Name`` () =
        data
        |> List.map (fun a -> AssemblyName.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AssemblyPlainName _ -> Assert.Pass()
                        | AssemblyRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Strings containing * anywhere other than end should return Plain Assembly Name`` () =
        data
        |> List.map (fun a -> a.Insert(System.Random().Next(a.Length), "*"))
        |> List.map (fun a -> AssemblyName.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AssemblyPlainName _ -> Assert.Pass()
                        | AssemblyRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty string should return Regex Assembly Name of *`` () =
        [""; "\t"; " "; "\n"; "  \t \n  "]
        |> List.map (fun a -> AssemblyName.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AssemblyPlainName _ -> Assert.Fail()
                        | AssemblyRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``ToString - Should always return string used to create the Assembly Name`` () =
        data
        |> List.map (fun a -> AssemblyName.Create a)
        |> List.map (fun a -> a.ToString())
        |> should equal data
