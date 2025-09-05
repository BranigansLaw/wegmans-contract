
create or replace TYPE TAT_RAW_DATA_OBJ AS OBJECT
(
     ORDER_NUMBER                        VARCHAR2(10 CHAR)
    ,FACILITY                            VARCHAR2(10 CHAR)
    ,MCKESSON_PATIENT_KEY                NUMBER(18,0)
    ,RX_NBR                              VARCHAR2(20 CHAR)
    ,REFILL_NBR                          NUMBER(18)
    ,DATE_IN                             DATE
    ,DATE_OUT                            DATE
    ,WFSD_KEY                            NUMBER(18)
    ,WFSD_STEP_DESCRIPTION               VARCHAR2(100 CHAR)
    ,Is_Intervention                     VARCHAR2(1 CHAR)
    ,Is_Exception                        VARCHAR2(1 CHAR)
    ,Elapsed_Days_In_Step                NUMBER
    ,Elapsed_Days_In_Step_Off_Hrs        NUMBER
    ,Date_In_Rank                        NUMBER
    ,Date_Out_Rank                       NUMBER
);

create or replace TYPE TAT_RAW_DATA_TABLE_TYPE AS TABLE OF TAT_RAW_DATA_OBJ;



