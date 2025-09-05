SELECT *
FROM (
		SELECT PATIENT.PD_PATIENT_NUM "patient_num",
				PATIENT.PD_TITLE_ABBR "title_abbr",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(PATIENT.PD_FIRST_NAME,'[,^<>|]','')),1,30) "first_name",
				PATIENT.PD_MIDDLE_NAME "m_initial",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(PATIENT.PD_LAST_NAME,'[,^<>|]','')),1,30) "last_name",
				PATIENT.PD_NAME_SUFFIX_ABBR "suffix_abbr",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(AUDPATIENT.PREFERRED_NAME,'[,^<>|]','')),1,30) "preferred_name",
				TRUNC(((SYSDATE - PATIENT.PD_BIRTH_DATE)/1461)*4) "pt_age",
				CASE WHEN
				(TO_NUMBER(TO_CHAR(PATIENT.PD_BIRTH_DATE,  'DDD'))-TO_NUMBER(TO_CHAR(SYSDATE, 'DDD'))<0)
				THEN
				(366+TO_NUMBER(TO_CHAR(PATIENT.PD_BIRTH_DATE,  'DDD'))-TO_NUMBER(TO_CHAR(SYSDATE, 'DDD')))
				ELSE
				TO_NUMBER(TO_CHAR(PATIENT.PD_BIRTH_DATE,  'DDD'))-TO_NUMBER(TO_CHAR(SYSDATE, 'DDD'))+1
				END "next_bday",
				To_Number(TO_CHAR(PATIENT.PD_BIRTH_DATE,'YYYYMMDD')) "dob",
				To_Number(TO_CHAR(PATIENT.PD_DATE_OF_DEATH,'YYYYMMDD')) "date_of_death",

				PATIENT.PD_STATUS "pt_status",
        
				PATIENT.PD_INACTIVE_REASON "inactive_reason",
				PATIENT.PD_MERGED_TO_PATIENT_NUM "merge_patient_num",
				PATIENTDEMO.GENDER "pt_gender",
        
				PATIENT.PD_PARENT_GUARDIAN_NAME "parent_guardian",
				PATIENT.PD_SPECIES_NAME "pet_species",
				PATIENT.PD_OWNER_NAME "pet_owner",
				To_Number(TO_CHAR(PATIENT.PD_CREATE_DT,'YYYYMMDD')) "pt_create_date",
				DWFACILITY.FD_FACILITY_ID "create_store_num",
				Substr(WEGMANS.Clean_String_ForTenUp(AUDPATIENT.NDC_PREFERENCES),1,800) "ndc_pref",

				PHONE_CELL.PATP_AREA_CODE "area_code_cell",
				SUBSTR(PHONE_CELL.PATP_PHONE_NUMBER,1,3)||'-'||SUBSTR(PHONE_CELL.PATP_PHONE_NUMBER,4,4) "phone_num_cell",
				PHONE_CELL.PATP_EXT "phone_ext_cell",
        
				PHONE_ALTONE.PATP_AREA_CODE "area_code_alt1",
				SUBSTR(PHONE_ALTONE.PATP_PHONE_NUMBER,1,3)||'-'||SUBSTR(PHONE_ALTONE.PATP_PHONE_NUMBER,4,4) "phone_num_alt1",
				PHONE_ALTONE.PATP_EXT "phone_ext_alt1",
        
				PHONE_ALTTWO.PATP_AREA_CODE "area_code_alt2",
				SUBSTR(PHONE_ALTTWO.PATP_PHONE_NUMBER,1,3)||'-'||SUBSTR(PHONE_ALTTWO.PATP_PHONE_NUMBER,4,4) "phone_num_alt2",
				PHONE_ALTTWO.PATP_EXT "phone_ext_alt2",
        
				PHONE_ALTTHREE.PATP_AREA_CODE "area_code_alt3",
				SUBSTR(PHONE_ALTTHREE.PATP_PHONE_NUMBER,1,3)||'-'||SUBSTR(PHONE_ALTTHREE.PATP_PHONE_NUMBER,4,4) "phone_num_alt3",
				PHONE_ALTTHREE.PATP_EXT "phone_ext_alt3",

				Substr(WEGMANS.Clean_String_ForTenUp(PATIENT.PATIENT_EMAIL),1,100) "email1",
				Substr(WEGMANS.Clean_String_ForTenUp(PATIENT.PATIENT_EMAIL2),1,100) "email2",
				Substr(WEGMANS.Clean_String_ForTenUp(PATIENT.PATIENT_EMAIL3),1,100) "email3",
        
				DECODE(PATIENTOPT.CENTRAL_FILL_OPTION,'N',0,1)"cf_option",
				DECODE(PATIENTOPT.MAIL_ORDER_OPTION,'N',0,1) "mail_option",
				DECODE(PATIENTOPT.TARGETED_MARKETING_OPTION,'Y',1,0) "exclude_marketing",
				DECODE(PATIENTOPT.AUTO_NOTIFY_OPTION,'N',0,1) "auto_notify_opt",
				DECODE(PATIENTOPT.AUTO_REFILL_OPTION,'N',0,1) "auto_refill_opt",
				ROUND(PATIENT.PD_HEIGHT,2) "height_cm",
				ROUND(PATIENT.PD_WEIGHT,2) "weight_kg",
				ROUND(PATIENT.PD_BODY_SURFACE_AREA,2) "bsa_msq",

				To_Number(TO_CHAR(PATIENT.PD_ERP_ENROLLMENT_STATUS_DATE,'YYYYMMDD')) "erp_date",
				DECODE(PATIENT.PD_IS_ERP_AUTO_ENROLLED,'N',0,1) "erp_enrolled",
        
				PATIENT.PD_DRIVERS_LICENSE "license_num",
				PATIENT.PD_DRIVERS_LICENSE_STATE "license_state",
				To_Number(TO_CHAR(PATIENT.PD_DRIVERS_LICENSE_EXP_DATE,'YYYYMMDD')) "license_expdate",
				PATIENT.PD_PATIENT_ID "freq_shopper_num",
				PATIENT.PD_OTHER_ID "other_id",
				patient.pd_patient_key "patient_key",
				PATIENT.PD_PRESCRIBER_NUM "pcp_num",

				MEDCOND.MCD_MED_COND_REF_TYPE "mc_code_type",
				MEDCOND.MCD_FRMTTD_COND_REF_NUM "mc_code",
				Substr(WEGMANS.Clean_String_ForTenUp(MEDCOND.MCD_DESCRIPTION),1,255) "mc_desc",
				To_Number(TO_CHAR(PMB.PMB_REPORTED_DATE,'YYYYMMDD')) "mc_reported_date",
				To_Number(TO_CHAR(PMB.PMB_BEGIN_EFFECTIVE_DT,'YYYYMMDD')) "mc_date_added",
				To_Number(TO_CHAR(PMB.PMB_UPDATE_DT,'YYYYMMDD')) "mc_last_modified",
				To_Number(TO_CHAR(PMB.PMB_END_EFFECTIVE_DT,'YYYYMMDD')) "mc_deactivated_date",

				To_Number(SUBSTR(To_Char(NVL(PATIENT.H_LEVEL,20070101)),1,8)) "last_update",
        
				FIRST_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num1",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIRST_TP.TPD_CARDHOLDER_ID,'[,^<>|]','')),1,20) "cardholder_id1",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIRST_PLAN.BIN,'[,^<>|]','')),1,6) "bin1",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIRST_PLAN.PCN,'[,^<>|]','')),1,10) "pcn1",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(first_tp.tpd_benefit_group_id,'[,^<>|]','')),1,15) "benefit_group_id1",
				first_plan.tpld_plan_code "plan_code1",
				FIRST_PLAN.TPLD_PLAN_NAME "plan_name1",
				DECODE(FIRST_TP.TPD_RELATIONSHIP_NUM
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code1",
				DECODE(FIRST_TP.TPD_USE_FOR_COB,'Y',1,0) "cob1",
        
				SEC_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num2",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(sec_tp.tpd_cardholder_id,'[,^<>|]','')),1,20) "cardholder_id2",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(SEC_PLAN.BIN,'[,^<>|]','')),1,6) "bin2",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(sec_plan.pcn,'[,^<>|]','')),1,10) "pcn2",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(sec_tp.tpd_benefit_group_id,'[,^<>|]','')),1,15) "benefit_group_id2",
				SEC_PLAN.TPLD_PLAN_CODE "plan_code2",
				SEC_PLAN.TPLD_PLAN_NAME "plan_name2",
				DECODE(SEC_TP.TPD_RELATIONSHIP_NUM
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code2",
				DECODE(SEC_TP.TPD_USE_FOR_COB,'Y',1,0) "cob2",
        
				TRE_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num3",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(tre_tp.tpd_cardholder_id,'[,^<>|]','')),1,20) "cardholder_id3",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(TRE_PLAN.BIN,'[,^<>|]','')),1,6) "bin3",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(tre_plan.pcn,'[,^<>|]','')),1,10) "pcn3",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(TRE_TP.TPD_BENEFIT_GROUP_ID,'[,^<>|]','')),1,15) "benefit_group_id3",
				tre_plan.tpld_plan_code "plan_code3",
				TRE_PLAN.TPLD_PLAN_NAME "plan_name3",
				DECODE(tre_tp.tpd_relationship_num
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code3",
				DECODE(TRE_TP.TPD_USE_FOR_COB,'Y',1,0) "cob3",
        
				FOUR_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num4",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FOUR_TP.TPD_CARDHOLDER_ID,'[,^<>|]','')),1,20) "cardholder_id4",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FOUR_PLAN.BIN,'[,^<>|]','')),1,6) "bin4",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FOUR_PLAN.PCN,'[,^<>|]','')),1,10) "pcn4",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FOUR_TP.TPD_BENEFIT_GROUP_ID,'[,^<>|]','')),1,15) "benefit_group_id4",
				FOUR_PLAN.TPLD_PLAN_CODE "plan_code4",
				FOUR_PLAN.TPLD_PLAN_NAME "plan_name4",
				DECODE(FOUR_tp.tpd_relationship_num
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code4",
				DECODE(FOUR_TP.TPD_USE_FOR_COB,'Y',1,0) "cob4",
        
				FIVE_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num5",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIVE_TP.TPD_CARDHOLDER_ID,'[,^<>|]','')),1,20) "cardholder_id5",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIVE_PLAN.BIN,'[,^<>|]','')),1,6) "bin5",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIVE_PLAN.PCN,'[,^<>|]','')),1,10) "pcn5",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(FIVE_TP.TPD_BENEFIT_GROUP_ID,'[,^<>|]','')),1,15) "benefit_group_id5",
				FIVE_PLAN.TPLD_PLAN_CODE "plan_code5",
				FIVE_PLAN.TPLD_PLAN_NAME "plan_name5",
				DECODE(FIVE_tp.tpd_relationship_num
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code5",
				DECODE(FIVE_TP.TPD_USE_FOR_COB,'Y',1,0) "cob5",       

				SIX_TP.TPD_CARDHOLDER_PATIENT_NUM "cardholder_patient_num6",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(SIX_TP.TPD_CARDHOLDER_ID,'[,^<>|]','')),1,20) "cardholder_id6",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(SIX_PLAN.BIN,'[,^<>|]','')),1,6) "bin6",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(SIX_PLAN.PCN,'[,^<>|]','')),1,10) "pcn6",
				Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(SIX_TP.TPD_BENEFIT_GROUP_ID,'[,^<>|]','')),1,15) "benefit_group_id6",
				SIX_PLAN.TPLD_PLAN_CODE "plan_code6",
				SIX_PLAN.TPLD_PLAN_NAME "plan_name6",
				DECODE(SIX_tp.tpd_relationship_num
									,1,'1=Cardholder'
									,2,'2=Spouse'
									,3,'3=Child'
									,4,'4=Other'
									,0,'0=Not Specified'
									,NULL) "relationship_code6",
				DECODE(SIX_TP.TPD_USE_FOR_COB,'Y',1,0) "cob6"

		FROM TREXONE_DW_DATA.PATIENT PATIENT

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_DEMOGRAPHIC PATIENTDEMO
		ON patientdemo.PATIENT_DEMOGRAPHIC_KEY = patient.PATIENT_DEMOGRAPHIC_KEY

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_Option PATIENTOPT
		ON patientopt.PATIENT_OPTION_KEY = patient.PATIENT_OPTION_KEY

		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT FIRST_TP
		  ON FIRST_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND FIRST_TP.TPD_IS_ACTIVE = 'Y'
		  AND FIRST_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND FIRST_TP.TPD_BILLING_ORDER = 1
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN FIRST_PLAN
		  ON FIRST_PLAN.TPLD_PLAN_NUM = FIRST_TP.TPD_TP_PLAN_NUM

		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT SEC_TP
		  ON SEC_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND SEC_TP.TPD_IS_ACTIVE = 'Y'
		  AND SEC_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND SEC_TP.TPD_BILLING_ORDER = 2
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN SEC_PLAN
		  ON SEC_PLAN.TPLD_PLAN_NUM = SEC_TP.TPD_TP_PLAN_NUM
  
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT TRE_TP
		  ON TRE_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND TRE_TP.TPD_IS_ACTIVE = 'Y'
		  AND TRE_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND TRE_TP.TPD_BILLING_ORDER = 3
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN TRE_PLAN
		  ON TRE_PLAN.TPLD_PLAN_NUM = TRE_TP.TPD_TP_PLAN_NUM

		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT FOUR_TP
		  ON FOUR_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND FOUR_TP.TPD_IS_ACTIVE = 'Y'
		  AND FOUR_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND FOUR_TP.TPD_BILLING_ORDER = 4
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN FOUR_PLAN
		  ON FOUR_PLAN.TPLD_PLAN_NUM = FOUR_TP.TPD_TP_PLAN_NUM

		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT FIVE_TP
		  ON FIVE_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND FIVE_TP.TPD_IS_ACTIVE = 'Y'
		  AND FIVE_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND FIVE_TP.TPD_BILLING_ORDER = 5
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN FIVE_PLAN
		  ON FIVE_PLAN.TPLD_PLAN_NUM = FIVE_TP.TPD_TP_PLAN_NUM

		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PATIENT SIX_TP
		  ON SIX_TP.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		  AND SIX_TP.TPD_IS_ACTIVE = 'Y'
		  AND SIX_TP.TPD_EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		  AND SIX_TP.TPD_BILLING_ORDER = 6
		LEFT OUTER JOIN TREXONE_DW_DATA.TP_PLAN SIX_PLAN
		  ON SIX_PLAN.TPLD_PLAN_NUM = SIX_TP.TPD_TP_PLAN_NUM
  
		LEFT OUTER JOIN TREXONE_AUD_DATA.PATIENT AUDPATIENT
		ON AUDPATIENT.PATIENT_NUM = PATIENT.PD_PATIENT_NUM
		AND TRUNC(AUDPATIENT.EFF_END_DATE) = TO_DATE('12/31/2999','MM/DD/YYYY')

		LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY DWFACILITY
		ON AUDPATIENT.CREATE_FACILITY_NUM = DWFACILITY.FD_FACILITY_NUM

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_PHONE PHONE_CELL
		ON PHONE_CELL.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		AND PHONE_CELL.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		AND PHONE_CELL.PAD_KEY = 0
		AND PHONE_CELL.PATP_PHONE_USAGE = 16

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_PHONE PHONE_ALTONE
		ON PHONE_ALTONE.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		AND PHONE_ALTONE.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		AND PHONE_ALTONE.PAD_KEY = 0
		AND PHONE_ALTONE.PATP_PHONE_USAGE = 32

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_PHONE PHONE_ALTTWO
		ON PHONE_ALTTWO.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		AND PHONE_ALTTWO.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		AND PHONE_ALTTWO.PAD_KEY = 0
		AND PHONE_ALTTWO.PATP_PHONE_USAGE = 8

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_PHONE PHONE_ALTTHREE
		ON PHONE_ALTTHREE.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		AND PHONE_ALTTHREE.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')
		AND PHONE_ALTTHREE.PAD_KEY = 0
		AND PHONE_ALTTHREE.PATP_PHONE_USAGE = 64

		LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_MEDICAL_BRDG PMB
		ON PMB.PD_PATIENT_KEY = PATIENT.PD_PATIENT_KEY
		AND PMB.PMB_STATUS = 'A'
		--AND ROWNUM=1

		LEFT OUTER JOIN TREXONE_DW_DATA.MEDICAL_CONDITION MEDCOND
		ON MEDCOND.MCD_KEY = pmb.MCD_KEY


		WHERE PATIENT.PD_CURRENT_IND = '1'
     )
WHERE "last_update" = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
   OR :RunDate = To_Date('20200101','YYYYMMDD')
--When RunDate is 1/1/2020 then the whole view will return.