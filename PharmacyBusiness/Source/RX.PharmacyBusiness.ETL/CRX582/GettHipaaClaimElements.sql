
SELECT
     --keys:
     FILL_STATE_CHG_TS AS ClaimDate
    ,FD_FACILITY_ID AS OriginatingStoreId
    ,RX_NUMBER AS PrescriptionNumber
    ,REFILL_NUM AS RefillNumber               
    ,split_billing_code AS ERxSplitFillNumber
    ,fill_state_code AS FillFactFillStateCode
                
     --hipaa fields:
    ,ADDRESS1 AS CardholderAddress1
    ,ADDRESS2 AS CardholderAddress2
    ,CITY AS CardholderCity
    ,BIRTH_DATE AS CardholderDateOfBirth
    ,PA_FIRST_NAME AS CardholderFirstName
    ,PA_LAST_NAME AS CardholderLastName
    ,PA_MIDDLE_NAME AS CardholderMiddleInitial
    ,PHONE_NUM AS CardholderPhoneNumber
    ,BIRTH_DATE AS OtherPayerDateOfBirth
    ,PA_FIRST_NAME AS OtherPayerFirstName
    ,PA_LAST_NAME AS OtherPayerLastName
    ,PA_MIDDLE_NAME AS OtherPayerMiddleInitial
    ,ADDRESS1 AS PatientAddress1
    ,ADDRESS2 AS PatientAddress2
    ,CITY AS PatientCity
    ,BIRTH_DATE AS PatientDateOfBirth
    ,P_FIRST_NAME AS PatientFirstName
    ,P_LAST_NAME AS PatientLastName
    ,P_MIDDLE_NAME AS PatientMiddleInitial
    ,PHONE_NUM AS PatientPhoneNumber
    ,PADR_ADDRESS1 AS PrescriberAddress1
    ,PADR_ADDRESS2 AS PrescriberAddress2
    ,PADR_CITY AS PrescriberCity
    ,PRESCRIBER_ID_DEA AS PrescriberDEA 
    ,PRS_FIRST_NAME AS PrescriberFirstName
    ,PRS_PRESCRIBER_KEY AS PrescriberId
    ,PRS_LAST_NAME AS PrescriberLastName
            
FROM Wegmans.THIRD_PARTY_CLAIMS_STAGING
WHERE run_date = :RunDate