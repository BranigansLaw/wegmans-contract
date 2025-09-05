SELECT * 
FROM (
		SELECT UNIQUE
			prescrib.PRS_PRESCRIBER_KEY prescriber_key,
			prescrib.PRS_PRESCRIBER_NUM prescriber_num,
			prescrib.PRS_ACTIVE_PRESCRIBER_NUM active_prescriber_num,
			NVL(prescrib.PRS_TITLE_ABBR,prescrib.PRS_ESV_TITLE_ABBR) title_abbr,
			NVL(prescrib.PRS_FIRST_NAME,prescrib.PRS_ESV_FIRST_NAME) first_name,
			NVL(prescrib.PRS_MIDDLE_NAME,prescrib.PRS_ESV_MIDDLE_NAME) middle_name,
			NVL(prescrib.PRS_LAST_NAME,prescrib.PRS_ESV_LAST_NAME) last_name,
			NVL(prescrib.PRS_NAME_SUFFIX_ABBR,prescrib.PRS_ESV_NAME_SUFFIX_ABBR) suffix_abbr,
			prescrib.PRS_GENDER_CODE gender_code,
			prescrib.PRS_STATUS status,
			NVL(prescrib.PRS_PROFESSIONAL_ACTIVITY,prescrib.PRS_ESV_PROFESSIONAL_ACTIVITY) ama_activity,
			DECODE(prescrib.PRS_INACTIVITY_CODE,
				1,'No RX Activity',
				2,'Merged',
				3,'Replaced',
				4,'License/DEA Revoked',
				5,'Retired',
				9,'Other',NULL) inactive_code,
			prescrib.PRS_PREF_GENERIC_IF_AVAIL pref_generic,
			prescrib.PRS_PREF_THERAPEUTIC_SUB pref_ther_sub,
			prescrib.SYS_USER_KEY create_user_key,
			CASE prescrib.PRS_SOURCE_CODE WHEN 'E' THEN 'ESV' ELSE 'MANUAL' END add_source,
			prescrib.PRS_SUPV_PRESCRIBER_NUM supv_prs_num,
			prescrib.PRS_PRESCRIBER_BASE_NUM uniq_prs_num,
			prescrib.PRS_ESV_PRESCRIBER_ID prescriber_id,
			prescrib.PRS_ESV_MATCHED_IND esv_match,
			prescrib.PRS_VALIDATED is_esv_valid,
			prescrib.PRS_CREATE_DATE create_date,
			NVL(prescrib.PRS_BIRTH_DATE,prescrib.PRS_ESV_BIRTH_DATE) birth_date,
			NVL(prescrib.PRS_DECEASED_DATE,prescrib.PRS_ESV_DECEASED_DATE) deceased_date,
			prescrib.PRS_INACTIVE_STATUS_DATE inactive_date,
			prescrib.PRS_IS_PPI ppi_enabled,
			npi.NPI npi,
			NVL(npi.USE_FOR_BILLING,'N') npi_billing,
			npi.EXPIRE_DATE npi_expire_date,
			statelic.STATEID state_lic_num,
			NVL(statelic.USE_FOR_BILLING,'N') state_lic_billing,
			statelic.STATE license_state,
			statelic.EXPIRE_DATE state_lic_expire_date,
			dea.DEA_NUM dea_num,
			NVL(dea.USE_FOR_BILLING,'N') dea_billing,
			dea.EXPIRE_DATE dea_expire_date,
			medicaid.MEDICAID_NUM medicaid_num,
			fedid.FEDTAXID_NUM fedtaxid_num,
			stateissue.STATEISSUE_NUM stateissue_num,
			narcdea.NARCDEA_NUM narcdea_num,
			ncpdp.NCPDP_NUM ncpdp_num,
			TO_NUMBER(SUBSTR(TO_CHAR(prescrib.H_LEVEL),1,8)) last_update       
            
		FROM TREXONE_DW_DATA.PRESCRIBER prescrib

		--PIT_KEY
		--509 - National Provider Identifier (NPI)
		--511 - State License
		--515 - Drug Enforcement Administration (DEA) Number-
		--504 - Medicaid
		--505 - UPIN (Unique Physician/Practitioner Identification Number)-Replaced
		--510 - Federal Tax ID
		--517 - State Issued
		--518 - Narcotic Addiction DEA Number
		--506 - NCPDP Provider ID-


		--Start - Join to get NPI info
		LEFT OUTER JOIN
		(SELECT pid_npi.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_npi.PID_ID_TEXT AS NPI,
				pid_npi.USE_FOR_BILLING AS USE_FOR_BILLING,
				pid_npi.PID_EE_DT AS EXPIRE_DATE

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_npi
		WHERE pid_npi.PID_STATUS = 'A'
		AND pid_npi.PIT_KEY = 509

		ORDER BY pid_npi.PID_EE_DT desc nulls last) npi
		ON prescrib.PRS_PRESCRIBER_KEY = npi.PRESCRIBER_KEY
		--End - Join to get NPI info

		--Start - Join to get StateID License info
		LEFT OUTER JOIN
		(SELECT pid_stateid.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_stateid.PID_ID_TEXT AS STATEID,
				pid_stateid.USE_FOR_BILLING AS USE_FOR_BILLING,
				pid_stateid.PID_EE_DT AS EXPIRE_DATE,
				pid_stateid.PID_STATE_CODE AS STATE

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_stateid
		WHERE pid_stateid.PID_STATUS = 'A'
		AND pid_stateid.PIT_KEY = 511

		ORDER BY pid_stateid.PID_EE_DT desc nulls last) statelic
		ON prescrib.PRS_PRESCRIBER_KEY = statelic.PRESCRIBER_KEY
		--End - Join to get StateID License info

		--Start - Join to get DEA info
		LEFT OUTER JOIN
		(SELECT pid_dea.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_dea.PID_ID_TEXT AS DEA_NUM,
				pid_dea.USE_FOR_BILLING AS USE_FOR_BILLING,
				pid_dea.PID_EE_DT AS EXPIRE_DATE

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_dea
		WHERE pid_dea.PID_STATUS = 'A'
		AND pid_dea.PIT_KEY = 515

		ORDER BY pid_dea.PID_EE_DT desc nulls last) dea
		ON prescrib.PRS_PRESCRIBER_KEY = dea.PRESCRIBER_KEY
		--End - Join to get DEA info

		--Start - Join to get Medicaid info
		LEFT OUTER JOIN
		(SELECT pid_medicaid.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_medicaid.PID_ID_TEXT AS MEDICAID_NUM

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_medicaid
		WHERE pid_medicaid.PID_STATUS = 'A'
		AND pid_medicaid.PIT_KEY = 504

		ORDER BY pid_medicaid.PID_EE_DT desc nulls last) medicaid
		ON prescrib.PRS_PRESCRIBER_KEY = medicaid.PRESCRIBER_KEY
		--End - Join to get Medicaid info


		--Start - Join to get FedID info
		LEFT OUTER JOIN
		(SELECT pid_fedid.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_fedid.PID_ID_TEXT AS FEDTAXID_NUM

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_fedid
		WHERE pid_fedid.PID_STATUS = 'A'
		AND pid_fedid.PIT_KEY = 510

		ORDER BY pid_fedid.PID_EE_DT desc nulls last) fedid
		ON prescrib.PRS_PRESCRIBER_KEY = fedid.PRESCRIBER_KEY
		--End - Join to get FedID info

		--Start - Join to get StateIssue info
		LEFT OUTER JOIN
		(SELECT pid_stateiss.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_stateiss.PID_ID_TEXT AS STATEISSUE_NUM

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_stateiss
		WHERE pid_stateiss.PID_STATUS = 'A'
		AND pid_stateiss.PIT_KEY = 517

		ORDER BY pid_stateiss.PID_EE_DT desc nulls last) stateissue
		ON prescrib.PRS_PRESCRIBER_KEY = stateissue.PRESCRIBER_KEY
		--End - Join to get StateIssue info

		--Start - Join to get Narcotic DEA info
		LEFT OUTER JOIN
		(SELECT pid_narcdea.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_narcdea.PID_ID_TEXT AS NARCDEA_NUM

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_narcdea
		WHERE pid_narcdea.PID_STATUS = 'A'
		AND pid_narcdea.PIT_KEY = 518

		ORDER BY pid_narcdea.PID_EE_DT desc nulls last) narcdea
		ON prescrib.PRS_PRESCRIBER_KEY = narcdea.PRESCRIBER_KEY
		--End - Join to get Narcotic DEA info

		--Start - Join to get NCPDP info
		LEFT OUTER JOIN
		(SELECT pid_ncpdp.PRS_PRESCRIBER_KEY AS PRESCRIBER_KEY,
				pid_ncpdp.PID_ID_TEXT AS NCPDP_NUM

		FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_ncpdp
		WHERE pid_ncpdp.PID_STATUS = 'A'
		AND pid_ncpdp.PIT_KEY = 506

		ORDER BY pid_ncpdp.PID_EE_DT desc nulls last) ncpdp
		ON prescrib.PRS_PRESCRIBER_KEY = ncpdp.PRESCRIBER_KEY
		--End - Join to get NCPDP info
     )
WHERE last_update = To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) OR prescriber_key = 6915303