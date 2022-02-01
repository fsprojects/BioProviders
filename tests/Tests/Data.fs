namespace BioProviders.Tests

open ProviderImplementation.ProvidedTypes
open BioProviders.DesignTime.Context
open System.Reflection

module Data = 

    // ----------------------------------------------------------------------------------
    // Data.
    // ----------------------------------------------------------------------------------
    
    let possibleTaxons = [
        "archae"
        "bacteria"
        "fungi"
        "invertebrate"
        "plant"
        "protozoa"
        "vertebrate_mammalian"
        "vertebrate_other"
        "viral"
    ]

    let possibleArchaeSpecies = [
        "Aigarchaeota_archaeon_SCGC_AAA471-E14"
        "Candidatus_Aenigmarchaeota_archaeon_JGI_0000106-F11"
        "Desulfurococcaceae_archaeon_AG1"
        "Euryarchaeota_archaeum_SCGC_AAA287-N16"
        "Ferroplasma_acidarmanus"
        "Geoglobus_ahangari"
        "Haloferax_sp._ATCC_BAA-646"
        "Infirmifilum_lucidum"
        "Methanospirillum_sp._J.3.6.1-F.2.7.3"
        "Natronolimnohabitans_innermongolicus"
        "Pyrobaculum_sp."
        "Salinarchaeum_sp._Harcht-Bsk1"
        "Thermoplasmatales_archaeon_A-plasma"
        "Vulcanisaeta_moutnovskia"
        "crenarchaeote_SCGC_AAA261-L14"
        "euryarchaeote_SCGC_AAA261-G15"
        "halophilic_archaeon_SHRA6"
        "methanogenic_archaeon_ISO4-H5"
        "uncultured_Acidilobus_sp._OSP8"
    ]

    let possibleBacteriaSpecies = [
        "Anaeromassilibacillus_sp._D41t1_190614_C2"
        "Bacillus_sp._B1-WWTP-T-0.5-Post-4"
        "Candidatus_Dependentiae_bacterium_ex_Spumella_elongata_CCAP_955_1_"
        "Diaphorobacter_polyhydroxybutyrativorans"
        "Ectothiorhodospiraceae_bacterium_2226"
        "Fannyhessea_vaginae"
        "Gemmatimonadetes_bacterium_JGI_0000112-M07"
        "Halodesulfovibrio_spirochaetisodalis"
        "Imperialibacter_sp._89"
        "Janthinobacterium_agaricidamnosum"
        "Kluyvera_genomosp._1"
        "Lachnoclostridium_sp._Marseille-P6806"
        "Marinobacter_sp."
        "Nevskia_soli"
        "Oceanicola_sp._MCTG156_1a_"
        "PVC_group_bacterium"
        "Raoultella_sp._18105"
        "Staphylococcus_borealis"
        "Thauera_selenatis"
        "Urbifossiella_limnaea"
        "Variovorax_sp._PAMC26660"
        "WS1_bacterium_JGI_0000059-K21"
        "Xanthobacteraceae_bacterium_501b"
        "Yonghaparkia_sp._Soil809"
        "Zunongwangia_sp._SCSIO_43204"
        "_Sphingomonas_ginsengisoli_Hoang_et_al._2012"
        "aff._Roholtiella_sp._LEGE_12411"
        "bacteria_symbiont_BFo1_of_Frankliniella_occidentalis"
        "cf._Phormidesmis_sp._LEGE_11477"
        "delta_proteobacterium_JGI_0000059-J07"
        "endosymbiont_of_unidentified_scaly_snail_isolate_Monju"
        "filamentous_cyanobacterium_Phorm_46"
        "gamma_proteobacterium_SCGC_AAA076-F14"
        "methane-oxidizing_endosymbiont_of_Gigantopelta_aegis"
        "sulfur-oxidizing_endosymbiont_of_Gigantopelta_aegis"
        "thiotrophic_endosymbiont_of_Bathymodiolus_puteoserpentis_Logatchev_"
        "unidentified_eubacterium_SCB49"
        "zeta_proteobacterium_SCGC_AB-604-B04"
    ]

    let possibleFungiSpecies = [
        "Acaromyces_ingoldii"
        "Blastomyces_dermatitidis"
        "Cutaneotrichosporon_oleaginosum"
        "Daldinia_childiae"
        "Eutypa_lata"
        "Fusarium_fujikuroi"
        "Guyanagaster_necrorhizus"
        "Histoplasma_mississippiense_nom._inval._"
        "Jaminaea_rosea"
        "Kockovaella_imperatae"
        "Lachancea_lanzarotensis"
        "Mycena_indigotica"
        "Neurospora_tetrasperma"
        "Ogataea_parapolymorpha"
        "Penicillium_arizonense"
        "Rhizophagus_irregularis"
        "Sparassis_crispa"
        "Tetrapisispora_blattae"
        "Ustilaginoidea_virens"
        "Verticillium_alfalfae"
        "Westerdykella_ornata"
        "Xylona_heveae"
        "Yamadazyma_tenuis"
        "Zygosaccharomyces_rouxii"
        "_Candida_haemuloni"
    ]

    let possibleInvertebrateSpecies = [
        "Anoplophora_glabripennis"
        "Bombyx_mandarina"
        "Cryptotermes_secundus"
        "Drosophila_melanogaster"
        "Exaiptasia_diaphana"
        "Frankliniella_occidentalis"
        "Galleria_mellonella"
        "Helicoverpa_armigera"
        "Ixodes_scapularis"
        "Loa_loa"
        "Manduca_sexta"
        "Neodiprion_lecontei"
        "Onthophagus_taurus"
        "Papilio_xuthus"
        "Rhopalosiphum_maidis"
        "Stylophora_pistillata"
        "Trachymyrmex_septentrionalis"
        "Varroa_jacobsoni"
        "Wasmannia_auropunctata"
        "Zootermopsis_nevadensis"
    ]

    let possiblePlantSpecies = [
        "Arachis_hypogaea"
        "Brassica_napus"
        "Cannabis_sativa"
        "Durio_zibethinus"
        "Erythranthe_guttata"
        "Glycine_soja"
        "Hibiscus_syriacus"
        "Ipomoea_triloba"
        "Juglans_microcarpa_x_Juglans_regia"
        "Macadamia_integrifolia"
        "Nicotiana_tabacum"
        "Ostreococcus_sp._lucimarinus_"
        "Prunus_mume"
        "Quercus_lobata"
        "Rhodamnia_argentea"
        "Salvia_splendens"
        "Tripterygium_wilfordii"
        "Volvox_carteri"
        "Zingiber_officinale"
    ]

    let possibleProtozoaSpecies = [
        "Acanthamoeba_castellanii"
        "Blastocystis_sp._subtype_4"
        "Cryptosporidium_ubiquitum"
        "Dictyostelium_discoideum"
        "Eimeria_maxima"
        "Gregarina_niphandrodes"
        "Hemiselmis_andersenii"
        "Ichthyophthirius_multifiliis"
        "Leishmania_panamensis"
        "Nannochloropsis_gaditana"
        "Plasmodium_sp._gorilla_clade_G2"
        "Saprolegnia_parasitica"
        "Trypanosoma_rangeli"
    ]

    let possibleVertebrateMammalianSpecies = [
        "Arvicanthis_niloticus"
        "Bos_indicus_x_Bos_taurus"
        "Camelus_ferus"
        "Dipodomys_ordii"
        "Elephantulus_edwardii"
        "Felis_catus"
        "Grammomys_surdaster"
        "Homo_sapiens"
        "Ictidomys_tridecemlineatus"
        "Jaculus_jaculus"
        "Loxodonta_africana"
        "Molossus_molossus"
        "Nomascus_leucogenys"
        "Ornithorhynchus_anatinus"
        "Prionailurus_bengalensis"
        "Rattus_rattus"
        "Sorex_araneus"
        "Tupaia_chinensis"
        "Ursus_arctos"
        "Vombatus_ursinus"
        "Zalophus_californianus"
    ]

    let possibleVertebrateOtherSpecies = [
        "Antrostomus_carolinensis"
        "Betta_splendens"
        "Centrocercus_urophasianus"
        "Danio_rerio"
        "Electrophorus_electricus"
        "Ficedula_albicollis"
        "Gopherus_evgoodei"
        "Hippocampus_comes"
        "Ictalurus_punctatus"
        "Kryptolebias_marmoratus"
        "Lonchura_striata"
        "Micropterus_salmoides"
        "Nestor_notabilis"
        "Oryzias_latipes"
        "Pseudochaenichthys_georgianus"
        "Rhinatrema_bivittatum"
        "Salvelinus_sp._IW2-2015"
        "Tyto_alba"
        "Varanus_komodoensis"
        "Xiphophorus_hellerii"
        "Zootoca_vivipara"
    ]

    let possibleViralSpecies = [
        "Actinidia_chlorotic_ringspot-associated_emaravirus"
        "Bandicoot_papillomatosis_carcinomatosis_virus_type_1"
        "Carnation_ringspot_virus"
        "Delisea_pulchra_RNA_virus"
        "Eggplant_mottled_dwarf_nucleorhabdovirus"
        "False_black_widow_spider_associated_circular_virus_1"
        "Gannoruwa_bat_lyssavirus"
        "Haloferax_virus_HF1"
        "Indian_citrus_ringspot_virus"
        "Jasmine_virus_T"
        "Kedougou_virus"
        "Lactobacillus_virus_A2"
        "Madariaga_virus"
        "Nanay_virus"
        "Only_Syngen_Nebraska_virus_5"
        "Palyam_virus"
        "Quailpox_virus"
        "Raphanus_sativus_cryptic_virus_1"
        "Sclerotinia_sclerotiorum_debilitation-associated_RNA_virus"
        "Tea_plant_line_pattern_virus"
        "Upsilonpapillomavirus_1"
        "Varroa_mite_associated_genomovirus_1"
        "Watermelon_virus_A"
        "Xapuri_mammarenavirus"
        "Yerba_mate-associated_circular_DNA_virus_1"
        "Zurich_hartmanivirus"
        "crAssphage_cr113_1"
        "uncultured_Caudovirales_phage"
        "unidentified_human_coronavirus"
    ]

    let possibleSpecies = List.concat([
        possibleArchaeSpecies
        possibleBacteriaSpecies
        possibleFungiSpecies
        possibleInvertebrateSpecies
        possiblePlantSpecies
        possibleProtozoaSpecies
        possibleVertebrateMammalianSpecies
        possibleVertebrateOtherSpecies
        possibleViralSpecies
    ])

    let possibleAssemblies = [
        "GCF_014905175.1_ASM1490517v1"
        "GCF_000355655.1_DendPond_male_1.0"
        "GCF_002803265.2_SCAv2.0"
        "GCF_002237135.1_ASM223713v2"
        "GCF_002201575.1_ASM220157v1"
        "GCF_000001405.27_GRCh38.p1"
        "GCF_000001405.8_NCBI33"
        "GCF_000956105.1_Pcoq_1.0"
        "GCF_011064425.1_Rrattus_CSIRO_v1"
        "GCF_015852505.1_mTacAcu1.pri"
        "GCF_003431325.1_ASM343132v1"
        "GCF_005543295.1_ASM554329v1"
        "GCF_000730175.1_ASM73017v1"
        "GCF_004698125.1_ASM469812v1"
        "GCF_017874455.1_ASM1787445v1"
        "GCF_013839515.1_ASM1383951v1"
        "GCF_000313525.1_ASM31352v1"
        "GCF_000149585.1_ASM14958v1"
        "GCA_003706315.1_ASM370631v1"
        "GCA_001931935.1_ASM193193v1"
        "GCA_019976455.1_ASM1997645v1"
        "GCA_002006685.1_Batr_sala_BS_V1"
        "GCA_020617725.1_AMFP15.3.pb"
        "GCA_020617735.1_ASM2061773v1"
        "GCA_020617735.1_ASM2061773v1"
        "GCA_002914405.1_ByssAF2.0"
        "GCA_016906875.1_ASM1690687v1"
        "GCA_000372705.1_YLSCbra_1.0"
        "GCA_020087005.1_ASM2008700v1"
        "GCA_018360325.1_CSU_LM81_v1"
    ]

    let possibleEmptyValues = [
        ""
        " "
        "\t"
        "\n"
        "\r"
        "\f"
        "\t\f\n \t  \f\r"
    ]

    let possibleNames = List.concat [ possibleTaxons; possibleSpecies; possibleAssemblies ]
   

    // ----------------------------------------------------------------------------------
    // Generation Functions.
    // ----------------------------------------------------------------------------------

    let generateEmptyString () = 
        FsCheck.Gen.elements possibleEmptyValues

    let generatePlainName () =
        FsCheck.Gen.elements possibleNames

    let generatePlainTaxonString () = 
        FsCheck.Gen.elements possibleTaxons

    let generatePlainSpeciesString () = 
        FsCheck.Gen.elements possibleSpecies

    let generatePlainAssemblyString () = 
        FsCheck.Gen.elements possibleAssemblies

    let generateRegexTaxonString () = 
        possibleTaxons
        |> List.map (fun taxon -> taxon + "*")
        |> FsCheck.Gen.elements

    let generateRegexSpeciesString () = 
        possibleSpecies
        |> List.map (fun species -> species + "*")
        |> FsCheck.Gen.elements

    let generateRegexAssemblyString () = 
        possibleAssemblies
        |> List.map (fun assembly -> assembly + "*")
        |> FsCheck.Gen.elements

    let generateProvidedType () =
        let namespaceName = "BioProviders"
        let thisAssembly = Assembly.GetExecutingAssembly()
        let providedType = ProvidedTypeDefinition(thisAssembly, namespaceName, "GenBankProvider", Some typeof<obj>)

        [ providedType ]
        |> FsCheck.Gen.elements

    let generateDatabase () = 
        [ RefSeq; GenBank ]
        |> FsCheck.Gen.elements

    let generatePlainTaxon () = 
        (generatePlainTaxonString ())
        |> FsCheck.Gen.map (fun taxon -> TaxonName.Create taxon)

    let generateRegexTaxon () = 
        (generateRegexTaxonString ())
        |> FsCheck.Gen.map (fun taxon -> TaxonName.Create taxon)

    let generateEmptyTaxon () = 
        (generateEmptyString ())
        |> FsCheck.Gen.map (fun taxon -> TaxonName.Create taxon)

    let generatePlainSpecies () = 
        (generatePlainSpeciesString ())
        |> FsCheck.Gen.map (fun species -> SpeciesName.Create species)

    let generateRegexSpecies () = 
        (generateRegexSpeciesString ())
        |> FsCheck.Gen.map (fun species -> SpeciesName.Create species)

    let generateEmptySpecies () =
        (generateEmptyString ())
        |> FsCheck.Gen.map (fun species -> SpeciesName.Create species)

    let generatePlainAssembly () = 
        (generatePlainAssemblyString ())
        |> FsCheck.Gen.map (fun assembly -> AssemblyName.Create assembly)

    let generateRegexAssembly () = 
        (generateRegexAssemblyString ())
        |> FsCheck.Gen.map (fun assembly -> AssemblyName.Create assembly)

    let generateEmptyAssembly () = 
        (generateEmptyString ())
        |> FsCheck.Gen.map (fun assembly -> AssemblyName.Create assembly)

    let generatePartialContext () = 
        let (<!>) = FsCheck.Gen.map
        let (<*>) = FsCheck.Gen.apply

        let numRegex = System.Random().Next(1, 4)
        let regexFields = [0; 1; 2]
                          |> FsCheck.Gen.shuffle
                          |> FsCheck.Gen.sample 0 1
                          |> Seq.head
                          |> Seq.take numRegex
                          |> Seq.toList

        let providedType = generateProvidedType ()
        let database = generateDatabase ()
        let taxon = if (List.contains 0 regexFields) then generateRegexTaxon () else generatePlainTaxon ()
        let species = if (List.contains 1 regexFields) then generateRegexSpecies () else generatePlainSpecies ()
        let assembly = if (List.contains 2 regexFields) then generateRegexAssembly () else generatePlainAssembly ()

        PartialContext.Create 
        <!> providedType
        <*> database
        <*> taxon
        <*> species
        <*> assembly


    let generateCompleteContext () = 
        let (<!>) = FsCheck.Gen.map
        let (<*>) = FsCheck.Gen.apply

        let providedType = generateProvidedType ()
        let database = generateDatabase ()
        let taxon = generatePlainTaxon ()
        let species = generatePlainSpecies ()
        let assembly = generatePlainAssembly ()

        CompleteContext.Create 
        <!> providedType
        <*> database
        <*> taxon
        <*> species
        <*> assembly

