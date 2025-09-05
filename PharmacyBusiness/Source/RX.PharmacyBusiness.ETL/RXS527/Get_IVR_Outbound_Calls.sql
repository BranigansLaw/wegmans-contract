SELECT
     finalphonenum AS PhoneNbr
	,job_code AS JobCode
	,store AS StoreNbr
	,rx AS RxNbr
	,refill_num AS RefillNum
	,NVL(PARTIAL_FILL_SEQ,0) AS PartialFillSeq
	,To_Char(patient_id) AS PatientNum
	,NULL AS PosSoldDate
	,Is_Flu_Shot AS IsFluShot
	,ready_dt
FROM (
  SELECT DECODE(0,NVL(preferredphonenum,0),phonenum, preferredphonenum) finalphonenum, store, rx, refill_num, PARTIAL_FILL_SEQ, patient_id, '4' AS job_code, Is_Flu_Shot, ready_dt
        FROM
            (SELECT STEP2.*--, POS.SOLD_DATE
             FROM
               (select step1.*, preferred.patp_area_code || preferred.patp_phone_number preferredphonenum from
                 (SELECT PA.PHONE_NUM phonenum, F.FD_FACILITY_ID store, FF.RX_NUMBER rx, FF.refill_num, FF.PARTIAL_FILL_SEQ, p.pd_patient_key patientkey, p.PD_CONTACT_BY_CODE contactbycode, p.PD_PATIENT_NUM patient_id, FF.READY_DATE_KEY ready_dt
					   ,(CASE WHEN ( (dispdrug.DRUG_GC3 = 'INFLUENZA VIRUS VACCINES' AND dispdrug.DRUG_AHFS_DESCRIPTION = 'VACCINES') OR
									 (dispdrug.DRUG_GC3 = 'W7C INFLUENZA VIRUS VACCINES' AND dispdrug.DRUG_AHFS_DESCRIPTION IS NULL)
								   ) THEN 'Y'
							  ELSE 'N'
						 END) AS Is_Flu_Shot
                  FROM TREXONE_DW_DATA.FILL_FACT FF, TREXONE_DW_DATA.PATIENT P, TREXONE_DW_DATA.FACILITY F,
                       TREXONE_DW_DATA.PATIENT_OPTION PO, TREXONE_AUD_DATA.PATIENT_ADDRESS PA, TREXONE_DW_DATA.DRUG dispdrug
                   WHERE FF.ERP_ENROLLMENT_CODE_NUM=3 AND --Auto Scripts Only
                         FF.READY_DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) AND
                     FF.PD_PATIENT_KEY = P.PD_PATIENT_KEY AND
                     P.PD_PATIENT_NUM = PA.PATIENT_NUM AND
                     FF.CANCEL_DATE_KEY = 0 AND
                     FF.SOLD_DATE_KEY = 0 AND
                     FF.FD_FACILITY_KEY = F.FD_FACILITY_KEY AND
                     P.PATIENT_OPTION_KEY = PO.PATIENT_OPTION_KEY AND
                     PO.AUTO_NOTIFY_OPTION='Y' AND
                     F.FD_IS_ACTIVE = 'Y' AND
                     F.FD_FACILITY_ID < '199' AND
					 FF.DISPENSED_DRUG_KEY = dispdrug.DRUG_KEY
                  ORDER BY F.FD_FACILITY_ID, FF.RX_NUMBER, FF.REFILL_NUM) STEP1 --get all valid recs
        left outer join trexone_dw_data.patient_phone preferred on 
            (step1.patientkey = preferred.pd_patient_key 
            AND preferred.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
            AND preferred.patp_phone_usage =
                (CASE step1.contactbycode
                    WHEN '1' then 1    -- Home 1
                    WHEN '3' then 1    -- Home 2
                    WHEN '5' then 1    -- Work
                    WHEN '7' then 1    -- Other
                    WHEN '9' then 2    -- Fax (Home 1)
                    WHEN '8' then 16   -- Cell
                    WHEN '10' then 32  -- Alt 1
                    WHEN '11' then 8   -- Alt 2
                    WHEN '18' then 64  -- Alt 3
                END) 
            AND preferred.patp_usage =
                (CASE step1.contactbycode
                    WHEN '1' then 1    -- Home 1
                    WHEN '3' then 2    -- Home 2
                    WHEN '5' then 4    -- Work
                    WHEN '7' then 8    -- Other
                    WHEN '9' then 1    -- Fax (Home 1)
                    WHEN '8' then 0    -- Cell
                    WHEN '10' then 0   -- Alt 1
                    WHEN '11' then 0   -- Alt 2
                    WHEN '18' then 0   -- Alt 3
                END))) step2
             --LEFT OUTER JOIN WEGMANS.POSTRANSACTION POS ON
             -- (POS.FACILITY_ID = STEP2.store AND
             --  POS.RX_NUMBER = STEP2.RX AND
             --  POS.REFILL_NUM = STEP2.REFILL_NUM)
			   )  --get pos sold date info
        --WHERE SOLD_DATE IS NULL --filter our recs that have a pos sold date
		--WHERE Is_Flu_Shot = 'N'

        UNION ALL

        SELECT DECODE(0,NVL(preferredphonenum,0),phonenum, preferredphonenum) finalphonenum, store, rx, refill_num, PARTIAL_FILL_SEQ, patient_id, '3' AS job_code, Is_Flu_Shot, ready_dt
        FROM                     
            (SELECT step2.*--, pos.sold_date
            FROM
                (select step1.*, preferred.patp_area_code || preferred.patp_phone_number preferredphonenum from 
                                (SELECT  pa.phone_num phonenum, f.fd_facility_id store, ff.rx_number rx, ff.refill_num, FF.PARTIAL_FILL_SEQ, p.pd_patient_key patientkey, p.PD_CONTACT_BY_CODE contactbycode, p.PD_PATIENT_NUM patient_id, FF.READY_DATE_KEY ready_dt
									   ,(CASE WHEN ( (dispdrug.DRUG_GC3 = 'INFLUENZA VIRUS VACCINES' AND dispdrug.DRUG_AHFS_DESCRIPTION = 'VACCINES') OR
													 (dispdrug.DRUG_GC3 = 'W7C INFLUENZA VIRUS VACCINES' AND dispdrug.DRUG_AHFS_DESCRIPTION IS NULL)
												   ) THEN 'Y'
											  ELSE 'N'
										 END) AS Is_Flu_Shot
                                  FROM trexone_dw_data.fill_fact ff, trexone_dw_data.patient p, trexone_dw_data.facility f,
                                       trexone_dw_data.patient_option po, trexone_aud_data.patient_address pa, TREXONE_DW_DATA.DRUG dispdrug
                                  WHERE ff.ready_date_key = To_Number(To_Char((:RunDate - 6),'YYYYMMDD')) AND
                                    ff.pd_patient_key = p.pd_patient_key AND
                                    p.pd_patient_num = pa.patient_num AND
                                    p.pd_species_name IS NULL AND
                                    ff.cancel_date_key = 0 AND
                                    ff.sold_date_key = 0 AND
                                    ff.fd_facility_key = f.fd_facility_key AND
                                    p.patient_option_key = po.patient_option_key AND
                                    po.auto_notify_option='Y' AND
                                    f.fd_is_active = 'Y' AND
                                    f.fd_facility_id < '199' AND
					                ff.DISPENSED_DRUG_KEY = dispdrug.DRUG_KEY
                                    ORDER BY ff.fd_facility_key, ff.rx_number, ff.refill_num) step1
                left outer join trexone_dw_data.patient_phone preferred on 
                    (step1.patientkey = preferred.pd_patient_key 
                    AND preferred.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
                    AND preferred.patp_phone_usage =
                        (CASE step1.contactbycode
                            WHEN '1' then 1    -- Home 1
                            WHEN '3' then 1    -- Home 2
                            WHEN '5' then 1    -- Work
                            WHEN '7' then 1    -- Other
                            WHEN '9' then 2    -- Fax (Home 1)
                            WHEN '8' then 16   -- Cell
                            WHEN '10' then 32  -- Alt 1
                            WHEN '11' then 8   -- Alt 2
                            WHEN '18' then 64  -- Alt 3
                        END) 
                    AND preferred.patp_usage =
                        (CASE step1.contactbycode
                            WHEN '1' then 1    -- Home 1
                            WHEN '3' then 2    -- Home 2
                            WHEN '5' then 4    -- Work
                            WHEN '7' then 8    -- Other
                            WHEN '9' then 1    -- Fax (Home 1)
                            WHEN '8' then 0    -- Cell
                            WHEN '10' then 0   -- Alt 1
                            WHEN '11' then 0   -- Alt 2
                            WHEN '18' then 0   -- Alt 3
                        END))) step2
               -- LEFT OUTER JOIN wegmans.postransaction pos ON
               -- (pos.facility_id = step2.store AND
               -- pos.rx_number = step2.rx AND
               -- pos.refill_num = step2.refill_num)
				) --get pos sold date info
        --WHERE sold_date IS NULL --filter our recs that have a pos sold date
		--WHERE Is_Flu_Shot = 'N'
)
ORDER BY store, rx