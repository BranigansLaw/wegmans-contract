SELECT
	Fac.fd_ncpdp_provider_id AS NcpdpNumber,
	Fac.fd_address1 AS PharmacyAddress1,
	Fac.fd_city AS PharmacyCity,
	Fac.fd_state_code AS PharmacyState,
	Fac.fd_zipcode AS PharmacyZip,
	Fac.fd_phone1 AS PharmacyPhoneNumber
FROM TREXONE_DW_DATA.Facility Fac
WHERE Fac.fd_is_active = 'Y'
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
)