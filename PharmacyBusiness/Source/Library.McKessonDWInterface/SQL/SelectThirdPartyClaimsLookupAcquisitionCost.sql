SELECT NVL(Sum(pcr.total_user_defined),0) AS total_user_defined
              FROM TREXONE_AUD_DATA.Price_Calc_Result pcr
             WHERE pcr.price_calc_result_num IN (
                       SELECT MAX (Ipcr.price_calc_result_num)
                         FROM TREXONE_AUD_DATA.Rx_Fill Rf
                              INNER JOIN TREXONE_AUD_DATA.Item
                                      ON Item.rx_record_num = Rf.rx_record_num
                                     AND Item.rx_fill_seq = Rf.rx_fill_seq
                              INNER JOIN TREXONE_AUD_DATA.Item_Price_Calc_Result Ipcr
                                      ON Ipcr.order_num = Item.order_num
                                     AND Ipcr.item_seq = Item.item_seq
                        WHERE Rf.rx_record_num = :RX_RECORD_NUM
                          AND (Rf.internal_fill_id =
                                  (SELECT DISTINCT Rf2.internal_fill_id
                                     FROM TREXONE_AUD_DATA.Rx_Fill Rf2
                                    WHERE Rf2.rx_record_num = :RX_RECORD_NUM
                                      AND Rf2.rx_fill_seq = :RX_FILL_SEQ)
                                OR Rf.rx_fill_seq = :RX_FILL_SEQ
                              )
                          AND Rf.eff_start_date <= to_date(substr(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND Rf.eff_end_date > to_date(substr(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          --  When a completion is cancelled from FOA, we don't want to count it,
                          --   the original partial is reprocessed for the full amount.
                          AND Rf.fill_status_num != 4
                          AND Item.eff_start_date <= to_date(substr(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND Item.eff_end_date > to_date(substr(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND Ipcr.eff_start_date <= to_date(substr(:FILL_STATE_KEY, 1, 14), 'YYYYMMDDHH24MISS')
                          AND Ipcr.h_type != 'D'
                          AND Ipcr.price_calc_result_num IN (
                                  SELECT Pcr.price_calc_result_num
                                    FROM TREXONE_AUD_DATA.Price_Calc_Result Pcr
                                   WHERE Pcr.price_type_code = 'CD'
                              )
                     GROUP BY Rf.rx_fill_seq
                   )
               AND pcr.eff_end_date = TO_DATE('12/31/2999','MM/DD/YYYY')
 