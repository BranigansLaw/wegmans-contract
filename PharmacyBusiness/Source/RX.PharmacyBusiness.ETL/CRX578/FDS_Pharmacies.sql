SELECT
     f.fd_ncpdp_provider_id AS ncpdpId
    ,fid.FD_VALUE AS npi
    ,f.FD_FACILITY_NAME AS pharmacyName
    ,f.FD_ADDRESS1 AS addressline1
    ,f.FD_ADDRESS2 AS addressline2
    ,f.FD_CITY AS City
    ,f.FD_STATE_CODE AS state
    ,f.FD_ZIPCODE AS zipcode
    ,f.FD_PHONE1 AS storephone
    ,f.FD_FAX_NUMBER AS faxphone    
    ,NULL AS banner
FROM TREXONE_DW_DATA.Facility f
     INNER JOIN TREXONE_DW_DATA.Facility_ID fid
        ON fid.fd_facility_num = f.fd_facility_num
       AND fid.fd_type = 'F08'
       AND f.FD_IS_ACTIVE = 'Y'
       AND f.FD_IS_PPI_ENABLED = 'Y'
ORDER BY f.FD_FACILITY_NAME
