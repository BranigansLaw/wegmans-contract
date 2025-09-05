create or replace TYPE TAT_ORDERS_RTP_OBJ AS OBJECT
(
     ORDER_NUMBER                        VARCHAR2(10 CHAR)
    ,FACILITY                            VARCHAR2(10 CHAR)
    ,MCKESSON_PATIENT_KEY                NUMBER(18,0)
    ,RX_NBR                              VARCHAR2(20 CHAR)
    ,REFILL_NBR                          NUMBER(18)
);

create or replace TYPE TAT_ORDERS_RTP_TABLE_TYPE AS TABLE OF TAT_ORDERS_RTP_OBJ;
