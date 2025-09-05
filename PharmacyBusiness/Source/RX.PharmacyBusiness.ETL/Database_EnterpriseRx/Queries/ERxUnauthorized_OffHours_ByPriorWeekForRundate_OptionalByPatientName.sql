WITH login_times AS (
    SELECT
         sea.DATE_KEY AS Login_Date
        ,user_name.SYS_USER_LOGIN_NAME AS User_Login_Name
        ,user_name.SYS_USER_FNAME AS User_First_Name 
        ,user_name.SYS_USER_LNAME AS User_Last_Name
        ,CASE WHEN user_role.ROLE_SET LIKE '%super%' THEN 'Super User'
              WHEN user_role.ROLE_SET LIKE '%rph%' THEN 'RPh'
              WHEN user_role.ROLE_SET LIKE '%intern%' THEN 'Intern'
              WHEN user_role.ROLE_SET LIKE '%tech%' THEN 'Tech'
              WHEN user_role.ROLE_SET LIKE '%extern%' THEN 'Extern'
              WHEN user_role.ROLE_SET LIKE '%stl%' THEN 'STL'      
              ELSE 'Other'
         END AS User_Job_Role 
        ,fac.FD_FACILITY_ID AS Store_Nbr
        ,sea.FD_FACILITY_KEY
        ,ListAgg(Distinct To_Char(sea.SAE_START_TIME,'HH24:MI:SS'),',')
         Within Group (ORDER BY sea.SAE_START_TIME) AS Login_Times_CSV
    FROM TREXONE_DW_DATA.SYSTEM_EVENT_AUDIT sea
         INNER JOIN TREXONE_DW_DATA.FACILITY fac
            ON fac.FD_FACILITY_KEY = sea.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.SYS_USER user_name
            ON user_name.SYS_USER_KEY = sea.SYS_USER_KEY
         INNER JOIN TREXONE_AUD_DATA.SYSTEM_USER user_role
            ON user_role.SYSTEM_USER_NUM = user_name.SYS_USER_NUM
           AND user_role.H_LEVEL = user_name.H_LEVEL
    WHERE user_name.SYS_USER_LOGIN_NAME NOT IN ('SYSTEM','SCHEDULEDTASK')
      AND sea.SAE_AUDIT_NAME = 'com.techrx.security.module.RDACLoginModule.Login'
      AND sea.SAE_START_TIME BETWEEN
          Next_Day(Trunc(To_Date(:RunDate_YYYYMMDD,'YYYYMMDD')) - 14, 'sun') AND
          Next_Day(Trunc(To_Date(:RunDate_YYYYMMDD,'YYYYMMDD')) - 7, 'sat') + INTERVAL '23:59:59' HOUR TO SECOND
      AND (sea.TIME_KEY BETWEEN 0 AND 25200 --Between midnight and 7 AM
           OR
           sea.TIME_KEY BETWEEN 79200 AND 86400 --Between 10 PM and midnight
          )
    GROUP BY
         sea.DATE_KEY
        ,user_name.SYS_USER_LOGIN_NAME
        ,user_name.SYS_USER_FNAME
        ,user_name.SYS_USER_LNAME
        ,CASE WHEN user_role.ROLE_SET LIKE '%super%' THEN 'Super User'
              WHEN user_role.ROLE_SET LIKE '%rph%' THEN 'RPh'
              WHEN user_role.ROLE_SET LIKE '%intern%' THEN 'Intern'
              WHEN user_role.ROLE_SET LIKE '%tech%' THEN 'Tech'
              WHEN user_role.ROLE_SET LIKE '%extern%' THEN 'Extern'
              WHEN user_role.ROLE_SET LIKE '%stl%' THEN 'STL'      
              ELSE 'Other'
         END
        ,fac.FD_FACILITY_ID
        ,sea.FD_FACILITY_KEY
    ORDER BY 1, 2, 3
), prescriptions_accessed AS (
    SELECT Distinct 
         sea.DATE_KEY AS Login_Date
        ,user_name.SYS_USER_LOGIN_NAME AS User_Login_Name
        ,user_name.SYS_USER_FNAME AS User_First_Name 
        ,user_name.SYS_USER_LNAME AS User_Last_Name 
        ,fac.FD_FACILITY_ID AS Store_Nbr
        ,(SELECT Max(fwf.PD_PATIENT_KEY)
          FROM TREXONE_DW_DATA.FILL_WORKFLOW_FACT fwf
          WHERE fwf.FWFS_RX_RECORD_NUM = sea.SAE_RX_RECORD_NUM
            AND fwf.FWFS_RX_FILL_SEQ = sea.SAE_RX_FILL_SEQ
            AND fwf.FD_FACILITY_KEY = sea.FD_FACILITY_KEY
            AND fwf.FWFS_DT_OUT <= sysdate
         ) AS PD_PATIENT_KEY
        ,sea.PATIENT_KEY
    FROM TREXONE_DW_DATA.SYSTEM_EVENT_AUDIT sea
         INNER JOIN TREXONE_DW_DATA.FACILITY fac
            ON fac.FD_FACILITY_KEY = sea.FD_FACILITY_KEY
         INNER JOIN TREXONE_DW_DATA.SYS_USER user_name
            ON user_name.SYS_USER_KEY = sea.SYS_USER_KEY
         INNER JOIN TREXONE_AUD_DATA.SYSTEM_USER user_role
            ON user_role.SYSTEM_USER_NUM = user_name.SYS_USER_NUM
           AND user_role.H_LEVEL = user_name.H_LEVEL
    WHERE (sea.DATE_KEY, user_name.SYS_USER_LOGIN_NAME, fac.FD_FACILITY_ID) IN (
              SELECT Login_Date, Login_Name, Store_Nbr
              FROM login_times
          )
      AND sea.SAE_START_TIME BETWEEN
          Next_Day(Trunc(To_Date(:RunDate_YYYYMMDD,'YYYYMMDD')) - 14, 'sun') AND
          Next_Day(Trunc(To_Date(:RunDate_YYYYMMDD,'YYYYMMDD')) - 7, 'sat') + INTERVAL '23:59:59' HOUR TO SECOND
      AND (sea.TIME_KEY BETWEEN 0 AND 25200 --Between midnight and 7 AM
           OR
           sea.TIME_KEY BETWEEN 79200 AND 86400 --Between 10 PM and midnight
          )
    ORDER BY 1, 2, 3, 4
), patients_accessed AS (
    SELECT
         pa.Login_Date
        ,pa.User_Login_Name
        ,pa.User_First_Name 
        ,pa.User_Last_Name 
        ,pa.Store_Nbr
        ,ListAgg(Distinct p.PD_PATIENT_NUM,',')
         Within Group (ORDER BY p.PD_PATIENT_NUM) AS Patient_Nums_CSV
        ,ListAgg(Distinct p.PD_LAST_NAME,',')
         Within Group (ORDER BY p.PD_PATIENT_NUM) AS Patient_Names_CSV
    FROM prescriptions_accessed pa
         LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT p
            ON (p.PD_PATIENT_KEY = pa.PD_PATIENT_KEY OR
                p.PD_PATIENT_KEY = pa.PATIENT_KEY)
           --Bind variable for patient last name is optional.
           AND (:PatientLastName IS NULL OR Upper(:PatientLastName) = Upper(p.PD_LAST_NAME))
    GROUP BY
         pa.Login_Date
        ,pa.User_Login_Name
        ,pa.User_First_Name 
        ,pa.User_Last_Name
        ,pa.Store_Nbr
    ORDER BY 1, 2, 3
)
SELECT
     lt.User_Login_Name
    ,lt.User_First_Name 
    ,lt.User_Last_Name 
    ,lt.User_Job_Role
    ,lt.Store_Nbr
    ,lt.Login_Date
    ,lt.Login_Times_CSV
    ,pa.Patient_Nums_CSV
    ,pa.Patient_Names_CSV
FROM login_times lt
     LEFT OUTER JOIN patients_accessed pa
        ON pa.Login_Date = lt.Login_Date
       AND pa.User_Login_Name = lt.User_Login_Name
       AND pa.Store_Nbr = lt.Store_Nbr
WHERE :PatientLastName IS NULL 
   OR Upper(pa.Patient_Names_CSV) LIKE '%' || Upper(:PatientLastName) || '%'
ORDER BY 3, 4, 5