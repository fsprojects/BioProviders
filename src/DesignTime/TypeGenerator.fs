namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.Common.Context
open BioProviders

// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

module internal TypeGenerator =

    /// <summary>
    /// Creates a typed representation of a GenBank Flat File Sequence.
    /// </summary>
    let createGenomicGenBankFlatFileSequence () = 

        // Initialise the Genomic GBFF Sequence type.
        let genomicGBFFSequence = ProvidedProperty(
                                    propertyName = "Sequence", 
                                    propertyType = typeof<BioFSharp.BioSeq.BioSeq<BioFSharp.Nucleotides.Nucleotide>>,
                                    getterCode = fun args -> <@@ (%%args.[0]: GenBankFlatFile.GenBankFlatFile).Sequence @@>)
        let genomicGBFFSequenceHelpText = 
            """<summary>Typed representation of the Sequence of a Genomic GenBank Flat File.</summary>"""
        genomicGBFFSequence.AddXmlDocDelayed(fun () -> genomicGBFFSequenceHelpText)

        genomicGBFFSequence


    /// <summary>
    /// Creates a typed representation of a GenBank Flat File Metadata.
    /// </summary>
    let createGenomicGenBankFlatFileMetadata () = 

        // Initialise the Genomic GBFF Metadata type.
        let genomicGBFFMetadata = ProvidedProperty(
                                    propertyName = "Metadata", 
                                    propertyType = typeof<Metadata.Metadata>,
                                    getterCode = fun args -> <@@ (%%args.[0]: GenBankFlatFile.GenBankFlatFile).Metadata @@>)
        let genomicGBFFMetadataHelpText = 
            """<summary>Typed representation of the Metadata of a Genomic GenBank Flat File.</summary>"""
        genomicGBFFMetadata.AddXmlDocDelayed(fun () -> genomicGBFFMetadataHelpText)

        genomicGBFFMetadata


    /// <summary>
    /// Creates a typed representation of a Genomic GenBank Flat File.
    /// </summary>
    /// <param name="path">The path to the GenBank Flat File.</param>
    let createGenomicGenBankFlatFile (path:string) = 
        
        // Initialise the Genomic GBFF type.
        let genomicGBFF = ProvidedTypeDefinition(className = "Genome", baseType = Some (typeof<GenBankFlatFile.GenBankFlatFile>), hideObjectMethods = true)
        let genomicGBFFHelpText = 
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""
        genomicGBFF.AddXmlDocDelayed(fun () -> genomicGBFFHelpText)
                
        // Create and add constructor to the Genomic GBFF type.
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ GenBankFlatFile.createGenBankFlatFile path @@>))
        let genomicGBFFConstructorHelpText = 
            """<summary>Generic constructor to initialise the Genomic GenBank Flat File.</summary>"""
        genomicGBFFConstructor.AddXmlDocDelayed(fun () -> genomicGBFFConstructorHelpText)
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFConstructor)

        // Create and add Genomic GBFF Sequence.
        let genomicGBFFSequence = createGenomicGenBankFlatFileSequence ()
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFSequence)
        
        // Create and add Genomic GBFF Metadata.
        let genomicGBFFMetadata = createGenomicGenBankFlatFileMetadata ()
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFMetadata)
        genomicGBFF


    /// <summary>
    /// Creates a typed representation of a GenBank Assembly.
    /// </summary>
    /// <param name="providedType">The assembly type to be constructed.</param>
    /// <param name="context">The context of the Type Provider.</param>
    let createAssembly (providedType:ProvidedTypeDefinition) (context:Context) = 

        // Make sure GenBankFlatFilePath is defined.
        let genomicGBFFPath = 
            match context.Accession.GenBankFlatFilePath with
            | Some path -> path
            | None -> GenBank.createAssembly context 
                      |> (fun c -> 
                            match c.Accession.GenBankFlatFilePath with
                            | Some path -> path
                            | None -> failwith "GenBank Flat File path must exist to create assembly. Something when wrong creating the assembly context.")

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF () = createGenomicGenBankFlatFile genomicGBFFPath
        providedType.AddMemberDelayed genomicGBFF

        // Add documentation to assembly type and return.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Creates a typed representation of a GenBank Species.
    /// </summary>
    /// <param name="providedType">The species type to be constructed.</param>
    /// <param name="context">The context of the Type Provider.</param>
    let createSpecies (providedType:ProvidedTypeDefinition) (context:Context) = 

        // Create the assembly types for the species.
        let assemblyTypes () = 
            match context.DatabaseName with
            | DatabaseName.GenBank -> GenBank.createAssemblies context
                                      |> List.map (fun assemblyContext -> 
                                            let assemblyType = 
                                                ProvidedTypeDefinition(
                                                    className = assemblyContext.Accession.AccessionName.ToString(), 
                                                    baseType = Some typeof<obj>, 
                                                    hideObjectMethods = true )
                                            createAssembly assemblyType assemblyContext)
            | DatabaseName.RefSeq -> invalidArg "DatabaseName" "RefSeq database is not currently supported."

        // Add the assembly types to the species type.
        providedType.AddMembersDelayed assemblyTypes

        // Add documentation to species type and return.
        let helpText = """<summary>Typed representation of a collection of GenBank assemblies for a species.</summary>"""
        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Creates a group of GenBank Species matching the provided species name pattern.
    /// </summary>
    /// <param name="providedType">The taxon type to be constructed.</param>
    /// <param name="context">The context of the Type Provider.</param>
    let createTaxon (providedType:ProvidedTypeDefinition) (context:Context) = 

        // Create the species types for the taxon.
        let speciesTypes () = 
            match context.DatabaseName with
            | DatabaseName.GenBank -> GenBank.createSpecies context
                                      |> List.map (fun speciesContext -> 
                                            let speciesType = 
                                                ProvidedTypeDefinition(
                                                    className = speciesContext.Species.SpeciesName.ToString(), 
                                                    baseType = Some typeof<obj>, 
                                                    hideObjectMethods = true )
                                            createSpecies speciesType speciesContext)
            | DatabaseName.RefSeq -> invalidArg "DatabaseName" "RefSeq database is not currently supported."

        // Add the species types to the taxon type.
        providedType.AddMembersDelayed speciesTypes

        // Add documentation to taxon type and return.
        let helpText = """<summary>Typed representation of a collection of GenBank species.</summary>"""
        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the Type Provider.
    /// </summary>
    /// <param name="providedType">The Type Provider type being constructed.</param>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (providedType:ProvidedTypeDefinition) (context:Context) =
        match context.Species.SpeciesName, context.Accession.AccessionName with
        | SpeciesRegexName _, _ -> createTaxon providedType context
        | SpeciesPlainName _, AccessionRegexName _ -> createSpecies providedType context
        | SpeciesPlainName _, AccessionPlainName _ -> createAssembly providedType context
