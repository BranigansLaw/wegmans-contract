                                  Oracle Syntax                                     |                                         Snowflake Syntax
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
 sysdate                                                                            | sysdate()
                                                                                    |
 Interval 'HH:MM:SS' HOUR TO SECOND                                                 | INTERVAL 'HH Hours, MM Minutes, SS seconds'
                                                                                    |
 TO_CHAR(TO_DATE('20240821','YYYYMMDD'))                                            | TO_CHAR(TO_DATE('20240821','YYYYMMDD')::DATE, 'YYYYMMDD')
                                                                                    | 
 TRUNC(SOME NUMBER)                                                                 | TRUNC(SOME NUMBER)
                                                                                    | 
 TRUNC(SOME DATE)                                                                   | TRUNC(SOME DATE, 'PART OF DAY TO TRUNCATE TO')
                                                                                    | 
 DECODE(COLUMN, SEARCH1, REPLACE1 ...) **1                                          | DECODE(COLUMN, SEARCH1, REPLACE1 ...) **1
                                                                                    | 
 NVL(E1, E2)                                                                        | NVL(E1, E2)
                                                                                    | 
 SELECT DISTINCT                                                                    | SELECT DISTINCT
                                                                                    | 
 SELECT UNIQUE                                                                      | SELECT DISTINCT
                                                                                    | 
 REGEXP_REPLACE(source, pattern [**2, replacement, position, occurence, parameters])| REGEXP_REPLACE(source, pattern [**2, replacement, position, occurence, parameters])
                                                                                    | 
 REGEXP_COUNT(source, pattern [**2, position, parameters])                          | REGEXP_COUNT(source, pattern [**2, position, parameters])
                                                                                    | 
 EXISTS                                                                             | EXISTS
                                                                                    |
 TRIM(to_char(DRUG_NDC,'00000g0000g00','nls_numeric_characters=.-'))                | TRIM(LPAD(SUBSTR(DRUG_NDC::TEXT, 1, 5), 5, '0') || '-' ||
                                                                                    |      LPAD(SUBSTR(DRUG_NDC::TEXT, 6, 4), 4, '0') || '-' ||
                                                                                    |      LPAD(SUBSTR(DRUG_NDC::TEXT, 10, 2), 2, '0')
                                                                                    |
 TO_CHAR(PATIENT_PRICE_PAID,'000000.00')                                            | TO_CHAR(PATIENT_PRICE_PAID, '000000.00')
                                                                                    |
 AVG()                                                                              | AVG()
                                                                                    |
 TRANSLATE(column, from, to)                                                        | TRANSLATE(column, from, to)
                                                                                    |
 NULLIF(expr1, expr2)                                                               | NULLIF(expr1, expr2)
                                                                                    |
 CEIL(NUMBER)                                                                       | CEIL(NUMBER)
                                                                                    |
 LISTAGG([DISTINCT] column, delimeter)                                              | LISTAGG([DISTINCT] column, delimeter)                                                                            |
                                                                                    |
 NEXT_DAY(date, dow)                                                                | NEXT_DAY(date, dow)
                                                                                    |
 ROWNUM                                                                             | select ROW_NUMBER() OVER (ORDER BY some column) as rownum ... ORDER BY rownum
                                                                                    |
 ROWID                                                                              | select ROW_NUMBER() OVER (ORDER BY some column unique to each row) as rowid ... ORDER BY rowid
                                                                                    |
 CAST(FROM_TZ(CAST(AUD__RX_FILL.DATESTAMP AS TIMESTAMP), 'UTC')                     | CONVERT_TIMEZONE('UTC', 'US/Eastern', CAST(AUD__RX_FILL.DATESTAMP AS TIMESTAMP))
                            AT TIME ZONE 'US/EASTERN' AS DATE)                      |
                                                                                    |
ListAgg(Distinct To_Char(sea.SAE_START_TIME,'HH24:MI:SS'),',')                      | ListAgg(Distinct TO_CHAR(sea.SAE_START_TIME, 'HH24:MI:SS'), ',')                                                                               
         Within Group (ORDER BY sea.SAE_START_TIME) AS Login_Times_CSV              |          WITHIN GROUP (ORDER BY TO_CHAR(sea.SAE_START_TIME, 'HH24:MI:SS')) AS Login_Times_CSV
                                                                                    |
                                                                                    |
                                                                                    |
            

**1 - "..." is not included in the actual syntax, it is a way of showing that it can be used N times.
**2 - inside of the brackets "[]" are optional parameters.
# reference 
- https://wegmans.sharepoint.com/sites/DevStore&Rx/SiteAssets/PharmacyandInovationTeamHub/
- https://docs.snowflake.com/en/reference