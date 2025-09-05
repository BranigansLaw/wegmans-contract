create or replace TYPE TAT_SUMMARY_MAXRX_OBJ AS OBJECT
(
     ORDER_NUMBER                        VARCHAR2(10 CHAR)
    ,FACILITY                            VARCHAR2(10 CHAR)
    ,DATE_IN                             DATE
    ,DATE_OUT                            DATE
    ,DAYS_OVERALL                        NUMBER
    ,DEDUCT_DAYS_INTERVENTION            NUMBER
    ,DEDUCT_DAYS_OFF_HOURS               NUMBER
    ,DAYS_NET_TAT                        NUMBER
    ,COUNT_RX_IN_ORDER                   NUMBER
	,HAS_EXCEPTIONS                      VARCHAR2(1 CHAR)
);

create or replace TYPE TAT_SUMMARY_MAXRX_TABLE_TYPE AS TABLE OF TAT_SUMMARY_MAXRX_OBJ;
