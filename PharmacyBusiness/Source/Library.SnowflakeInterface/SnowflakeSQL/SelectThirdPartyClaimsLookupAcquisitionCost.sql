SELECT NVL(SUM(PCR.TOTAL_USER_DEFINED),0) AS TOTAL_USER_DEFINED
              FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PRICE_CALC_RESULT PCR
             WHERE PCR.PRICE_CALC_RESULT_NUM IN (
                       SELECT MAX (IPCR.PRICE_CALC_RESULT_NUM)
                         FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX_FILL RF
                              INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.ITEM
                                      ON ITEM.RX_RECORD_NUM = RF.RX_RECORD_NUM
                                     AND ITEM.RX_FILL_SEQ = RF.RX_FILL_SEQ
                              INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.ITEM_PRICE_CALC_RESULT IPCR
                                      ON IPCR.ORDER_NUM = ITEM.ORDER_NUM
                                     AND IPCR.ITEM_SEQ = ITEM.ITEM_SEQ
                        WHERE RF.RX_RECORD_NUM = :RX_RECORD_NUM
                          AND (RF.INTERNAL_FILL_ID =
                                  (SELECT DISTINCT RF2.INTERNAL_FILL_ID
                                     FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX_FILL RF2
                                    WHERE RF2.RX_RECORD_NUM = :RX_RECORD_NUM
                                      AND RF2.RX_FILL_SEQ = :RX_FILL_SEQ)
                                OR RF.RX_FILL_SEQ = :RX_FILL_SEQ
                              )
                          AND RF.EFF_START_DATE <= TO_DATE(SUBSTR(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND RF.EFF_END_DATE > TO_DATE(SUBSTR(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          --  WHEN A COMPLETION IS CANCELLED FROM FOA, WE DON'T WANT TO COUNT IT,
                          --   THE ORIGINAL PARTIAL IS REPROCESSED FOR THE FULL AMOUNT.
                          AND RF.FILL_STATUS_NUM != 4
                          AND ITEM.EFF_START_DATE <= TO_DATE(SUBSTR(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND ITEM.EFF_END_DATE > TO_DATE(SUBSTR(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND IPCR.EFF_START_DATE <= TO_DATE(SUBSTR(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND IPCR.H_TYPE != 'D'
                          AND IPCR.PRICE_CALC_RESULT_NUM IN (
                                  SELECT PCR.PRICE_CALC_RESULT_NUM
                                    FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PRICE_CALC_RESULT PCR
                                   WHERE PCR.PRICE_TYPE_CODE = 'CD'
                              )
                     GROUP BY RF.RX_FILL_SEQ
                   )
               AND PCR.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')