SELECT
	  PharmacyNpi
	, PrescriberNpi
	, DaysSupply
	, RxNumber
	, RefillNumber
	, Gender
	, TpPlanNum
	, QuantityDispensed
	, IsCompound
	, TotalUserDefined
	, FinalPrice
	, TotalCost
	, PatientPayAmount
	, DispenseDate
	, BirthDate
	, PrescribedDate
	, DawCode
	, TpPersonCode
	, RefillsAuthorized
	, BinNumber
	, UnexpandedDirections
	, NcpdpNumber
	, PlanCode
	, CompoundNumber
	, DispensedNdc
	, ProductSchedule
	, WrittenNdc
	, PrescriberDea
	, PrescriberZip
	, PatientZip
	, PlanGroup
	, PrescriberFirstName
	, PrescriberMiddleName
	, PrescriberLastName
	, PlanName
	, DrugDescription
	, PatientFirstName
	, PatientLastName
	, PatientMiddleName
	, FillStateChangeTimestamp
FROM ( SELECT
	(CASE WHEN Fac.fd_facility_id = '198' THEN
	'1902172612'
	ELSE FI.FD_VALUE END) AS PharmacyNpi,
	Ff.PRESCRIBER_ID_NPI AS PrescriberNpi,
	Ff.days_supply AS DaysSupply,
	Ff.rx_number AS RxNumber,
	Ff.refill_num AS RefillNumber,
	Pat.gender AS Gender,
	Ff.tp_plan_num AS TpPlanNum,
	Ff.qty_dispensed AS QuantityDispensed,
	Prod.prd_is_compound AS IsCompound,
	Ff.total_user_defined AS TotalUserDefined,
	Ff.final_price AS FinalPrice,
	Ff.total_cost AS TotalCost,
	Ff.patient_pay_amount AS PatientPayAmount,
	Ff.dispense_date AS DispenseDate,
	Pat.pd_birth_date AS BirthDate,
	Ff.prescribed_date AS PrescribedDate,
	Ff.daw_code AS DawCode,
	Ff.tp_person_code AS TpPersonCode,
	Ff.refills_authorized AS RefillsAuthorized,
	Tppd.bin_number AS BinNumber,
    (CASE WHEN Ff.unexpanded_directions IS NULL THEN 'na' ELSE Ff.unexpanded_directions END) AS UnexpandedDirections,
	(CASE WHEN Fac.fd_facility_id = '198' THEN
	'5804819'
	ELSE Fac.fd_ncpdp_provider_id END) AS NcpdpNumber, 
	Tp.tpld_plan_code AS PlanCode,
	prod.prd_product_compound_num AS CompoundNumber,
	Disp_Drug.drug_ndc AS DispensedNdc,
	prod.prd_product_schedule AS ProductSchedule,
	Written_Drug.drug_ndc AS WrittenNdc,
	Ff.prescriber_id_dea AS PrescriberDea,
	Pres_Add.padr_zipcode AS PrescriberZip,
	Pa.pad_zip AS PatientZip,
	Ff.benefit_group_id AS PlanGroup,
	Pres.prs_first_name AS PrescriberFirstName,
	Pres.prs_middle_name AS PrescriberMiddleName,
	Pres.prs_last_name AS PrescriberLastName,
	Tp.tpld_plan_name AS PlanName,
	Prod.prd_name AS DrugDescription,
	Pat.pd_first_name AS PatientFirstName,
	Pat.pd_last_name AS PatientLastName,
	Pat.pd_middle_name AS PatientMiddleName,
	Ff.fill_state_chg_ts AS FillStateChangeTimestamp,
    Rank() OVER (PARTITION BY Pat.pd_patient_num 
           ORDER BY (CASE WHEN Pa.pad_end_date = TO_DATE('12/31/2999','MM/DD/YYYY') THEN 
						CASE Pa.pad_usage
							WHEN '2' THEN 1
							WHEN '3' THEN 2
							WHEN '7' THEN 3
							WHEN '6' THEN 4
							WHEN '5' THEN 5
							WHEN '4' THEN 6
							ELSE 7
						END
						ELSE 8
					 END)
                    ,Pa.pad_create_date Desc
                    ,Pa.rowid
    ) AS AddressRank       
FROM Fill_Fact Ff
INNER JOIN TREXONE_DW_DATA.Patient Pat ON (Pat.pd_patient_num = Ff.patient_num)
INNER JOIN TREXONE_DW_DATA.Facility Fac ON (Fac.fd_facility_num = Ff.facility_num)
INNER JOIN TREXONE_DW_DATA.Facility_ID FI ON (Fac.fd_facility_num = FI.fd_facility_num AND FI.FD_TYPE = 'F08') -- pharmacy npi
INNER JOIN TREXONE_DW_DATA.Prescriber Pres ON (Pres.prs_prescriber_num = Ff.prescriber_num)
INNER JOIN TREXONE_DW_DATA.Prescriber_Address Pres_Add ON (Pres_Add.padr_prescriber_address_num = Ff.prescriber_address_num)
INNER JOIN TREXONE_DW_DATA.Product Prod ON (Prod.prd_product_num = Ff.dispensed_prod_num)
INNER JOIN TREXONE_DW_DATA.Drug Disp_Drug ON (Disp_Drug.drug_num = Ff.dispensed_drug_num)
INNER JOIN TREXONE_DW_DATA.Drug Written_Drug ON (Written_Drug.drug_num = Ff.written_drug_num)
LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan Tp ON (Tp.tpld_plan_num = Ff.tp_plan_num)
LEFT OUTER JOIN Tp_Processor_Destination Tppd ON (Tppd.tp_processor_destination_seq = Ff.tp_processor_dest_num AND Tppd.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD'))
LEFT OUTER JOIN TREXONE_DW_DATA.Medical_Condition Med_Cond ON (Med_Cond.mcd_med_condition_num = Ff.medical_condition_num)
LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Address Pa ON (Pat.pd_patient_key = Pa.pd_patient_key AND Pa.status = 'A' AND NVL(INSTR(Pa.erx_client_id, 'DELETED'), 0) = 0)
WHERE Ff.fill_state_chg_ts BETWEEN (:RunDate - 7) AND :RunDate
AND Ff.fill_state_code = 14
AND (NVL(Ff.has_reversal, 'N') = 'N')
AND Fac.fd_is_active = 'Y'
AND Fac.fd_facility_id not in (
	'275', --Corp POS Testing
	'121', --Old Henrietta Store
	'801', --TRAINING - ROCHESTER
	'802', --TRAINING - PA
	'803', --TRAINING - VA
	'804', --TRAINING - NJ
	'805', --TRAINING - MD
	'806', --TRAINING - BUFFALO
	'807', --TRAINING - SYRACUSE
	'808', --TRAINING - SOUTHERN TIER
	'809', --TRAINING - MASSACHUSETTS
	'810', --TRAINING - CALL CENTER
	'997', --Central Fill
	'998'  --Call Center
)) f
WHERE f.AddressRank = 1