{call sp_call_csq_agent(
	(EXTEND(RunFor - 1, YEAR TO DAY) || ' ' || (DATETIME (04:00:00) HOUR TO SECOND))
    ,(EXTEND(RunFor, YEAR TO DAY) || ' ' || (DATETIME (03:59:59) HOUR TO SECOND))
	,'0'
	,null
	,null
	,null
	,null
	,null
	,null
	,null
	,'Buffalo Queue,Covid Vaccine Queue,Managed Care Queue,Maryland Queue,Massachusetts Queue,Medicare Support Queue,New Jersey Queue,Pennsylvania Queue,Pharmacy Queue,RX EPAO Queue,RX Excellus Queue,RX HUB Queue,RX Independent Health Queue,RX Janssen Select Queue,RX Mail Support Queue,RX Pharmacist Queue,RX Specialty Pharmacist Queue,RX Specialty Support Queue,RX Trial Card Enrollment Queue,RX Web Support Queue,Roch High Volume Queue,Roch Low Volume Queue,Roch Med Volume Queue,Rochester Queue,Smartfill Queue,Southern Tier Queue,Syracuse Queue,Virginia Queue'
	,null
	,null
	,'CCX\154872'
)}