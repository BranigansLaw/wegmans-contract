SELECT
	 REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(STR_REQUESTER,'"',''),chr(13),''),chr(10),'') AS mes_request_ref_id
	,STR_SUBMITTER AS requestor_name
	,ID_STORENBR AS requesting_store
	,To_Number(To_Char(DT_REPORTSTART,'YYYYMMDD')) AS report_start_date
	,To_Number(To_Char(DT_REPORTEND,'YYYYMMDD')) AS report_end_date
	,N_COPIES AS copies
	,REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ID_PATIENT,'"',''),chr(13),''),chr(10),'') AS patient_number
	,REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE((CASE WHEN REGEXP_COUNT(STR_PATIENTNAME,',') > 0
				THEN Substr(STR_PATIENTNAME, 1, (InStr(STR_PATIENTNAME,',', 1) - 1))
		   ELSE STR_PATIENTNAME
	  END),'"',''),chr(13),''),chr(10),'') AS last_name
	,REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE((CASE WHEN REGEXP_COUNT(STR_PATIENTNAME,',') > 0
				THEN Trim(Substr(STR_PATIENTNAME, (InStr(STR_PATIENTNAME,',', 1) + 1), Length(STR_PATIENTNAME)))
		   ELSE STR_PATIENTNAME
	  END),'"',''),chr(13),''),chr(10),'') AS first_name
	,NULL AS middle_initial
	,NULL AS date_of_birth
	,NULL AS gender
	,NULL AS phone_number
	,NULL AS address
	,NULL AS city
	,NULL AS state
	,NULL AS zip_code
	,REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(STR_ADDENDUM,'"',''),chr(13),''),chr(10),'') AS additional_information
	,C_RPTTYPE AS report_type
	,(CASE ID_REQTYPE
		   WHEN 1 THEN 'Law_Enforcement'
		   WHEN 2 THEN 'DEA'
		   WHEN 3 THEN 'FDA'
		   WHEN 4 THEN 'Board_Of_Pharmacy'
		   WHEN 5 THEN 'Public_Health_Authorities'
		   WHEN 6 THEN 'Judicial_Inquiry'
		   WHEN 7 THEN 'Accidental'
		   WHEN 8 THEN 'Power_Of_Attorney'
		   WHEN 9 THEN 'Patient_Mail'
		   WHEN 10 THEN 'Patient_Pick_Up'
		   WHEN 11 THEN 'Non_Patient'
		   ELSE NULL
	  END) AS request_type
	,NULL AS patient_request
	,NULL AS non_patient_requesting_entity
	,To_Number(To_Char(DT_CREATED,'YYYYMMDD')) AS form_submit_date
	,STR_SUBMITTER AS form_submit_uid
FROM RX.MESREQUESTQUEUE
ORDER BY
     ID_STORENBR
	,DT_REPORTSTART
	,DT_REPORTEND