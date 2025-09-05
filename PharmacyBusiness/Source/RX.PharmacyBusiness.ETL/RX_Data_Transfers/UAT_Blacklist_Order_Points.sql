SELECT
     (CASE WHEN N_STORENBR = 0 THEN 'Corp'
           ELSE To_Char(N_STORENBR)
      END) AS store
    ,(CASE WHEN C_BRANDGENIND = 'G' THEN 'Generic'
           WHEN C_BRANDGENIND = 'B' THEN 'Brand'
           ELSE NULL
      END) AS brand_generic
    ,Substr(STR_NDC,1,5) || '-' || Substr(STR_NDC,6,4) || '-' || Substr(STR_NDC,10,2) AS ndc
    ,DEC_PACKSIZE AS pack_size
    ,C_DECILE AS decile
    ,STR_GCN AS gcn_seq_num
    ,Round((DTM_BLACKLISTED - To_Date('20350101','YYYYMMDD')),6) AS date_blacklist_start
    ,(CASE WHEN N_REASONCODE = 1 THEN 'Slow Mover'
           WHEN N_REASONCODE = 2 THEN 'Inventory CM'
           WHEN N_REASONCODE = 3 THEN 'Customer Specific NDC'
           WHEN N_REASONCODE = 4 THEN 'OTHER'
           ELSE NULL
      END) AS reason
    ,NULL AS date_blacklist_expire
    ,STR_USERID AS user_added
    ,NULL AS blacklist_id
    ,REGEXP_REPLACE(STR_DRUGNAME,'"','') AS drug_name
FROM RX.SMARTOPBLACKLISTNDCS
WHERE (N_STORENBR = 0)
   OR (N_STORENBR > 0 AND c_brandgenind='B' and c_subpackind='Y')
   OR (N_STORENBR > 0 AND ndcblacklistedind = 'Y' AND ((c_brandgenind='B' and c_subpackind='N') or (c_brandgenind = 'G')))
ORDER BY STR_GCN, N_STORENBR, STR_NDC