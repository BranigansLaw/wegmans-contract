SELECT  ir.id_inc_incident,
        ir.id_inc_storenbr,
        ir.id_inc_rxnbr,
        ir.n_inc_incidenttype,
        NVL2(ir.dt_fu_coaching, 1, 0) AS n_incstatus,
        CASE WHEN ir.dt_fu_coaching IS NULL THEN
            CASE WHEN TRUNC(SYSDATE) - TRUNC(ir.dt_inc_increported) > 7
                THEN 4 -- OVERDUE
                ELSE 1 -- PENDING
            END
        ELSE
            CASE WHEN TRUNC(ir.dt_fu_coaching) - TRUNC(ir.dt_inc_increported) > 7
                THEN 3 -- OVER 7 DAYS
                ELSE 2 -- ON TIME
            END
        END AS n_coachingstatus,
        se.n_pom,
        se.n_division,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(it.str_incidenttypedesc,'"',''),chr(13),''),chr(10),'') AS str_incidenttypedesc,
        Round((ir.dt_inc_increported - To_Date('20350101','YYYYMMDD')),6) AS dt_inc_increported,
        Round((ir.dt_inc_incident - To_Date('20350101','YYYYMMDD')),6) AS dt_inc_incident,
        Round((ir.dt_fu_coaching - To_Date('20350101','YYYYMMDD')),6) AS dt_fu_coaching,
        ir.c_inc_incidentclosed,
        ir.c_inc_pathealthaffected,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_qa_pharm,'"',''),chr(13),''),chr(10),'') AS id_qapharm,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.c_inc_patientcode,'"',''),chr(13),''),chr(10),'') AS c_inc_patientcode,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_dispensedmedname,'"',''),chr(13),''),chr(10),'') AS str_inc_dispensedmedname,
        CASE WHEN u.str_QALastname IS NULL THEN w.str_QALastname ELSE u.str_QALastname END as str_QALastname,
        CASE WHEN u.str_QAFirstname IS NULL THEN w.str_QAFirstname ELSE u.str_QAFirstname END as str_QAFirstname,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_authorname,'"',''),chr(13),''),chr(10),'') AS str_inc_authorname,
        ir.id_patientstore,
        ir.c_inc_origin,
        ir.id_incrptsourcetype,
        ir.c_inc_callin,
        Round((ir.dt_inc_filled - To_Date('20350101','YYYYMMDD')),6) AS dt_inc_filled,
        Round((ir.dt_inc_notification - To_Date('20350101','YYYYMMDD')),6) AS dt_inc_notification,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxordermedname,'"',''),chr(13),''),chr(10),'') AS str_inc_rxordermedname,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxlabelmedname,'"',''),chr(13),''),chr(10),'') AS str_inc_rxlabelmedname,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxorderstrength,'"',''),chr(13),''),chr(10),'') AS str_inc_rxorderstrength,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxlabelstrength,'"',''),chr(13),''),chr(10),'') AS str_inc_rxlabelstrength,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_dispensedstrength,'"',''),chr(13),''),chr(10),'') AS str_inc_dispensedstrength,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxorderdirections,'"',''),chr(13),''),chr(10),'') AS str_inc_rxorderdirections,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_rxlabeldirections,'"',''),chr(13),''),chr(10),'') AS str_inc_rxlabeldirections,
        REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(ir.str_inc_dispenseddirections,'"',''),chr(13),''),chr(10),'') AS str_inc_dispenseddirections,
        ir.c_inc_ingested,
        ir.c_inc_medicationgiven,
        ir.c_inc_repmedication,
        ir.c_inc_medattention,
        ir.c_inc_outsideagency,
        ir.id_inc_wlboccuredfacility
FROM    Rx.HIPAAIncidentReporting ir
        JOIN Rx.HIPAAIncidentType it ON (ir.n_inc_incidenttype = it.id_incidenttype)
        JOIN Rx.StoreExt se ON (ir.id_inc_storenbr = se.id_storenbr)
        LEFT JOIN (
            SELECT  id_user,
                    id_storenbr,
                    dt_exportdat,
                    REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(str_userslastname,'"',''),chr(13),''),chr(10),'') AS str_qalastname,
                    REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(str_usersfirstname,'"',''),chr(13),''),chr(10),'') AS str_qafirstname,
                    ROW_NUMBER() OVER(
                        PARTITION BY id_user, id_storenbr
                        ORDER BY dt_exportdat DESC NULLS LAST
                    ) AS rn
            FROM    Rx.RxUser
            WHERE   str_userslastname IS NOT NULL
        ) u ON (
                ir.id_inc_storenbr  = u.id_storenbr
            AND ir.str_inc_qa_pharm = u.id_user
            AND u.rn = 1
        )
         LEFT JOIN (
            SELECT  id_user,
                    id_storenbr,
                    dt_exportdat,
                    REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(str_userslastname,'"',''),chr(13),''),chr(10),'') AS str_qalastname,
                    REGEXP_REPLACE(REGEXP_REPLACE(REGEXP_REPLACE(str_usersfirstname,'"',''),chr(13),''),chr(10),'') AS str_qafirstname,
                    ROW_NUMBER() OVER(
                        PARTITION BY id_user, id_storenbr
                        ORDER BY dt_exportdat DESC NULLS LAST
                    ) AS rn
            FROM    Rx.RxUser
            WHERE   str_userslastname IS NOT NULL
        ) w ON (
                ir.ID_INC_WLBOCCUREDFACILITY  = w.id_storenbr
            AND ir.str_inc_qa_pharm = w.id_user
            AND w.rn = 1
        )
ORDER BY DT_INC_INCIDENT, ID_INC_STORENBR