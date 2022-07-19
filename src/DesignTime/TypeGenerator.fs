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
                                    propertyType = typeof<IGenBankGenomicSequence>,
                                    getterCode = fun args -> <@@ ((%%args.[0]:obj) :?> IGenomicGenBankFlatFile).Sequence @@>)
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
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ new GenomicGenBankFlatFile(path) @@>))
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
    let createAssembly (providedType:ProvidedTypeDefinition) (assembly:IGenBankAssembly) = 

        let genomicGBFFPath = assembly.GenBankFlatFilePath

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF = createGenomicGenBankFlatFile genomicGBFFPath
        providedType.AddMemberDelayed (fun () -> genomicGBFF)

        // Add documentation to assembly type and return.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        providedType.AddXmlDocDelayed(fun () -> helpText)
        providedType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the 
    /// Type Provider.
    /// </summary>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (providedType:ProvidedTypeDefinition) (context:Context) =
        let databasePath = context.DatabaseName.GetPath()

        match context.SpeciesName, context.Accession with
        | SpeciesRegexName _, _ -> failwith "Wildcards are currently not supported for Species names."
        | SpeciesPlainName species, AccessionRegexName accession -> failwith "Wildcards are currently not supported for Assembly names."
        | SpeciesPlainName species, AccessionPlainName accession -> GenBankAssembly.Create databasePath species accession
                                                                    |> createAssembly providedType 
