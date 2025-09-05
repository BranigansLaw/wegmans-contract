{call sp_agent_detail(
     (EXTEND(RunFor - 1, YEAR TO DAY) || ' ' || (DATETIME (04:00:00) HOUR TO SECOND))
     ,(EXTEND(RunFor, YEAR TO DAY) || ' ' || (DATETIME (03:59:59) HOUR TO SECOND))
     ,'0'
     ,null
     ,null
     ,null
     ,'Pharmacy Core,Pharmacy Mail Specialty'
     ,null
     ,'CCX\154872'
)}