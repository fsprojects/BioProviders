# Project Progress
This document will outline the development goals that have been completed, and are scheduled for completion in the future.

## Completed
The following have been completed, in no particular order:

**Development:**
&#9989; Implemented separation between TypeProvider and TypeProvider dependent files
* BioProviders.sln 
* BioProviders.TestAndDocs.sln

&#9989; Implemented DesignTime/RunTime separation
&#9989; GenBank Flat File RunTime Type implemented with following features:
* Metadata
	* Accession
	* Base Count
	* Comments
	* Contig
	* DbLinks
	* DbSource
	* Definition
	* Features
	* Keywords
	* Locus
	* Origin
	* Primary
	* Project
	* References
	* Segment
	* Source
	* Version

**Testing:**
&#9989; Implemented initial testing structure

**Release:**

**Documentation:**
&#9989; Overview document
&#9989; Understanding Genbank and RefSeq document
&#9989; Project Purpose and Scope document

## Scheduled
The following are scheduled to be completed, in no particular order:

**Development:**
&#9203; Make entire GenBank FTP structure explorable
* Taxon
* Species
* Assemblies
* Assembly Files

&#9203; Only expose non-empty fields for each GenBank Flat File document
* Is this possible?

&#9203; Allow wildcard operators in parameters
* E.g., `GenBankProvider<"bacteria", "Staphylococcus_borealis", "*">` would provide all assemblies for the species Staphylococcus_borealis

**Testing:**
&#9203; Unit Tests for Common.Cache
&#9203; Unit Tests for Common.FTP
&#9203; Unit Tests for DesignTime.Context
&#9203; Unit Tests for DesignTime.TypeGenerator
&#9203; Test suite for ProviderTests
&#9203; Test script for ProviderTests

**Release:**
&#9203; Properly structure output directory
* Refer to [this](https://github.com/dotnet/fsharp/issues/3736).
* Properly package RunTime dependencies - do these need to be packaged alongside the RunTime component, or do we rely on the user to install the dependencies?

&#9203; Package BioProviders with Nuget
&#9203; Initial release of BioProviders

**Documentation:**
&#9203; Complete Understanding Type Providers document
&#9203; Complete Understanding F# document
&#9203; Complete Understanding Visual Studio Development document
&#9203; Complete BioProviders Examples document
&#9203; Complete BioProviders Code Discussion document
