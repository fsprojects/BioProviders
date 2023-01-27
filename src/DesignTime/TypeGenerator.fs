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
        let genomicGBFFSequence =
            ProvidedProperty(
                propertyName = "Sequence",
                propertyType = typeof<BioFSharp.BioSeq.BioSeq<BioFSharp.Nucleotides.Nucleotide>>,
                getterCode = fun args -> <@@ (%%args.[0]: GenBankFlatFile.GenBankFlatFile).Sequence @@>
            )

        let genomicGBFFSequenceHelpText =
            """<summary>Typed representation of the Sequence of a Genomic GenBank Flat File.</summary>"""

        genomicGBFFSequence.AddXmlDocDelayed(fun () -> genomicGBFFSequenceHelpText)

        genomicGBFFSequence


    /// <summary>
    /// Creates a typed representation of a GenBank Flat File Metadata.
    /// </summary>
    let createGenomicGenBankFlatFileMetadata () =

        // Initialise the Genomic GBFF Metadata type.
        let genomicGBFFMetadata =
            ProvidedProperty(
                propertyName = "Metadata",
                propertyType = typeof<Metadata.Metadata>,
                getterCode = fun args -> <@@ (%%args.[0]: GenBankFlatFile.GenBankFlatFile).Metadata @@>
            )

        let genomicGBFFMetadataHelpText =
            """<summary>Typed representation of the Metadata of a Genomic GenBank Flat File.</summary>"""

        genomicGBFFMetadata.AddXmlDocDelayed(fun () -> genomicGBFFMetadataHelpText)

        genomicGBFFMetadata


    /// <summary>
    /// Creates a typed representation of a Genomic GenBank Flat File.
    /// </summary>
    /// <param name="path">The path to the GenBank Flat File.</param>
    let createGenomicGenBankFlatFile (path: string) =

        // Initialise the Genomic GBFF type.
        let genomicGBFF =
            ProvidedTypeDefinition(
                className = "Genome",
                baseType = Some(typeof<GenBankFlatFile.GenBankFlatFile>),
                hideObjectMethods = true
            )

        let genomicGBFFHelpText =
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""

        genomicGBFF.AddXmlDocDelayed(fun () -> genomicGBFFHelpText)

        // Create and add constructor to the Genomic GBFF type.
        let genomicGBFFConstructor =
            ProvidedConstructor(
                parameters = [],
                invokeCode = (fun _ -> <@@ GenBankFlatFile.createGenBankFlatFile path @@>)
            )

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
    /// <param name="assembly">The runtime representation of the assembly to be created.</param>
    let createAssembly (providedType: ProvidedTypeDefinition) (assembly: GenBankAssembly) =

        let genomicGBFFPath = assembly.GenBankFlatFilePath

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF () =
            createGenomicGenBankFlatFile genomicGBFFPath

        providedType.AddMemberDelayed genomicGBFF

        // Add documentation to assembly type and return.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Creates a typed representation of a GenBank Species.
    /// </summary>
    /// <param name="providedType">The species type to be constructed.</param>
    /// <param name="species">The runtime representation of the species to be created.</param>
    /// <param name="accessionPattern">The accession pattern for the species assemblies.</param>
    let createSpecies (providedType: ProvidedTypeDefinition) (species: GenBankSpecies) (accessionPattern: string) =

        // Create the assembly types for the species.
        let assemblyTypes () =
            species.GetAssemblies accessionPattern
            |> List.map (fun assembly ->
                let assemblyType =
                    ProvidedTypeDefinition(assembly.Accession, Some typeof<obj>, hideObjectMethods = true)

                createAssembly assemblyType assembly)

        // Add the assembly types to the species type.
        providedType.AddMembersDelayed assemblyTypes

        // Add documentation to species type and return.
        let helpText =
            """<summary>Typed representation of a collection of GenBank assemblies for a species.</summary>"""

        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Creates a typed representation of a GenBank Taxon.
    /// </summary>
    /// <param name="providedType">The taxon type to be constructed.</param>
    /// <param name="taxon">The runtime representation of the taxon to be created.</param>
    /// <param name="speciesPattern">The species name pattern for species to be added to the taxon.</param>
    /// <param name="accessionPattern">The accession pattern for the species assemblies.</param>
    let createTaxon
        (providedType: ProvidedTypeDefinition)
        (taxon: GenBankTaxon)
        (speciesPattern: string)
        (accessionPattern: string)
        =

        // Create the species types for the taxon.
        let speciesTypes () =
            taxon.GetSpecies speciesPattern
            |> List.map (fun species ->
                let speciesType =
                    ProvidedTypeDefinition(species.SpeciesName, Some typeof<obj>, hideObjectMethods = true)

                createSpecies speciesType species accessionPattern)

        // Add the species types to the taxon type.
        providedType.AddMembersDelayed speciesTypes

        // Add documentation to taxon type and return.
        let helpText =
            """<summary>Typed representation of a collection of GenBank species.</summary>"""

        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the Type Provider.
    /// </summary>
    /// <param name="providedType">The Type Provider type being constructed.</param>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (providedType: ProvidedTypeDefinition) (context: Context) =
        match context.SpeciesName, context.Accession with
        | SpeciesRegexName _, _ ->
            createTaxon
                providedType
                (new GenBankTaxon(context))
                (context.SpeciesName.ToString())
                (context.Accession.ToString())
        | SpeciesPlainName _, AccessionRegexName _ ->
            createSpecies providedType (new GenBankSpecies(context)) (context.Accession.ToString())
        | SpeciesPlainName _, AccessionPlainName _ -> createAssembly providedType (new GenBankAssembly(context))
