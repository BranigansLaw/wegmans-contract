--NOTE: Eons ago...the Wegmans.POSTransaction table was designed to only contain sold POS, not refunds.
--      The SELECT query below provides the business rules for which sold POS records to include.
BEGIN
    INSERT INTO WEGMANS.POSTRANSACTION (
         Facility_Id
        ,Sold_Date
        ,Rx_Number
        ,Refill_Num
        ,Partial_Fill_Seq
        ,POS_Price
        ,Ins_Pays
        ,You_Pay
    )
    SELECT     
         Facility_Id
        ,Sold_Date
        ,Rx_Number
        ,Refill_Num
        ,Partial_Fill_Seq
        ,POS_Price
        ,Ins_Pays
        ,You_Pay
    FROM (
          SELECT
               LPad(To_Char(STORE_NUMBER), 3, '0') AS Facility_Id
              ,SALE_DATE AS Sold_Date
              ,To_Char(RX_NUMBER) AS Rx_Number
              ,REFILL_NUMBER AS Refill_Num
              ,PARTIAL_FILL_SEQ_NUMBER AS Partial_Fill_Seq
              ,RX_PRICE AS POS_Price
              ,INSURANCE_PAY AS Ins_Pays
              ,CUSTOMER_PAY AS You_Pay
              ,TRANSACTION_TYPE AS Transaction_Type
              ,Rank() OVER
                  (PARTITION BY STORE_NUMBER, RX_NUMBER, REFILL_NUMBER, PARTIAL_FILL_SEQ_NUMBER, SALE_DATE
                   ORDER BY STORE_NUMBER, RX_NUMBER, REFILL_NUMBER, PARTIAL_FILL_SEQ_NUMBER, TRANSACTION_TYPE, SALE_DATE Desc
                  ) AS Sold_Rank
              ,Sum(RX_PRICE) OVER 
                  (PARTITION BY STORE_NUMBER, RX_NUMBER, REFILL_NUMBER, PARTIAL_FILL_SEQ_NUMBER, SALE_DATE
                  ) AS Sum_Rx_Price
              ,Count(TRANSACTION_TYPE) OVER 
                  (PARTITION BY STORE_NUMBER, RX_NUMBER, REFILL_NUMBER, PARTIAL_FILL_SEQ_NUMBER, SALE_DATE
                  ) AS Count_Transaction_Type
          FROM Wegmans.SALE
		  WHERE SALE_DATE BETWEEN (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
         )
    WHERE Sold_Rank = 1
      AND Transaction_Type = '0' --Sale
      AND Count_Transaction_Type = 1
      AND (POS_Price = 0 OR Sum_Rx_Price > 0);

    COMMIT;
END;
