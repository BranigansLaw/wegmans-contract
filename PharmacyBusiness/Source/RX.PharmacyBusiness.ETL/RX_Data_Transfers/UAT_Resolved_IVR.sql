SELECT
     resolved_datetime
    ,userdata_uid
    ,store_num
    ,phone_nbr_formatted
    ,call_made_date
    ,is_resolved
--select count(*)
FROM (
  SELECT
     Round((DTM_RESOLVED - To_Date('20350101','YYYYMMDD')),6) AS resolved_datetime
    ,STR_RESOLVEDUSER AS userdata_uid
    ,N_STORENBR AS store_num
    ,STR_PHONENBR AS phone_nbr_formatted
    ,To_Char(DT_CALLMADE,'YYYY-MM-DD') AS call_made_date
    ,1 AS is_resolved
    ,Rank() OVER (PARTITION BY DT_CALLMADE, N_STORENBR, N_RXNBR
                  ORDER BY ID_IVROUTBOUNDCALL Desc) AS Rank_Resolved
  FROM Rx.IVROutboundCalls
  WHERE DTM_RESOLVED BETWEEN To_Date('20210101','YYYYMMDD') AND sysdate
)
WHERE Rank_Resolved = 1 --1794062 rows as of 11/9/2021
ORDER BY 
     store_num
    ,phone_nbr_formatted
    ,call_made_date Desc