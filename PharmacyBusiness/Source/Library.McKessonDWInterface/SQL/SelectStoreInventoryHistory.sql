SELECT /* index (inv IB_INVENTORY_ITEM_FACT_07) */
    last_updated.CAL_DATE "date_of_service",
    facility.FD_FACILITY_ID "store_num",
    to_char(drug.DRUG_NDC) "ndc_wo",
    drug.DRUG_LABEL_NAME "drug_name",
    product.PRD_IS_GENERIC "sdgi",
    drug.DRUG_GCN "gcn",
    drug.DRUG_GCN_SEQ "gcn_seq_num",
    drug.DRUG_ORANGE_BOOK_CODE "orange_book_code",
    drug.DRUG_DRUG_FORM_CODE "form_code",
    drug.DRUG_PACKAGE_SIZE "pack_size",
    NVL(drug.DRUG_CASE_SIZE,1)*drug.DRUG_PACKAGE_SIZE "true_pack",
    product.PRD_IS_PRICE_MAINTAINED "pm",
    product.IS_PREFERRED_PRODUCT "is_preferred",
    inv.LAST_ACQ_COST "lastacqcost_pack",
    inv.LAST_ACQ_UNIT_COST "lastacqcost_unit",
    inv.LAST_ACQ_COST_DATE "lastacqcost_date",
    inv.CURRENT_QTY "onhand_qty",
    (inv.CURRENT_QTY * NVL(inv.EXPECTED_UNIT_COST,inv.LAST_ACQ_UNIT_COST)) "onhand_value",
    INV.COMMITTED_QTY "commited_qty",
    (INV.COMMITTED_QTY * NVL(INV.EXPECTED_UNIT_COST,INV.LAST_ACQ_UNIT_COST)) "commited_value",
    (INV.CURRENT_QTY + INV.COMMITTED_QTY) "total_qty",
    ((inv.COMMITTED_QTY * NVL(inv.EXPECTED_UNIT_COST,inv.LAST_ACQ_UNIT_COST)) + (inv.CURRENT_QTY * NVL(inv.EXPECTED_UNIT_COST,inv.LAST_ACQ_UNIT_COST))) "total_value",
    NVL(inv.EXPECTED_COST,inv.LAST_ACQ_COST) "acq_cost_pack",
    NVL(inv.EXPECTED_UNIT_COST,inv.LAST_ACQ_UNIT_COST) "acq_cost_unit",
    vend.NAME "prim_supplier",
    last_updated.CAL_DATE "last_chg_date"
FROM TREXONE_DW_DATA.INVENTORY_ITEM_FACT inv
     LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM last_updated
       ON inv.DATE_KEY = last_updated.DATE_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
       ON inv.DRUG_KEY = drug.DRUG_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facility
       ON inv.FD_FACILITY_KEY = facility.FD_FACILITY_KEY
      AND facility.FD_IS_PPI_ENABLED = 'Y'
     LEFT OUTER JOIN TREXONE_DW_DATA.VENDOR vend
       ON inv.PRIMARY_VENDOR_NUM = vend.VENDOR_NUM
     LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT product
       ON inv.PRD_PRODUCT_KEY = product.PRD_PRODUCT_KEY
WHERE last_updated.CAL_DATE = (:RunDate - 1)
  AND (inv.CURRENT_QTY + inv.COMMITTED_QTY) <> 0
ORDER BY facility.FD_FACILITY_ID
