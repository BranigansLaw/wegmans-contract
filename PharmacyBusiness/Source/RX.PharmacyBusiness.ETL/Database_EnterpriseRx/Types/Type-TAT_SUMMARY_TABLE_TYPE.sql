
create or replace TYPE TAT_SUMMARY_OBJ AS OBJECT
(
     ORDER_NUMBER                        VARCHAR2(10 CHAR)
    ,FACILITY                            VARCHAR2(10 CHAR)
    ,RX_NBR                              VARCHAR2(20 CHAR)
    ,REFILL_NBR                          NUMBER(18)
    ,DATE_IN                             DATE
    ,DATE_OUT                            DATE
    ,DAYS_OVERALL                        NUMBER
    ,DEDUCT_DAYS_INTERVENTION            NUMBER
    ,DEDUCT_DAYS_OFF_HOURS               NUMBER
    ,DAYS_NET_TAT                        NUMBER
	,HAS_EXCEPTIONS                      VARCHAR2(1 CHAR)
);

create or replace TYPE TAT_SUMMARY_TABLE_TYPE AS TABLE OF TAT_SUMMARY_OBJ;
