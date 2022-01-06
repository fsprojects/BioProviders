namespace BioProviders.DesignTime

open FSharp.Core.CompilerServices
open System.Reflection
open ProviderImplementation.ProvidedTypes

open BioProviders.DesignTime.Context
open BioProviders.DesignTime.TypeGenerator

[<TypeProvider>]
type public GenBankProvider (config:TypeProviderConfig) as this =

    // Inherit basic Type Provider functionality and type construction.
    inherit TypeProviderForNamespaces(
        config, 
        assemblyReplacementMap=[("FSharp.Data.BioProviders.DesignTime", "FSharp.Data.BioProviders")], 
        addDefaultProbingLocation=true
    )

    // Define structure of the Type Provider
    let namespaceName = "GenBank"
    let thisAssembly = Assembly.GetExecutingAssembly()
    let assemblyType = ProvidedTypeDefinition(thisAssembly, namespaceName, "AssemblyProvider", Some typeof<obj>)

    // Instantiation function for parameterised Assembly Type Provider
    let buildAssemblyType (typeName : string) (args : obj[]) =
        
        // Extract parameters
        let taxon = args.[0] :?> string |> TaxonName
        let species = args.[1] :?> string |> SpeciesName
        let assembly = args.[2] :?> string |> AssemblyName

        // Define the assembly type
        let assemblyType = ProvidedTypeDefinition(thisAssembly, namespaceName, typeName, Some typeof<obj>)

        // Generate types
        ( taxon, species, assembly )
        |||> AssemblyContext.Parse
        |||> AssemblyContext.Create assemblyType GenBank
        |> createAssemblyType

    // Define static parameters for the Type Provider
    let assemblyParameters = 
        [ ProvidedStaticParameter("Taxon", typeof<string>)
          ProvidedStaticParameter("Species", typeof<string>)
          ProvidedStaticParameter("Assembly", typeof<string>) ]

    do assemblyType.DefineStaticParameters(assemblyParameters, buildAssemblyType)

    // Add XML documentation to the Type Provider
    let assemblyHelpText = 
        """<summary>Typed representation of a GenBank assembly.</summary>
           <param name="Taxon">The top-level GenBank taxonomic group of the organism (e.g. "Bacteria").</param>
           <param name="Species">The name of the species whose genome is being accessed (e.g. "Staphylococcus_borealis").</param>
           <param name="Assembly">The name of the genome assembly being accessed (e.g. "GCA_003042555.1_ASM304255v1/").</param>"""

    do assemblyType.AddXmlDoc(assemblyHelpText)

    // Register the main type with the Type Provider
    do this.AddNamespace(namespaceName, [ assemblyType ])
