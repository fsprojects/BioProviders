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
    member _.``Create - All plain names should return CompleteContext`` () =
        Context.Create providedType database plainTaxon plainSpecies plainAssembly
        |> (function | PartialContext _ -> Assert.Fail()
                     | CompleteContext _ -> Assert.Pass())

    [<Test>]
    member __.``Create - Regex Taxon name should return PartialContext`` () =
        Context.Create providedType database regexTaxon plainSpecies plainAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Regex Species name should return PartialContext`` () =
        Context.Create providedType database plainTaxon regexSpecies plainAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Regex Assembly name should return PartialContext`` () =
        Context.Create providedType database plainTaxon plainSpecies regexAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty Assembly should return PartialContext`` () =
        Context.Create providedType database regexTaxon regexSpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

        Context.Create providedType database plainTaxon regexSpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

        Context.Create providedType database regexTaxon plainSpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

        Context.Create providedType database plainTaxon plainSpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty Assembly and Species should return PartialContext`` () =
        Context.Create providedType database regexTaxon emptySpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

        Context.Create providedType database plainTaxon emptySpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())

    [<Test>]
    member __.``Create - Empty Assembly, Species, and Taxon should return PartialContext`` () =
        Context.Create providedType database emptyTaxon emptySpecies emptyAssembly
        |> (function | PartialContext _ -> Assert.Pass()
                     | CompleteContext _ -> Assert.Fail())


// --------------------------------------------------------------------------------------
// Partial Context Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type PartialContext() = 

    let mutable data = []
    
    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count (generatePartialContext ())

    [<Test>]
    member __.``Create - Assembly Group Name should always be all_assembly_versions`` () =
        data
        |> Seq.iter (fun ctx -> 
                        ctx.AssemblyGroupName.ToString() |> should equal "all_assembly_versions")

    [<Test>]
    member __.``Create - One or more of Taxon, Species, and Assembly must be regex`` () =
        let size = 0
        let count = 1
        let providedType = FsCheck.Gen.sample size count (generateProvidedType ()) |> Seq.head
        let database = FsCheck.Gen.sample size count (generateDatabase ()) |> Seq.head
        let regexTaxon = FsCheck.Gen.sample size count (generateRegexTaxon ()) |> Seq.head
        let regexSpecies = FsCheck.Gen.sample size count (generateRegexSpecies ()) |> Seq.head
        let regexAssembly = FsCheck.Gen.sample size count (generateRegexAssembly ()) |> Seq.head
        let plainTaxon = FsCheck.Gen.sample size count (generatePlainTaxon ()) |> Seq.head
        let plainSpecies = FsCheck.Gen.sample size count (generatePlainSpecies ()) |> Seq.head
        let plainAssembly = FsCheck.Gen.sample size count (generatePlainAssembly ()) |> Seq.head

        (fun () -> (PartialContext.Create providedType database plainTaxon plainSpecies plainAssembly) |> ignore)
        |> should throw typeof<System.Exception>

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database regexTaxon plainSpecies plainAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database plainTaxon regexSpecies plainAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database plainTaxon plainSpecies regexAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database plainTaxon regexSpecies regexAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database regexTaxon regexSpecies plainAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database regexTaxon plainSpecies regexAssembly |> ignore)

        Assert.DoesNotThrow (fun () -> 
            PartialContext.Create providedType database regexTaxon regexSpecies regexAssembly |> ignore)

    [<Test>]
    member __.``GetCompletedPath - Should always begin with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetCompletedPath())
        |> Seq.iter (fun path -> path |> should startWith "/")

    [<Test>]
    member __.``GetCompletedPath - Should never end with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetCompletedPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetCompletedPath - Regex Taxon name should return Database Path`` () =
        data
        |> Seq.iter (fun ctx ->
                        match ctx.TaxonName with
                        | TaxonPlainName _ -> Assert.Pass()
                        | TaxonRegexName _ when 
                            ctx.GetCompletedPath() = "/" + ctx.DatabaseName.ToString()
                            -> Assert.Pass()
                        | _ -> Assert.Fail())

    [<Test>]
    member __.``GetCompletedPath - Regex Species name should return Taxon Path`` () =
        data
        |> Seq.iter (fun ctx ->
                        match ctx.TaxonName, ctx.SpeciesName with
                        | _, SpeciesPlainName _ | TaxonRegexName _, _ -> Assert.Pass()
                        | TaxonPlainName _, SpeciesRegexName _ when 
                            ctx.GetCompletedPath() = "/" + String.concat "/" [ ctx.DatabaseName.ToString()
                                                                               ctx.TaxonName.ToString() ] -> Assert.Pass()
                        | _ -> Assert.Fail())

    [<Test>]
    member __.``GetCompletedPath - Regex Assembly name should return Species Path`` () =
        data
        |> Seq.iter (fun ctx ->
                        match ctx.TaxonName, ctx.SpeciesName, ctx.AssemblyName with
                        | _, _, AssemblyPlainName _ | TaxonRegexName _, _, _ | _, SpeciesRegexName _, _ -> Assert.Pass()
                        | _, _, AssemblyRegexName _ when 
                            ctx.GetCompletedPath() = "/" + String.concat "/" [ ctx.DatabaseName.ToString()
                                                                               ctx.TaxonName.ToString()
                                                                               ctx.SpeciesName.ToString()
                                                                               ctx.AssemblyGroupName.ToString() ] -> Assert.Pass()
                        | _ -> Assert.Fail())


// --------------------------------------------------------------------------------------
// Complete Context Tests.
// --------------------------------------------------------------------------------------

[<TestFixture>]
type CompleteContext() = 

    let mutable data = []

    [<SetUp>]
    member __.``Setup`` () =
        let size = 0
        let count = 30
        data <- FsCheck.Gen.sample size count (generateCompleteContext ())

    [<Test>]
    member __.``Create - Assembly Group Name should always be all_assembly_versions`` () =
        data
        |> Seq.iter (fun ctx -> 
                        ctx.AssemblyGroupName.ToString() |> should equal "all_assembly_versions")

    [<Test>]
    member __.``Create - None of Taxon, Species, and Assembly should be regex`` () =
        let size = 0
        let count = 1
        let providedType = FsCheck.Gen.sample size count (generateProvidedType ()) |> Seq.head
        let database = FsCheck.Gen.sample size count (generateDatabase ()) |> Seq.head
        let regexTaxon = FsCheck.Gen.sample size count (generateRegexTaxon ()) |> Seq.head
        let regexSpecies = FsCheck.Gen.sample size count (generateRegexSpecies ()) |> Seq.head
        let regexAssembly = FsCheck.Gen.sample size count (generateRegexAssembly ()) |> Seq.head
        let plainTaxon = FsCheck.Gen.sample size count (generatePlainTaxon ()) |> Seq.head
        let plainSpecies = FsCheck.Gen.sample size count (generatePlainSpecies ()) |> Seq.head
        let plainAssembly = FsCheck.Gen.sample size count (generatePlainAssembly ()) |> Seq.head

        Assert.DoesNotThrow (fun () -> 
            CompleteContext.Create providedType database plainTaxon plainSpecies plainAssembly |> ignore)

        ( fun () -> (CompleteContext.Create providedType database regexTaxon plainSpecies plainAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database plainTaxon regexSpecies plainAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database plainTaxon plainSpecies regexAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database plainTaxon regexSpecies regexAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database regexTaxon regexSpecies plainAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database regexTaxon plainSpecies regexAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>

        ( fun () -> (CompleteContext.Create providedType database regexTaxon regexSpecies regexAssembly) |> ignore)
        |> should throw typeof<System.ArgumentException>


    [<Test>]
    member __.``GetAssemblyPath - Should always begin with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetAssemblyPath())
        |> Seq.iter (fun path -> path |> should startWith "/")

    [<Test>]
    member __.``GetAssemblyPath - Should never end with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetAssemblyPath())
        |> Seq.iter (fun path -> path |> should not' (endWith "/"))

    [<Test>]
    member __.``GetGBFFPath - Should always begin with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetGBFFPath())
        |> Seq.iter (fun path -> path |> should startWith "/")

    [<Test>]
    member __.``GetGBFFPath - Should never end with /`` () =
        data
        |> List.map (fun ctx -> ctx.GetGBFFPath())
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
