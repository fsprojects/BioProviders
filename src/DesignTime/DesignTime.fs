namespace BioProviders.DesignTime

open FSharp.Core.CompilerServices
open System.Reflection
open ProviderImplementation.ProvidedTypes

open BioProviders.Common.Context
open BioProviders.DesignTime.TypeGenerator

// GenBank Type Provider.
[<TypeProvider>]
type public GenBankProvider(config: TypeProviderConfig) as this =

    // Inherit basic Type Provider functionality and type construction.
    inherit
        TypeProviderForNamespaces(
            config,
            assemblyReplacementMap = [ ("BioProviders.DesignTime", "BioProviders") ],
            addDefaultProbingLocation = true
        )

    // Define structure of the Type Provider
    let namespaceName = "BioProviders"
    let thisAssembly = Assembly.GetExecutingAssembly()

    let assemblyProvidedType =
        ProvidedTypeDefinition(thisAssembly, namespaceName, "GenBankProvider", Some typeof<obj>)

    // Instantiation function for parameterised Assembly Type Provider
    let buildAssemblyType (typeName: string) (args: obj[]) =

        // Extract parameters
        let species = args.[0] :?> string
        let assembly = args.[1] :?> string

        // Define the assembly type
        let providedType =
            ProvidedTypeDefinition(thisAssembly, namespaceName, typeName, Some typeof<obj>)

        // Generate types
        (species, assembly)
        ||> Context.Parse
        ||> Context.Create GenBank
        |> createType providedType

    // Define static parameters for the Type Provider
    let assemblyParameters =
        [ ProvidedStaticParameter("Species", typeof<string>, parameterDefaultValue = "")
          ProvidedStaticParameter("Accession", typeof<string>, parameterDefaultValue = "") ]

    do assemblyProvidedType.DefineStaticParameters(assemblyParameters, buildAssemblyType)

    // Add XML documentation to the Type Provider
    let assemblyHelpText =
        """<summary>Typed representation of the GenBank FTP server.</summary>
           <param name="Species">The name of the species whose genome is being accessed (e.g. "Staphylococcus borealis"). Defaults to <c>""</c>.</param>
           <param name="Accession">The accession of the genome assembly being accessed (e.g. "GCA_003042555.1"). Defaults to <c>""</c>.</param>"""

    do assemblyProvidedType.AddXmlDoc(assemblyHelpText)

    // Register the main type with the Type Provider
    do this.AddNamespace(namespaceName, [ assemblyProvidedType ])
