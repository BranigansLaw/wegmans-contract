/* NOTES on Turnaround Time:

    We can get the earliest Date In and latest Date Out values to calculate the overall turnaround time of an Order.
    While the overall turn around time of an Order includes all items within an Order, the results are filtered to 
    only show items within the Order that identified by TAT_Target (i.e., 'SPECIALTY').
    Other attributes of the overall Order can be calculated, like total time the Order was in "intervention" or in "off hours".
*/
WITH query_variables AS (
    SELECT
         :Start_Date AS v_Start_Date --Typically is previous Sunday
        ,:End_Date AS v_End_Date --Typically is previous Saturday
        ,:TAT_Target AS v_TAT_Target --One of these: 'SPECIALTY', 'EXCELLUS', 'IHA'
        ,'SPECIALTY' AS TAT_Target_SPECIALTY
        ,'EXCELLUS' AS TAT_Target_EXCELLUS
        ,'IHA' AS TAT_Target_IHA
    FROM dual
), specialty_gcn_list AS (
    SELECT '30788' AS specialty_gcn FROM dual UNION ALL
    SELECT '39483' AS specialty_gcn FROM dual UNION ALL
    SELECT '40869' AS specialty_gcn FROM dual UNION ALL
    SELECT '45877' AS specialty_gcn FROM dual UNION ALL
    SELECT '45878' AS specialty_gcn FROM dual UNION ALL
    SELECT '48664' AS specialty_gcn FROM dual UNION ALL
    SELECT '48814' AS specialty_gcn FROM dual UNION ALL
    SELECT '48815' AS specialty_gcn FROM dual UNION ALL
    SELECT '48899' AS specialty_gcn FROM dual UNION ALL
    SELECT '49812' AS specialty_gcn FROM dual UNION ALL
    SELECT '50035' AS specialty_gcn FROM dual UNION ALL
    SELECT '50039' AS specialty_gcn FROM dual UNION ALL
    SELECT '50210' AS specialty_gcn FROM dual UNION ALL
    SELECT '50736' AS specialty_gcn FROM dual UNION ALL
    SELECT '51599' AS specialty_gcn FROM dual UNION ALL
    SELECT '51637' AS specialty_gcn FROM dual UNION ALL
    SELECT '52882' AS specialty_gcn FROM dual UNION ALL
    SELECT '53430' AS specialty_gcn FROM dual UNION ALL
    SELECT '58214' AS specialty_gcn FROM dual UNION ALL
    SELECT '58776' AS specialty_gcn FROM dual UNION ALL
    SELECT '58877' AS specialty_gcn FROM dual UNION ALL
    SELECT '58878' AS specialty_gcn FROM dual UNION ALL
    SELECT '58879' AS specialty_gcn FROM dual UNION ALL
    SELECT '58880' AS specialty_gcn FROM dual UNION ALL
    SELECT '60102' AS specialty_gcn FROM dual UNION ALL
    SELECT '60107' AS specialty_gcn FROM dual UNION ALL
    SELECT '60108' AS specialty_gcn FROM dual UNION ALL
    SELECT '61205' AS specialty_gcn FROM dual UNION ALL
    SELECT '61938' AS specialty_gcn FROM dual UNION ALL
    SELECT '62602' AS specialty_gcn FROM dual UNION ALL
    SELECT '62624' AS specialty_gcn FROM dual UNION ALL
    SELECT '63724' AS specialty_gcn FROM dual UNION ALL
    SELECT '63903' AS specialty_gcn FROM dual UNION ALL
    SELECT '64481' AS specialty_gcn FROM dual UNION ALL
    SELECT '64967' AS specialty_gcn FROM dual UNION ALL
    SELECT '65113' AS specialty_gcn FROM dual UNION ALL
    SELECT '65114' AS specialty_gcn FROM dual UNION ALL
    SELECT '65189' AS specialty_gcn FROM dual UNION ALL
    SELECT '65409' AS specialty_gcn FROM dual UNION ALL
    SELECT '65410' AS specialty_gcn FROM dual UNION ALL
    SELECT '65411' AS specialty_gcn FROM dual UNION ALL
    SELECT '65993' AS specialty_gcn FROM dual UNION ALL
    SELECT '65994' AS specialty_gcn FROM dual UNION ALL
    SELECT '66396' AS specialty_gcn FROM dual UNION ALL
    SELECT '66709' AS specialty_gcn FROM dual UNION ALL
    SELECT '67394' AS specialty_gcn FROM dual UNION ALL
    SELECT '67628' AS specialty_gcn FROM dual UNION ALL
    SELECT '67681' AS specialty_gcn FROM dual UNION ALL
    SELECT '68047' AS specialty_gcn FROM dual UNION ALL
    SELECT '68048' AS specialty_gcn FROM dual UNION ALL
    SELECT '69046' AS specialty_gcn FROM dual UNION ALL
    SELECT '69151' AS specialty_gcn FROM dual UNION ALL
    SELECT '70233' AS specialty_gcn FROM dual UNION ALL
    SELECT '70586' AS specialty_gcn FROM dual UNION ALL
    SELECT '70587' AS specialty_gcn FROM dual UNION ALL
    SELECT '70588' AS specialty_gcn FROM dual UNION ALL
    SELECT '71017' AS specialty_gcn FROM dual UNION ALL
    SELECT '71250' AS specialty_gcn FROM dual UNION ALL
    SELECT '71262' AS specialty_gcn FROM dual UNION ALL
    SELECT '71590' AS specialty_gcn FROM dual UNION ALL
    SELECT '71708' AS specialty_gcn FROM dual UNION ALL
    SELECT '71748' AS specialty_gcn FROM dual UNION ALL
    SELECT '71942' AS specialty_gcn FROM dual UNION ALL
    SELECT '72075' AS specialty_gcn FROM dual UNION ALL
    SELECT '72785' AS specialty_gcn FROM dual UNION ALL
    SELECT '72786' AS specialty_gcn FROM dual UNION ALL
    SELECT '72926' AS specialty_gcn FROM dual UNION ALL
    SELECT '73237' AS specialty_gcn FROM dual UNION ALL
    SELECT '73370' AS specialty_gcn FROM dual UNION ALL
    SELECT '73394' AS specialty_gcn FROM dual UNION ALL
    SELECT '73395' AS specialty_gcn FROM dual UNION ALL
    SELECT '73444' AS specialty_gcn FROM dual UNION ALL
    SELECT '74510' AS specialty_gcn FROM dual UNION ALL
    SELECT '74511' AS specialty_gcn FROM dual UNION ALL
    SELECT '74512' AS specialty_gcn FROM dual UNION ALL
    SELECT '74513' AS specialty_gcn FROM dual UNION ALL
    SELECT '74564' AS specialty_gcn FROM dual UNION ALL
    SELECT '74663' AS specialty_gcn FROM dual UNION ALL
    SELECT '75514' AS specialty_gcn FROM dual UNION ALL
    SELECT '75641' AS specialty_gcn FROM dual UNION ALL
    SELECT '75731' AS specialty_gcn FROM dual UNION ALL
    SELECT '75732' AS specialty_gcn FROM dual UNION ALL
    SELECT '76265' AS specialty_gcn FROM dual UNION ALL
    SELECT '76305' AS specialty_gcn FROM dual UNION ALL
    SELECT '76353' AS specialty_gcn FROM dual UNION ALL
    SELECT '77469' AS specialty_gcn FROM dual UNION ALL
    SELECT '77470' AS specialty_gcn FROM dual UNION ALL
    SELECT '77565' AS specialty_gcn FROM dual UNION ALL
    SELECT '77584' AS specialty_gcn FROM dual UNION ALL
    SELECT '77637' AS specialty_gcn FROM dual UNION ALL
    SELECT '77783' AS specialty_gcn FROM dual UNION ALL
    SELECT '77870' AS specialty_gcn FROM dual UNION ALL
    SELECT '78185' AS specialty_gcn FROM dual UNION ALL
    SELECT '78186' AS specialty_gcn FROM dual UNION ALL
    SELECT '78187' AS specialty_gcn FROM dual UNION ALL
    SELECT '78258' AS specialty_gcn FROM dual UNION ALL
    SELECT '78538' AS specialty_gcn FROM dual UNION ALL
    SELECT '78672' AS specialty_gcn FROM dual UNION ALL
    SELECT '78672' AS specialty_gcn FROM dual UNION ALL
    SELECT '78707' AS specialty_gcn FROM dual UNION ALL
    SELECT '79520' AS specialty_gcn FROM dual UNION ALL
    SELECT '79673' AS specialty_gcn FROM dual UNION ALL
    SELECT '79675' AS specialty_gcn FROM dual UNION ALL
    SELECT '80271' AS specialty_gcn FROM dual UNION ALL
    SELECT '82340' AS specialty_gcn FROM dual
), release_to_patient AS (
    /* NOTES on release_to_patient:

       The FILL_WORKFLOW_FACT table holds Date In and Date Out values for each step taken to fulfill an Order.
       Other attributes for each step can be obtained, like if the step was in "intervention" or elapsed time off hours.

       The Fill_Fact table holds Order Numbers (FACILITY_ORDER_NUMBER) that were released to patient (RTP_DATE_KEY) on specific dates.
       We can filter results based on various sets of business rules identified by a Turn Around Time Target variable (TAT_Target).
       For example, when TAT_Target = 'SPECIALTY' we filter on Orders having a specialty item whose GCN is in the WEGMANS.SPECIALTY_GCN table.
       The Order Number values (FACILITY_ORDER_NUMBER) are needed to query the Fill_Workflow_Fact table for turn around times of the Order.
    */
    SELECT /*+ cardinality(ff,1) leading (ff) index (ff IB_FILL_FACT_64)  full(f)  */
         ff.FACILITY_ORDER_NUMBER AS ORDER_NUMBER
        ,f.FD_FACILITY_ID AS FACILITY
        ,ff.PD_PATIENT_KEY AS MCKESSON_PATIENT_KEY
        ,ff.RX_NUMBER AS RX_NBR
        ,ff.REFILL_NUM AS REFILL_NBR
    FROM TREXONE_DW_DATA.FILL_FACT ff
         INNER JOIN TREXONE_DW_DATA.TP_PLAN tpp ON tpp.TPLD_PLAN_KEY = ff.PRIMARY_TPLD_PLAN_KEY
         INNER JOIN TREXONE_DW_DATA.FACILITY f ON f.FD_FACILITY_KEY = ff.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.PRODUCT prd ON ff.DISPENSED_PRD_PRODUCT_KEY = prd.PRD_PRODUCT_KEY
         INNER JOIN TREXONE_DW_DATA.DRUG d ON d.DRUG_NUM = prd.PRD_DRUG_NUM AND prd.PRD_DEACTIVATE_DATE IS NULL
         INNER JOIN specialty_gcn_list sp ON sp.specialty_gcn = d.DRUG_GCN_SEQ
         CROSS JOIN query_variables qv
    WHERE ff.partition_date BETWEEN (qv.v_Start_Date - 10) AND (qv.v_End_Date + 10) --filter on partition_date simply to increase performance.
          --RTP_DATE_KEY index is IB_FILL_FACT_64
      AND ff.RTP_DATE_KEY BETWEEN To_Number(To_Char(qv.v_Start_Date,'YYYYMMDD'))
                              AND To_Number(To_Char(qv.v_End_Date,'YYYYMMDD'))
      AND d.drug_ndc NOT IN (
              '00000999990' --d.drug_label_name='TREMFYA NO DISPENSE'
          )
      AND ff.SOLD_DATE_KEY != 0
      AND ff.CANCEL_DATE_KEY = 0
      AND ff.IS_SAME_DAY_REVERSAL = 'N'
      AND qv.v_TAT_Target = qv.TAT_Target_SPECIALTY
    GROUP BY
         ff.FACILITY_ORDER_NUMBER
        ,ff.PD_PATIENT_KEY
        ,f.FD_FACILITY_ID
        ,ff.RX_NUMBER
        ,ff.REFILL_NUM

    UNION ALL

    SELECT /*+ cardinality(ff,1) leading (ff) index (ff IB_FILL_FACT_64)  full(f)  */
         ff.FACILITY_ORDER_NUMBER AS ORDER_NUMBER
        ,f.FD_FACILITY_ID AS FACILITY
        ,ff.PD_PATIENT_KEY AS MCKESSON_PATIENT_KEY
        ,ff.RX_NUMBER AS RX_NBR
        ,ff.REFILL_NUM AS REFILL_NBR
    FROM TREXONE_DW_DATA.FILL_FACT ff
         INNER JOIN TREXONE_DW_DATA.TP_PLAN tpp ON tpp.TPLD_PLAN_KEY = ff.PRIMARY_TPLD_PLAN_KEY
         INNER JOIN TREXONE_DW_DATA.FACILITY f ON f.FD_FACILITY_KEY = ff.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.PRODUCT prd ON ff.DISPENSED_PRD_PRODUCT_KEY = prd.PRD_PRODUCT_KEY
         INNER JOIN TREXONE_DW_DATA.DRUG d ON d.DRUG_NUM = prd.PRD_DRUG_NUM AND prd.PRD_DEACTIVATE_DATE IS NULL
         CROSS JOIN query_variables qv
    WHERE ff.partition_date BETWEEN (qv.v_Start_Date - 10) AND (qv.v_End_Date + 10) --filter on partition_date simply to increase performance.
          --RTP_DATE_KEY index is IB_FILL_FACT_64
      AND ff.RTP_DATE_KEY BETWEEN To_Number(To_Char(qv.v_Start_Date,'YYYYMMDD'))
                              AND To_Number(To_Char(qv.v_End_Date,'YYYYMMDD'))
      AND f.fd_facility_id = '199'
      AND d.drug_ndc NOT IN (
              '00000999990' --d.drug_label_name='TREMFYA NO DISPENSE'
          )
      AND tpp.BIN IN ('610014','003858') --Excellus
      AND EXISTS (
                  SELECT *
                  FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
                       INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
                          ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
                  WHERE (AUD__Fill_Fact.BENEFIT_GROUP_ID LIKE 'LHPG%' OR AUD__Fill_Fact.BENEFIT_GROUP_ID LIKE 'EXL%')
                    AND AUD__Fill_Fact.FILL_STATE_CHG_TS BETWEEN To_Date(To_Char(ff.RTP_DATE_KEY ),'YYYYMMDD') AND sysdate
                    AND DW__Facility.FD_FACILITY_KEY = ff.FD_FACILITY_KEY
                    AND AUD__Fill_Fact.RX_NUMBER = ff.RX_NUMBER
                    AND AUD__Fill_Fact.REFILL_NUM = ff.REFILL_NUM
          )
      AND ff.SOLD_DATE_KEY != 0
      AND ff.CANCEL_DATE_KEY = 0
      AND ff.IS_SAME_DAY_REVERSAL = 'N'
      AND qv.v_TAT_Target = qv.TAT_Target_EXCELLUS
    GROUP BY
         ff.FACILITY_ORDER_NUMBER
        ,ff.PD_PATIENT_KEY
        ,f.FD_FACILITY_ID
        ,ff.RX_NUMBER
        ,ff.REFILL_NUM

    UNION ALL

    SELECT /*+ cardinality(ff,1) leading (ff) index (ff IB_FILL_FACT_64)  full(f)  */
         ff.FACILITY_ORDER_NUMBER AS ORDER_NUMBER
        ,f.FD_FACILITY_ID AS FACILITY
        ,ff.PD_PATIENT_KEY AS MCKESSON_PATIENT_KEY
        ,ff.RX_NUMBER AS RX_NBR
        ,ff.REFILL_NUM AS REFILL_NBR
    FROM TREXONE_DW_DATA.FILL_FACT ff
         INNER JOIN TREXONE_DW_DATA.TP_PLAN tpp ON tpp.TPLD_PLAN_KEY = ff.PRIMARY_TPLD_PLAN_KEY
         INNER JOIN TREXONE_DW_DATA.FACILITY f ON f.FD_FACILITY_KEY = ff.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.PRODUCT prd ON ff.DISPENSED_PRD_PRODUCT_KEY = prd.PRD_PRODUCT_KEY
         INNER JOIN TREXONE_DW_DATA.DRUG d ON d.DRUG_NUM = prd.PRD_DRUG_NUM AND prd.PRD_DEACTIVATE_DATE IS NULL
         CROSS JOIN query_variables qv
    WHERE ff.partition_date BETWEEN (qv.v_Start_Date - 10) AND (qv.v_End_Date + 10) --filter on partition_date simply to increase performance.
          --RTP_DATE_KEY index is IB_FILL_FACT_64
      AND ff.RTP_DATE_KEY BETWEEN To_Number(To_Char(qv.v_Start_Date,'YYYYMMDD'))
                              AND To_Number(To_Char(qv.v_End_Date,'YYYYMMDD'))
      AND f.fd_facility_id = '199'
      AND d.drug_ndc NOT IN (
              '00000999990' --d.drug_label_name='TREMFYA NO DISPENSE'
          )
      AND tpp.BIN IN ('004626','012635','016557','012486') --IHA
      AND ff.SOLD_DATE_KEY != 0
      AND ff.CANCEL_DATE_KEY = 0
      AND ff.IS_SAME_DAY_REVERSAL = 'N'
      AND qv.v_TAT_Target = qv.TAT_Target_IHA
    GROUP BY
         ff.FACILITY_ORDER_NUMBER
        ,ff.PD_PATIENT_KEY
        ,f.FD_FACILITY_ID
        ,ff.RX_NUMBER
        ,ff.REFILL_NUM
)
/* NOTES on Workflow Steps: 

Order Numbers are shared by different stores at the same time, so an ORDER_NUMBER alone is not enough to uniquely identify an order.
One might think that a combination of ORDER_NUMBER and FACILITY would be unique, but various steps can be performed at different stores,
    and the last step is frequently shared with store 997 or 998 for many orders.
So, for SPECIALTY we use ORDER_NUMBER and MCKESSON_PATIENT_KEY to uniquely identify an order.
For IHA and EXCELLUS we do not need this because they only query on store 199, but is used for consistent data integrity.
*/
SELECT
     fwf.FACILITY_ORDER_NUMBER AS ORDER_NUMBER
    ,fac.FD_FACILITY_ID AS FACILITY
    ,fwf.PD_PATIENT_KEY AS MCKESSON_PATIENT_KEY
    ,fwf.FWFS_RX_NUMBER AS RX_NBR
    ,fwf.FWFS_REFILL_NUM AS REFILL_NBR
    ,fwf.FWFS_DT_IN AS DATE_IN
    ,fwf.FWFS_DT_OUT AS DATE_OUT
    ,ws.WFSD_KEY
    ,ws.WFSD_STEP_DESCRIPTION
FROM TREXONE_DW_DATA.FILL_WORKFLOW_FACT fwf
        LEFT OUTER JOIN TREXONE_DW_DATA.WF_STEP ws 
           ON ws.WFSD_KEY = fwf.WFSD_KEY
        LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY fac 
           ON fac.FD_FACILITY_KEY = fwf.FD_FACILITY_KEY
        CROSS JOIN query_variables qv
WHERE fwf.FWFS_DT_IN BETWEEN (qv.v_Start_Date - 365) AND (qv.v_End_Date + 1) --Order Numbers can be repeated after a few years, but are not reused within a year.
  AND (fwf.FACILITY_ORDER_NUMBER, fwf.PD_PATIENT_KEY) IN (
          SELECT rtp.ORDER_NUMBER, rtp.MCKESSON_PATIENT_KEY
          FROM release_to_patient rtp
      )
ORDER BY
     fwf.FACILITY_ORDER_NUMBER
    ,fwf.FWFS_RX_NUMBER
    ,fwf.FWFS_REFILL_NUM
    ,fwf.FWFS_DT_IN
    ,fwf.FWFS_DT_OUT