namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Project Helpers.
// --------------------------------------------------------------------------------------

module Helpers =

    /// Parses an optional string. Returns the None option type if the provided string
    /// is null or empty. Returns the Some option type containing the provided string
    /// otherwise.
    let parseOptionString (str: string) =
        match System.String.IsNullOrEmpty(str) with
        | true -> None
        | _ -> Some str

    /// Parses an optional list. Returns the None option type if the provided list
    /// is empty. Returns the Some option type containing the provided list otherwise.
    let parseOptionList (lst: 'a list) =
        match lst.Length with
        | 0 -> None
        | _ -> Some lst

    /// Parses an optional date. Returns the None option type if the provided date
    /// is the default DateTime object (i.e., 1/01/0001 12:00:00 AM). Returns the Some
    /// option type containing the provided date otherwise.
    let parseOptionDate (date: System.DateTime) =
        match date = new System.DateTime() with
        | true -> None
        | _ -> Some date
