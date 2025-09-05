--Query to create file "RxDrugList.js" for the Medicare Plan Advisor Tool but was discontinued in 2019.
SELECT * FROM TABLE(RxMedicare.PLAN_COMPARISON_PKG.GET_DRUG_SEARCH_JS)