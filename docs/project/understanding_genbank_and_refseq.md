# Understanding GenBank and RefSeq
This document provides the overview necessary to understand GenBank and RefSeq in the context of this project. [GenBank](https://www.ncbi.nlm.nih.gov/genbank/) and [RefSeq](https://www.ncbi.nlm.nih.gov/refseq/about/) are considerably complex, so if you wish to learn more, please refer to their proper documentation.

The following questions are covered in this document:

**General**
1. What is an Assembly?
2. What is GenBank Flat File Format?

**GenBank**
1. What is GenBank?
2. How Often is GenBank Updated?
3. How is GenBank Accessed?
4. How Does GenBank Store DNA Sequences?
5. How is GenBank’s FTP Server Structured?

**RefSeq**
1. What is RefSeq?
2. How Often is RefSeq Updated?
3. How is RefSeq Accessed?
4. How Does RefSeq Store DNA Sequences?
5. How is RefSeq's FTP Server Structured?

## General
### 1. What is an Assembly?
An assembly is a computational representation of a genome sequence. 

### 2. What is GenBank Flat File Format?
To store DNA sequences, GenBank uses the Flat File format, a sample of this which can be viewed [here](https://www.ncbi.nlm.nih.gov/genbank/samplerecord/).  This format separates content into two sections: an annotation section, and a sequence section.

#### Annotations
The annotations section begins at the LOCUS field and concludes at the ORIGIN field. 

This section contains four essential fields that are used to identify and maintain the sequence data: LOCUS, DEFINITION, ACCESSION, and VERSION. All other fields in this section are intended to provide additional information that is important but not necessary and can be excluded. These additional fields are often referred to as metadata and can include more detailed information about the sequence such as cross-references to other databases and a list of publications the sequence has appeared in.

#### Sequence
Sequence data begins on the line immediately below the ORIGIN field and concludes on the line above the end-of-file delimiter ("//").

This section contains the sequence data of the DNA.

## GenBank
### 1. What is GenBank?
GenBank is the National Institutes of Health's ([NIH](https://www.nih.gov/)) genetic sequence database, produced and maintained by the National Center for Biotechnology Information ([NCBI](https://www.ncbi.nlm.nih.gov/)). This database contains an annotated collection of all publicly available DNA sequences. 

Being a public database, GenBank may contain inconsistencies and inaccuracies such as incomplete annotations or sequences assigned to a species incorrectly.

### 2. How Often is GenBank Updated?
GenBank has a new release every two months. Information about the most current release can be found [here](https://www.ncbi.nlm.nih.gov/genbank/release/current/).

### 3. How is GenBank Accessed?
There are several methods for searching and retrieving GenBank data including:
* [Entrez Nucleotide](https://www.ncbi.nlm.nih.gov/nucleotide/)
* [Basic Local Alignment Search Tool (BLAST)](https://www.ncbi.nlm.nih.gov/blast/)
* [GenBank Anonymous FTP](https://ftp.ncbi.nlm.nih.gov/genomes/genbank/)

For this project, it is only necessary to understand the GenBank FTP server.


### 4. How Does GenBank Store DNA Sequences?
GenBank uses the GenBank Flat File format to store DNA sequences with annotations.

### 5. How is GenBank's FTP Server Structured?
GenBank's anonymous FTP server follows a hierarchical structure consisting of the levels:

1. Root
2. Taxon
3. Species
4. Assembly
5. Content

From an abstract view, a species content can be found at the URL:

``` 
root/taxon/species/assembly/content
```

For example:

``` 
ftp://ftp.ncbi.nlm.nih.gov/genomes/genbank/bacteria/Staphylococcus_borealis/all_assembly_versions/GCA_003042555.1_ASM304255v1/GCA_003042555.1_ASM304255v1_genomic.gbff.gz
```

This URL contains the following hierarchical levels:

1. Root = ftp:/<span>/</span>ftp.ncbi.nlm.nih<span>.</span>gov/genomes/genbank
2. Taxon = bacteria
3. Species = Staphylococcus_borealis
4. Assembly = all_assembly_versions/GCA_003042555.1_ASM304255v1
5. Content = GCA_003042555.1_ASM304255v1_genomic.gbff.gz


#### Root 
GenBank's root FTP directory is ftp://ftp.ncbi.nlm.nih.gov/genomes/genbank/.

> ftp.ncbi.nlm.nih.gov and  ftp.ncbi.nih.gov provide the same content, though the latter may not be supported indefinitely.

#### Taxon
``` 
E.g. bacteria
```

GenBank's root FTP directory is split into a set of taxonomic subdirectories. Each of the following directories will be referred to as a taxon:

1. Archaea
2. Bacteria
3. Fungi
4. Invertebrate
5. Metagenomes
6. Other
7. Plant
8. Protozoa
9. Vertebrate Mammalian
10. Vertebrate Other
11. Viral

#### Species
``` 
E.g. Staphylococcus_borealis
```

Species are organised in a taxon using directories with the binomial nomenclature *Genus_species*.

> GenBank's species naming is quite flexible, so caution must be used when working with these names (e.g., _Massilia_aquatica_Lu_et_al._2020).

#### Assembly
``` 
E.g. GCA_003042555.1_ASM304255v1
```

Each species directory contains a set of all assemblies, latest assemblies, and representative assemblies for the species (if any). These assemblies following the naming convention: 

*[Assembly accession.version]_[assembly name]*

#### Contents
``` 
E.g. GCA_003042555.1_ASM304255v1_genomic.gbff.gz
```

Data files are provided for each assembly using the naming convention:

*[Assembly accession.version]\_[assembly name]\_[content type].[optional format]*

A full list of file and content formats is available in this [document](https://ftp.ncbi.nlm.nih.gov/genomes/genbank/README.txt). However, for this project, it is only important to know the following:

Extension | Description
:--- | :---
*_genomic.gbff.gz| Gzipped GenBank flat file format of the genomic sequence(s) in the assembly. This file includes both the genomic sequence and the CONTIG description.



## RefSeq
### 1. What is RefSeq?
Reference Sequence (RefSeq) is a genomic sequence database produced and maintained by the National Center for Biotechnology Information ([NCBI](https://www.ncbi.nlm.nih.gov/)). This database contains a comprehensive, integrated, non-redundant, and well-annotated subset of assembled genomes available in GenBank.

The main features of the RefSeq collection include:

* Non-redundancy
* Explicitly linked nucleotide and protein sequences
* Updates to reflect current knowledge of sequence data and biology
* Data validation and format consistency
* [Distinct accession series](https://www.ncbi.nlm.nih.gov/books/NBK21091/table/ch18.T.refseq_accession_numbers_and_mole/?report=objectonly) 
* Ongoing curation by NCBI staff and collaborators, with reviewed records indicated

Essentially, RefSeq is a heavily curated subset of GenBank.

### 2. How Often is RefSeq Updated?
According to [this](https://ftp.ncbi.nlm.nih.gov/refseq/release/README), RefSeq releases are scheduled for odd numbered months: January, March, May, July, September, November. 

Historically, however, RefSeq releases coincide with GenBank releases.

### 3. How is RefSeq Accessed?
Similarly to GenBank access, RefSeq data can be searched and retrieved via:

* [Entrez Nucleotide](https://www.ncbi.nlm.nih.gov/nucleotide/)
* [Basic Local Alignment Search Tool (BLAST)](https://www.ncbi.nlm.nih.gov/blast/)
* [RefSeq Anonymous FTP](https://ftp.ncbi.nlm.nih.gov/genomes/refseq/)

Again, it is only necessary to understand the RefSeq FTP server for this project.

### 4. How Does RefSeq Store DNA Sequences?
RefSeq uses the GenBank Flat File format to store DNA sequences with annotations.

### 5. How is RefSeq's FTP Server Structured?


#### Root
RefSeq's root FTP directory is ftp://ftp.ncbi.nlm.nih.gov/genomes/refseq/.

> ftp.ncbi.nlm.nih.gov and  ftp.ncbi.nih.gov provide the same content, though the latter may not be supported indefinitely.

#### Taxon
RefSeq's root FTP directory is split into a set of taxonomic subdirectories. Each of the following directories will be referred to as a taxon:

1. Archaea
2. Bacteria
3. Fungi
4. Invertebrate
5. Plant
6. Plasmid
7. Plastid
8. Protozoa
9. Vertebrate Mammalian
10. Vertebrate Other
11. Viral

#### Species
Same as GenBank's structure.

#### Assembly
Same as GenBank's structure.

#### Content
Same as GenBank's structure.

## Further Resources
### General Information
* [About GenBank (NCBI)](https://www.ncbi.nlm.nih.gov/genbank/)
* [About GenBank (Wikipedia)](https://en.wikipedia.org/wiki/GenBank)
* [About RefSeq (NCBI)](https://www.ncbi.nlm.nih.gov/refseq/about/)
* [About RefSeq (Wikipedia)](https://en.wikipedia.org/wiki/RefSeq)

### GenBank Flat File Information
* [Example File](https://www.ncbi.nlm.nih.gov/genbank/samplerecord/)
* [GenBank Flat File Structure and Purpose](https://www.futurelearn.com/info/courses/bacterial-genomes-bioinformatics/0/steps/47012)

### GenBank and RefSeq FTP
* [GenBank and RefSeq FTP FAQ](https://www.ncbi.nlm.nih.gov/genome/doc/ftpfaq/)
* [GenBank and RefSeq FTP Structure](https://ftp.ncbi.nlm.nih.gov/genomes/genbank/README.txt)
* [GenBank File Naming Conventions](https://ftp.ncbi.nlm.nih.gov/genomes/genbank/README.txt)

