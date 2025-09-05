SELECT
     1 AS version
    ,pd_patient_num AS clientpatientid
    ,NULL AS lastname
    ,NULL AS firstname
    ,NVL(gender,'O') AS gender
    ,NULL AS addressLine1
    ,NULL AS addressLine2
    ,NULL AS city
    ,NULL AS state
    ,pad_zip AS zipcode
    ,To_Char(pd_birth_date,'YYYY-MM-DD') AS dob
    ,NULL AS email
    ,NULL AS phone
    ,NULL AS phoneType
    ,NULL AS language
    ,drug_ndc AS NDC
    ,NVL(INTENDED_QTY_DISPENSED,0) AS qty
    ,NVL(INTENDED_DAYS_SUPPLY,0) AS daysSupply
    ,drug_label_name AS ProductName
    ,sold_date_key AS fillDate
    ,refill_num AS fillNum
    ,NVL(total_refills_allowed,0) AS authorizedRefills
    ,FD_VALUE AS NPI
    ,NULL AS medicareContractId
    ,NULL AS medicarePlanId
    ,NULL AS Bin
    ,NULL AS Pcn
    ,NULL AS GroupID
    ,NULL AS Plan
    ,pid_id_text AS prescriberNPI
    ,NULL AS medicareD
    ,NULL AS medicareID
FROM ( 
	  SELECT
		   p.pd_patient_num
		  ,pd.gender
		  ,pa.pad_zip
          ,p.pd_birth_date
		  ,d.drug_ndc
		  ,ff.INTENDED_QTY_DISPENSED
		  ,ff.INTENDED_DAYS_SUPPLY
		  ,d.drug_label_name
		  ,ff.ready_date_key
		  ,ff.sold_date_key
          ,ff.refill_num
          ,ff.total_refills_allowed
          ,fid.FD_VALUE
          ,pres_npi.pid_id_text
		  ,Rank() OVER (PARTITION BY p.pd_patient_num, d.drug_label_name, d.drug_strength
				  ORDER BY ff.dispensed_date_key Desc
						  ,ff.dispensed_time_key --partial fills will have same dispensed_date_key, but 
						  ,ff.rowid              --earliest dispensed_time_key will have accurate INTENDED_QTY_DISPENSED and INTENDED_DAYS_SUPPLY
		   ) AS LatestNdcRank
		  ,Rank() OVER (PARTITION BY p.pd_patient_num 
				  ORDER BY (CASE WHEN (pa.PAD_USAGE = 8 OR pa.PAD_ADDRESS_USAGE = 8) THEN 1 ELSE (pa.PAD_USAGE + 10) END)
						   ,pa.PAD_CREATE_DATE Desc
						   ,pa.rowid
		   ) AS AddressRank
          ,Rank() OVER (PARTITION BY ff.fill_fact_key, pres_npi.prs_prescriber_key
                        ORDER BY
                             pres_npi.h_level Desc
                            ,pres_npi.rowid
           ) AS PrescriberNpiRank
	  FROM TREXONE_DW_DATA.Patient p
		   INNER JOIN TREXONE_DW_DATA.Patient_Address pa
			 ON pa.pd_patient_key = p.pd_patient_key
			AND pa.pad_end_date = TO_DATE('29991231','YYYYMMDD')
		   INNER JOIN TREXONE_DW_DATA.Fill_Fact ff 
			 ON ff.pd_patient_key = p.pd_patient_key
		   INNER JOIN TREXONE_DW_DATA.Facility f
			 ON f.fd_facility_key = ff.fd_facility_key
           INNER JOIN TREXONE_DW_DATA.Facility_ID fid
             ON fid.fd_facility_num = f.fd_facility_num
            AND fid.fd_type = 'F08'
		   INNER JOIN TREXONE_DW_DATA.Drug d
			 ON d.drug_key = ff.dispensed_drug_key
           LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Demographic pd
             ON pd.patient_demographic_key = ff.patient_demographic_key
           LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber pres
             ON pres.prs_prescriber_num = p.pd_prescriber_num
           LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Ids pres_npi
             ON pres_npi.prs_prescriber_key = pres.prs_prescriber_key
            AND pres_npi.pit_key = 509 --See TREXONE_DW_DATA.Prescriber_Id_Type.pit_id_description = 'National Provider Identifier (NPI)'
            AND pres_npi.pid_status = 'A'
	  WHERE p.pd_status = 'A'
        AND p.PD_SPECIES_NAME IS NULL
        AND p.pd_birth_date <= To_Date(To_Char((To_Number(To_Char(:RunDate,'YYYY')) - 64)) || To_Char(:RunDate,'MMDD'),'YYYYMMDD')
        AND ff.sold_date_key = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
   )
WHERE LatestNdcRank = 1
  AND AddressRank = 1
  AND PrescriberNpiRank = 1