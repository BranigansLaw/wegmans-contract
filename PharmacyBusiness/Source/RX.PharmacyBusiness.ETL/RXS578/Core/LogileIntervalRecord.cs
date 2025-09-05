using System;

namespace RX.PharmacyBusiness.ETL.RXS578.Core
{
    public class LogileIntervalRecord
    {
        public LogileIntervalRecord()
        {
        }

        public LogileIntervalRecord(CpsRecord cpsRecord, LogileMatrixRecord logileMatrixRecord)
        {
            //The output file is fixed-width and comma delimited.
            //Set the fixed-width using string.Format("{0,<width>}", <data>)
            //See https://stackoverflow.com/questions/644017/net-format-a-string-with-fixed-spaces
            this.INTV_STR_NUM_OUT = string.Format("{0,3}", cpsRecord.StoreNbr);
            this.INTV_DEPT_NAME_OUT = string.Format("{0,-15}", (cpsRecord.StoreNbr == "199") ? "RXCC DEPARTMENT" : "PHRE DEPARTMENT");
            this.INTV_DATE_OUT = string.Format("{0,-10}", cpsRecord.CreateDate.ToString("MM/dd/yyyy"));
            this.INTV_DEP_OUT = string.Format("{0,3}", "007");
            this.INTV_DEP_DESC = string.Format("{0,-7}", string.Empty);
            this.INTV_CAT_OUT = string.Format("{0,2}", "10");
            this.INTV_CAT_DESC = string.Format("{0,-7}", string.Empty);
            this.INTV_CLS_OUT = string.Format("{0,2}", "10");
            this.INTV_CLS_DESC = string.Format("{0,-7}", string.Empty);
            this.FILLER1_OUT = string.Format("{0,2}", string.Empty);
            this.FILLER1_DESC_OUT = string.Format("{0,-7}", string.Empty);
            this.FILLER2_OUT = string.Format("{0,2}", string.Empty);
            this.FILLER2_DESC_OUT = string.Format("{0,-7}", string.Empty);
            this.FILLER3_OUT = string.Format("{0,2}", string.Empty);
            this.FILLER3_DESC_OUT = string.Format("{0,-7}", string.Empty);
            this.INTV_COM_CODE_OUT = string.Format("{0,-64}", ((cpsRecord.StoreNbr == "199") ? "199" : string.Empty) + logileMatrixRecord.LOGILECode);
            this.INTV_DESCRP_OUT = string.Format("{0,-30}", logileMatrixRecord.LOGILEDescription);
            this.INTV_DSD_ITEM_OUT = string.Format("{0,1}", "N");
            this.INTV_DATA_TYPE_OUT = string.Format("{0,1}", "M");
            this.INTV_ITEM_PER_CASE_OUT = string.Format("{0,7}", "0000000");
            //this.INTV_UNITS_OUT = 1; //NOTE: This gets rolled up/summed by Clock Interval "INTV_TIME_OUT".
            this.INTV_UNITS_OUT = string.Empty;
            this.INTV_ITEM_SEL_UNIT_OUT = string.Format("{0:0000000.00}", 1);
            this.INTV_WEIGHT_OUT = string.Format("{0:0000000.00}", 0);
            this.INTV_ITEM_PERLB_OUT = string.Format("{0:0000000.00}", 1);
            this.INTV_ITEM_SHRINK_OUT = string.Format("{0:0000000.00}", 1);
            this.INTV_SALES_OUT = string.Format("{0:0000000.00}", 0);

            /* NOTES regarding "INTV_TIME_OUT" copied from Oracle portion of old job:
             --Round time of day to nearest / previous 15 minute clock time (i.e., 16:28 rounds down to 16:15)
             To_Char((Trunc(v.Measure_Create_Date,'dd') + (Floor(To_Char(v.Measure_Create_Date,'sssss') / 900) / 96)),'HH24:MI') AS Clock_Interval
             * */
            this.INTV_TIME_OUT = string.Format("{0,-5}", new TimeSpan(0, cpsRecord.CreateDate.Hour, cpsRecord.CreateDate.Minute, cpsRecord.CreateDate.Second).RoundToNearestMinutes(15).ToString(@"hh\:mm"));
            this.INTV_DRVR_NAME_OUT = string.Format("{0,-9}", (cpsRecord.StoreNbr == "199") ? "CALL CTR" : "PHARMACY");
        }
        public string INTV_STR_NUM_OUT { get; set; }
        public string INTV_DEPT_NAME_OUT { get; set; }
        public string INTV_DATE_OUT { get; set; }
        public string INTV_DEP_OUT { get; set; }
        public string INTV_DEP_DESC { get; set; }
        public string INTV_CAT_OUT { get; set; }
        public string INTV_CAT_DESC { get; set; }
        public string INTV_CLS_OUT { get; set; }
        public string INTV_CLS_DESC { get; set; }
        public string FILLER1_OUT { get; set; }
        public string FILLER1_DESC_OUT { get; set; }
        public string FILLER2_OUT { get; set; }
        public string FILLER2_DESC_OUT { get; set; }
        public string FILLER3_OUT { get; set; }
        public string FILLER3_DESC_OUT { get; set; }
        public string INTV_COM_CODE_OUT { get; set; }
        public string INTV_DESCRP_OUT { get; set; }
        public string INTV_DSD_ITEM_OUT { get; set; }
        public string INTV_DATA_TYPE_OUT { get; set; }
        public string INTV_ITEM_PER_CASE_OUT { get; set; }
        public string INTV_UNITS_OUT { get; set; }
        public string INTV_ITEM_SEL_UNIT_OUT { get; set; }
        public string INTV_WEIGHT_OUT { get; set; }
        public string INTV_ITEM_PERLB_OUT { get; set; }
        public string INTV_ITEM_SHRINK_OUT { get; set; }
        public string INTV_SALES_OUT { get; set; }
        public string INTV_TIME_OUT { get; set; }
        public string INTV_DRVR_NAME_OUT { get; set; }
    }
}
