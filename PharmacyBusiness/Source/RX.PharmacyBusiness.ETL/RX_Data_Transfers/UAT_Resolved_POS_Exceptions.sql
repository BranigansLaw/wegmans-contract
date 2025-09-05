SELECT
     resolved_datetime
    ,userdata_uid
    ,exception_date
    ,store_num
    ,rx_num
    ,refill_num
    ,part_seq_num
    ,classification_code
    ,in_wcb
    ,is_resolved
    ,sold_datetime
    ,mny_custpays
--select count(*)
FROM (
  SELECT
     Round((DTM_RESOLVED - To_Date('20350101','YYYYMMDD')),6) AS resolved_datetime
    ,STR_RESOLVEUSER AS userdata_uid
    ,To_Number(To_Char(DTM_ADDED,'YYYYMMDD')) AS exception_date
    ,N_STORENBR AS store_num
    ,N_RXNBR AS rx_num
    ,N_REFILLNBR AS refill_num
    ,N_PFSEQNBR AS part_seq_num
    ,ID_RTSECLASSIFICATION AS classification_code
    ,(CASE WHEN B_INWILLCALLBIN = '1' THEN 'Y' WHEN B_INWILLCALLBIN = '0' THEN 'N' ELSE '' END) AS in_wcb
    ,(CASE WHEN DTM_RESOLVED Is Null THEN 0 ELSE 1 END) AS is_resolved
    --,Round((DTM_POSSOLD - To_Date('20350101','YYYYMMDD')),6) AS sold_datetime
    --,mny_copay AS mny_custpays
    ,Rank() OVER (PARTITION BY To_Number(To_Char(DTM_ADDED,'YYYYMMDD')), N_STORENBR, N_RXNBR, N_REFILLNBR, N_PFSEQNBR
                  ORDER BY ID_RTSEXCEPTION Desc) AS Rank_Exception
  FROM Rx.RTSException
  WHERE DTM_ADDED BETWEEN To_Date('20210101','YYYYMMDD') AND sysdate
    AND ID_RTSECLASSIFICATION IN (1,2,3,4,8,9,10)
)
WHERE Rank_Exception = 1 --16546 rows as of 11/9/2021
ORDER BY
     store_num
    ,rx_num
    ,refill_num
    ,part_seq_num
    ,resolved_datetime