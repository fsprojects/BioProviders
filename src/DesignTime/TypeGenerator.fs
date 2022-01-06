// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------
namespace BioProviders.DesignTime.Utility

open BioProviders.DesignTime.Utility.Context

module TypeGenerator =

    let createAssemblyType (context:AssemblyContext) =
        context.ProvidedType
