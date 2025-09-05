WITH r0 AS (
    SELECT  rf.rx_record_num,
            rf.rx_fill_seq,
            it.order_num,
            it.item_seq,
            rf.image_group_num,
            DW__Drug.DRUG_NDC,
            rf.QTY_DISPENSED,
            (SELECT MAX (Fs.fill_state_chg_ts)
               FROM Fill_State Fs
              WHERE Fs.rx_record_num = rf.rx_record_num
                AND Fs.rx_fill_seq = rf.rx_fill_seq
                AND Fs.fill_state_code IN (3, 14)) sold_date
    FROM RX_FILL rf
         JOIN ITEM it
            ON it.rx_record_num = rf.rx_record_num
           AND it.rx_fill_seq = rf.rx_fill_seq
           AND it.H_TYPE != 'D'
         INNER JOIN TREXONE_DW_DATA.Product DW__Product
            ON DW__Product.prd_product_num = rf.PRODUCT_NUM
         INNER JOIN TREXONE_DW_DATA.Drug DW__Drug
            ON DW__Drug.drug_num = DW__Product.PRD_DRUG_NUM
    WHERE (rf.rx_record_num, rf.rx_fill_seq) = (
               SELECT ri.rx_record_num, MAX (ri.rx_fill_seq)
               FROM RX rx
                    JOIN RX_FILL ri
                       ON ri.rx_record_num = rx.rx_record_num
                    JOIN TREXONE_DW_DATA.FACILITY fa
                       ON fa.fd_facility_num = rx.facility_num
               WHERE rx.rx_number = :RxNumber 
                 AND ri.refill_num = :RefillNumber
                 AND (
                       (:PartialFillSequence = 0 AND ri.partial_fill_seq IS NULL)
                       OR 
                       (ri.partial_fill_seq = :PartialFillSequence2)
                     )
                 AND fa.fd_facility_id = :Store
                 AND rx.eff_end_date = TO_DATE ('2999-12-31', 'YYYY-MM-DD')
                 AND ri.eff_end_date = rx.eff_end_date
               GROUP BY ri.rx_record_num
          )
      AND rf.eff_end_date = TO_DATE ('2999-12-31', 'YYYY-MM-DD')
      AND it.eff_end_date = TO_DATE ('2999-12-31', 'YYYY-MM-DD')
)
SELECT r0.sold_date LastSoldDate,
         (SELECT MAX (pf.pos_datestamp)
            FROM POS_FINALIZATION pf
           WHERE pf.rx_record_num = r0.rx_record_num
             AND pf.rx_fill_seq = r0.rx_fill_seq
             AND pf.txn_type = 'S') POSSoldDate,
         (SELECT MAX (fs.fill_state_chg_ts)
            FROM FILL_STATE fs
           WHERE fs.rx_record_num = r0.rx_record_num
             AND fs.rx_fill_seq = r0.rx_fill_seq
             AND fs.fill_state_code = 4
             AND (fs.fill_state_chg_ts > r0.sold_date
                  OR 
                  r0.sold_date IS NULL)) ReturnedDate,
         (SELECT MAX (im.image_scan_date)
            FROM TREXONE_DW_DATA.IMAGE im, IMAGE_GROUP_LIST ig
           WHERE ig.image_group_num = r0.image_group_num
             AND im.image_num = ig.image_num
             AND im.image_type = 'Signature'
             AND im.image_is_assigned = 'Y') ScannedSignatureDate,
         r1.final_price TotalPrice,
         r1.tp_plan_amount_paid InsurancePayment,
         r1.patient_pay_amount PatientPayment,
         r2.total_user_defined AcquisitionCost,
         r0.DRUG_NDC,
         To_Char(r0.QTY_DISPENSED) QTY_DISPENSED
FROM r0
     LEFT JOIN (
         SELECT pr.final_price,
                pr.tp_plan_amount_paid,
                pr.patient_pay_amount,
                ip.order_num,
                ip.item_seq
         FROM ITEM_PRICE_CALC_RESULT ip
              JOIN PRICE_CALC_RESULT pr
                 ON pr.price_calc_result_num = ip.price_calc_result_num
         WHERE ip.is_current = 'Y' 
           AND ip.h_type != 'D'
         ORDER BY pr.h_level DESC
     ) r1 ON r1.order_num = r0.order_num 
         AND r1.item_seq = r0.item_seq
     LEFT JOIN (
         SELECT pr.total_user_defined, ip.order_num, ip.item_seq
         FROM ITEM_PRICE_CALC_RESULT ip
              JOIN PRICE_CALC_RESULT pr
                 ON pr.price_calc_result_num = ip.price_calc_result_num
         WHERE ip.h_type != 'D' 
           AND pr.price_type_code = 'CD'
         ORDER BY pr.h_level DESC
     ) r2 ON r2.order_num = r0.order_num 
         AND r2.item_seq = r0.item_seq
WHERE ROWNUM = 1