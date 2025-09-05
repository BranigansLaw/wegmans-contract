select
    patient.PD_PATIENT_NUM as patient_num,
    patient.PD_SPECIES_NAME as species,
    patient.PD_CREATE_DT as create_date,
    'Y' as pet
from TREXONE_DW_DATA.PATIENT patient
where patient.PD_SPECIES_NAME is not null
  and patient.PD_STATUS = 'A'