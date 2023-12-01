namespace BioProviders

open BioProviders.Common

// --------------------------------------------------------------------------------------
// GenBank Flat File Metadata Representation.
// --------------------------------------------------------------------------------------

module Metadata =

    /// <summary>
    /// Identifier assigned to each GenBank sequence record.
    /// </summary>
    /// <remarks>Records can have more than one accession assigned to them. The primary accession number is the newest identifier for the sequence record, and the secondary accession numbers are any of those that were previously assigned to it. A sequence record can have more than one secondary accession.</remarks>
    type Accession =
        { Primary: string option
          Secondary: string list option }

    let private createAccession (accession: Bio.IO.GenBank.GenBankAccession) =
        match accession with
        | null -> None
        | _ ->
            Some
                { Primary = accession.Primary |> Helpers.parseOptionString
                  Secondary = accession.Secondary |> Seq.toList |> Helpers.parseOptionList }


    /// <summary>
    /// Specifies the type of DBLink.
    /// </summary>
    type LinkType =
        | Project
        | TraceAssemblyArchive
        | BioProject

    let private createLinkType (crossReferenceType: Bio.IO.GenBank.CrossReferenceType) =
        match crossReferenceType with
        | Bio.IO.GenBank.CrossReferenceType.None -> None
        | Bio.IO.GenBank.CrossReferenceType.Project -> Some LinkType.Project
        | Bio.IO.GenBank.CrossReferenceType.TraceAssemblyArchive -> Some LinkType.TraceAssemblyArchive
        | Bio.IO.GenBank.CrossReferenceType.BioProject -> Some LinkType.BioProject
        | _ -> invalidArg "LinkType" "An invalid LinkType has been encountered."


    /// <summary>
    /// Cross-references to resources that support the existence of a sequence
    /// record, such as the Project Database and the NCBI Trace Assembly
    /// Archive.
    /// </summary>
    type DbLink =
        { Numbers: string list option
          Type: LinkType option }

    let private createDbLink (crossReferenceLink: Bio.IO.GenBank.CrossReferenceLink) =
        { Numbers = crossReferenceLink.Numbers |> Seq.toList |> Helpers.parseOptionList
          Type = crossReferenceLink.Type |> createLinkType }

    let private createDbLinks (dbLinks: Bio.IO.GenBank.CrossReferenceLink list) =
        dbLinks |> List.map (fun link -> createDbLink link) |> Helpers.parseOptionList


    /// <summary>
    /// Specifies which family a sequence belongs to.
    /// </summary>
    type DivisionCode =
        /// <summary>Primate sequences</summary>
        | PRI
        /// <summary>Rodent sequences</summary>
        | ROD
        /// <summary>Other mammalian sequences</summary>
        | MAM
        /// <summary>Other vertebrate sequences</summary>
        | VRT
        /// <summary>Invertebrate sequences</summary>
        | INV
        /// <summary>=Plant and Fungal sequences</summary>
        | PLN
        /// <summary>Bacterial sequences</summary>
        | BCT
        /// <summary>Viral sequences</summary>
        | VRL
        /// <summary>Phage sequences</summary>
        | PHG
        /// <summary>Synthetic and chimeric sequences</summary>
        | SYN
        /// <summary>Unannotated sequences</summary>
        | UNA
        /// <summary>Expressed Sequence Tags</summary>
        | EST
        /// <summary>Patent sequences</summary>
        | PAT
        /// <summary>Sequence Tagged Sites</summary>
        | STS
        /// <summary>Genome Survey Sequences</summary>
        | GSS
        /// <summary>High Throughput Genomic sequences</summary>
        | HTG
        /// <summary>Unfinished High-Throughput cDNA sequencing</summary>
        | HTC
        /// <summary>Environmental samples</summary>
        | ENV
        /// <summary>Constructed (for contig assembly)</summary>
        | CON

    let private createDivisionCode (divisionCode: Bio.IO.GenBank.SequenceDivisionCode) =
        match divisionCode with
        | Bio.IO.GenBank.SequenceDivisionCode.None -> None
        | Bio.IO.GenBank.SequenceDivisionCode.PRI -> Some DivisionCode.PRI
        | Bio.IO.GenBank.SequenceDivisionCode.ROD -> Some DivisionCode.ROD
        | Bio.IO.GenBank.SequenceDivisionCode.MAM -> Some DivisionCode.MAM
        | Bio.IO.GenBank.SequenceDivisionCode.VRT -> Some DivisionCode.VRT
        | Bio.IO.GenBank.SequenceDivisionCode.INV -> Some DivisionCode.INV
        | Bio.IO.GenBank.SequenceDivisionCode.PLN -> Some DivisionCode.PLN
        | Bio.IO.GenBank.SequenceDivisionCode.BCT -> Some DivisionCode.BCT
        | Bio.IO.GenBank.SequenceDivisionCode.VRL -> Some DivisionCode.VRL
        | Bio.IO.GenBank.SequenceDivisionCode.PHG -> Some DivisionCode.PHG
        | Bio.IO.GenBank.SequenceDivisionCode.SYN -> Some DivisionCode.SYN
        | Bio.IO.GenBank.SequenceDivisionCode.UNA -> Some DivisionCode.UNA
        | Bio.IO.GenBank.SequenceDivisionCode.EST -> Some DivisionCode.EST
        | Bio.IO.GenBank.SequenceDivisionCode.PAT -> Some DivisionCode.PAT
        | Bio.IO.GenBank.SequenceDivisionCode.STS -> Some DivisionCode.STS
        | Bio.IO.GenBank.SequenceDivisionCode.GSS -> Some DivisionCode.GSS
        | Bio.IO.GenBank.SequenceDivisionCode.HTG -> Some DivisionCode.HTG
        | Bio.IO.GenBank.SequenceDivisionCode.HTC -> Some DivisionCode.HTC
        | Bio.IO.GenBank.SequenceDivisionCode.ENV -> Some DivisionCode.ENV
        | Bio.IO.GenBank.SequenceDivisionCode.CON -> Some DivisionCode.CON
        | _ -> invalidArg "DivisionCode" "An invalid DivisionCode has been encountered."


    /// <summary>
    /// Specifies the type of biological sequence.
    /// </summary>
    type MoleculeType =
        /// <summary>No valid type (but set in metadata)</summary>
        | Invalid
        /// <summary>Nucleic acid</summary>
        | NA
        /// <summary>Deoxyribonucleic acid (DNA)</summary>
        | DNA
        /// <summary>Ribonucleic acid (RNA)</summary>
        | RNA
        /// <summary>Transfer RNA</summary>
        | TRNA
        /// <summary>Ribosomal RNA</summary>
        | RRNA
        /// <summary>Messenger RNA</summary>
        | MRNA
        /// <summary>Alternate name for SnRNA</summary>
        | URNA
        /// <summary>Small nuclear RNA</summary>
        | SnRNA
        /// <summary>Small nucleolar RNA</summary>
        | SnoRNA
        /// <summary>Protein</summary>
        | Protein

    let private createMoleculeType (moleculeType: Bio.IO.GenBank.MoleculeType) =
        match moleculeType with
        | Bio.IO.GenBank.MoleculeType.Invalid -> None
        | Bio.IO.GenBank.MoleculeType.NA -> Some MoleculeType.NA
        | Bio.IO.GenBank.MoleculeType.DNA -> Some MoleculeType.DNA
        | Bio.IO.GenBank.MoleculeType.RNA -> Some MoleculeType.RNA
        | Bio.IO.GenBank.MoleculeType.tRNA -> Some MoleculeType.TRNA
        | Bio.IO.GenBank.MoleculeType.rRNA -> Some MoleculeType.RRNA
        | Bio.IO.GenBank.MoleculeType.mRNA -> Some MoleculeType.MRNA
        | Bio.IO.GenBank.MoleculeType.uRNA -> Some MoleculeType.URNA
        | Bio.IO.GenBank.MoleculeType.snRNA -> Some MoleculeType.SnRNA
        | Bio.IO.GenBank.MoleculeType.snoRNA -> Some MoleculeType.SnoRNA
        | Bio.IO.GenBank.MoleculeType.Protein -> Some MoleculeType.Protein
        | _ -> invalidArg "MoleculeType" "An invalid MoleculeType has been encountered."


    /// <summary>
    /// Specifies whether the sequence occurs as a single stranded, double stranded
    /// or mixed stranded.
    /// </summary>
    type StrandType =
        | Single
        | Double
        | Mixed

    let private createStrandType (strandType: Bio.IO.GenBank.SequenceStrandType) =
        match strandType with
        | Bio.IO.GenBank.SequenceStrandType.None -> None
        | Bio.IO.GenBank.SequenceStrandType.Single -> Some StrandType.Single
        | Bio.IO.GenBank.SequenceStrandType.Double -> Some StrandType.Double
        | Bio.IO.GenBank.SequenceStrandType.Mixed -> Some StrandType.Mixed
        | _ -> invalidArg "Strand" "An invalid Strand has been encountered."


    /// <summary>
    /// Specifies whether the strand is linear or circular.
    /// </summary>
    type StrandTopology =
        | Linear
        | Circular

    let private createStrandTopology (strandTopology: Bio.IO.GenBank.SequenceStrandTopology) =
        match strandTopology with
        | Bio.IO.GenBank.SequenceStrandTopology.None -> None
        | Bio.IO.GenBank.SequenceStrandTopology.Linear -> Some StrandTopology.Linear
        | Bio.IO.GenBank.SequenceStrandTopology.Circular -> Some StrandTopology.Circular
        | _ -> invalidArg "StrandTopology" "An invalid StrandTopology has been encountered."


    /// <summary>
    /// Short mnemonic name for the entry, chosen to suggest the sequence's
    /// definition.
    /// </summary>
    type Locus =
        { Date: System.DateTime option
          DivisionCode: DivisionCode option
          MoleculeType: MoleculeType option
          Name: string option
          SequenceLength: int
          SequenceType: string option
          Strand: StrandType option
          StrandTopology: StrandTopology option }

    let private createLocus (locus: Bio.IO.GenBank.GenBankLocusInfo) =
        match locus with
        | null -> None
        | _ ->
            Some
                { Date = locus.Date |> Helpers.parseOptionDate
                  DivisionCode = locus.DivisionCode |> createDivisionCode
                  MoleculeType = locus.MoleculeType |> createMoleculeType
                  Name = locus.Name |> Helpers.parseOptionString
                  SequenceLength = locus.SequenceLength
                  SequenceType = locus.SequenceType |> Helpers.parseOptionString
                  Strand = locus.Strand |> createStrandType
                  StrandTopology = locus.StrandTopology |> createStrandTopology }


    /// <summary>
    /// Citations for all articles containing data reported in this sequence.
    /// Citations in PubMed that do not fall within Medline's scope will have only a
    /// PUBMED identifier. Similarly, citations that *are* in Medline's scope but
    /// which have not yet been assigned Medline UIs will have only a PUBMED
    /// identifier. If a citation is present in both the PubMed and Medline
    /// databases, both a MEDLINE and a PUBMED line will be present.
    /// </summary>
    type Reference =
        { Authors: string option
          Consortiums: string option
          Journal: string option
          Location: string option
          Medline: string option
          Number: int
          PubMed: string option
          Remarks: string option
          Title: string option }

    let private createReference (reference: Bio.IO.GenBank.CitationReference) =
        { Authors = reference.Authors |> Helpers.parseOptionString
          Consortiums = reference.Consortiums |> Helpers.parseOptionString
          Journal = reference.Journal |> Helpers.parseOptionString
          Location = reference.Location |> Helpers.parseOptionString
          Medline = reference.Medline |> Helpers.parseOptionString
          Number = reference.Number
          PubMed = reference.PubMed |> Helpers.parseOptionString
          Remarks = reference.Remarks |> Helpers.parseOptionString
          Title = reference.Title |> Helpers.parseOptionString }

    let private createReferences (references: Bio.IO.GenBank.CitationReference list) =
        references
        |> List.map (fun reference -> createReference reference)
        |> Helpers.parseOptionList


    /// <summary>
    /// Information on the order in which this entry appears in a series of
    /// discontinuous sequences from the same molecule.
    /// </summary>
    type Segment = { Count: int; Current: int }

    let private createSegment (segment: Bio.IO.GenBank.SequenceSegment) =
        match segment with
        | null -> None
        | _ ->
            Some
                { Count = segment.Count
                  Current = segment.Current }


    /// <summary>
    /// Genus, Species and taxonomic classification levels of the sequence.
    /// </summary>
    type OrganismInfo =
        { ClassLevels: string option
          Genus: string option
          Species: string option }

    let private createOrganismInfo (organismInfo: Bio.IO.GenBank.OrganismInfo) =
        match organismInfo with
        | null -> None
        | _ ->
            Some
                { ClassLevels = organismInfo.ClassLevels |> Helpers.parseOptionString
                  Genus = organismInfo.Genus |> Helpers.parseOptionString
                  Species = organismInfo.Species |> Helpers.parseOptionString }


    /// <summary>
    /// Common name of the organism or the name most frequently used in the
    /// literature along with the taxonomic classification levels.
    /// </summary>
    type Source =
        { CommonName: string option
          Organism: OrganismInfo option }

    let private createSource (source: Bio.IO.GenBank.SequenceSource) =
        match source with
        | null -> None
        | _ ->
            Some
                { CommonName = source.CommonName |> Helpers.parseOptionString
                  Organism = source.Organism |> createOrganismInfo }


    /// <summary>
    /// Compound identifier consisting of the primary accession number and a numeric
    /// version number associated with the current version of the sequence data in
    /// the record. This is followed by an integer key (a "GI") assigned to the
    /// sequence by NCBI.
    /// </summary>
    type Version =
        { Accession: string option
          CompoundAccession: string option
          GiNumber: string option
          Version: string option }

    let private createVersion (version: Bio.IO.GenBank.GenBankVersion) =
        match version with
        | null -> None
        | _ ->
            Some
                { Accession = version.Accession |> Helpers.parseOptionString
                  CompoundAccession = version.CompoundAccession |> Helpers.parseOptionString
                  GiNumber = version.GiNumber |> Helpers.parseOptionString
                  Version = version.Version |> Helpers.parseOptionString }


    /// <summary>
    /// Metadata related to the GenBank Flat File.
    /// </summary>
    type Metadata =
        { Locus: Locus option
          Definition: string option
          Accession: Accession option
          Version: Version option
          DbLinks: DbLink list option
          DbSource: string option
          Keywords: string option
          Primary: string option
          Source: Source option
          References: Reference list option
          Comments: string list option
          Contig: string option
          Segment: Segment option
          Origin: string option }

    let createMetadata (metadata: Bio.IO.GenBank.GenBankMetadata) =
        { Locus = metadata.Locus |> createLocus
          Definition = metadata.Definition |> Helpers.parseOptionString
          Accession = metadata.Accession |> createAccession
          Version = metadata.Version |> createVersion
          DbLinks = metadata.DbLinks |> Seq.toList |> createDbLinks
          DbSource = metadata.DbSource |> Helpers.parseOptionString
          Keywords = metadata.Keywords |> Helpers.parseOptionString
          Primary = metadata.Primary |> Helpers.parseOptionString
          Source = metadata.Source |> createSource
          References = metadata.References |> Seq.toList |> createReferences
          Comments = metadata.Comments |> Seq.toList |> Helpers.parseOptionList
          Contig = metadata.Contig |> Helpers.parseOptionString
          Segment = metadata.Segment |> createSegment
          Origin = metadata.Origin |> Helpers.parseOptionString }
