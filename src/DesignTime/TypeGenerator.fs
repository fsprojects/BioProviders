namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.RunTime
open BioProviders.DesignTime.Context

// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

module internal TypeGenerator =

    /// <summary>
    /// Creates a typed representation of a GenBank Flat File.
    /// </summary>
    /// <param name="path">The path to the GenBank Flat File.</param>
    let createGenomicGenBankFlatFile (path:string) = 
        
        // Initialise the Genomic GBFF type.
        let genomicGBFF = ProvidedTypeDefinition(className = "GenomicGBFF", baseType = Some (typeof<GenBankFlatFile>))
        let genomicGBFFHelpText = 
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""
        genomicGBFF.AddXmlDocDelayed(fun () -> genomicGBFFHelpText)
                
        // Create and add constructor to the Genomic GBFF type.
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ new GenBankFlatFile(path) @@>))
        let genomicGBFFConstructorHelpText = 
            """<summary>Generic constructor to initialise the Genomic GenBank Flat File.</summary>"""
        genomicGBFFConstructor.AddXmlDocDelayed(fun () -> genomicGBFFConstructorHelpText)
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFConstructor)
        genomicGBFF


    /// <summary>
    /// Creates a typed representation of a GenBank Assembly.
    /// </summary>
    /// <param name="context">The context for the GenBank Assembly.</param>
    let createAssembly (context:Context) = 

        let assemblyType = context.ProvidedType
        let genomicGBFFPath = context.GetGenomicGBFFPath()

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF = createGenomicGenBankFlatFile genomicGBFFPath
        assemblyType.AddMemberDelayed (fun () -> genomicGBFF)

        // Add documentation to assembly type and return.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        assemblyType.AddXmlDocDelayed(fun () -> helpText)
        assemblyType


    /// <summary>
    /// Creates a typed representation of a GenBank Species.
    /// </summary>
    /// <param name="context">The context for the GenBank Species.</param>
    let createSpecies (context:Context) =
        let speciesType = context.ProvidedType
        let database = context.DatabaseName
        let taxon = context.TaxonName
        let species = context.SpeciesName
        let assembly = context.AssemblyName
        let speciesPath = context.GetSpeciesPath()

        // Determine all assembly names for the given species.
        let assemblyNames = Species(speciesPath, assembly.ToString())
                            |> (fun species -> species.Assemblies)
                            |> List.map (fun name -> AssemblyName.Create name)

        // Add assembly types to the species.
        let assemblyTypes = assemblyNames 
                            |> List.map(fun assemblyName -> 
                                let assemblyType = ProvidedTypeDefinition(assemblyName.ToString(), Some typeof<obj>)
                                Context.Create assemblyType database taxon species assemblyName)
                            |> List.map(fun context -> createAssembly context)
        speciesType.AddMembersDelayed(fun () -> assemblyTypes)

        // Add documentation to the species type and return.
        let helpText = """<summary>Typed representation of a GenBank species.</summary>"""
        speciesType.AddXmlDocDelayed(fun () -> helpText)
        speciesType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the 
    /// Type Provider.
    /// </summary>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (context:Context) =
        match context.TaxonName, context.SpeciesName, context.AssemblyName with
        | TaxonRegexName _, _, _ -> context.ProvidedType
        | _, SpeciesRegexName _, _ -> context.ProvidedType
        | _, _, AssemblyRegexName _ -> createSpecies context
        | TaxonPlainName _, SpeciesPlainName _, AssemblyPlainName _ -> createAssembly context
