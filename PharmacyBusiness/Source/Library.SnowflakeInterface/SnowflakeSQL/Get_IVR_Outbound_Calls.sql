SELECT
     FINALPHONENUM AS PHONENBR
	,JOB_CODE AS JOBCODE
	,STORE AS STORENBR
	,RX AS RXNBR
	,REFILL_NUM AS REFILLNUM
	,NVL(PARTIAL_FILL_SEQ,0) AS PARTIALFILLSEQ
	,TO_CHAR(PATIENT_ID) AS PATIENTNUM
	,NULL AS POSSOLDDATE
	,IS_FLU_SHOT AS ISFLUSHOT
	,READY_DT
FROM (
  SELECT DECODE(0,NVL(PREFERREDPHONENUM,0),PHONENUM, PREFERREDPHONENUM) FINALPHONENUM, STORE, RX, REFILL_NUM, PARTIAL_FILL_SEQ, PATIENT_ID, '4' AS JOB_CODE, IS_FLU_SHOT, READY_DT
        FROM
            (SELECT STEP2.*--, POS.SOLD_DATE
             FROM
               (SELECT STEP1.*, PREFERRED.PATP_AREA_CODE || PREFERRED.PATP_PHONE_NUMBER PREFERREDPHONENUM FROM
                 (SELECT PA.PHONE_NUM PHONENUM, F.FD_FACILITY_ID STORE, FF.RX_NUMBER RX, FF.REFILL_NUM, FF.PARTIAL_FILL_SEQ, P.PD_PATIENT_KEY PATIENTKEY, P.PD_CONTACT_BY_CODE CONTACTBYCODE, P.PD_PATIENT_NUM PATIENT_ID, FF.READY_DATE_KEY READY_DT
					   ,(CASE WHEN ( (DISPDRUG.DRUG_GC3 = 'INFLUENZA VIRUS VACCINES' AND DISPDRUG.DRUG_AHFS_DESCRIPTION = 'VACCINES') OR
									 (DISPDRUG.DRUG_GC3 = 'W7C INFLUENZA VIRUS VACCINES' AND DISPDRUG.DRUG_AHFS_DESCRIPTION IS NULL)
								   ) THEN 'Y'
							  ELSE 'N'
						 END) AS IS_FLU_SHOT
                  FROM {DW}.ERXDW_PLS_ARCHIVE_VIEW.FILL_FACT FF, {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT P, {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY F,
                       {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_OPTION PO, {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PA, {DW}.ERXDW_PLS_ARCHIVE_VIEW.DRUG DISPDRUG
                   WHERE FF.ERP_ENROLLMENT_CODE_NUM=3 AND --AUTO SCRIPTS ONLY
                         FF.READY_DATE_KEY = TO_NUMBER(TO_CHAR((:RUNDATE - 1)::DATE,'YYYYMMDD')) AND
                     FF.PD_PATIENT_KEY = P.PD_PATIENT_KEY AND
                     P.PD_PATIENT_NUM = PA.PATIENT_NUM AND
                     FF.CANCEL_DATE_KEY = 0 AND
                     FF.SOLD_DATE_KEY = 0 AND
                     FF.FD_FACILITY_KEY = F.FD_FACILITY_KEY AND
                     P.PATIENT_OPTION_KEY = PO.PATIENT_OPTION_KEY AND
                     PO.AUTO_NOTIFY_OPTION='Y' AND
                     F.FD_IS_ACTIVE = 'Y' AND
                     F.FD_FACILITY_ID < '199' AND
					 FF.DISPENSED_DRUG_KEY = DISPDRUG.DRUG_KEY
                  ORDER BY F.FD_FACILITY_ID, FF.RX_NUMBER, FF.REFILL_NUM) STEP1 --GET ALL VALID RECS
        LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_PHONE PREFERRED ON 
            (STEP1.PATIENTKEY = PREFERRED.PD_PATIENT_KEY 
            AND PREFERRED.EFF_END_DATE = TO_DATE('2999-12-31', 'YYYY-MM-DD')
            AND PREFERRED.PATP_PHONE_USAGE =
                (CASE STEP1.CONTACTBYCODE
                    WHEN '1' THEN 1    -- HOME 1
                    WHEN '3' THEN 1    -- HOME 2
                    WHEN '5' THEN 1    -- WORK
                    WHEN '7' THEN 1    -- OTHER
                    WHEN '9' THEN 2    -- FAX (HOME 1)
                    WHEN '8' THEN 16   -- CELL
                    WHEN '10' THEN 32  -- ALT 1
                    WHEN '11' THEN 8   -- ALT 2
                    WHEN '18' THEN 64  -- ALT 3
                END) 
            AND PREFERRED.PATP_USAGE =
                (CASE STEP1.CONTACTBYCODE
                    WHEN '1' THEN 1    -- HOME 1
                    WHEN '3' THEN 2    -- HOME 2
                    WHEN '5' THEN 4    -- WORK
                    WHEN '7' THEN 8    -- OTHER
                    WHEN '9' THEN 1    -- FAX (HOME 1)
                    WHEN '8' THEN 0    -- CELL
                    WHEN '10' THEN 0   -- ALT 1
                    WHEN '11' THEN 0   -- ALT 2
                    WHEN '18' THEN 0   -- ALT 3
                END))) STEP2
             --LEFT OUTER JOIN WEGMANS.POSTRANSACTION POS ON
             -- (POS.FACILITY_ID = STEP2.STORE AND
             --  POS.RX_NUMBER = STEP2.RX AND
             --  POS.REFILL_NUM = STEP2.REFILL_NUM)
			   )  --GET POS SOLD DATE INFO
        --WHERE SOLD_DATE IS NULL --FILTER OUR RECS THAT HAVE A POS SOLD DATE
		--WHERE IS_FLU_SHOT = 'N'

        UNION ALL

        SELECT DECODE(0,NVL(PREFERREDPHONENUM,0),PHONENUM, PREFERREDPHONENUM) FINALPHONENUM, STORE, RX, REFILL_NUM, PARTIAL_FILL_SEQ, PATIENT_ID, '3' AS JOB_CODE, IS_FLU_SHOT, READY_DT
        FROM                     
            (SELECT STEP2.*--, POS.SOLD_DATE
            FROM
                (SELECT STEP1.*, PREFERRED.PATP_AREA_CODE || PREFERRED.PATP_PHONE_NUMBER PREFERREDPHONENUM FROM 
                                (SELECT  PA.PHONE_NUM PHONENUM, F.FD_FACILITY_ID STORE, FF.RX_NUMBER RX, FF.REFILL_NUM, FF.PARTIAL_FILL_SEQ, P.PD_PATIENT_KEY PATIENTKEY, P.PD_CONTACT_BY_CODE CONTACTBYCODE, P.PD_PATIENT_NUM PATIENT_ID, FF.READY_DATE_KEY READY_DT
									   ,(CASE WHEN ( (DISPDRUG.DRUG_GC3 = 'INFLUENZA VIRUS VACCINES' AND DISPDRUG.DRUG_AHFS_DESCRIPTION = 'VACCINES') OR
													 (DISPDRUG.DRUG_GC3 = 'W7C INFLUENZA VIRUS VACCINES' AND DISPDRUG.DRUG_AHFS_DESCRIPTION IS NULL)
												   ) THEN 'Y'
											  ELSE 'N'
										 END) AS IS_FLU_SHOT
                                  FROM {DW}.ERXDW_PLS_ARCHIVE_VIEW.FILL_FACT FF, {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT P, {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY F,
                                       {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_OPTION PO, {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PA, {DW}.ERXDW_PLS_ARCHIVE_VIEW.DRUG DISPDRUG
                                  WHERE FF.READY_DATE_KEY = TO_NUMBER(TO_CHAR((:RUNDATE - 6)::DATE,'YYYYMMDD')) AND
                                    FF.PD_PATIENT_KEY = P.PD_PATIENT_KEY AND
                                    P.PD_PATIENT_NUM = PA.PATIENT_NUM AND
                                    P.PD_SPECIES_NAME IS NULL AND
                                    FF.CANCEL_DATE_KEY = 0 AND
                                    FF.SOLD_DATE_KEY = 0 AND
                                    FF.FD_FACILITY_KEY = F.FD_FACILITY_KEY AND
                                    P.PATIENT_OPTION_KEY = PO.PATIENT_OPTION_KEY AND
                                    PO.AUTO_NOTIFY_OPTION='Y' AND
                                    F.FD_IS_ACTIVE = 'Y' AND
                                    F.FD_FACILITY_ID < '199' AND
					                FF.DISPENSED_DRUG_KEY = DISPDRUG.DRUG_KEY
                                    ORDER BY FF.FD_FACILITY_KEY, FF.RX_NUMBER, FF.REFILL_NUM) STEP1
                LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_PHONE PREFERRED ON 
                    (STEP1.PATIENTKEY = PREFERRED.PD_PATIENT_KEY 
                    AND PREFERRED.EFF_END_DATE = TO_DATE('2999-12-31', 'YYYY-MM-DD')
                    AND PREFERRED.PATP_PHONE_USAGE =
                        (CASE STEP1.CONTACTBYCODE
                            WHEN '1' THEN 1    -- HOME 1
                            WHEN '3' THEN 1    -- HOME 2
                            WHEN '5' THEN 1    -- WORK
                            WHEN '7' THEN 1    -- OTHER
                            WHEN '9' THEN 2    -- FAX (HOME 1)
                            WHEN '8' THEN 16   -- CELL
                            WHEN '10' THEN 32  -- ALT 1
                            WHEN '11' THEN 8   -- ALT 2
                            WHEN '18' THEN 64  -- ALT 3
                        END) 
                    AND PREFERRED.PATP_USAGE =
                        (CASE STEP1.CONTACTBYCODE
                            WHEN '1' THEN 1    -- HOME 1
                            WHEN '3' THEN 2    -- HOME 2
                            WHEN '5' THEN 4    -- WORK
                            WHEN '7' THEN 8    -- OTHER
                            WHEN '9' THEN 1    -- FAX (HOME 1)
                            WHEN '8' THEN 0    -- CELL
                            WHEN '10' THEN 0   -- ALT 1
                            WHEN '11' THEN 0   -- ALT 2
                            WHEN '18' THEN 0   -- ALT 3
                        END))) STEP2
               -- LEFT OUTER JOIN WEGMANS.POSTRANSACTION POS ON
               -- (POS.FACILITY_ID = STEP2.STORE AND
               -- POS.RX_NUMBER = STEP2.RX AND
               -- POS.REFILL_NUM = STEP2.REFILL_NUM)
				) --GET POS SOLD DATE INFO
        --WHERE SOLD_DATE IS NULL --FILTER OUR RECS THAT HAVE A POS SOLD DATE
		--WHERE IS_FLU_SHOT = 'N'
)
ORDER BY STORE, RX