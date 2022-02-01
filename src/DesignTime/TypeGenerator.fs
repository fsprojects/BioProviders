// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.RunTime
open BioProviders.DesignTime.Context

module internal TypeGenerator =

    let createGenomicGenBankFlatFile (path:string) = 
        
        // Initialise the Genomic GBFF type
        let genomicGBFF = ProvidedTypeDefinition(className = "GenomicGBFF", baseType = Some (typeof<GenomicGBFF>))
        let genomicGBFFHelpText = 
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""
        genomicGBFF.AddXmlDocDelayed(fun () -> genomicGBFFHelpText)
                
        // Create and add constructor to the Genomic GBFF type
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ new GenomicGBFF(path) @@>))
        let genomicGBFFConstructorHelpText = 
            """<summary>Generic constructor to initialise the Genomic GenBank Flat File.</summary>"""
        genomicGBFFConstructor.AddXmlDocDelayed(fun () -> genomicGBFFConstructorHelpText)
        genomicGBFF.AddMemberDelayed(fun () -> genomicGBFFConstructor)
        
        // Return the Genomic GBFF type
        genomicGBFF


    let createAssembly (context:Context) = 
        // Extract necessary information from context.
        let assemblyType = context.ProvidedType
        let genomicGBFFPath = context.GetGenomicGBFFPath()

        // Add the genomic GenBank flat file to the assembly type.
        let genomicGBFF = createGenomicGenBankFlatFile genomicGBFFPath
        assemblyType.AddMemberDelayed (fun () -> genomicGBFF)

        // Add documentation to assembly type.
        let helpText = """<summary>Typed representation of a GenBank assembly.</summary>"""
        assemblyType.AddXmlDocDelayed(fun () -> helpText)

        // Return the constructed assembly type.
        assemblyType


    let createSpecies (context:Context) =
        // Extract necessary information from context.
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

        // Add documentation to the species type.
        let helpText = """<summary>Typed representation of a GenBank species.</summary>"""
        speciesType.AddXmlDocDelayed(fun () -> helpText)

        // Return the constructed species type.
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
