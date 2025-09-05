SELECT *
FROM (
	SELECT 
             patient.PD_FIRST_NAME AS STR_FIRST_NAME
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
			,NULL AS ADMINISTRATION_SITE
			--Consent is required in NJ, and tested in Production through test stores.
			,(CASE WHEN fac.fd_state_code = 'NJ' THEN 'Y' --The default value is 'Y' meaning do share data (not hide).
				   ELSE NULL                              --Other states are not using this flag.
			  END) AS PROTECTION_INDICATOR
			,NULL AS RECIP_RACE_1
			,NULL AS RECIP_ETHNICITY
			,NULL AS NY_PRIORITY_GROUP
			,REGEXP_REPLACE(pp.patp_area_code || pp.patp_phone_number,'[^0-9]','') AS PHONE_NUMBER
			,Rank() OVER (PARTITION BY fillfact.fill_fact_key, pp.pd_patient_key
								  ORDER BY 
								      (CASE WHEN pp.patp_phone_usage = '1' THEN NULL ELSE pp.patp_phone_usage END) nulls first
									 ,pp.rowid
					 ) AS PatientPhoneRank
			,fillfact.refill_num

	FROM TREXONE_DW_DATA.FILL_FACT fillfact

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
										and PAD_END_DATE = TO_DATE('2999-12-31', 'YYYY-MM-DD'))
										
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
							where partition_date BETWEEN To_Date('20230801','YYYYMMDD') AND To_Date('20231001','YYYYMMDD')
							GROUP BY fd_facility_key, rx_number, sold_date_key, cancel_date_key) fpf
					ON fpf.FD_FACILITY_KEY = fac.FD_FACILITY_KEY
					AND fpf.RX_NUMBER = fillfact.RX_NUMBER 
					AND fpf.SOLD_DATE_KEY = fillfact.SOLD_DATE_KEY
					AND fpf.cancel_date_key = fillfact.cancel_date_key

	WHERE fillfact.partition_date BETWEEN To_Date('20230801','YYYYMMDD') AND To_Date('20231001','YYYYMMDD') 
	AND (   --Sold and not cancelled on dtSold date
		(       fillfact.SOLD_DATE_KEY between 20230801 and 20230919
			AND fillfact.cancel_date_key = 0) 
		OR   --Not sold on dtSold date but was sold within prior two weeks and cancelled on dtSold date
		(       fillfact.cancel_date_key between 20230801 and 20230919
			AND fillfact.SOLD_DATE_KEY between 20230718 and 20230919)
		)
	AND (fac.FD_IS_PPI_ENABLED = 'Y' --Licensed stores
		 OR
		 fac.FD_FACILITY_ID IN ('275','299') --Allow testing in Production through test stores.
		)
	AND drug.DRUG_NDC IN (
'00000000093', --Flu Vaccine: OFF-SITE QUAD Senior Flu Vaccine (Fluad Quad) $50
'00000000094', --Flu Vaccine: OFF-SITE Quad Flu Vaccine $40.99
'00000000095', --Flu Vaccine: OFF-SITE QUAD Senior Flu Vaccine (Fluad Quad) $69.99
'00000000096', --Flu Vaccine: OFF-SITE Quad Flu Vaccine (Flucelvax) $55.99
'00000000099', --Flu Vaccine: OFF-SITE QUAD Senior Flu Vaccine (Fluad Quad) $86.99
'00000111108', --Flu Vaccine: OFF-SITE Cornell - Fluzone QUAD PFS
'00000111124', --Flu Vaccine: OFF-SITE QUAD Senior Flu Vaccine (Fluad Quad) $77.49
'00000111125', --Flu Vaccine: OFF-SITE Quad Flu Vaccine (Flucelvax) $48.99
'00000111127', --Flu Vaccine: OFF-SITE Quad Flu Vaccine (Flucelvax) $43.99 SYR
'00000111128'  --Flu Vaccine: OFF-SITE Quad Flu Vaccine (Flucelvax) $37.99 SYR
)
	AND NVL(fillfact.IS_BAR, 'N') = 'N'
)
WHERE PatientPhoneRank = 1