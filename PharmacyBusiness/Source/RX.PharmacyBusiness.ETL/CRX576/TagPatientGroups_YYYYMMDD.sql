SELECT unique audpg.PATIENT_NUM "patient_num",
        patient.CREATE_DATE "pt_create_date",
        fac.FD_FACILITY_ID "create_store_num",
        audpg.DATESTAMP "pt_tagged_date",
        audpg.EFF_START_DATE "pt_ingroup_start",
        audpg.EFF_END_DATE "pt_ingroup_end",
        audpg.GROUP_NUM "group_num",
        dwg.GRP_NAME "group_name",
        dwg.GRP_DESCRIPTION "group_desc",
        dwg.GRP_TYPE "group_status",
        dwg.GRP_IS_ACTIVE "group_is_active",
        dwg.GRP_CREATE_DATE "group_create_dt",
        dwg.GRP_LAST_UPD_DATE "group_last_updated"

FROM TREXONE_AUD_DATA.PATIENT_GROUP audpg

LEFT OUTER JOIN TREXONE_AUD_DATA.PATIENT patient
ON patient.PATIENT_NUM = audpg.PATIENT_NUM
AND patient.EFF_END_DATE = TO_DATE('2999-12-31','YYYY-MM-DD')

LEFT OUTER JOIN TREXONE_DW_DATA.GROUPS dwg
ON audpg.GROUP_NUM = dwg.GRP_GROUP_NUM

LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY fac
ON patient.CREATE_FACILITY_NUM = fac.FD_FACILITY_NUM

WHERE audpg.DATESTAMP BETWEEN (:RunDate - 1)
  AND ((:RunDate - 1) + INTERVAL '23:59:59' HOUR TO SECOND)