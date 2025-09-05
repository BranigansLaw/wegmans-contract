SELECT unique (:RunDate - 1) "date_of_service",
        vendor.NAME "supplier_name",
        price_cat.VENDOR_PRODUCT_ID "vendor_item_number",
        to_char(drug.DRUG_NDC,'00000g0000g00','nls_numeric_characters=.-') "ndc",
        to_char(drug.DRUG_NDC) "ndc_wo",
        drug.DRUG_LABEL_NAME "drug_name",
        drug.DRUG_DOSAGE_FORM "drug_form",
        drug.DRUG_DEA_CLASS "dea_class",
        drug.DRUG_STRENGTH "strength",
        drug.DRUG_STRENGTH_UNITS "strength_unit",
        drug.DRUG_GNN "generic_name",
        drug.DRUG_PACKAGE_SIZE "pack_size",
        drug.DRUG_IS_MAINT_DRUG "is_maint_drug",
        product.PRD_IS_GENERIC "sdgi",
        drug.DRUG_GCN "gcn",
        drug.DRUG_GCN_SEQ "gcn_seq_num",
        drug.DRUG_LABELER "drug_manufacturer",
        drug.DRUG_ORANGE_BOOK_CODE "orange_book_code",
        price_cat.UNIT_PRICE "pack_price",
        round(price_cat.UNIT_PRICE/drug.DRUG_PACKAGE_SIZE,3) "price_per_unit",
        price_cat.UNIT_PRICE_DATE "unit_price_date",
        price_cat.PURCHASE_INCREMENT "purch_incr",
        price_cat.PKG_SIZE "pkg_size_incr",
        price_cat.STATUS "status",
        price_cat.EFFECTIVE_START_DATE "eff_start_date"

FROM TREXONE_DW_DATA.VENDOR_PRICE_CATALOG price_cat

LEFT OUTER JOIN TREXONE_DW_DATA.VENDOR vendor
ON vendor.VENDOR_KEY = price_cat.VENDOR_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
ON drug.DRUG_KEY = price_cat.DRUG_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT product
ON product.PRD_PRODUCT_KEY = price_cat.PRD_PRODUCT_KEY

WHERE price_cat.VENDOR_KEY in (19617, 19616,29018,29019,2,1,29518,30019)
AND price_cat.EFFECTIVE_END_DATE = TO_DATE('2999-12-31','YYYY-MM-DD')

Order by vendor.NAME,price_cat.STATUS,drug.DRUG_LABEL_NAME