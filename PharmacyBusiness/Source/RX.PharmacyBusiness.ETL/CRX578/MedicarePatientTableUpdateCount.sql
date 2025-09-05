--Required for Medicare Plan Advisor Tool to populate a table within McKesson. 
--No output file is required here, but rather just execute a procedure.
SELECT wegmans.MEDICARE_PKG.INSERT_BY_SOLD_DATE(NULL) AS Patient_Rx_Count FROM dual