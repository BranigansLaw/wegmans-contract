SELECT
   Field01
  ,Field02
  ,Field03
  ,Field04
  ,Field05
  ,Field06
  ,Field07
  ,Field08
  ,Field09
  ,Field10
  ,Field11
  ,Field12
  ,Field13
  ,Field14
  ,Field15
  ,Field16
  ,Field17
  ,Field18
  ,Field19
  ,Field20
  ,Field21
  ,Field22
  ,Field23
  ,Field24
  ,Field25
  ,Field26
  ,Field27
  ,Field28
  ,Field29
  ,Field30
  ,Field31
  ,Field32
  ,Field33
  ,Field34
  ,Field35
  ,Field36
  ,Field37
  ,Field38
  ,Field39
  ,Field40
  ,Field41
  ,Field42
  ,Field43
  ,Field44
  ,Field45
  ,Field46
  ,Field47
  ,Field48
  ,Field49
  ,Field50
  ,Field51
  ,Field52
  ,Field53
  ,Field54
  ,Field55
  ,Field56
  ,Field57
  ,Field58
  ,Field59
  ,Field60
  ,Field61
  ,Field62
  ,Field63
  ,Field64
  ,Field65
  ,Field66
  ,Field67
  ,Field68
  ,Field69
  ,Field70
  ,Field71
  ,Field72
  ,Field73
  ,Field74
  ,Field75
  ,Field76
  ,Field77
  ,Field78
  ,Field79
  ,Field80
  ,Field81
  ,Field82
  ,Field83
  ,Field84
  ,Field85
  ,Field86
  ,Field87
  ,Field88
  ,Field89
  ,Field90
  ,Field91
  ,Field92
  ,Field93
  ,Field94
  ,Field95
  ,Field96
  ,Field97
  ,Field98
  ,Field99
  ,Field100
  ,Field101
  ,Field102
  ,Field103
  ,Field104
  ,Field105
  ,Field106
  ,Field107
  ,Field108
 FROM (
		 SELECT
			 Cast(Set_To_Fixed_Width(Field01, 2, 'L') as CHAR(2 CHAR)) AS Field01
			,Cast(Set_To_Fixed_Width(Field02, 1, 'L') as CHAR(1 CHAR)) AS Field02
			,Cast(Set_To_Fixed_Width(Field03, 8, 'L') as CHAR(8 CHAR)) AS Field03
			,Cast(Set_To_Fixed_Width(Field04, 6, 'L') as CHAR(6 CHAR)) AS Field04
			,Cast(Set_To_Fixed_Width(Field05, 15, 'L') as CHAR(15 CHAR)) AS Field05
			,Cast(Set_To_Fixed_Width(Field06, 10, 'L') as CHAR(10 CHAR)) AS Field06
			,Cast(Set_To_Fixed_Width(Field07, 6, 'L') as CHAR(6 CHAR)) AS Field07
			,Cast(Set_To_Fixed_Width(Field08, 15, 'L') as CHAR(15 CHAR)) AS Field08
			,Cast(Set_To_Fixed_Width(Field09, 8, 'L') as CHAR(8 CHAR)) AS Field09
			,Cast(Set_To_Fixed_Width(Field10, 1, 'R') as CHAR(1 CHAR)) AS Field10
			,Cast(Set_To_Fixed_Width(Field11, 1, 'R') as CHAR(1 CHAR)) AS Field11
			,Cast(Set_To_Fixed_Width(Field12, 2, 'R') as CHAR(2 CHAR)) AS Field12
			,Cast(Set_To_Fixed_Width(Field13, 2, 'R') as CHAR(2 CHAR)) AS Field13
			,Cast(Set_To_Fixed_Width(Field14, 1, 'R') as CHAR(1 CHAR)) AS Field14
			,Cast(Set_To_Fixed_Width(Field15, 8, 'L') as CHAR(8 CHAR)) AS Field15
			,Cast(Set_To_Fixed_Width(Field16, 3, 'L') as CHAR(3 CHAR)) AS Field16
			,Cast(Set_To_Fixed_Width(Field17, 3, 'R') as CHAR(3 CHAR)) AS Field17
			,Cast(Set_To_Fixed_Width(Field18, 10, 'L') as CHAR(10 CHAR)) AS Field18
			,Cast(Set_To_Fixed_Width(Field19, 7, 'L') as CHAR(7 CHAR)) AS Field19
			,Cast(Set_To_Fixed_Width(Field20, 2, 'R') as CHAR(2 CHAR)) AS Field20
			,Cast(Set_To_Fixed_Width(Field21, 10, 'R') as CHAR(10 CHAR)) AS Field21
			,Cast(Set_To_Fixed_Width(Field22, 3, 'R') as CHAR(3 CHAR)) AS Field22
			,Cast(Set_To_Fixed_Width(Field23, 1, 'R') as CHAR(1 CHAR)) AS Field23
			,Cast(Set_To_Fixed_Width(Field24, 19, 'L') as CHAR(19 CHAR)) AS Field24
			,Cast(Set_To_Fixed_Width(Field25, 2, 'L') as CHAR(2 CHAR)) AS Field25
			,Cast(Set_To_Fixed_Width(Field26, 1, 'L') as CHAR(1 CHAR)) AS Field26
			,Cast(Set_To_Fixed_Width(Field27, 8, 'R') as CHAR(8 CHAR)) AS Field27
			,Cast(Set_To_Fixed_Width(Field28, 8, 'R') as CHAR(8 CHAR)) AS Field28
			,Cast(Set_To_Fixed_Width(Field29, 9, 'L') as CHAR(9 CHAR)) AS Field29
			,Cast(Set_To_Fixed_Width(Field30, 6, 'L') as CHAR(6 CHAR)) AS Field30
			,Cast(Set_To_Fixed_Width(Field31, 2, 'L') as CHAR(2 CHAR)) AS Field31
			,Cast(Set_To_Fixed_Width(Field32, 8, 'R') as CHAR(8 CHAR)) AS Field32
			,Cast(Set_To_Fixed_Width(Field33, 8, 'R') as CHAR(8 CHAR)) AS Field33
			,Cast(Set_To_Fixed_Width(Field34, 8, 'L') as CHAR(8 CHAR)) AS Field34
			,Cast(Set_To_Fixed_Width(Field35, 2, 'R') as CHAR(2 CHAR)) AS Field35
			,Cast(Set_To_Fixed_Width(Field36, 2, 'R') as CHAR(2 CHAR)) AS Field36
			,Cast(Set_To_Fixed_Width(Field37, 11, 'R') as CHAR(11 CHAR)) AS Field37
			,Cast(Set_To_Fixed_Width(Field38, 2, 'R') as CHAR(2 CHAR)) AS Field38
			,Cast(Set_To_Fixed_Width(Field39, 1, 'R') as CHAR(1 CHAR)) AS Field39
			,Cast(Set_To_Fixed_Width(Field40, 2, 'R') as CHAR(2 CHAR)) AS Field40
			,Cast(Set_To_Fixed_Width(Field41, 15, 'L') as CHAR(15 CHAR)) AS Field41
			,Cast(Set_To_Fixed_Width(Field42, 2, 'L') as CHAR(2 CHAR)) AS Field42
			,Cast(Set_To_Fixed_Width(Field43, 2, 'L') as CHAR(2 CHAR)) AS Field43
			,Cast(Set_To_Fixed_Width(Field44, 15, 'L') as CHAR(15 CHAR)) AS Field44
			,Cast(Set_To_Fixed_Width(Field45, 2, 'L') as CHAR(2 CHAR)) AS Field45
			,Cast(Set_To_Fixed_Width(Field46, 1, 'R') as CHAR(1 CHAR)) AS Field46
			,Cast(Set_To_Fixed_Width(Field47, 8, 'R') as CHAR(8 CHAR)) AS Field47
			,Cast(Set_To_Fixed_Width(Field48, 15, 'L') as CHAR(15 CHAR)) AS Field48
			,Cast(Set_To_Fixed_Width(Field49, 1, 'R') as CHAR(1 CHAR)) AS Field49
			,Cast(Set_To_Fixed_Width(Field50, 8, 'R') as CHAR(8 CHAR)) AS Field50
			,Cast(Set_To_Fixed_Width(Field51, 8, 'R') as CHAR(8 CHAR)) AS Field51
			,Cast(Set_To_Fixed_Width(Field52, 8, 'L') as CHAR(8 CHAR)) AS Field52
			,Cast(Set_To_Fixed_Width(Field53, 30, 'L') as CHAR(30 CHAR)) AS Field53
			,Cast(Set_To_Fixed_Width(Field54, 8, 'R') as CHAR(8 CHAR)) AS Field54
			,Cast(Set_To_Fixed_Width(Field55, 2, 'L') as CHAR(2 CHAR)) AS Field55
			,Cast(Set_To_Fixed_Width(Field56, 2, 'L') as CHAR(2 CHAR)) AS Field56
			,Cast(Set_To_Fixed_Width(Field57, 2, 'L') as CHAR(2 CHAR)) AS Field57
			,Cast(Set_To_Fixed_Width(Field58, 8, 'L') as CHAR(8 CHAR)) AS Field58
			,Cast(Set_To_Fixed_Width(Field59, 15, 'L') as CHAR(15 CHAR)) AS Field59
			,Cast(Set_To_Fixed_Width(Field60, 2, 'L') as CHAR(2 CHAR)) AS Field60
			,Cast(Set_To_Fixed_Width(Field61, 19, 'L') as CHAR(19 CHAR)) AS Field61
			,Cast(Set_To_Fixed_Width(Field62, 2, 'L') as CHAR(2 CHAR)) AS Field62
			,Cast(Set_To_Fixed_Width(Field63, 10, 'R') as CHAR(10 CHAR)) AS Field63
			,Cast(Set_To_Fixed_Width(Field64, 2, 'L') as CHAR(2 CHAR)) AS Field64
			,Cast(Set_To_Fixed_Width(Field65, 8, 'R') as CHAR(8 CHAR)) AS Field65
			,Cast(Set_To_Fixed_Width(Field66, 8, 'R') as CHAR(8 CHAR)) AS Field66
			,Cast(Set_To_Fixed_Width(Field67, 8, 'R') as CHAR(8 CHAR)) AS Field67
			,Cast(Set_To_Fixed_Width(Field68, 8, 'R') as CHAR(8 CHAR)) AS Field68
			,Cast(Set_To_Fixed_Width(Field69, 8, 'R') as CHAR(8 CHAR)) AS Field69
			,Cast(Set_To_Fixed_Width(Field70, 3, 'L') as CHAR(3 CHAR)) AS Field70
			,Cast(Set_To_Fixed_Width(Field71, 3, 'L') as CHAR(3 CHAR)) AS Field71
			,Cast(Set_To_Fixed_Width(Field72, 3, 'L') as CHAR(3 CHAR)) AS Field72
			,Cast(Set_To_Fixed_Width(Field73, 8, 'R') as CHAR(8 CHAR)) AS Field73
			,Cast(Set_To_Fixed_Width(Field74, 8, 'R') as CHAR(8 CHAR)) AS Field74
			,Cast(Set_To_Fixed_Width(Field75, 8, 'R') as CHAR(8 CHAR)) AS Field75
			,Cast(Set_To_Fixed_Width(Field76, 30, 'L') as CHAR(30 CHAR)) AS Field76
			,Cast(Set_To_Fixed_Width(Field77, 8, 'R') as CHAR(8 CHAR)) AS Field77
			,Cast(Set_To_Fixed_Width(Field78, 8, 'R') as CHAR(8 CHAR)) AS Field78
			,Cast(Set_To_Fixed_Width(Field79, 8, 'R') as CHAR(8 CHAR)) AS Field79
			,Cast(Set_To_Fixed_Width(Field80, 8, 'R') as CHAR(8 CHAR)) AS Field80
			,Cast(Set_To_Fixed_Width(Field81, 8, 'R') as CHAR(8 CHAR)) AS Field81
			,Cast(Set_To_Fixed_Width(Field82, 2, 'R') as CHAR(2 CHAR)) AS Field82
			,Cast(Set_To_Fixed_Width(Field83, 8, 'R') as CHAR(8 CHAR)) AS Field83
			,Cast(Set_To_Fixed_Width(Field84, 8, 'L') as CHAR(8 CHAR)) AS Field84
			,Cast(Set_To_Fixed_Width(Field85, 7, 'L') as CHAR(7 CHAR)) AS Field85
			,Cast(Set_To_Fixed_Width(Field86, 8, 'L') as CHAR(8 CHAR)) AS Field86
			,Cast(Set_To_Fixed_Width(Field87, 15, 'L') as CHAR(15 CHAR)) AS Field87
			,Cast(Set_To_Fixed_Width(Field88, 5, 'R') as CHAR(5 CHAR)) AS Field88
			,Cast(Set_To_Fixed_Width(Field89, 32, 'L') as CHAR(32 CHAR)) AS Field89
			,Cast(Set_To_Fixed_Width(Field90, 1, 'L') as CHAR(1 CHAR)) AS Field90
			,Cast(Set_To_Fixed_Width(Field91, 31, 'L') as CHAR(31 CHAR)) AS Field91
			,Cast(Set_To_Fixed_Width(Field92, 15, 'R') as CHAR(15 CHAR)) AS Field92
			,Cast(Set_To_Fixed_Width(Field93, 1, 'L') as CHAR(1 CHAR)) AS Field93
			,Cast(Set_To_Fixed_Width(Field94, 10, 'L') as CHAR(10 CHAR)) AS Field94
			,Cast(Set_To_Fixed_Width(Field95, 1, 'L') as CHAR(1 CHAR)) AS Field95
			,Cast(Set_To_Fixed_Width(Field96, 15, 'L') as CHAR(15 CHAR)) AS Field96
			,Cast(Set_To_Fixed_Width(Field97, 2, 'L') as CHAR(2 CHAR)) AS Field97
			,Cast(Set_To_Fixed_Width(Field98, 5, 'L') as CHAR(5 CHAR)) AS Field98
			,Cast(Set_To_Fixed_Width(Field99, 2, 'L') as CHAR(2 CHAR)) AS Field99
			,Cast(Set_To_Fixed_Width(Field100, 11, 'L') as CHAR(11 CHAR)) AS Field100
			,Cast(Set_To_Fixed_Width(Field101, 20, 'L') as CHAR(20 CHAR)) AS Field101
			,Cast(Set_To_Fixed_Width(Field102, 30, 'L') as CHAR(30 CHAR)) AS Field102
			,Cast(Set_To_Fixed_Width(Field103, 5, 'R') as CHAR(5 CHAR)) AS Field103
			,Cast(Set_To_Fixed_Width(Field104, 9, 'L') as CHAR(9 CHAR)) AS Field104
			,Cast(Set_To_Fixed_Width(Field105, 8, 'L') as CHAR(8 CHAR)) AS Field105
			,Cast(Set_To_Fixed_Width(Field106, 1, 'L') as CHAR(1 CHAR)) AS Field106
			,Cast(Set_To_Fixed_Width(Field107, 10, 'L') as CHAR(10 CHAR)) AS Field107
			,Cast(Set_To_Fixed_Width(Field108, 1, 'L') as CHAR(1 CHAR)) AS Field108
			,Ff_fill_state_chg_ts
		FROM (
			  SELECT
				  '5X' AS Field01 --Version/Release Number
				 ,(CASE WHEN Ff.fill_state_code = 14 THEN 'C'
						WHEN Ff.fill_state_code = 16 THEN 'R'
						ELSE ' '
				   END) AS Field02 --Record Type
				 ,To_Char(Ff.ADJUDICATION_DATE,'YYYYMMDD') AS Field03 --Date Authorized
				 ,To_Char(Ff.ADJUDICATION_DATE, 'HH24MISS') AS Field04 --Time Authorized
				 ,Fac_Id.fd_value AS Field05 --Service Provider ID
				 ,Tppd.processor_control_number AS Field06 --Processor Control Number
				 ,Tppd.bin_number AS Field07 --BIN Number
				 ,Tpp.tpd_benefit_group_id AS Field08 --Group ID
				 ,(CASE WHEN Pat_Add.birth_date IS NULL THEN '        '
						ELSE TO_CHAR(Pat_Add.birth_date,'YYYYMMDD')
				   END) AS Field09 --Date of Birth
				 ,(CASE WHEN Pat_Add.gender = 'M' THEN '1'
						WHEN Pat_Add.gender = 'F' THEN '2'
						ELSE '0'
				   END) AS Field10 --Patient Gender Code
				 ,To_Char(NVL(Tpp.tpd_relationship_num,0)) AS Field11 --Patient Relationship Code
				 ,To_Char(NVL(Tpp.tpd_patient_location,0)) AS Field12 --Patient Location Code
				 ,To_Char(NVL(Ff.other_coverage_code_num,0)) AS Field13 --Other Coverage Code
				 ,To_Char(NVL(Tpp.tpd_eligibility_clarif_num,0)) AS Field14 --Eligibility Clarification Code
				 ,(CASE WHEN Ff.date_of_service IS NULL THEN '        '
						ELSE TO_CHAR(Ff.date_of_service,'YYYYMMDD')
				   END) AS Field15 --Date of Service
				 ,Tpp.tpd_home_plan AS Field16 --Home Plan
				 ,(CASE WHEN Pat_Add.zip IS NULL THEN '   '
						ELSE Substr(Pat_Add.zip,1,3)
				   END) AS Field17 --Patient Zip Code
				 ,Substr(Tp.tpld_plan_code, 1, 3) AS Field18 --Carrier ID
				 ,'       ' AS Field19 --Filler
				 ,To_Char(Ff.refill_num) AS Field20 --Fill Number
				 ,To_Char((Ff.qty_dispensed * 1000)) AS Field21 --Quantity Dispensed
				 ,To_Char(Ff.days_supply) AS Field22 --Days Supply
				 ,(CASE WHEN Prod.prd_is_compound = 'Y' THEN '2'
						ELSE '1'
				   END) AS Field23 --Compound Code
				 ,Drug.drug_ndc AS Field24 --Product/Service ID
				 ,'03' AS Field25 --Product/Service ID Qualifier
				 ,NVL(Ff.daw_code,'0') AS Field26 --DAW Product Selection Code
				,To_Char((CASE WHEN Substr(Tp.tpld_plan_code, 4, 9) IN (
						'TREMVCH'
						,'TREMLIN'
						,'TREMND'
						,'STEL90V'
						,'STEL90'
						,'STEL90N'
						,'STEL45V'
						,'STEL45'
						,'STEL45N'
						,'SIMP50V'
						,'SIMP50'
						,'SIMP50N'
						,'SIMP100V'
						,'SIMP100'
						,'SIMP100N'
						,'JVR'
						,'JLR'
						,'EPAON'
						,'EPAOR'
						,'PAR'
						,'OVF'
						,'OVR'
						,'OVND'
						,'ZFF'
						,'ZYR')
					THEN 0
					ELSE Ff.DISPENSED_QTY_AWP END)*100) AS Field27 --Ingredient Cost Submitted (AWP)
				 ,'        ' AS Field28 --Flat Sales Tax Amt Submitted
				 ,Ff.PRESCRIBER_ID_DEA AS Field29 --Prescriber ID / DEA#
				 ,'      ' AS Field30 --Filler
				 ,'  ' AS Field31 --Filler
				 ,To_Char((CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN (Ff.dispensing_fee_amount * 100)
								ELSE (NVL(Ff.tb_dispensing_fee_amount, Ff.tc_dispensing_fee_amount) * 100)
						   END)) AS Field32 --Dispensing fee Submitted
				 ,To_Char((CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN (Ff.final_price * 100)
								ELSE (NVL(Ff.tb_final_price, Ff.tc_final_price) * 100)
						   END)) AS Field33 --Gross Amt Due
				 ,To_Char(Ff.prescribed_date,'YYYYMMDD') AS Field34 --Date Prescription was written
				 ,To_Char(Ff.refills_authorized) AS Field35 --Number of Refills Authorized
				 ,To_Char(Tppa.auth_type_num) AS Field36 --Prior Authorization Type Code
				 ,Tppa.authorization_number AS Field37 --Prior Auth Number Submitted
				 ,'  ' AS Field38 --Level of Service
				 ,Ff.rx_origin_code AS Field39 --Prescription Origin Code
				 ,To_Char(Ff.submission_clarif_num) AS Field40 --Submission Clarification Code
				 ,CASE WHEN Fac.fd_facility_ID = '198' THEN '5804819'
				  ELSE Fac.FD_NCPDP_PROVIDER_ID END AS Field41 --Primary Care Provider ID
				 ,'07' AS Field42 --Primary Care Provider ID Qualifier
				 ,To_Char((CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN Ff.basis_of_cost
								ELSE NVL(Ff.tb_basis_of_cost, Ff.tc_basis_of_cost)
						   END)) AS Field43 --Basis of Cost Determination
				 ,Med_Cond.MCD_FRMTTD_COND_REF_NUM AS Field44 --Diagnosis Code
				 ,(CASE WHEN Med_Cond.MCD_FRMTTD_COND_REF_NUM IS NOT NULL THEN '01'
						ELSE '  '
				   END) AS Field45 --Diagnosis Code Qualifier
				 ,(CASE WHEN ((Prod.prd_product_schedule = 'OTC') OR (Prod.prd_product_schedule IS NULL AND Drug.drug_drug_class = 'O')) THEN '4'
						 WHEN Prod.prd_is_generic = 1 THEN '3'
						 ELSE '1' 
					END) AS Field46 --Drug Type
				 ,To_Char((Ff.total_uandc * 100)) AS Field47 --Usual and Customary Charge
				 ,Pres.prs_last_name AS Field48 --Prescriber Last Name
				 ,Drug.drug_is_unit_dose AS Field49 --Unit Dose Indicator
				 ,'0' AS Field50 --Other Payer Amount Paid
				 ,To_Char((CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN (Ff.patient_pay_amount * 100)
								ELSE (NVL(Ff.tb_patient_pay_amount, Ff.tc_patient_pay_amount) * 100)
						   END)) AS Field51 --Patient Paid Amount Paid
				 ,To_Char(Tpwcc.twed_date_of_injury,'YYYYMMDD') AS Field52 --Date of Injury
				 ,Tpwcc.twed_claim_reference_id AS Field53 --Claim/Reference ID
				 ,To_Char( (NVL(Ff.INCENTIVE_FEE_AMOUNT,0) * 100) ) AS Field54 --Incentive Amt Submitted
				 ,' ' AS Field55 --Reason for Service Code
				 ,' ' AS Field56 --Professional Service Code
				 ,Ff.dur_result_service_cd AS Field57 --Result of Service Code
				 ,' ' AS Field58 --Other Payer Date
				 ,Fac.fd_dea_number AS Field59 --Provider ID
				 ,(CASE WHEN Fac.fd_dea_number IS NULL THEN '  '
						ELSE '01'
				   END) AS Field60 --Provider ID Qualifier
				 ,Written_Drug.drug_ndc AS Field61 --Originally Prescribed Product
				 ,'03' AS Field62 --Originally Prescribed Product ID Qualifier
				 ,To_Char((Ff.original_product_qty * 1000)) AS Field63 --Originally Prescribed Quantity
				 ,Drug.drug_package_units AS Field64 --Unit of Measure 
				 ,To_Char(((CASE WHEN     NVL(Ff.patient_pay_amount,0) < 1
									  AND fac.fd_facility_id IN ('198','199') 
									  AND Drug.drug_ndc IN (
											   '50458057760' --XARELTO 2.5 MG TABLET
											  ,'50458058030' --XARELTO 10 MG TABLET      
											  ,'50458057830' --XARELTO 15 MG TABLET      
											  ,'50458057930' --XARELTO 20 MG TABLET      
										  )
									  THEN 1
								 ELSE NVL(Ff.patient_pay_amount,0)
							END) * 100)) AS Field65 --Patient Pay Amount
				 ,To_Char((Ff.total_cost * 100)) AS Field66 --Ingerdient Cost Paid
				 ,To_Char((Ff.total_fee * 100)) AS Field67 --Dispensing Fee Paid
				 ,To_Char((Ff.tp_tax_flat_amount * 100)) AS Field68 --Flat Sales Tax Amount Paid
				 ,To_Char(((CASE WHEN     NVL(Ff.tp_plan_amount_paid,0) < 1
									  AND fac.fd_facility_id IN ('198','199')
									  AND Drug.drug_ndc IN (
											   '50458057760' --XARELTO 2.5 MG TABLET
											  ,'50458058030' --XARELTO 10 MG TABLET      
											  ,'50458057830' --XARELTO 15 MG TABLET      
											  ,'50458057930' --XARELTO 20 MG TABLET      
										  )
									  THEN 1
								 ELSE (CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN NVL(Ff.patient_pay_amount,0)
											ELSE NVL(Ff.tp_plan_amount_paid,0)
									   END)
							END) * 100)) AS Field69 --Total Amount Paid
				 ,'   ' AS Field70 --Reject Data Reject Reason 1
				 ,'   ' AS Field71 --Reject Data Reject Reason 2
				 ,'   ' AS Field72 --Reject Data Reject Reason 3
				 ,'        ' AS Field73 --Accumulated Deduction Amount
				 ,'        ' AS Field74 --Remaining Deduction Amount
				 ,'        ' AS Field75 --Remaining Benefit Amount
				 ,Drug.drug_gnn AS Field76 --Product Generic Name
				 ,'        ' AS Field77 --Amount Applied to Periodic Deductable
				 ,To_Char((Ff.total_copay * 100)) AS Field78 --Amount of Copay/Coinsurance
				 ,'        ' AS Field79 --Amount Attributed to Product Selection
				 ,'        ' AS Field80 --Amount Exceeding Periodic Benefit
				 ,To_Char((NVL(Ff.incentive_fee_amount,0) * 100)) AS Field81 --Incentive Amount Paid
				 ,To_Char(Ff.basis_of_cost) AS Field82 --Basis for Reim Determination
				 ,'        '  AS Field83 --Amount Attributed to Sales Tax
				 ,Substr(Tp.tpld_plan_code, 4, 10) AS Field84 --Plan ID
				 ,CASE WHEN Fac.fd_facility_ID = '198' THEN '5804819'
				  ELSE Fac.FD_NCPDP_PROVIDER_ID END AS Field85 --NCPDP Provider Number
				 ,Tp.tpld_plan_name AS Field86 --Plan Short Name
				 ,Tppr.name AS Field87 --Processor Name
				 ,Fac.fd_zipcode AS Field88 --Pharmacy Zip Code
				 ,Pat_Add.last_name AS Field89 --Patient Last Name
				 ,(CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN 'C'
						WHEN Ff.external_billing_indicator IS NULL THEN 'T'
						ELSE 'P'
				   END) AS Field90 --Claim Type
				 ,Pat_Add.first_name AS Field91 --Patient First Name
				 ,Ff.rx_number AS Field92 --Prescription/Service Reference No
				 ,(CASE WHEN NVL(Ff.tp_plan_num,0) = 0 THEN '1'
						WHEN Tp.tpld_is_medicaid_plan = 'Y' THEN '2'
						ELSE '3'
				   END) AS Field93 --IMS Payment Type
				 ,Pres.PRS_FIRST_NAME AS Field94 --Prescriber First Name
				 ,Pres.prs_middle_name AS Field95 --Prescriber Middle Initial
				 ,Pres_Add.PADR_CITY AS Field96 --Prescriber City
				 ,Ff.PRESCRIBER_STATE AS Field97 --Prescriber State
				 ,Ff.PRESCRIBER_ZIP AS Field98 --Prescriber Zip Code
				 ,(CASE WHEN Ff.SPLIT_BILLING_CODE = '3' THEN ' 2'
						WHEN Ff.SPLIT_BILLING_CODE = '2' THEN ' 1'
						WHEN Ff.SPLIT_BILLING_CODE = '1' THEN ' 0'
						WHEN Ff.SPLIT_BILLING_CODE = '0' THEN ' 0'
						ELSE '  '
				   END) AS Field99 --Coordination of Benefits Counter          
				 ,'           ' AS Field100 --De-identified Patient Code         
				 ,Ff.CARDHOLDER_ID AS Field101 --Patient Cardholder ID
				 ,Pat_Add.address1 AS Field102 --Patient Street Address
				 ,Pat_Add.zip AS Field103 --Patient ZIP - Encrypted
				 ,Substr(Tp.tpld_plan_code, 4, 9) AS Field104 --IMS Plan Code
				 ,To_Char(Ff.fill_state_chg_ts,'YYYYMMDD') AS Field105 --Date Delivered/Fulfilled
				 ,Ff.short_fill_status AS Field106 --Partial Fill Status
				 ,Ff.PRESCRIBER_ID_NPI AS Field107 --Prescriber NPI Number
				 ,(CASE WHEN i.target_cf_facility_num IS NULL THEN 'N' ELSE 'Y' END) AS Field108 --Central Fill Flag (if applicable)
				 ,Ff.fill_state_chg_ts AS Ff_fill_state_chg_ts
				FROM Fill_Fact Ff
					 INNER JOIN TREXONE_DW_DATA.Facility Fac
						 ON Fac.fd_facility_num = Ff.facility_num
						AND Fac.fd_is_active = 'Y'
					 INNER JOIN TREXONE_DW_DATA.Facility_Id Fac_Id
						 ON Fac_Id.fd_facility_num = Ff.facility_num
						AND Fac_Id.fd_type = 'F08'
					 INNER JOIN Patient_Address Pat_Add
						 ON Pat_Add.patient_num = Ff.patient_num
					 INNER JOIN TREXONE_DW_DATA.Prescriber Pres
						 ON Pres.prs_prescriber_num = Ff.prescriber_num
					 INNER JOIN TREXONE_DW_DATA.Prescriber_Address Pres_Add
						 ON Pres_Add.padr_prescriber_address_num = Ff.prescriber_address_num
					 INNER JOIN TREXONE_DW_DATA.Product Prod
						 ON Prod.prd_product_num = Ff.dispensed_prod_num
					 INNER JOIN TREXONE_DW_DATA.Drug Drug
						 ON Drug.drug_num = Ff.dispensed_drug_num
					 INNER JOIN TREXONE_DW_DATA.Drug Written_Drug
						 ON Written_Drug.drug_num = Ff.written_drug_num
					 LEFT OUTER JOIN TREXONE_AUD_DATA.Item i
						 ON i.rx_record_num = Ff.RX_RECORD_NUM
						AND i.rx_fill_seq = Ff.RX_FILL_SEQ
						AND i.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
						AND i.h_type <> 'D'
					 LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan Tp
						 ON Tp.tpld_plan_num = Ff.tp_plan_num
					 LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient Tpp
						 ON Tpp.tpd_tp_patient_num = Ff.tp_patient_num
						AND Tpp.tpd_eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
					 LEFT OUTER JOIN Tp_Processor_Destination Tppd
						 ON Tppd.tp_processor_destination_seq = Ff.tp_processor_dest_num
						AND Tppd.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
					 LEFT OUTER JOIN Tp_Processor Tppr
						 ON Tppr.tp_processor_num = Ff.tp_processor_num
						AND Tppr.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
					 LEFT OUTER JOIN Tp_Prior_Auth Tppa
						 ON Tppa.tp_prior_auth_num = Ff.tp_prior_auth_num
						AND Tppa.eff_end_date = TO_DATE('2999-12-31', 'YYYY-MM-DD')
					 LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Workers_Comp Tpwcc
						 ON Tpwcc.twed_key = Ff.tp_work_comp_num
					 LEFT OUTER JOIN TREXONE_DW_DATA.Medical_Condition Med_Cond
						 ON Med_Cond.mcd_key = Ff.medical_condition_num
			   WHERE Ff.is_same_day_reversal = 'N'
				 AND Ff.fill_state_code IN (14, 16)
				 AND NVL(Ff.is_bar,'N')= 'N'
			 )
      )
 WHERE Ff_fill_state_chg_ts BETWEEN (:RunDate - 1)
   AND ((:RunDate - 1) + INTERVAL '23:59:59' HOUR TO SECOND)
 ORDER BY Field85, Field92