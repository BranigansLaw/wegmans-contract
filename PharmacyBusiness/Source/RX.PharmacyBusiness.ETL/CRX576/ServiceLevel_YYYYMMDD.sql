SELECT /*+ NO_MERGE(tabA) */
  Trim(leading 0 from tabA.facility_id) AS store_num,
  tabA.rx_number AS rx_num,
  tabA.refill_num AS refill_num,
  itf.pickup_time AS orig_promise_time,
  tabA.maxfoa AS time_left_foa,
  tabA.sold_date AS date_time_sold,
  tabA.sold_date AS date_sold
 FROM (SELECT
  pos.facility_id,
  pos.rx_number,
  pos.refill_num,
  Max(fwf.fwfs_dt_out) maxfoa,
  Min(it.h_level) minh,
  Substr(pos.rx_number,1,1) schedule,
  Max(pos.sold_date) sold_date,
  Max(it.order_num) maxo
 FROM wegmans.postransaction pos
  INNER JOIN trexone_dw_data.facility f ON pos.facility_id = f.fd_facility_id
  INNER JOIN trexone_dw_data.fill_workflow_fact fwf
     ON pos.rx_number = fwf.fwfs_rx_number
    AND pos.refill_num = fwf.fwfs_refill_num
    AND f.fd_facility_key = fwf.fd_facility_key
    AND fwf.wfsd_key=508
    AND fwf.fwfs_is_declined = 'N'
  LEFT OUTER JOIN trexone_aud_data.rx rb
     ON rb.rx_number = pos.rx_number AND rb.facility_num = f.fd_facility_num AND rb.eff_end_date = TO_DATE('12/31/2999','MM/DD/YYYY')
  LEFT OUTER JOIN trexone_aud_data.rx_fill rf
     ON rf.refill_num = pos.refill_num AND rf.rx_record_num = rb.rx_record_num AND rf.eff_end_date = TO_DATE('12/31/2999','MM/DD/YYYY')
  LEFT OUTER JOIN trexone_aud_data.item it
     ON it.rx_record_num = rb.rx_record_num AND it.rx_fill_seq = rf.rx_fill_seq AND it.pickup_time is not null
 WHERE pos.partial_fill_seq=0
   AND pos.sold_date BETWEEN (:RunDate - 1)
   AND ((:RunDate - 1) + INTERVAL '23:59:59' HOUR TO SECOND)
 GROUP BY pos.facility_id, pos.rx_number, pos.refill_num
) tabA
  LEFT OUTER JOIN trexone_aud_data.item itf ON itf.h_level = tabA.minh
 WHERE itf.pickup_time < tabA.maxfoa
   AND tabA.schedule <> '8'