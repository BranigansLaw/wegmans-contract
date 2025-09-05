SELECT UNIQUE audpatientgroup.DATESTAMP "pt_add_date",
              dwfacility.FD_FACILITY_ID "store_num",
              audpatientgroup.PATIENT_NUM "patient_num",
              audpatientgroup.GROUP_NUM "group_num",
              dwgroups.GRP_NAME "group_name",
              dwgroups.GRP_DESCRIPTION "group_desc",
              dwsysuser.SYS_USER_LOGIN_NAME "emp_username",
              dwsysuser.SYS_USER_FNAME "emp_fname",
              dwsysuser.SYS_USER_LNAME "emp_lname",
              (CASE WHEN (audpatientgroup.EFF_END_DATE = to_date('12/31/2999','MM/DD/YYYY'))
              THEN('Added')
              ELSE('Removed') END) as "event",
              dwgroups.GRP_CREATE_DATE "group_start_date"

FROM TREXONE_AUD_DATA.PATIENT_GROUP audpatientgroup

LEFT OUTER JOIN TREXONE_AUD_DATA.facility_login facilitylogin
ON facilitylogin.SYSTEM_USER_NUM = audpatientgroup.SYS_USER
AND audpatientgroup.EFF_START_DATE BETWEEN facilitylogin.EFF_START_DATE AND facilitylogin.EFF_END_DATE

LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY dwfacility
ON dwfacility.FD_FACILITY_NUM = facilitylogin.FACILITY_NUM

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER dwsysuser
ON dwsysuser.SYS_USER_NUM = audpatientgroup.SYS_USER

LEFT OUTER JOIN TREXONE_DW_DATA.GROUPS dwgroups
ON dwgroups.GRP_GROUP_NUM = audpatientgroup.GROUP_NUM

WHERE TRUNC(audpatientgroup.DATESTAMP) = TO_DATE(SYSDATE-1)

--WHERE audpatientgroup.DATESTAMP BETWEEN
--TO_DATE('12/14/2020 00:00:00','MM/DD/YYYY HH24:MI:SS') AND
--TO_DATE('12/20/2020 23:59:59','MM/DD/YYYY HH24:MI:SS')
--^ This is for AdHoc pull or backpopulate to pick date range

AND dwgroups.GRP_TYPE = 'A'