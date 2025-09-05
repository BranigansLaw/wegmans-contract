{call sp_agent_state_detail(
     (EXTEND(TODAY - 1, YEAR TO DAY) || ' ' || (DATETIME (04:00:00) HOUR TO SECOND))
     ,(EXTEND(TODAY, YEAR TO DAY) || ' ' || (DATETIME (03:59:59) HOUR TO SECOND))
     ,'0'
     ,null
     ,null
     ,null
     ,'Pharmacy Core,Pharmacy Mail Specialty'
     ,'CCX\154872'
)}