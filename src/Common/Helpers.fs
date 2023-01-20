namespace BioProviders.Common

// --------------------------------------------------------------------------------------
// Project Helpers.
// --------------------------------------------------------------------------------------

module Helpers = 

    /// <summary>
    /// Parses an optional string. Returns the None option type if the provided string
    /// is null or empty. Returns the Some option type containing the provided string
    /// otherwise.
    /// </summary>
    /// <param name="str">The string to be parsed.</param>
    let parseOptionString (str:string) =
        match System.String.IsNullOrEmpty(str) with
        | true -> None
        | _ -> Some str

    /// <summary>
    /// Parses an optional list. Returns the None option type if the provided list
    /// is empty. Returns the Some option type containing the provided list otherwise.
    /// </summary>
    /// <param name="lst">The list to be parsed.</param>
    let parseOptionList (lst:'a list) =
        match lst.Length with
        | 0 -> None
        | _ -> Some lst

    /// <summary>
    /// Parses an optional date. Returns the None option type if the provided date
    /// is the default DateTime object (i.e., 1/01/0001 12:00:00 AM). Returns the Some 
    /// option type containing the provided date otherwise.
    /// </summary>
    /// <param name="date">The date to be parsed.</param>
    let parseOptionDate (date:System.DateTime) =
        match date = new System.DateTime() with
        | true -> None
        | _ -> Some date
