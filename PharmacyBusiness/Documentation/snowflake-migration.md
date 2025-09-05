# Snowflake Migration Plan

This document outlines the high level steps for migrating the Control-M jobs that reference McKesson DW Orcale data and migrating them to:

1. Use the Snowflake data connection instead of the Oracle ODBC connection
2. Run via INN commands coded in .NET 8

## Project Focus

The focus of this project is ONLY to migrate McKesson DW queries exactly as they are into calls to Snowflake for the exact same information, formatted identically.

The scope of this project does not include:

- Query Optimization
- Adding Snowflake reporting for clients
- Import/Export functionality with Snowflake

## Data Security and Compliance

Our connection to Snowflake has been assessed by the Security team (Dan Severance and Mike Wolford) and the following security measures have been agreed upon:

### SSO

- Access to the Snowflake UI will be managed by SSO via the Wegmans network
- Access rights to data will be managed by an admin account in Snowflake

### Private Link

- Calls to the API will be managed by an Azure Private Link connecting the VNET of Wegmans to that of Snowflake providing a secure tunnel for data transfer

### Login and Query Auditing

- All queries for data are logged in Snowflake and can be audited by the Security team

## Job Replacement Strategy

After some investigation, it was discovered that data in Oracle and Snowflake might not always be identical. It would not be a 1-to-1 change from Oracle to Snowflake as originally stated. As such, the approach has changed.

### Data Differences

Data can be different for two main reasons:

- The ETL process importing data into Oracle and Snowflake run at different times, but between 4 - 6 AM ET run around the same time. The data between the systems can be compared around them for the best chance of having identical data
- Due to schema changes, queries may need heavy updates to return the same data result. When there is a difference, these changes can be passed on to McKesson and they will identify the issues and give fixes for them

### Next Steps

1. Identify which jobs perform what actions primarily:
  - Is it uploading to TenTen, sending a file to vendor, etc [see Jobs that upload to TenTen](#jobs-that-upload-to-tenten)
  - Can the job handle duplicates being added
2. Review with the Pharmacy Business team which of these jobs could be retired, which have data that isn't extremely important to match perfectly, and which data needs to be identical

#### Jobs that upload to TenTen

In the case where a job uploads data to TenTen, instead of appending directly to the existing feed, we will create a new feed that exists of either a copy of the historical feed, or a feed of historical data from TenTen. Then, we will append to this new feed and compare the differences between it and the existing feed, verify they are the same with the business, and cut over to the new Snowflake feed.

#### Can the job handle duplicates being added

In these cases, if the job can handle duplicates, the new job could be run for previous dates after the cutover to pick up data that was missed. In these cases, the data is expected to be deduplicated once it is uploaded. 

### Innovation Project Architectural Rules

See [Innovation Project Architecture Rules](./architecture-rules.md).

### Testing (Unit Tests vs Integration Tests)

#### What to Unit Test

- Testing that the `RunAsync` command for the new job passes return values to their external dependencies correctly
  - Ex1. A job that calls Snowflake to pull data and then writes that data to a file should have a test that calls the mocked Snowflake library, gets a mock returned `IEnumerable` of data, and then passes that data to a mocked file writer
  - Ex2. A job that calls Snowflake to pull data, modifies that data applying some business rules, and then writes that data to a file should have a test that calls the mocked Snowflake library, passes the mocked return to a mocked `IHelper` class, and passes the mocked modified data to a mocked file writer
- Business logic that transforms the data in some way should be in a `IHelper` class that can be mocked
  - This mocked helper method that transforms the data should have unit tests that test the transformation logic

#### What to Integration Test

- The actual connection to Snowflake
- Your Snowflake method that runs your Snowflake query

## Control-M Jobs

In Google Sheets: https://docs.google.com/spreadsheets/d/1oeLuM9simZeelzS3O1mGIU3VMxoDbLT_H9TkP4vEEsU/edit#gid=938031124

| Job Number | Mini Job Stream Number | Project | SQL File Name | SQL File Complexity | SQL File Complexity Reasoning |
| --- | --- | --- | --- | --- | --- |
| CRX515 | ? | RX.PharmacyBusiness.ETL | Omnisys_Claim_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| CRX515/INN601* | ? | RX.PharmacyBusiness.ETL | SelectOmnisysClaim.sql | Moderate | WITH/RANK/HINTS/FORMATTING |
| CRX540 | ? | RX.PharmacyBusiness.ETL | MightBeRefundTransactions.sql | Complex | WITH/RANK/HINTS/UNION/BUSINESS LOGIC/FORMATTING |
|  |  | RX.PharmacyBusiness.ETL | MightBeSoldTransactions.sql | Moderate | RANK/HINTS/FORMATTING |
| CRX572 | ? | RX.PharmacyBusiness.ETL | WEG_086_YYYYMMDD_01.sql | Complex | FORMATTING/BUSINESS LOGIC/FUNCTION CALLS |
| CRX575 | ? | RX.PharmacyBusiness.ETL | TURN_AROUND_TIME_PKG.sql | Complex | DATABASE PACKAGE/WEGMANS TABLE/HINTS |
|  |  | RX.PharmacyBusiness.ETL | TAT_Excellus_MaxRx_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_Excellus_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_Excellus_Raw_Data_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_IHA_MaxRx_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_IHA_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_IHA_Raw_Data_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_Specialty_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_Specialty_Raw_Data_Prior_Week_YYYYMMDD.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | TAT_Specialty_YTD_YYYYMMDD.sql | Moderate | PACKAGE CALL |
| CRX576 | ? | RX.PharmacyBusiness.ETL | DurConflict_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | EXTENDED_DRUG_FILE_YYYYMMDD.sql | Moderate | FORMATTING/BUSINESS LOGIC |
|  |  | RX.PharmacyBusiness.ETL | InvAdj_YYYYMMDD.sql | Simple | FORMATTING |
|  |  | RX.PharmacyBusiness.ETL | PetPtNums_YYYYMMDD.sql | Simple | FORMATTING |
|  |  | RX.PharmacyBusiness.ETL | PRESCRIBER_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | PRESCRIBERADDRESS_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | RX_READY_YYYYMMDD.sql | Moderate | FORMATTING/BUSINESS LOGIC |
|  |  | RX.PharmacyBusiness.ETL | RxTransfer_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | ServiceLevel_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | SupplierPriceDrugFile_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | TagPatientGroups_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| CRX578 | ? | RX.PharmacyBusiness.ETL | FDS_Pharmacies.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | FDS_Prescriptions.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | Wegmans_HPOne_Pharmacies_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | Wegmans_HPOne_Prescriptions_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| CRX582 | ? | RX.PharmacyBusiness.ETL | THIRD_PARTY_CLAIMS_PKG.sql | Complex | DATABASE PACKAGE/WEGMANS TABLE/HINTS |
|  |  | RX.PharmacyBusiness.ETL | CallPackage.sql | Complex | PACKAGE CALL |
| CRX587 | ? | RX.PharmacyBusiness.ETL | SURESCRIPTS_PKG.sql | Complex | DATABASE PACKAGE/WEGMANS TABLE/FORMATTING/BUSINESS LOGIC/FUNCTION CALLS |
|  |  | RX.PharmacyBusiness.ETL | IMMWegmansPAYYYYMMDD01.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | IMMWegmansYYYYMMDD01.sql | Moderate | PACKAGE CALL |
|  |  | RX.PharmacyBusiness.ETL | PEFWegmansYYYYMMDD01.sql | Moderate | PACKAGE CALL |
| CRX801 | ? | RX.PharmacyBusiness.ETL | WorkersCompMonthly_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| INN510* | ? | RX.PharmacyBusiness.ETL | SelectNewTagPatientGroups.sql | Simple | JOIN |
| INN511* | ? | RX.PharmacyBusiness.ETL | SelectRxErp.sql | Simple | JOIN |
| INN512* | ? | RX.PharmacyBusiness.ETL | SelectSoldDetail.sql | Simple | JOIN |
| INN520* | ? | RX.PharmacyBusiness.ETL | SelectStoreInventoryHistory.sql | Simple | JOIN |
| RXS501 | ? | RX.PharmacyBusiness.ETL | SelectAllActiveSystemUser.sql | Trivial | ONE TABLE SELECT |
|  |  | RX.PharmacyBusiness.ETL | SelectSystemEventAuditYYYYMMDD.sql | Trivial | ONE TABLE SELECT |
|  |  | RX.PharmacyBusiness.ETL | SelectSystemUserYYYYMMDD.sql | Trivial | ONE TABLE SELECT |
| RXS512 | ? | RX.PharmacyBusiness.ETL | DailyDrug_YYYYMMDD.sql | Moderate | FORMATTING/BUSINESS LOGIC |
| RXS527 | ? | RX.PharmacyBusiness.ETL | Get_IVR_Outbound_Calls.sql | Moderate | UNION/BUSINESS LOGIC |
| RXS532 | ? | RX.PharmacyBusiness.ETL | McKesson_Oracle_DW.sql | Complex | RANK/FORMATTING/BUSINESS LOGIC/FUNCTION CALLS/WEGMANS TABLE |
| RXS578 | ? | RX.PharmacyBusiness.ETL | SCRIPTS_WORKLOAD_BALANCING.sql | Complex | DATABASE PACKAGE/WEGMANS TABLE/BUSINESS LOGIC/FORMATTING/UNION |
|  |  | RX.PharmacyBusiness.ETL | Super_Workflow_Steps_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | Workflow_Steps_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | CallPackage.sql | Moderate | PACKAGE CALL |
| RXS580 | ? | RX.PharmacyBusiness.ETL | AtebMaster_YYYYMMDD.sql | Complex | WITH/RANK/BUSINESS LOGIC/FORMATTING/UNION |
| RXS582 | ? | RX.PharmacyBusiness.ETL | McKesson_Oracle_DW_COVID.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | McKesson_Oracle_DW.sql | Moderate | FORMATTING/BUSINESS LOGIC/RANK |
| RXS605 | ? | RX.PharmacyBusiness.ETL | Alternative_Payments_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS608 | ? | RX.PharmacyBusiness.ETL | CC_Payments_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS609 | ? | RX.PharmacyBusiness.ETL | Sold_Detail_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS610 | ? | RX.PharmacyBusiness.ETL | RxReady_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | Super_Duper_Claim_YYYYMMDD.sql | Complex | RANK/FORMATTING/BUSINESS LOGIC/FUNCTION CALLS |
| RXS611 | ? | RX.PharmacyBusiness.ETL | GetSmartOrderPointsMinMax.sql | Simple | MULTIPLE JOINS |
| RXS613 | ? | RX.PharmacyBusiness.ETL | Patients_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS614 | ? | RX.PharmacyBusiness.ETL | Patient_Addresses_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS617 | ? | RX.PharmacyBusiness.ETL | VeriFone_Payments_YYYYMMDD.sql | Moderate | HINTS/FORMATTING/BUSINESS LOGIC |
| RXS622 | ? | RX.Specialty.ETL | GetSpecialtyDispenses.sql | Simple | MULTIPLE JOINS |
| RXS642 | ? | RX.Specialty.ETL | GetClaims.sql | Simple | MULTIPLE JOINS |
| RXS652 | ? | RX.Specialty.ETL | GetCpsCommSettingsByPatientNum.sql | Simple | MULTIPLE JOINS |
|  |  | RX.Specialty.ETL | GetEnterpriseRxDataByPatientNum.sql | Simple | MULTIPLE JOINS |
| RXS660 | ? | RX.PharmacyBusiness.ETL | GetPOAudit_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS661 | ? | RX.PharmacyBusiness.ETL | GetPOFact_YYYYMMDD.sql | Simple | MULTIPLE JOINS |
| RXS710 | ? | RX.PharmacyBusiness.ETL | SelectNDCP3Detail.sql | Simple | MULTIPLE JOINS |
|  |  | RX.PharmacyBusiness.ETL | SelectNDCP3Header.sql | Trivial | ONE TABLE SELECT |
| RXS720 | ? | RX.PharmacyBusiness.ETL | Deceased_YYYYMMDD.sql | Simple | WITH/AS/JOIN |
| RXS777 | ? | RX.PharmacyBusiness.ETL | SelectERxUnauthorized.sql | Moderate | WITH/BUSINESS LOGIC/FUNCTION CALLS |
| RXS812 | ? | RX.PharmacyBusiness.ETL | McKesson_DW_Naloxone.sql | Moderate | WITH/HINTS/BUSINESS LOGIC/REGEX |

\* INN jobs are all done in one step, so these jobs will be an in replacement rather than a single job step