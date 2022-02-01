// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.RunTime
open BioProviders.DesignTime.Context

module internal TypeGenerator =

    /// <summary>
    /// Constructs a provided type based on the complete context provided. As the context
    /// contains no regex fields, a complete assembly type will be returned.
    /// </summary>
    /// <param name="context">The complete context for the type generation.</param>
    let createPlainType (context:CompleteContext) =
        context.ProvidedType


    /// <summary>
    /// Constructs a provided type based on the partial context provided. Any context
    /// fields of a regex type will be replaced by a list of plain types corresponding
    /// to the regex pattern.
    /// </summary>
    /// <param name="context">The partial context for the regex type generation.</param>
    let createRegexType (context:PartialContext) =
        context.ProvidedType


    /// <summary>
    /// Construct the appropriate provided type based on the context of the 
    /// Type Provider.
    /// </summary>
    /// <param name="context">The context of the Type Provider.</param>
    let createType (context:Context) =
        match context with 
        | PartialContext ctx -> createRegexType ctx
        | CompleteContext ctx -> createPlainType ctx
