SELECT *
FROM (
	SELECT patient.PD_FIRST_NAME AS STR_FIRST_NAME
			,patient.PD_MIDDLE_NAME AS STR_MIDDLE_NAME
			,patient.PD_LAST_NAME AS STR_LAST_NAME
			,To_Char(patient.PD_BIRTH_DATE,'YYYY-MM-DD HH24:MI:SS') AS DT_BIRTH_DATE
			,ptdem.GENDER AS C_GENDER
			,ptaddr.PAD_ADDRESS1 AS STR_ADDRESS_ONE
			,ptaddr.PAD_ADDRESS2 AS STR_ADDRESS_TWO
			,ptaddr.PAD_CITY AS STR_CITY
			,ptaddr.PAD_STATE AS STR_STATE
			,ptaddr.PAD_ZIP AS STR_ZIP
			,drug.DRUG_LABEL_NAME AS STR_PRODUCT_NAME
			,drug.DRUG_NDC AS STR_PRODUCT_NDC
			,(CASE WHEN Substr(To_Char(fillfact.QTY_DISPENSED),1,1) = '.'
				   THEN '0' || To_Char(fillfact.QTY_DISPENSED)
				   ELSE To_Char(fillfact.QTY_DISPENSED)
			  END) AS DEC_DISPENSED_QTY
			,product.PRD_IS_GENERIC AS C_GENERIC_IDENTIFIER
			,sysuser.SYS_USER_FNAME AS STR_VERIFIED_RPH_FIRST
			,sysuser.SYS_USER_LNAME AS STR_VERIFIED_RPH_LAST
			,fac.FD_FACILITY_ID AS STR_FACILITY_ID
			,fillfact.RX_NUMBER AS STR_RX_NUMBER
			,drug.DRUG_DOSAGE_FORM AS STR_DOSAGE_FORM
			,drug.DRUG_STRENGTH AS STR_STRENGTH
			,To_Char(To_Date(To_Char(fillfact.SOLD_DATE_KEY),'YYYYMMDD'),'YYYY-MM-DD HH24:MI:SS') AS DT_SOLD_DATE
			,DECODE(fillfact.cancel_date_key,0,null,To_Char(fillfact.CANCEL_DATE_KEY)) AS DT_CANCELLED_DATE
			,DECODE(fillfact.cancel_date_key,0,'A','D') AS C_ACTION_CODE
			,fac.FD_DEA_NUMBER AS STR_DEA
			,facid.FD_Value AS STR_NPI
			,patient.PD_PATIENT_NUM AS DEC_INTERNALPTNUM
			,fpf.lot_number AS LOT_NUMBER
			,To_Char(fpf.exp_date,'YYYY-MM-DD HH24:MI:SS') AS EXP_DATE
			,patient.PATIENT_EMAIL AS STR_PATIENTEMAIL
			,To_Char(To_Date(To_Char(fillfact.SOLD_DATE_KEY),'YYYYMMDD'),'YYYY-MM-DD HH24:MI:SS') AS VIS_PRESENTED_DATE
			,fillfact.administering_site AS ADMINISTRATION_SITE

			--Consent is required in NJ, and tested in Production through test stores.
			,(CASE WHEN fac.fd_state_code = 'NJ' THEN 'Y' --The default value is 'Y' meaning do share data (not hide).
				   ELSE NULL                              --Other states are not using this flag.
			  END) AS PROTECTION_INDICATOR

			,NULL AS RECIP_RACE_1
			,NULL AS RECIP_ETHNICITY
			,fillfact.VFC_status AS VFC_STATUS
			,NULL AS NY_PRIORITY_GROUP
			,REGEXP_REPLACE(pp.patp_area_code || pp.patp_phone_number,'[^0-9]','') AS PHONE_NUMBER
			,Rank() OVER (PARTITION BY fillfact.fill_fact_key, pp.pd_patient_key
								  ORDER BY 
								      (CASE WHEN pp.patp_phone_usage = '1' THEN NULL ELSE pp.patp_phone_usage END) nulls first
									 ,pp.rowid
					 ) AS PatientPhoneRank
			,fillfact.refill_num
			,active_prescribers.NPI AS PRESCRIBED_BY_ID

	FROM TREXONE_DW_DATA.FILL_FACT fillfact

				INNER JOIN TREXONE_DW_DATA.Prescriber pres
				ON pres.PRS_PRESCRIBER_KEY = fillfact.PRS_PRESCRIBER_KEY
       
				LEFT OUTER JOIN
				(SELECT pid_npi.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
					pid_npi.PID_ID_TEXT AS NPI,
					pid_npi.USE_FOR_BILLING AS USE_FOR_BILLING,
					pid_npi.PID_EE_DT AS EXPIRE_DATE    
				FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_npi
				WHERE pid_npi.PID_STATUS = 'A'
				AND pid_npi.PIT_KEY = 509    
				ORDER BY pid_npi.PID_EE_DT desc nulls last) active_prescribers
				ON pres.PRS_PRESCRIBER_KEY = active_prescribers.PRESCRIBER_KEY

				INNER JOIN TREXONE_DW_DATA.FACILITY fac
				ON fac.FD_FACILITY_KEY = fillfact.FD_FACILITY_KEY

				INNER JOIN TREXONE_DW_DATA.FACILITY_ID facid
				ON facid.FD_FACILITY_NUM = fac.FD_FACILITY_NUM
				AND facid.FD_TYPE = 'F08' --National Provider Identifier

				INNER JOIN TREXONE_DW_DATA.PATIENT patient
				ON patient.PD_PATIENT_KEY = fillfact.PD_PATIENT_KEY

				INNER JOIN TREXONE_DW_DATA.PATIENT_DEMOGRAPHIC ptdem
				ON ptdem.PATIENT_DEMOGRAPHIC_KEY = fillfact.PATIENT_DEMOGRAPHIC_KEY

				INNER JOIN TREXONE_DW_DATA.PATIENT_ADDRESS ptaddr
				ON ptaddr.PD_PATIENT_KEY = fillfact.PD_PATIENT_KEY
				AND ptaddr.h_level = (select min(h_level) 
										from TREXONE_DW_DATA.PATIENT_ADDRESS 
										where pd_patient_key=fillfact.pd_patient_key 
										and PAD_END_DATE = TO_DATE('2999-12-31', 'YYYY-MM-DD')
										and PAD_ADDRESS_USAGE = 1)
										
				LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Phone pp
				ON pp.pd_patient_key = fillfact.pd_patient_key
				AND pp.eff_end_date = TO_DATE('29991231','YYYYMMDD')	
				AND pp.patp_usage = '1'  -- HOME1			
									
				INNER JOIN TREXONE_DW_DATA.PRODUCT product
				ON product.PRD_PRODUCT_KEY = fillfact.DISPENSED_PRD_PRODUCT_KEY                

				INNER JOIN TREXONE_DW_DATA.DRUG drug
				ON drug.DRUG_KEY = fillfact.DISPENSED_DRUG_KEY 

				INNER JOIN TREXONE_DW_DATA.SYS_USER sysuser
				ON sysuser.SYS_USER_KEY = fillfact.VERIFY_RPH_SYS_USER_KEY 

				INNER JOIN (SELECT fd_facility_key, rx_number, sold_date_key, cancel_date_key, max(dispensed_item_lot_number) lot_number, max(DISPENSED_ITEM_EXPIRATION_DATE) exp_date 
							FROM TREXONE_DW_DATA.FILL_PRICING_FACT 
							where partition_date BETWEEN (:RunDate - 60) AND (:RunDate + 1)
							GROUP BY fd_facility_key, rx_number, sold_date_key, cancel_date_key) fpf
					ON fpf.FD_FACILITY_KEY = fac.FD_FACILITY_KEY
					AND fpf.RX_NUMBER = fillfact.RX_NUMBER 
					AND fpf.SOLD_DATE_KEY = fillfact.SOLD_DATE_KEY
					AND fpf.cancel_date_key = fillfact.cancel_date_key

	WHERE fillfact.partition_date BETWEEN (:RunDate - 60) AND (:RunDate + 1)
	AND (   --Sold and not cancelled on dtSold date
		(fillfact.SOLD_DATE_KEY = TO_NUMBER(TO_CHAR((:RunDate - 1), 'YYYYMMDD')) 
			AND fillfact.cancel_date_key = 0) 
		OR   --Not sold on dtSold date but was sold within prior two weeks and cancelled on dtSold date
		(fillfact.cancel_date_key = TO_NUMBER(TO_CHAR((:RunDate - 1), 'YYYYMMDD')) 
			AND fillfact.SOLD_DATE_KEY between TO_NUMBER(TO_CHAR((:RunDate - 14), 'YYYYMMDD')) 
											AND TO_NUMBER(TO_CHAR((:RunDate - 2), 'YYYYMMDD')))
		)

	--AND fac.fd_facility_id in (select fd_facility_id from trexone_dw_data.facility where fd_state_code in ('NY','MA','MD','VA','PA','NJ') and FD_IS_ACTIVE = 'Y' and FD_FACILITY_ID < '200') 
	AND (fac.FD_IS_PPI_ENABLED = 'Y' --Licensed stores
		 OR
		 fac.FD_FACILITY_ID IN ('275','299') --Allow testing in Production through test stores.
		)
	AND drug.DRUG_NDC IN 

		(select d.drug_ndc
			from trexone_dw_data.drug d 
			INNER JOIN TREXONE_DW_DATA.PRODUCT P ON (D.DRUG_NUM = P.PRD_DRUG_NUM)
			LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT_IDENTIFIER PI on (P.PRD_PRODUCT_KEY = PI.PRD_PRODUCT_KEY)
			WHERE --P.PRD_DEACTIVATE_DATE IS NULL
				  (P.PRD_DEACTIVATE_DATE IS NULL OR P.PRD_DEACTIVATE_DATE > (:RunDate + 1))
			AND (D.DRUG_OBSOLETE_DATE > (:RunDate + 1) OR D.DRUG_OBSOLETE_DATE IS NULL) 
			and (
				(d.drug_gc3 in (
				 'COVID-19 VACCINES'
				,'ENTERIC VIRUS VACCINES'
				,'GRAM NEGATIVE COCCI VACCINES'
				,'GRAM POSITIVE COCCI VACCINES'
				,'INFLUENZA VIRUS VACCINES'
				,'NEUROTOXIC VIRUS VACCINES'
				,'TOXIN-PRODUCING BACILLI VACCINES/TOXOIDS'
				,'VACCINE/TOXOID PREPARATIONS,COMBINATIONS'
				,'GRAM (-) BACILLI (NON-ENTERIC) VACCINES'
				,'VIRAL/TUMORIGENIC VACCINES'
				,'ANTINEOPLASTICS,MISCELLANEOUS'))
				or
				(pi.IDENTIFIER = 'VAX' and pi.IS_ACTIVE='Y')
				)
			and drug_route <> 'oral'
		)
	AND NVL(fillfact.IS_BAR, 'N') = 'N'
	-- 5/7/18 Adding an exclusion for NY state - we do not want to report on immunizations that are dispensed to be administered at a doctor's office
	--AND ( NOT (fac.fd_state_code, drug.drug_gc3) IN (
	--		('NY', 'ENTERIC VIRUS VACCINES'),
	--		('NY', 'NEUROTOXIC VIRUS VACCINES'),
	--		('NY', 'TOXIN-PRODUCING BACILLI VACCINES/TOXOIDS'),
	--		('NY', 'GRAM (-) BACILLI (NON-ENTERIC) VACCINES'),
	--		('NY', 'VIRAL/TUMORIGENIC VACCINES'),
	--		('NY', 'ANTINEOPLASTICS,MISCELLANEOUS') )
	--	OR ( fac.fd_state_code = 'NY' AND drug.drug_gc3 = 'VIRAL/TUMORIGENIC VACCINES' AND upper(drug.drug_gnn) like '%ZOSTER%' )
	--	OR ( fac.fd_state_code = 'NY' AND drug.drug_gc3 IS NULL )
	--	)
)
WHERE PatientPhoneRank = 1 