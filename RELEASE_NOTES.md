### 2.2.1 - Nov 2 2023
* Fixed exception thrown when BioProviders folder does not exist in AppData/Local/Temp and the type provider attempts to download a data file.

### 2.2.0 - Oct 24 2023
* Added RefSeq type provider.

### 2.1.0 - Oct 9 2023
* Fixed bug in previous versions that prevented data files from being accessed.
* Data files are no longer included to reduce package size - automatically downloaded on demand.
* Cached files in temporary folder are removed after 90 days if not used within that time.

### 2.0.0 - Feb 2 2023
* Only species name and accession must now be provided to GenBankProvider.
* Added support for wildcards in both species name and accession.
* General changes to the GenBankProvider structure.

### 1.0.0 - Jun 30 2022
* Initial release.
* GenBankProvider included.