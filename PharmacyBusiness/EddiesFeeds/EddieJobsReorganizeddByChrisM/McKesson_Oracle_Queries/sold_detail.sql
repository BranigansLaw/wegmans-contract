SELECT Fac.fd_facility_id "store_num",
	Ff.rx_number "rx_num",
	Ff.refill_num "refill_num",
	NVL(Ff.partial_fill_seq, 0) AS "part_seq_num",
	solddate.CAL_DATE "sold_date",
	Ff.facility_order_number "order_num",
	Ff.qty_dispensed "qty_dispensed",
	dr.drug_ndc "ndc_wo",
	Ff.user_defined_step_amt "acq_cost",
	Ff.tp_price_paid "tp_pay",
	Ff.patient_price_paid "patient_pay",
	Ff.total_price_paid "tx_price"

FROM TREXONE_DW_DATA.Fill_Fact Ff

INNER JOIN TREXONE_DW_DATA.Facility Fac
	ON Fac.fd_facility_key = Ff.fd_facility_key
INNER JOIN TREXONE_DW_DATA.Drug dr
	ON dr.drug_key = Ff.dispensed_drug_key
INNER JOIN TREXONE_DW_DATA.DATE_DIM solddate
	ON Ff.SOLD_DATE_KEY = solddate.DATE_KEY

WHERE trunc(solddate.CAL_DATE) = trunc(sysdate-1)