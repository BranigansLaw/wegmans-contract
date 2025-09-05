SELECT /*+use_invisible_indexes*/
	TRUNC(rxerp.datestamp) "rxerp_change_date",--1
        RXERP.PATIENT_NUM "patient_num",--2
        facility.FD_FACILITY_ID "store_num",--3
        REGEXP_REPLACE(RXERP.RX_NUMBER, '[^0-9]+', '') "rx_num",--4
        RXERP.RX_NUMBER "rx_num_txt",--5
        drug.drug_ndc "ndcwo",--6
	REPLACE(DRUG.DRUG_LABEL_NAME,',','') "drug_name",--7
        rxerp.DAYS_SUPPLY "days_supply",--8
        RXERP.refills_authorized "refills_authorized",--9
        rxerp.LAST_FILL_DATE "last_fill_date",--10
        
        rxerp.ERP_ORIGINAL_TARGET_DATE "erp_orig_target_date",--11
        rxerp.ERP_TARGET_DATE "erp_target_date",--12
        
        DECODE(rxerp.ERP_ENROLLMENT_CODE_NUM,
                1,'1-Has Not Been Asked',
                2,'2-Declined',
                3,'3-Enrolled',
                4,'4-Unenrolled',
                5,'5-Ineligible',
		6,'6-Onetime',
                NULLIF(rxerp.ERP_ENROLLMENT_CODE_NUM||'-','-')) "erp_enroll_status",--13
        rxerp.ERP_ENROLLMENT_STATUS_DATE "erp_enroll_status_date",--14
        
        DECODE(rxerp.ERP_ENROLLMENT_REASON_CODE_NUM,
                1,'1-Auto Enroll',
                2,'2-Patient Request',
                3,'3-Return To Stock',
                4,'4-Rx Transfer',
                NULLIF(rxerp.ERP_ENROLLMENT_REASON_CODE_NUM||'-','-')) "erp_enroll_reason",--15
                
        DECODE(rxerp.ERP_EXCLUSION_REASON_CODE_NUM,
                1,'1-Not Active',
                2,'2-Maximum Days Supply',
                3,'3-Minimum Days Supply',
                4,'4-Product Not Eligible',
                5,'5-Sold Date',
                6,'6-Refills Allowed Check',
                7,'7-Third Party Not Eligible',
                8,'8-Facility not participating in ERP',
                9,'9-MedSync Rx',
                10,'10-Prescriber Inactive Reason',
                11,'11-Contact Event Found',
                12,'12-Patient Not Enrolled',
                13,'13-Patient Consent Not Provided',
            NULLIF(rxerp.ERP_EXCLUSION_REASON_CODE_NUM||'-','-')) "erp_exclude_reason",--16
            deliverymethod.DM_DELIVERY_METHOD_DESC "erp_delivery_method",--17
            rxerp.ERP_DELIVERY_METHOD_PERSIST "erp_delivery_method_persist",--18
            rxerp.IS_CONSENTED_TO_FILL "is_consented_to_fill",--19
            rxerp.USER_CONSENTED_TO_FILL "user_consented_to_fill",--20
            rxerp.DATE_CONSENTED_TO_FILL "date_consented_to_fill",--21
            rxerp.eff_start_date as "eff_start_date",--22
--RX-ERP Data--
                DECODE(rxerp.H_TYPE,
                        'U','Update',
                        'I','Added',
                        'D','Removed',
                        rxerp.H_TYPE) "record_type",--23
                        
                rxerp.ERP_USER_MODIFIED_TARGET_DATE "erp_usermodified_target_date",--24
                
                CASE WHEN rxerp.ERP_UM_DATE_BY IS NULL
                THEN 'System'
                ELSE
                    CASE WHEN pterp.status IS NULL
                    THEN 'ERx User'
                    ELSE 'Patient'
                    END
                END as "erp_modified_usertype",--25
                
                NVL(pterp.PATIENT_NUM,rxerp.ERP_UM_DATE_BY) as "erp_modified_user",--26
                rxerp.ERP_UM_DATE_BY_TYPE "erp_usermodified_rectype",--27
                rxerp.ERP_UM_DATE_MODIFIED_ON "erp_usermodified_ondate",--28
                rxerp.NUM_UPDATES "rec_updates",--29
--RX-ERP Data--
                
                
                
--PATIENT-ERP Data--
                CASE patient.ERP_ENROLLMENT_STATUS_CODE WHEN 3 THEN
                    CASE patient.REQUIRE_CONSENT_TO_FILL WHEN 'N' THEN 'Autofill' ELSE 'Consent Required' END
                ELSE 'Not Enrolled' END
                AS "pt_autofill_enroll_status",--30
        
            DECODE(PATIENT.ERP_ENROLLMENT_STATUS_CODE,
		1,'1-Has Not Been Asked',
		2,'2-Declined',
		3,'3-Enrolled',
		4,'4-Unenrolled',
		5,'5-Ineligible',
		6,'6-Onetime',
		NULLIF(PATIENT.ERP_ENROLLMENT_STATUS_CODE||'-','-')) "pt_erp_enroll_status",--31
            patient.ERP_ENROLLMENT_STATUS_DATE "pt_erp_enroll_date",--32
            PATIENT.IS_ERP_AUTO_ENROLLED "pt_autoenroll_future_rxs",--33
            deliverymethod.DM_DELIVERY_METHOD_DESC "pt_preferred_delivery_method",--34
            patient.ERP_PREFERS_MAIL_DELIVERY "pt_prefers_mail_delivery",--35
            patient.ERP_PAYMENT_METHOD "pt_erp_payment_method",--36
            patient.REQUIRE_CONSENT_TO_FILL "pt_req_consent_tofill",--37
            patient.require_consent_to_erp_process "pt_req_consent_toerp",--38
            patient.external_patient_id "pt_external_id",--39
--PATIENT-ERP Data--
     
--RX/Drug Data--
            rxerp.PROHIBIT_RENEWAL_REQUEST "prohibit_renewal_request",--40
            rxerp.CSR_DIAGNOSIS_CODE_NC_CDT "csr_diagnosis_code",--41
            drug.drug_dosage_form "drug_form",--42
            rxerp.TOTAL_AUTHORIZED_QTY "qty_authorized",--43
            RXERP.refill_qty "refill_qty",--44
            --CAST(REPLACE(REPLACE(REPLACE(SUBSTR(rxerp.DIRECTIONS,1,500),CHR(30),''),CHR(13),''),CHR(10),'') AS varchar(500)) "directions",--45
            rxerp.EXPIRY_DATE "expire_date",--46
            RXERP.reassigned_rx_num "reassigned_rx_num",--47
            TRUNC(RXERP.create_date) "create_date",--48
            rxerp.create_user "create_user",--49
            TO_CHAR(rxerp.RX_ORIGINATION_NUM)||'-'||origin.DESCRIPTION "rx_origin_code",--50
            rxstat.description "rx_status",--51
            RXERP.RX_RECORD_NUM "rx_record_num",--52
            RXERP.first_fill_qty "first_fill_qty",--53
            RXERP.prescribed_date "date_prescribed",--54
            orig_drug.drug_ndc "original_ndcwo",--55
            orig_drug.drug_label_name "original_drug_name",--56
            RXERP.original_product_qty "original_qty",--57
            rxerp.TRIPLICATE_SERIAL_NUM "serial",--58
            rxerp.DAW_CODE "daw_code",--59
            rxerp.SELECTED_UPC "selected_upc",--60
--RX/Drug Data--

--MISC RX Data--
        rxerp.ACKNOWLEDGE_TYPE_CODE "ack_code",--61
        rxerp.IS_LETTER_ON_FILE "letter_on_file",--62
        TRIM(SYSUSER.SYS_USER_LOGIN_NAME) "emp_login_name",--63
        rxerp.LAST_ACK_CHANGE_DATE "last_ack_change_date",--64
        TRIM(ACK_SYSUSER.SYS_USER_LOGIN_NAME) "last_ack_change_user",--65
        ACK_SYSUSER.SYS_USER_FNAME||' '||ACK_SYSUSER.SYS_USER_LNAME "last_ack_change_username",--66
        rxerp.ORIGINAL_FILL_DATE "orig_fill_date",--67
        rxerp.TOTAL_DAILY_DOSE "total_daily_dose",--68
        rxerp.TOTAL_QTY_DISPENSED "total_qty_disp",--69
        rxerp.TOTAL_QTY_TRANSFERRED "total_qty_xferred",--70
        rxerp.NUMBER_OF_LEGAL_FILLS "legal_fills",--71
        rxerp.LATEST_FILL "latest_fill",--72
        rxerp.LATEST_FILL_DUR "latest_fill_dur",--73
        rxerp.LATEST_FILL_NON_IN_PROCESS "latest_fill_non_inprocess",--74
        rxerp.LATEST_CANCELLED_FILL "latest_fill_canceled",--75
        rxerp.LAST_DISPENSED_FILL "last_disp_fill",--76
        rxerp.LAST_FILL_FOR_REFILL "last_fill_refill",--77
        rxerp.LAST_FILL_RELEASED "last_fill_released",--78
        rxerp.LAST_SOLD_FILL_NON_COMPLETION "last_sold_fill",--79
        
        rxerp.PROFILE_INCLUSION_DATE "profile_include_date",--80
        
        rxerp.FIRST_FILL "first_fill",--81
        rxerp.FIRST_REFILL "first_refill",--82
        rxerp.EARLIEST_FILL_DATE "earliest_fill_date",--83
        rxerp.PREVIOUS_FILL_RELEASED "prev_fill_released",--84
        rxerp.SYNC_DATE "sync_date",--85
        rxerp.IS_LINKED_FROM_RX_PROFILE "is_linked",--86
        rxerp.CANCEL_REASON_CODE "cancel_reason",--87
        rxerp.PRESCRIBER_NUM "prescriber_num",--88
        rxerp.PRESCRIBER_ADDRESS_NUM "prescriber_address_num",--89
        rxerp.PRESCRIBER_ORDER_NUMBER "prescriber_order_num",--90
        rxerp.IS_EPCS "is_epcs",--91
        rxerp.IS_RRR_DENIED "rrr_denied"--92
--MISC RX Data--

FROM TREXONE_AUD_DATA.RX RXERP

LEFT OUTER JOIN TREXONE_AUD_DATA.PATIENT patient
    ON rxerp.PATIENT_NUM = patient.PATIENT_NUM
    --AND (SYSDATE) BETWEEN patient.EFF_START_DATE AND patient.EFF_END_DATE
    AND patient.EFF_END_DATE = TO_DATE('12312999','MMDDYYYY')

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER ptum
    ON rxerp.ERP_UM_DATE_BY = TO_CHAR(ptum.SYS_USER_NUM)
LEFT OUTER JOIN TREXONE_AUD_DATA.PATIENT pterp
    ON ptum.SYS_USER_LOGIN_NAME = To_CHAR(pterp.PATIENT_NUM)
    --AND (SYSDATE) BETWEEN pterp.EFF_START_DATE AND pterp.EFF_END_DATE
    AND pterp.EFF_END_DATE = TO_DATE('12312999','MMDDYYYY')

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER ACK_SYSUSER
    ON rxerp.LAST_ACK_CHANGE_USER = ACK_SYSUSER.SYS_USER_NUM
LEFT OUTER JOIN TREXONE_AUD_DATA.RX_STATUS rxstat
    ON rxerp.rx_status_num = rxstat.rx_status_num
LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facility
    ON RXERP.FACILITY_NUM = Facility.FD_FACILITY_NUM
    --AND facility.FD_FACILITY_KEY in (530,531,532,533,502,534,535,536,537,51894,539,540,553,554,555,557,558,559,560,561,556,3659902,516015)
LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER SYSUSER
    ON rxerp.SYS_USER = SYSUSER.SYS_USER_NUM
LEFT OUTER JOIN TREXONE_DW_DATA.COURIER_SHIPMENT_TYPE shipmethod
    ON rxERP.ERP_DELIVERY_METHOD_NUM = shipmethod.SHIPPING_METHOD_NUM

LEFT OUTER JOIN TREXONE_AUD_DATA.RX_ORIGINATION origin
    ON rxerp.RX_ORIGINATION_NUM = origin.RX_ORIGINATION_NUM
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT orig_prod
    ON RXERP.ORIGINAL_PRODUCT_NUM = orig_prod.PRD_PRODUCT_NUM
        LEFT OUTER JOIN TREXONE_DW_DATA.DRUG orig_drug
            ON orig_prod.PRD_DRUG_NUM = orig_drug.DRUG_NUM

LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT prod
    ON RXERP.PRODUCT_NUM = prod.PRD_PRODUCT_NUM
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
    ON prod.PRD_DRUG_NUM = drug.DRUG_NUM
LEFT OUTER JOIN TREXONE_DW_DATA.DELIVERY_METHOD_TYPE deliverymethod
    ON rxerp.ERP_DELIVERY_METHOD_NUM = deliverymethod.DM_DELIVERY_METHOD

WHERE RXERP.EFF_START_DATE BETWEEN TRUNC(SYSDATE-1) AND TRUNC(SYSDATE-1)+17/24