SELECT /*+ ordered full(ctrs) cardinality( ctrs 1) INDEX_RS_ASC(i) */
     DISTINCT
     To_Number(fac.fd_facility_id) AS store_num
    ,ctr.cc_trans_gateway_client_id AS merchant_id
    ,ctr.cc_trans_command_type
    ,CASE WHEN (   ctrs.cc_trans_processor_trans_date IS NULL
                OR ctrs.cc_trans_processor_trans_time IS NULL)
               THEN Round(((:RunDate - 1) - To_Date('20350101','YYYYMMDD')),6)
          ELSE Round((TO_DATE(ctrs.cc_trans_processor_trans_date || ' ' || ctrs.cc_trans_processor_trans_time,'YYYY.MM.DD HH24:MI:SS') - To_Date('20350101','YYYYMMDD')),6)
     END AS transaction_datetime
    ,CASE WHEN ctrs.cc_trans_processor_trans_date IS NULL
               THEN To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
          ELSE To_Number(REGEXP_REPLACE(ctrs.cc_trans_processor_trans_date,'\.',''))
     END AS transaction_date
    ,ctrs.cc_trans_request_num
    ,ctrs.cc_trans_payment_media
    ,ctrs.cc_trans_payment_type
    ,ctrs.cc_trans_status_msg
    ,ctrs.cc_trans_status_code
    ,ctrs.cc_trans_processor_trans_num
    ,ctrs.cc_trans_reference_num
    ,RPAD(TRIM(ctrs.cc_trans_authorization_code),6) AS cc_trans_authorization_code
    ,ctrs.cc_trans_avs_code
    ,CASE ctr.cc_trans_command_type
          WHEN 'CREDIT' THEN ctrs.cc_trans_cc_amount * -1
          WHEN 'REVERSAL' THEN ctrs.cc_trans_cc_amount * -1
          ELSE ctrs.cc_trans_cc_amount
     END AS authorization_amount
    ,CASE ctr.cc_trans_command_type
          WHEN 'CREDIT' THEN ctr.cc_trans_cc_amount * -1
          WHEN 'REVERSAL' THEN ctr.cc_trans_cc_amount * -1
          ELSE ctr.cc_trans_cc_amount
     END AS transaction_amount
    ,oamcc.last4_pan AS cc_last4
    ,To_Number(To_Char(oamcc.expiration_date,'YYYYMMDD')) AS cc_expire_date     
FROM trexone_aud_data.cc_trans_response ctrs
     LEFT OUTER JOIN trexone_aud_data.cc_trans_request ctr
         ON ctr.cc_trans_request_num = ctrs.cc_trans_request_num 
        AND ctr.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
        AND ctr.CC_TRANS_REQUEST_TYPE != 'IPCRB_ADMIN'
     LEFT JOIN trexone_aud_data.payment_group_list pgl
         ON ctr.cc_trans_erx_payment_num = pgl.payment_num
        AND pgl.eff_end_date = TO_DATE('29991231','YYYYMMDD') 
     INNER JOIN trexone_aud_data.item i
         ON i.payment_group_num = pgl.payment_group_num
        AND i.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     INNER JOIN trexone_aud_data.order_record o
         ON o.order_num = i.order_num
        AND o.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     INNER JOIN trexone_aud_data.rx rx
         ON rx.rx_record_num = i.rx_record_num
        AND rx.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     INNER JOIN trexone_aud_data.rx_fill rf
         ON rf.rx_fill_seq = i.rx_fill_seq
        AND rf.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     INNER JOIN trexone_aud_data.payment pay
         ON pay.payment_num = pgl.payment_num
        AND pay.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     LEFT OUTER JOIN trexone_aud_data.account_member_credit_card oamcc
         ON pay.account_member_credit_card_num = oamcc.account_member_credit_card_num
        AND oamcc.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     LEFT OUTER JOIN trexone_aud_data.credit_card_type cct
         ON cct.credit_card_type_num = oamcc.credit_card_type_num
        AND cct.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     INNER JOIN trexone_dw_data.payment_type pt
         ON pt.payment_type_num = pay.payment_type_num
     INNER JOIN trexone_dw_data.patient pat
         ON pat.pd_patient_num = rx.patient_num
     INNER JOIN trexone_dw_data.product prd
         ON prd.prd_product_num = i.product_num
     INNER JOIN trexone_dw_data.drug drug
         ON drug.drug_num = prd.prd_drug_num
     LEFT JOIN trexone_dw_data.facility fac
         ON fac.fd_facility_num = o.order_owner_facility_num
        AND fac.fd_facility_id != '299'
WHERE ctrs.eff_start_date BETWEEN (:RunDate - 1) AND (:RunDate + 1)
  AND ctrs.eff_end_date = TO_DATE('29991231','YYYYMMDD')
  AND ctr.cc_trans_command_type || '' IS NOT NULL
  AND fac.fd_facility_id || '' IS NOT NULL
  AND (
          (    ctrs.cc_trans_processor_trans_date IS NULL
           AND ctrs.datestamp BETWEEN (:RunDate - 1) AND :RunDate
          )
       OR
          (    ctrs.cc_trans_processor_trans_date IS NOT NULL
           AND cc_trans_processor_trans_time IS NOT NULL
           AND TO_DATE(ctrs.cc_trans_processor_trans_date || ' ' || cc_trans_processor_trans_time,'YYYY.MM.DD HH24:MI:SS')
               BETWEEN (:RunDate - 1) AND :RunDate
          )
      )