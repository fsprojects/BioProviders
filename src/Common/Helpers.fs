namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Project Helpers.
// --------------------------------------------------------------------------------------

module Helpers = 

    // Optional string.
    let parseOptionString (str:string) =
        match System.String.IsNullOrEmpty(str) with
        | true -> None
        | _ -> Some str

    // Optional list.
    let parseOptionList (lst:'a list) =
        match lst.Length with
        | 0 -> None
        | _ -> Some lst

    // Optional date.
    let parseOptionDate (date:System.DateTime) =
        match date = new System.DateTime() with
        | true -> None
        | _ -> Some date