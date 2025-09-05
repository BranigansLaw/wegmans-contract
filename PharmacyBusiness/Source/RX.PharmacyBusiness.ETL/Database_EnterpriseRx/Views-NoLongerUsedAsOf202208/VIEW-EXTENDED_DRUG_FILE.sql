
  CREATE OR REPLACE FORCE EDITIONABLE VIEW "WEGMANS"."EXTENDED_DRUG_FILE" ("ndc_wo", "ndc", "drug_name", "drug_manufacturer", "generic_name", "pack_size", "true_pack", "pm", "is preferred", "con_unit", "con_source", "con_vendor", "con_eff_date", "supplier_eff_date", "itemno", "product_msg", "msg_eff_date", "msg_exp_date", "pog_name", "include_exclude", "pog_direct_inclusion", "pog_is_primary", "ordering_product", "pog_user_key", "pog_change_date", "last_mapped_to_ndc", "date_mapped", "cfblocked_store_count", "date_cfblocked", "decile", "product_group", "sdgi", "alt_label_name", "source", "gcn", "gcn_seq_num", "strength", "orange_book_code", "drug_key", "drug_num", "product_key", "product_num", "dea_sched", "date_added", "obsolete_date", "deactive_date", "dosage_form", "ahfs_class", "ahfs_desc", "ther_class", "hic3_code", "hic3_desc", "desi_code", "hcpcs_code", "last_provider_update", "fda_approve_date", "hcfa_type", "mfg_release_date") AS 
  SELECT 
        to_char(drug.DRUG_NDC) "ndc_wo",
        to_char(drug.DRUG_NDC,'00000g0000g00','nls_numeric_characters=.-') "ndc",
        drug.DRUG_LABEL_NAME "drug_name",
        drug.DRUG_LABELER "drug_manufacturer",
        drug.DRUG_GNN "generic_name",
        drug.DRUG_PACKAGE_SIZE "pack_size",
        drug.DRUG_CASE_SIZE*drug.DRUG_PACKAGE_SIZE "true_pack",
        product.PRD_IS_PRICE_MAINTAINED "pm",
        prod_p_msg.PREFERRED "is preferred",
        
        NVL(prodcostfact.COST,manualprdcostbasis.MANUAL_COST) as "con_unit",
        NVL(costbasissource.COST_BASIS_SOURCE_NAME,CASE WHEN manualprdcostbasis.MANUAL_COST is null THEN null ELSE 'Manual Cost' END) "con_source",
        costbasissource.PROVIDER "con_vendor",
        NVL(prodcostfact.EFFECTIVE_DATE,CASE WHEN manualprdcostbasis.MANUAL_COST is null THEN null ELSE manualprdcostbasis.MANUAL_COST_EFF_DATE END) "con_eff_date",
        NVL(prodcostbasis.EFFECTIVE_DATE,CASE WHEN manualprdcostbasis.MANUAL_COST is null THEN null ELSE manualprdcostbasis.MANUAL_COST_EFF_DATE END) "supplier_eff_date",
        
        vendor_item.ITEM_NUM "itemno",
        replace(prod_p_msg.PRODUCT_MSG, CHR(10), ' ') "product_msg",
        prod_p_msg.MSG_EFF_DATE "msg_eff_date",
        prod_p_msg.MSG_EXP_DATE "msg_exp_date",

        NVL(pog.NAME,secpog.NAME) "pog_name",
          NVL(DECODE(pog.INCLUDE_EXCLUDE_FLAG,'N','Include by NDC','G','Include by GCN','P','Include by Pack Size and GCN','X','Exclude by NDC','D','Default'),
          CASE pog.IS_PRIMARY_PRODUCT WHEN 'Y' THEN 'Primary Product'
          ELSE DECODE(secpog.INCLUDE_EXCLUDE_FLAG,'N','Include by NDC','G','Include by GCN','P','Include by Pack Size and GCN','X','Exclude by NDC','D','Default') END) "include_exclude",
        NVL(pog.IS_DIRECT_INCLUSION,secpog.IS_DIRECT_INCLUSION) "pog_direct_inclusion",
        NVL(pog.IS_PRIMARY_PRODUCT,secpog.IS_PRIMARY_PRODUCT) "pog_is_primary",
        NVL(pog.IS_ORDERING_PRODUCT,secpog.IS_ORDERING_PRODUCT) "ordering_product",
        NVL(pog.SYS_USER,secpog.SYS_USER) "pog_user_key",
        NVL(pog.DATESTAMP,secpog.DATESTAMP) "pog_change_date",
        
        to_char(prodmapping.DRUG_NDC) "last_mapped_to_ndc",
        prodmapping.MAP_START_DATE "date_mapped",
        
        cfblocked.BLOCK_STORE_COUNT "cfblocked_store_count",
        cfblocked.BLOCK_DATE "date_cfblocked",

        prodgroup.GROUP_NAME "decile",
        secprodgroup.GROUP_NAME "product_group",
        product.PRD_IS_GENERIC "sdgi",
        product.PRD_ALTERNATE_LABEL_NAME "alt_label_name",
        product.PRD_SOURCE_INDICATOR "source",
        drug.DRUG_GCN "gcn",
        drug.DRUG_GCN_SEQ "gcn_seq_num",
        UPPER(drug.DRUG_STRENGTH||' '||drug.DRUG_STRENGTH_UNITS) "strength",
        drug.DRUG_ORANGE_BOOK_CODE "orange_book_code",
        
        drug.DRUG_KEY "drug_key",
        drug.DRUG_NUM "drug_num",
        product.PRD_PRODUCT_KEY "product_key",
        product.PRD_PRODUCT_NUM "product_num",
       
        drug.DRUG_DEA_CLASS "dea_sched",
        drug.DRUG_DATE_OF_ADD "date_added",
        drug.DRUG_OBSOLETE_DATE "obsolete_date",
        product.PRD_DEACTIVATE_DATE "deactive_date",
        drug.DRUG_DOSAGE_FORM "dosage_form",
        SUBSTR(drug.DRUG_AHFS,1,2)||':'||SUBSTR(drug.DRUG_AHFS,3,2)||':'||SUBSTR(drug.DRUG_AHFS,5,5) "ahfs_class",
        drug.DRUG_AHFS_DESCRIPTION "ahfs_desc",
        drug.DRUG_GEN_THERAPEUTIC_CLASS "ther_class",
        drug.DRUG_GC3_CODE "hic3_code",
        drug.DRUG_GC3 "hic3_desc",
        drug.DRUG_HCFA_DESI "desi_code",
        drug.DRUG_HCPCS_CODE "hcpcs_code",
        drug.DRUG_LAST_PROVIDER_UPDATE "last_provider_update",
        drug.DRUG_HCFA_APPROVAL_DATE "fda_approve_date",
        
        CASE drug.DRUG_HCFA_TYPE WHEN '1' THEN 'RX'
        WHEN '2' THEN 'OTC'
        ELSE 'Not Provided' END as "hcfa_type",
        drug.DRUG_HCFA_MARKET_ENTRY_DATE "mfg_release_date"

FROM TREXONE_DW_DATA.DRUG drug

LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT product
ON drug.DRUG_NUM = product.PRD_DRUG_NUM

--linking table to get the next joins for cost basis source and cost
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_COST_BASIS prodcostbasis
ON drug.DRUG_KEY = prodcostbasis.DRUG_KEY
AND prodcostbasis.TERMINATION_DATE is null
AND prodcostbasis.COST_BASIS_KEY = 535
AND prodcostbasis.COST_BASIS_SOURCE_KEY <> 0

LEFT OUTER JOIN
(SELECT manualprdcostbasis.DRUG_KEY as "DRUG_KEY",
        AVG(DISTINCT manualprdcostbasis.ADJUST_MODIFIER) as "MANUAL_COST",
        MAX(manualprdcostbasis.EFFECTIVE_DATE) as "MANUAL_COST_EFF_DATE"
        
        FROM TREXONE_DW_DATA.PRODUCT_COST_BASIS manualprdcostbasis
        
WHERE manualprdcostbasis.TERMINATION_DATE is null
AND manualprdcostbasis.COST_BASIS_KEY = 535
AND manualprdcostbasis.COST_BASIS_SOURCE_KEY = 0

GROUP BY manualprdcostbasis.DRUG_KEY)manualprdcostbasis
        
ON drug.DRUG_KEY = manualprdcostbasis.DRUG_KEY

--to get the source of the cost basis (ANDA PRIMARY, HARVARD PRIMARY, Etc.)
LEFT OUTER JOIN TREXONE_DW_DATA.COST_BASIS_SOURCE costbasissource
ON prodcostbasis.COST_BASIS_SOURCE_KEY = costbasissource.COST_BASIS_SOURCE_KEY
AND trunc(costbasissource.EFF_END_DATE) = to_date('12/31/2999','MM/DD/YYYY')

--to get the CON (or any other cost factors) of the drug)
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_COST_FACT prodcostfact
ON prodcostfact.DRUG_KEY = drug.DRUG_KEY
AND prodcostfact.COST_BASIS_SOURCE_KEY = prodcostbasis.COST_BASIS_SOURCE_KEY
AND prodcostfact.TERMINATION_DATE is null

--this is for DECILE
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_GROUP prodgroup
ON product.PRD_PRODUCT_NUM = prodgroup.PRODUCT_NUM
AND prodgroup.MEMBER_STATUS = 'Active'
AND prodgroup.group_name in ('0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F','G','H','I','J')

--this is for non-DECILE secondary groups
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_GROUP secprodgroup
ON product.PRD_PRODUCT_NUM = secprodgroup.PRODUCT_NUM
AND secprodgroup.MEMBER_STATUS = 'Active'
AND secprodgroup.group_name not in ('0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F','G','H','I','J')
AND ROWNUM = 1

--Links in POG information --Direct Inclusions Only
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_ORDERING_GROUP_MEMBER pog
ON PRODUCT.PRD_PRODUCT_KEY = pog.PRD_PRODUCT_KEY
AND trunc(pog.EFF_END_DATE) = to_date('12/31/2999','MM/DD/YYYY') AND ROWNUM = 1
AND pog.IS_DIRECT_INCLUSION = 'Y'

--Link in POG information --Not Direct Inclusions
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_ORDERING_GROUP_MEMBER secpog
ON PRODUCT.PRD_PRODUCT_KEY = secpog.PRD_PRODUCT_KEY
AND trunc(secpog.EFF_END_DATE) = to_date('12/31/2999','MM/DD/YYYY') AND ROWNUM = 1
AND secpog.IS_DIRECT_INCLUSION = 'N'

--Start-- Links in Product Mapping information
LEFT OUTER JOIN
      (SELECT proddmap.PRD_PRODUCT_KEY as "MAP_PRODUCT_KEY",
              maptodrug.DRUG_NDC as "DRUG_NDC",
              proddmap.EFF_START_DATE as "MAP_START_DATE"
         FROM TREXONE_DW_DATA.CENTRAL_FILL_PRODUCT_MAP proddmap
       LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT maptoprod
            ON proddmap.MAP_TO_PRD_PRODUCT_KEY = maptoprod.PRD_PRODUCT_KEY
       LEFT OUTER JOIN TREXONE_DW_DATA.DRUG maptodrug
            ON maptoprod.PRD_DRUG_NUM = maptodrug.DRUG_NUM
       WHERE proddmap.FD_FACILITY_KEY = 573
        AND trunc(proddmap.EFF_END_DATE) = TO_DATE('12/31/2999','MM/DD/YYYY')) prodmapping
  ON product.PRD_PRODUCT_KEY = prodmapping.MAP_PRODUCT_KEY
--END-- Links in Product Mapping information

--Start -- Links in Preferred Product info  Message, from Audit tables
LEFT OUTER JOIN
      (SELECT prodp.PRODUCT_NUM as "PRODUCT_NUM",
              prodp.is_preferred_product as "PREFERRED",
              prodmsg.MESSAGE_TEXT as "PRODUCT_MSG",
              prodmsg.effective_date as "MSG_EFF_DATE",
              prodmsg.expiration_date as "MSG_EXP_DATE"
              
FROM TREXONE_AUD_DATA.PRODUCT prodp

    LEFT OUTER JOIN TREXONE_AUD_DATA.PRODUCT_MESSAGE prodmsg
          ON prodp.PRODUCT_NUM = prodmsg.PRODUCT_NUM
          AND prodmsg.EFF_END_DATE = to_date('12/31/2999','MM/DD/YYYY')
          AND prodmsg.SYS_USER <> 0
          AND prodmsg.priority = 1
          
WHERE trunc(prodp.EFF_END_DATE) = to_date('12/31/2999','MM/DD/YYYY')) prod_p_msg
    ON product.PRD_PRODUCT_NUM = prod_p_msg.PRODUCT_NUM
--End   -- Links in Preferred Product info  Message, from Audit tables

-- Links CF blocked list
LEFT OUTER JOIN
      (SELECT cfblock.PRD_PRODUCT_KEY as "BLOCK_PRODUCT_KEY",
        --drug.DRUG_NDC,
        --drug.DRUG_LABEL_NAME,
        count(unique cfblock.LOCAL_FD_FACILITY_KEY) as "BLOCK_STORE_COUNT",
        MAX(cfblock.EFF_START_DATE) as "BLOCK_DATE"
        
FROM TREXONE_DW_DATA.CENTRAL_FILL_PRODUCT_EXCLUSION cfblock
    
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT prod
   ON cfblock.PRD_PRODUCT_KEY = prod.PRD_PRODUCT_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
    ON drug.DRUG_NUM = prod.PRD_DRUG_NUM

LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facility
    ON cfblock.LOCAL_FD_FACILITY_KEY = facility.FD_FACILITY_KEY

WHERE cfblock.FD_FACILITY_KEY = 573
AND cfblock.PRD_PRODUCT_KEY <> 0
AND trunc(cfblock.EFF_END_DATE) = to_date('12/31/9999','MM/DD/YYYY')
AND cfblock.FD_FACILITY_KEY not in (574,634898)

GROUP BY cfblock.PRD_PRODUCT_KEY)cfblocked
ON cfblocked.BLOCK_PRODUCT_KEY = product.PRD_PRODUCT_KEY
-- Ends Link CF Blocked list

--Start Query for Vendor Item Number
LEFT OUTER JOIN
(SELECT pricecat.DRUG_KEY as "DRUG_KEY",
      pricecat.VENDOR_PRODUCT_ID as "ITEM_NUM",
      vendor.NAME as "VENDOR"

FROM TREXONE_DW_DATA.VENDOR_PRICE_CATALOG pricecat

LEFT OUTER JOIN TREXONE_DW_DATA.VENDOR vendor
ON pricecat.VENDOR_KEY = vendor.VENDOR_KEY

WHERE pricecat.VENDOR_KEY <> 0
AND pricecat.EFFECTIVE_END_DATE = TO_DATE('2999-12-31','YYYY-MM-DD'))vendor_item
ON drug.DRUG_KEY = vendor_item.DRUG_KEY
AND costbasissource.PROVIDER = vendor_item.VENDOR
--ENDs Query for Vendor Item Number

WHERE product.PRD_DEACTIVATE_DATE is null
AND NOT(product.PRD_DEACTIVATE_DATE IS NOT NULL AND product.PRD_SOURCE_INDICATOR = 'Corporate')
AND drug.DRUG_NDC <> '99999999999'
AND product.PRD_DEACTIVATE_DATE is null
AND NVL(drug.DRUG_OBSOLETE_DATE, '31-DEC-2999') > (SYSDATE - 730)
----AND drug.DRUG_GNN is not null --removed this line on 5/9/2018
--shows only active drugs--
AND drug.DRUG_NDC_MFG not in (00247,00363,00440,00490,00615,11917,12280,16590,
16881,19458,21140,21695,23490,24385,33358,34575,35356,37205,40986,49002,49614,
49999,50428,51655,52959,53265,54569,54868,55045,55289,55887,57866,58016,58864,
63874,65243,66116,66267,66336,67544,68016,68030,68071,68094,68115,68258,68387,91899,00000)
--Excluding Garbage Manufacturers

ORDER BY drug.DRUG_LABEL_NAME WITH READ ONLY;
