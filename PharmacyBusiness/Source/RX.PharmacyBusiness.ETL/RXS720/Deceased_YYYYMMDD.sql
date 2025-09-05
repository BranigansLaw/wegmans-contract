SELECT  PD_PATIENT_NUM
FROM trexone_dw_data.patient pd
WHERE PD_DATE_OF_DEATH IS not NULL
  AND pd_date_of_death BETWEEN (:RunDate - 180) and :RunDate
ORDER BY PD_PATIENT_NUM