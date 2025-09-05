SELECT PD_PATIENT_NUM AS PATIENTNUMBER, DECODE(0, NVL(PREFERRED_PHONE, 0), PHONE_NUM, PREFERRED_PHONE) AS PHONENUMBER
  FROM (SELECT STEP2.*
          FROM (SELECT STEP1.*
                     , PREFERRED.PATP_AREA_CODE || PREFERRED.PATP_PHONE_NUMBER AS PREFERRED_PHONE 
                  FROM (SELECT PA.PHONE_NUM
                             , P.PD_PATIENT_KEY
                             , P.PD_CONTACT_BY_CODE
                             , P.PD_PATIENT_NUM
                          FROM {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT P, {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PA
                         WHERE P.PD_PATIENT_NUM = PA.PATIENT_NUM 
                           AND P.PD_PATIENT_NUM IN :PATIENTNUMLIST) STEP1
                  LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_PHONE PREFERRED ON 
                       (STEP1.PD_PATIENT_KEY = PREFERRED.PD_PATIENT_KEY 
                        AND PREFERRED.EFF_END_DATE = TO_DATE('2999-12-31', 'YYYY-MM-DD')
                        AND PREFERRED.PATP_PHONE_USAGE =
                            (CASE STEP1.PD_CONTACT_BY_CODE
                                  WHEN '1' THEN 1    -- HOME 1
                                  WHEN '3' THEN 1    -- HOME 2
                                  WHEN '5' THEN 1    -- WORK
                                  WHEN '7' THEN 1    -- OTHER
                                  WHEN '9' THEN 2    -- FAX (HOME 1)
                                  WHEN '8' THEN 16   -- CELL
                                  WHEN '10' THEN 32  -- ALT 1
                                  WHEN '11' THEN 8   -- ALT 2
                                  WHEN '18' THEN 64  -- ALT 3
                             END) 
                        AND PREFERRED.PATP_USAGE =
                            (CASE STEP1.PD_CONTACT_BY_CODE
                                  WHEN '1' THEN 1    -- HOME 1
                                  WHEN '3' THEN 2    -- HOME 2
                                  WHEN '5' THEN 4    -- WORK
                                  WHEN '7' THEN 8    -- OTHER
                                  WHEN '9' THEN 1    -- FAX (HOME 1)
                                  WHEN '8' THEN 0    -- CELL
                                  WHEN '10' THEN 0   -- ALT 1
                                  WHEN '11' THEN 0   -- ALT 2
                                  WHEN '18' THEN 0   -- ALT 3
                             END))) STEP2
       ) STEP3