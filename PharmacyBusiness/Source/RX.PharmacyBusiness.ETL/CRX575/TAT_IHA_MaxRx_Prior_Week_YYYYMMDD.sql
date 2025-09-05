SELECT * 
FROM TABLE(wegmans.TURN_AROUND_TIME_PKG.Get_TAT_Summary_MaxRx(:RunDate,'IHA'))
