namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Project Helpers.
// --------------------------------------------------------------------------------------

module Helpers = 

    // Optional String
    let parseOptionString (str:string) =
        match System.String.IsNullOrEmpty(str) with
        | true -> None
        | _ -> Some str
