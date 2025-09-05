WITH patient_keys AS (
    SELECT /*+ cardinality(p,1) leading (p) index (p IN_PATIENT_02) */
         Distinct 
         p.PD_PATIENT_KEY
        --,p.PD_PATIENT_NUM
    FROM TREXONE_DW_DATA.PATIENT p
    WHERE p.PD_PATIENT_NUM = :PatientNum
), patient_prescriptions AS (
    SELECT /*+ cardinality(ff,1) leading (ff) index (ff IB_FILL_FACT_38) */
         Distinct 
         pk.PD_PATIENT_KEY
        --,pk.PD_PATIENT_NUM
        ,ff.RX_RECORD_NUM
        ,ff.RX_FILL_SEQ
    FROM patient_keys pk
         LEFT OUTER JOIN TREXONE_DW_DATA.FILL_FACT ff
            ON  ff.PD_PATIENT_KEY = pk.PD_PATIENT_KEY
), patients_accessed AS (
    SELECT
         sea.DATE_KEY AS Login_Date
        ,un.SYS_USER_LOGIN_NAME AS User_Login_Name
        ,un.SYS_USER_FNAME AS User_First_Name 
        ,un.SYS_USER_LNAME AS User_Last_Name
        ,CASE WHEN ur.ROLE_SET LIKE '%super%' THEN 'Super User'
              WHEN ur.ROLE_SET LIKE '%rph%' THEN 'RPh'
              WHEN ur.ROLE_SET LIKE '%intern%' THEN 'Intern'
              WHEN ur.ROLE_SET LIKE '%tech%' THEN 'Tech'
              WHEN ur.ROLE_SET LIKE '%extern%' THEN 'Extern'
              WHEN ur.ROLE_SET LIKE '%stl%' THEN 'STL'      
              ELSE 'Other'
         END AS User_Job_Role 
        ,fac.FD_FACILITY_ID AS Store_Nbr
        ,ListAgg(Distinct To_Char(sea.SAE_START_TIME,'HH24:MI:SS'),',')
         Within Group (ORDER BY sea.SAE_START_TIME) AS Login_Times_CSV
    FROM TREXONE_DW_DATA.SYSTEM_EVENT_AUDIT sea
         INNER JOIN patient_prescriptions pp
            ON (pp.PD_PATIENT_KEY = sea.PATIENT_KEY
                OR 
                  (pp.RX_RECORD_NUM = sea.SAE_RX_RECORD_NUM AND
                   pp.RX_FILL_SEQ = sea.SAE_RX_FILL_SEQ
                  )
               )
         left outer JOIN TREXONE_DW_DATA.FACILITY fac
            ON fac.FD_FACILITY_KEY = sea.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.SYS_USER un
            ON un.SYS_USER_KEY = sea.SYS_USER_KEY
         INNER JOIN TREXONE_AUD_DATA.SYSTEM_USER ur
            ON ur.SYSTEM_USER_NUM = un.SYS_USER_NUM
           AND ur.H_LEVEL = un.H_LEVEL
    WHERE un.SYS_USER_LOGIN_NAME NOT IN ('SYSTEM','SCHEDULEDTASK')
      AND sea.SAE_START_TIME BETWEEN
          To_Date(:StartDate_YYYYMMDD,'YYYYMMDD') AND
          To_Date(:EndDate_YYYYMMDD,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND
    GROUP BY
         sea.DATE_KEY
        ,un.SYS_USER_LOGIN_NAME
        ,un.SYS_USER_FNAME
        ,un.SYS_USER_LNAME
        ,CASE WHEN ur.ROLE_SET LIKE '%super%' THEN 'Super User'
              WHEN ur.ROLE_SET LIKE '%rph%' THEN 'RPh'
              WHEN ur.ROLE_SET LIKE '%intern%' THEN 'Intern'
              WHEN ur.ROLE_SET LIKE '%tech%' THEN 'Tech'
              WHEN ur.ROLE_SET LIKE '%extern%' THEN 'Extern'
              WHEN ur.ROLE_SET LIKE '%stl%' THEN 'STL'      
              ELSE 'Other'
         END
        ,fac.FD_FACILITY_ID
        ,sea.FD_FACILITY_KEY
    ORDER BY 1, 2, 3
)
SELECT
     pa.User_Login_Name
    ,pa.User_First_Name 
    ,pa.User_Last_Name 
    ,pa.User_Job_Role
    ,pa.Store_Nbr
    ,pa.Login_Date
    ,pa.Login_Times_CSV
FROM patients_accessed pa
ORDER BY 3, 4, 5
