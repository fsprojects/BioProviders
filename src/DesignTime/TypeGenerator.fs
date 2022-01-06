// --------------------------------------------------------------------------------------
// Type Generation.
// --------------------------------------------------------------------------------------

namespace BioProviders.DesignTime

open ProviderImplementation.ProvidedTypes
open BioProviders.DesignTime.Context
open BioProviders.RunTime.BaseTypes

module internal TypeGenerator =

    let createAssemblyType (context:AssemblyContext) =
        
        // Extract information from context
        let assemblyType = context.ProvidedType
        let path = context.GetPath()

        // Add method to load GenBank Flat File for the assembly
        let loadGBFF = 
            ProvidedMethod( 
                methodName = "LoadGBFF", 
                parameters = [], 
                returnType = typeof<GenBankFlatFile>, 
                invokeCode = (fun _ -> <@@ Assembly.LoadGBFF path @@>), 
                isStatic = true 
            )
        assemblyType.AddMember loadGBFF

        // Return the constructed assembly type
        assemblyType