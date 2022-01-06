// --------------------------------------------------------------------------------------
// Base RunTime Types for Type Provider Erasure.
// --------------------------------------------------------------------------------------

namespace BioProviders.RunTime

module BaseTypes = 

    type Assembly () =
        static member LoadGBFF (path:string) =
            new GenBankFlatFile (path)

    and GenBankFlatFile (path:string) = 
        
        member _.Accession = 1

        member _.BaseCount = 1

        member _.Comments = 1

        member _.Contig = 1

        member _.DbLinks = 1

        member _.DbSource = 1

        member _.Definitions = 1

        member _.Features = 1

        member _.Keywords = 1

        member _.Locus = 1

        member _.Origin = 1

        member _.Primary = 1

        member _.Project = 1

        member _.References = 1

        member _.Segment = 1

        member _.Source = 1

        member _.Version = 1

