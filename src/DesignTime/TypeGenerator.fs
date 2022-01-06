// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open BioProviders.DesignTime.Context

module TypeGenerator =

    let createAssemblyType (context:AssemblyContext) =
        context.ProvidedType
