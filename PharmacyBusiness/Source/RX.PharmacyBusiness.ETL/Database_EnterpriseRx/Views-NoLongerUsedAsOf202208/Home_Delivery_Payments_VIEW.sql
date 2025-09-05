
CREATE OR REPLACE FORCE EDITIONABLE VIEW WEGMANS.HOME_DELIVERY_PAYMENTS_VIEW (
	 STORE_NUM
	,SOLD_DATE
	,SOLD_DATETIME
	,ORDER_NUM
	,RX_NUM
	,REFILL_NUM
	,PART_SEQ_NUM
	,PAYMENT_TYPE_NAME
	,TOTAL_PRICE_PAID
	,PATIENT_PRICE_PAID
	,TOTAL_UNC
	,PAYMENT_DATE
	,TRACKING_NUMBER
	,SHIP_HANDLE_FEE
	,COURIER_SHIP_CHARGE
	,DATE_OF_SERVICE
	,DRUG_NDC
	,TPLD_PLAN_CODE
	,PRODUCT_NUM
	,PRIMARY_TPLD_PLAN_KEY
	,PD_PATIENT_NUM
	,FILL_FACT_KEY
	,RX_FILL_SEQ
	,RX_RECORD_NUM
	,IS_BAR
	,PARTITION_DATE
	,SOLD_DATE_KEY
) AS 
SELECT
     store_num
    ,sold_date
    ,sold_datetime
    ,order_num
    ,rx_num
    ,refill_num
    ,part_seq_num
    ,payment_type_name
    ,total_price_paid
    ,patient_price_paid
    ,total_unc        
    ,payment_date
    --,payment_num        
    ,tracking_number
    ,ship_handle_fee
    ,courier_ship_charge   
    ,date_of_service
    ,drug_ndc
    ,tpld_plan_code
    --,order_num 
    --,payment_group_num
    ,product_num
    --,tpd_key
    ,primary_tpld_plan_key    
    ,pd_patient_num
    ,fill_fact_key
    ,rx_fill_seq
    ,rx_record_num
    ,is_bar
    ,PARTITION_DATE
    ,sold_date_key
FROM (
    SELECT /*+ cardinality(ff,1) leading (ff) index (ff IB_FILL_FACT_29) */
           --IB_FILL_FACT_29 is an index on sold_date_key.    
         To_Number(f.fd_facility_id) AS store_num
        ,ff.sold_date_key AS sold_date
        ,Round(((To_date(To_Char(ff.sold_date_key),'YYYYMMDD') + ((ff.sold_time_key - 2)/86400)) - To_Date('20350101','YYYYMMDD')),6) AS sold_datetime
        ,ff.facility_order_number AS order_num
        ,To_Number(ff.rx_number) AS rx_num
        ,ff.refill_num
        ,ff.partial_fill_seq AS part_seq_num
        ,pt.payment_type_name
        ,ff.total_price_paid
        ,ff.patient_price_paid
        ,ff.total_unc        
        ,To_Number(To_Char(pay.payment_date,'YYYYMMDD')) AS payment_date
        --,pay.payment_num        
        ,sf.tracking_number
        ,sf.ship_handle_fee
        ,sf.courier_ship_charge        
     --   ,pa.pad_city
     --   ,pa.pad_state
     --   ,pa.pad_zip
        ,(CASE WHEN ff.adj_date_of_service_key != 0 THEN ff.adj_date_of_service_key
              WHEN ff.order_date_key != 0 THEN ff.order_date_key
              ELSE 19000101
         END) AS date_of_service
        ,d.drug_ndc
        ,tpplan.tpld_plan_code	--SUBSTR(tpplan.tpld_plan_code, 1, 3) planCarrier, SUBSTR(tpplan.tpld_plan_code, 4, 10) tpPlan
        --,i.order_num 
        --,i.payment_group_num
        ,i.product_num
        --,ff.tpd_key
        ,ff.primary_tpld_plan_key    
        ,pat.pd_patient_num
        ,ff.fill_fact_key
        ,ff.rx_fill_seq
        ,ff.rx_record_num
        ,ff.is_bar
        ,ff.PARTITION_DATE
        ,ff.sold_date_key

        ,Rank() OVER (PARTITION BY
             f.fd_facility_id
            ,ff.sold_date_key
            ,(ff.sold_time_key - 2)
            ,ff.facility_order_number
            ,ff.rx_number
            ,ff.refill_num
            ,ff.partial_fill_seq        
            ,pt.payment_type_name
            ,ff.total_price_paid
            ,ff.patient_price_paid
            ,ff.total_unc        
            ,pay.payment_date
            --,pay.payment_num        
            ,sf.tracking_number
            ,sf.ship_handle_fee
            ,sf.courier_ship_charge        
     --       ,pa.pad_city
     --       ,pa.pad_state
     --       ,pa.pad_zip
            ,ff.adj_date_of_service_key
            ,ff.order_date_key
            ,d.drug_ndc
            ,tpplan.tpld_plan_code
            --,i.order_num
            --,i.payment_group_num
            ,i.product_num
            --,ff.tpd_key
            ,ff.primary_tpld_plan_key    
            ,pat.pd_patient_num
            ,ff.fill_fact_key
            ,ff.rx_fill_seq
            ,ff.rx_record_num
            ,ff.is_bar
         ORDER BY ff.fill_fact_key Desc) AS Fill_Fact_Rank

    FROM TREXONE_DW_DATA.Fill_Fact ff
         INNER JOIN TREXONE_DW_DATA.Facility f
            ON f.fd_facility_key = ff.fd_facility_key
         INNER JOIN TREXONE_AUD_DATA.Item i
            ON i.rx_record_num = ff.rx_record_num
           AND i.rx_fill_seq = ff.rx_fill_seq
           AND i.order_num = ff.order_num
           AND i.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
         INNER JOIN TREXONE_DW_DATA.Patient pat
            ON pat.pd_patient_key = ff.pd_patient_key
         INNER JOIN TREXONE_DW_DATA.Product prd
            ON prd.prd_product_num = i.product_num
         INNER JOIN TREXONE_DW_DATA.Drug d
            ON d.drug_num = prd.prd_drug_num
         INNER JOIN TREXONE_DW_DATA.Shipment_Fact sf
            ON sf.order_num = i.order_num
         INNER JOIN TREXONE_DW_DATA.Patient_Address pa
            ON pa.pad_key = sf.pad_key     
         INNER JOIN TREXONE_AUD_DATA.Payment_Group_List pgl
            ON pgl.payment_group_num = i.payment_group_num
           AND pgl.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')  
         INNER JOIN TREXONE_AUD_DATA.Payment pay
            on pay.payment_num = pgl.payment_num
           and pay.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
           /* The following is McKessons 6.0 release fix to the Payment table for duplicate "no payment" records (cancelled or zero payment) */
           and pay.cancelled_date is null
           and (   pay.account_member_credit_card_num is null -- a non-cc payment
                or pay.completion_date is not null -- or a cc that has been completed
                or not exists (select 1 -- or a cc/incomplete payment where we do not find
                               from TREXONE_AUD_DATA.Payment_Group_List pgl2
                               inner join TREXONE_AUD_DATA.Payment p2 on (pgl2.payment_num = p2.payment_num )
                               where pgl2.payment_group_num = pgl.payment_group_num -- within the same payment group,
                               and p2.payment_num <> pay.payment_num -- a different cc payment
                               and p2.account_member_credit_card_num is not null
                               and p2.account_member_credit_card_num = pay.account_member_credit_card_num -- on the same card
                               and (p2.completion_date is not null or p2.payment_num < pay.payment_num ) ) -- that has been completed, or has the smallest payment num of these dups    
                )
         LEFT OUTER JOIN TREXONE_DW_DATA.Payment_Type pt
            ON pt.payment_type_num = pay.payment_type_num
         LEFT OUTER JOIN TREXONE_DW_DATA.TP_Plan tpplan
            ON tpplan.tpld_plan_key = ff.primary_tpld_plan_key
    WHERE ff.rx_number NOT LIKE 'RR%'
)
WHERE Fill_Fact_Rank = 1

WITH READ ONLY;
