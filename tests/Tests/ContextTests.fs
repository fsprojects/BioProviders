namespace BioProviders.Tests

open NUnit.Framework
open FsUnit
open BioProviders.Common.Context
open BioProviders.Tests.Data


// --------------------------------------------------------------------------------------
// Context Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type Context() = 

    let mutable contexts = []

    let mutable database = FsCheck.Gen.sample 0 1 (generateDatabase ()) |> Seq.head
    
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

        database <- FsCheck.Gen.sample size count (generateDatabase ()) |> Seq.head

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
        (fun () -> (Context.Parse emptySpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptySpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptySpeciesString plainAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>

        (fun () -> (Context.Parse emptySpeciesString regexAssemblyString |> ignore))
        |> should throw typeof<System.ArgumentException>


    [<Test>]
    member __.``Parse - Plain Species name`` () = 
        Context.Parse plainSpeciesString plainAssemblyString
        |> (function | (SpeciesPlainName _, _) -> Assert.Pass()
                     | (SpeciesRegexName _, _) -> Assert.Fail())

        Context.Parse plainSpeciesString regexAssemblyString
        |> (function | (SpeciesPlainName _, _) -> Assert.Pass()
                     | (SpeciesRegexName _, _) -> Assert.Fail())

    [<Test>]
    member __.``Parse - Regex Species name`` () = 
        Context.Parse regexSpeciesString plainAssemblyString
        |> (function | (SpeciesPlainName _, _) -> Assert.Fail()
                     | (SpeciesRegexName _, _) -> Assert.Pass())

        Context.Parse regexSpeciesString regexAssemblyString
        |> (function | (SpeciesPlainName _, _) -> Assert.Fail()
                     | (SpeciesRegexName _, _) -> Assert.Pass())

    [<Test>]
    member __.``Parse - Plain Assembly name`` () = 
        Context.Parse plainSpeciesString plainAssemblyString
        |> (function | (_, AccessionPlainName _) -> Assert.Pass()
                     | (_, AccessionRegexName _) -> Assert.Fail())

        Context.Parse regexSpeciesString plainAssemblyString
        |> (function | (_, AccessionPlainName _) -> Assert.Pass()
                     | (_, AccessionRegexName _) -> Assert.Fail())

    [<Test>]
    member __.``Parse - Regex Assembly name`` () = 
        Context.Parse plainSpeciesString regexAssemblyString
        |> (function | (_, AccessionPlainName _) -> Assert.Fail()
                     | (_, AccessionRegexName _) -> Assert.Pass())

        Context.Parse regexSpeciesString regexAssemblyString
        |> (function | (_, AccessionPlainName _) -> Assert.Fail()
                     | (_, AccessionRegexName _) -> Assert.Pass())


    [<Test>]
    member __.``Parse - Whitespace padding should be removed`` () = 

        let paddedPlainSpecies = " \n\r\t\f" + plainSpeciesString + " \n\r\t\f"
        let paddedRegexSpecies = " \n\r\t\f" + regexSpeciesString + " \n\r\t\f"
        let paddedPlainAssembly = " \n\r\t\f" + plainAssemblyString + " \n\r\t\f"
        let paddedRegexAssembly = " \n\r\t\f" + regexAssemblyString + " \n\r\t\f"

        Context.Parse paddedPlainSpecies paddedPlainAssembly
        |> should equal (SpeciesPlainName (plainSpeciesString.ToLower()), 
                         AccessionPlainName (plainAssemblyString.ToLower()))

        Context.Parse paddedRegexSpecies paddedPlainAssembly
        |> should equal (SpeciesRegexName (regexSpeciesString.ToLower()), 
                         AccessionPlainName (plainAssemblyString.ToLower()))

        Context.Parse paddedPlainSpecies paddedRegexAssembly
        |> should equal (SpeciesPlainName (plainSpeciesString.ToLower()), 
                         AccessionRegexName (regexAssemblyString.ToLower()))

        Context.Parse paddedRegexSpecies paddedRegexAssembly
        |> should equal (SpeciesRegexName (regexSpeciesString.ToLower()), 
                         AccessionRegexName (regexAssemblyString.ToLower()))

    [<Test>]
    member __.``Parse - Empty Assembly should be replaced by regex *`` () = 
        Context.Parse plainSpeciesString emptyAssemblyString
        |> (function | (_, AccessionRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

        Context.Parse regexSpeciesString emptyAssemblyString
        |> (function | (_, AccessionRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())

    [<Test>]
    member __.``Parse - Empty Species should be replaced by regex * (Given Assembly also empty)`` () = 
        Context.Parse emptySpeciesString emptyAssemblyString
        |> (function | (SpeciesRegexName "*", AccessionRegexName "*") -> Assert.Pass()
                     | _ -> Assert.Fail())


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
        refseq.GetPath() |> should equal "/genomes/all/GCF"

    [<Test>]
    member _.``ToString - GenBank should return genomes/genbank`` () =
        let genbank = GenBank
        genbank.GetPath() |> should equal "/genomes/all/GCA"


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
        |> List.map (fun a -> Accession.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AccessionPlainName _ -> Assert.Fail()
                        | AccessionRegexName _ -> Assert.Pass())
        
    [<Test>]
    member __.``Create - Strings not ending in * should return Plain Assembly Name`` () =
        data
        |> List.map (fun a -> Accession.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AccessionPlainName _ -> Assert.Pass()
                        | AccessionRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Strings containing * anywhere other than end should return Plain Assembly Name`` () =
        data
        |> List.map (fun a -> a.Insert(System.Random().Next(a.Length), "*"))
        |> List.map (fun a -> Accession.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AccessionPlainName _ -> Assert.Pass()
                        | AccessionRegexName _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty string should return Regex Assembly Name of *`` () =
        [""; "\t"; " "; "\n"; "  \t \n  "]
        |> List.map (fun a -> Accession.Create a)
        |> Seq.iter (fun a ->
                        match a with
                        | AccessionPlainName _ -> Assert.Fail()
                        | AccessionRegexName _ -> Assert.Pass())

    [<Test>]
    member __.``ToString - Should always return string used to create the Assembly Name`` () =
        data
        |> List.map (fun a -> Accession.Create a)
        |> List.map (fun a -> a.ToString())
        |> should equal data
