namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.RunTime.BaseTypes
open BioProviders.DesignTime.Context

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
                                    propertyType = typeof<IGenBankGenomeSequence>,
                                    getterCode = fun args -> <@@ ((%%args.[0]:obj) :?> GenBankGenome).Sequence @@>)
        let genomicGBFFSequenceHelpText = 
            """<summary>Typed representation of a Genomic GenBank Flat File Sequence.</summary>"""
        genomicGBFFSequence.AddXmlDocDelayed(fun () -> genomicGBFFSequenceHelpText)

        genomicGBFFSequence


    /// <summary>
    /// Creates a typed representation of a GenBank Flat File.
    /// </summary>
    /// <param name="path">The path to the GenBank Flat File.</param>
    let createGenomicGenBankFlatFile (path:string) = 
        
        // Initialise the Genomic GBFF type.
        let genomicGBFF = ProvidedTypeDefinition(className = "Genome", baseType = Some (typeof<obj>), hideObjectMethods = true)
        let genomicGBFFHelpText = 
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""
        genomicGBFF.AddXmlDocDelayed(fun () -> genomicGBFFHelpText)
                
        // Create and add constructor to the Genomic GBFF type.
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ new GenBankGenome(path) @@>))
        let genomicGBFFConstructorHelpText = 
            """<summary>Generic constructor to initialise the Genomic GenBank Flat File.</summary>"""
        genomicGBFFConstructor.AddXmlDocDelayed(fun () -> genomicGBFFConstructorHelpText)
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFConstructor)

        // Create and add Genomic GBFF Sequence.
        let genomicGBFFSequence = createGenomicGenBankFlatFileSequence ()
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFSequence)

        genomicGBFF


    /// <summary>
    /// Creates a typed representation of a GenBank Assembly.
    /// </summary>
    /// <param name="context">The context for the GenBank Assembly.</param>
    let createAssembly (context:Context) = 

        let assemblyType = context.ProvidedType
        let databasePath = context.DatabaseName.GetPath()
        let speciesName = context.SpeciesName.ToString()
        let accession = context.Accession.ToString()
        let genomicGBFFPath = (GenBankAssembly.Create databasePath speciesName accession).GenBankFlatFilePath

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF = createGenomicGenBankFlatFile genomicGBFFPath
        assemblyType.AddMemberDelayed (fun () -> genomicGBFF)

        // Add documentation to assembly type and return.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        assemblyType.AddXmlDocDelayed(fun () -> helpText)
        assemblyType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the 
    /// Type Provider.
    /// </summary>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (context:Context) =
        match context.SpeciesName, context.Accession with
        | SpeciesRegexName _, _ -> failwith "Wildcards are currently not supported for Species names."
        | _, AssemblyRegexName _ -> failwith "Wildcards are currently not supported for Assembly names."
        | SpeciesPlainName _, AssemblyPlainName _ -> createAssembly context
