WITH FILTER_PRESCRIBER AS (
    SELECT Distinct regexp_substr(:PrescriberNpiCSV,'[^,]+', 1, LEVEL) AS prescriber_npi FROM dual
    CONNECT BY LEVEL <= regexp_count(:PrescriberNpiCSV, '[^,]+')
), FILTER_NDC AS (
    SELECT Distinct drug_ndc
    FROM TREXONE_DW_DATA.Drug 
    WHERE drug_ndc IN (
        SELECT regexp_substr(:NdcCSV,'[^,]+', 1, LEVEL) AS ndc FROM dual
        CONNECT BY LEVEL <= regexp_count(:NdcCSV, '[^,]+')
    )
)
SELECT 
     ReportingName --"Name of person providing reports"
    ,ReportingPhone --"Telephone number of person providing report"
    ,ReportingEmail --"Email of person providing directory information"
    ,PharmacyName --"Pharmacy name (how pharmacy is generally known in the community)"
    ,PharmacyNbr --"Store Number"
	,ZipCode --"ZipCode"
    ,PharmacyNPI --"Pharmacy NPI"
    ,PrescriberNPI --"Dispensing Encounter: Linked NPI (provider)"
    ,DrugName --"Please include each  Dispensing Encounter: Formulation name"
    ,DrugNDC --"Dispensing per Encounter: Formulation NDC"
    ,To_Char(Sum(DrugQty)) AS DrugQty --"Dispensing Encounter: Count of NDC dispensed per encounter"
FROM (
		SELECT /*+ cardinality(ff,1) leading (ff) index (ff IN_FILL_FACT_03)  */
			 :ReportingName AS ReportingName
			,:ReportingPhone AS ReportingPhone
			,:ReportingEmail AS ReportingEmail
			,:ReportingName AS PharmacyName
			,fac.fd_facility_id AS PharmacyNbr
			,fac.fd_zipcode AS ZipCode
			,fac_id.fd_value AS PharmacyNPI
			,ff.prescriber_id_npi AS PrescriberNPI
			,d.drug_label_name AS DrugName
			,d.drug_ndc AS DrugNDC
			,(CASE WHEN ff.fill_state_code = '14' THEN 1   --Sold=14
			       WHEN ff.fill_state_code = '16' THEN -1  --DeclineSold/Refund=16
				   ELSE 0
			  END) AS DrugQty
		FROM TREXONE_AUD_DATA.Fill_Fact ff
			 INNER JOIN TREXONE_DW_DATA.Facility fac
				 ON fac.fd_facility_num = ff.facility_num
				AND fac.fd_is_active = 'Y'
			 INNER JOIN TREXONE_DW_DATA.Facility_Id fac_Id
				 ON fac_id.fd_facility_num = ff.facility_num
				AND fac_id.fd_type = 'F08'
			 INNER JOIN TREXONE_DW_DATA.Prescriber pres
				 ON pres.prs_prescriber_num = ff.prescriber_num
			 INNER JOIN TREXONE_DW_DATA.Drug d
				 ON d.drug_num = ff.dispensed_drug_num
			 INNER JOIN FILTER_PRESCRIBER fp
				 ON fp.prescriber_npi = ff.prescriber_id_npi
			 INNER JOIN FILTER_NDC fn
				 ON fn.drug_ndc = d.drug_ndc
		WHERE ff.fill_state_chg_ts BETWEEN :StartDate AND :EndDate
		  AND ff.fill_state_code IN ('14','16') --Sold=14, DeclineSold/Refund=16
		  AND ff.is_same_day_reversal = 'N' 
     )
GROUP BY
     ReportingName
    ,ReportingPhone
    ,ReportingEmail
    ,PharmacyName
    ,PharmacyNbr
	,ZipCode
    ,PharmacyNPI
    ,PrescriberNPI
    ,DrugName
    ,DrugNDC
ORDER BY 5, 8, 9