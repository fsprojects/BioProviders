// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.RunTime
open BioProviders.DesignTime.Context

module internal TypeGenerator =

    let createGenomicGenBankFlatFileType (path:string) () = 
    
        // Initialise the Genomic GBFF type
        let genomicGBFF = ProvidedTypeDefinition(className = "GenomicGBFF", baseType = Some (typeof<GenomicGenBankFlatFile>))
        let genomicGBFFHelpText = 
            """<summary>Typed representation of an Assembly's Genomic GenBank Flat File.</summary>"""
        genomicGBFF.AddXmlDoc(genomicGBFFHelpText)
            
        // Create and add constructor to the Genomic GBFF type
        let genomicGBFFConstructor = ProvidedConstructor(parameters = [], invokeCode = (fun _ -> <@@ new GenomicGenBankFlatFile(path) @@>))
        let genomicGBFFConstructorHelpText = 
            """<summary>Generic constructor to initialise the Genomic GenBank Flat File.</summary>"""
        genomicGBFFConstructor.AddXmlDoc(genomicGBFFConstructorHelpText)
        genomicGBFF.AddMember(genomicGBFFConstructor)
    
        // Return the Genomic GBFF type
        genomicGBFF

    let createAssemblyType (context:AssemblyContext) =
        
        // Extract information from context
        let assemblyType = context.ProvidedType
        let genomicGBFFPath = context.GetGenomicGBFFPath()

        // Add Genomic GBFF to the assembly type
        let genomicGBFF = createGenomicGenBankFlatFileType genomicGBFFPath
        assemblyType.AddMemberDelayed(genomicGBFF)

        // Return the constructed assembly type
        assemblyType