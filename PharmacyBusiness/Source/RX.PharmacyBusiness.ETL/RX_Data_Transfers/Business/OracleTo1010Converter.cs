namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers.Business
{
    using RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class OracleTo1010Converter
    {
        public static StringBuilder Get1010TableDefinition_ForUploadApi(List<OracleSchema> oracleSchema)
        {
            StringBuilder xml = new StringBuilder();

            xml.AppendLine(@"<in>");
            xml.AppendLine(@"<table>");
            xml.Append(@"<title>");
            xml.Append(oracleSchema.First().table_name);
            xml.AppendLine(@"</title>");
            xml.Append(@"<ldesc>");
            xml.Append("Scripts Oracle database archive of table name ");
            xml.Append(oracleSchema.First().table_owner);
            xml.Append(".");
            xml.Append(oracleSchema.First().table_name);
            xml.AppendLine(@"</ldesc>");
            xml.AppendLine(@"<cols>");

            foreach (var oracleColumnDefinition in oracleSchema)
            { 
                xml.Append("<th name=\"");
                xml.Append(oracleColumnDefinition.column_name.ToLower());

                if (oracleColumnDefinition.column_data_type.IndexOf("CHAR") > -1 || 
                    oracleColumnDefinition.column_data_type == "RAW" ||
                    oracleColumnDefinition.column_data_type == "BLOB" ||
                    oracleColumnDefinition.column_data_type == "CLOB")
                    xml.Append("\" type=\"a\" format=\"type:char;width:10\">");
                else if(oracleColumnDefinition.column_data_type == "DATE" ||
                        oracleColumnDefinition.column_data_type == "TIMESTAMP(6)")
                    xml.Append("\" type=\"f\" format=\"type:datehms24;width:1\">");
                else
                    xml.Append("\" type=\"f\" format=\"type:nocommas;width:10\">");

                xml.Append(oracleColumnDefinition.column_name);
                xml.AppendLine("</th>");
            }

            xml.AppendLine(@"</cols>");

            return xml;
        }
        public static List<MultipurposeString> Get1010TableDefinition_ForOdbcTenUp(List<OracleSchema> oracleSchema, string basePath1010)
        {
            StringBuilder xml = new StringBuilder();

            xml.Append("<table name=\"");
            xml.Append(basePath1010);
            xml.Append(".");
            xml.Append(oracleSchema.First().table_owner.ToLower());
            xml.Append(".");
            xml.Append(oracleSchema.First().table_name.ToLower());
            xml.AppendLine("\">");

            xml.Append("<title>");
            xml.Append(oracleSchema.First().table_name);
            xml.AppendLine("</title>");

            xml.Append("<ldesc>");
            xml.Append("Scripts Oracle database archive of table name ");
            xml.Append(oracleSchema.First().table_owner);
            xml.Append(".");
            xml.Append(oracleSchema.First().table_name);
            xml.AppendLine("</ldesc>");

            xml.AppendLine("<cols>");

            foreach (var oracleColumnDefinition in oracleSchema)
            {
                xml.Append("<th name=\"");
                xml.Append(oracleColumnDefinition.column_name.ToLower());

                if (oracleColumnDefinition.column_data_type.IndexOf("CHAR") > -1 ||
                    oracleColumnDefinition.column_data_type == "RAW" ||
                    oracleColumnDefinition.column_data_type == "BLOB" ||
                    oracleColumnDefinition.column_data_type == "CLOB" ||
                    oracleColumnDefinition.column_data_type == "T_NUMBER_TABLE")
                    xml.Append("\" type=\"a\" format=\"type:char;width:10\"");
                else if (oracleColumnDefinition.column_data_type == "DATE" ||
                        oracleColumnDefinition.column_data_type == "TIMESTAMP(6)")
                    xml.Append("\" type=\"f\" format=\"type:datehms24;width:1\"");
                else
                    xml.Append("\" type=\"f\" format=\"type:nocommas;width:10\"");

                xml.Append(" desc=\"Oracle_data_type:'");
                xml.Append(oracleColumnDefinition.column_data_type);
                xml.Append(oracleColumnDefinition.column_data_type == "BLOB" ? " (data moved to an archived image file)" : "");
                xml.Append("', column_id:");
                xml.Append(oracleColumnDefinition.column_id);
                xml.Append(", is_table_key:'");
                xml.Append(oracleColumnDefinition.is_table_key == "Y" ? "Y" : "N");
                xml.Append("'.\">");
                xml.Append(oracleColumnDefinition.column_name);
                xml.AppendLine("</th>");
            }

            xml.AppendLine("</cols>");
            xml.AppendLine("</table>");

            List<MultipurposeString> xmlList = new List<MultipurposeString>();
            xmlList.Add(new MultipurposeString(xml.ToString()));
            return xmlList;
        }

        public static StringBuilder GetOracleSqlWithFormatAllColumnsAsStrings_ForUploadApi(List<OracleSchema> oracleSchema)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ");
            sql.Append(oracleSchema.Count);

            foreach (var oracleColumn in oracleSchema)
            {
                if (oracleColumn.column_data_type.IndexOf("CHAR") > -1)
                {
                    sql.Append(" || '|' || ");
                    sql.Append("REPLACE(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(", '|', '')");
                }
                //else if (oracleColumnDefinition.column_data_type == "CLOB") //NOTE: Scripts uses plain text in these fields.
                //{
                //    sql.Append("dbms_lob.substr(");
                //    sql.Append(oracleColumnDefinition.column_name);
                //    sql.Append(", 64000, 1) AS ");
                //    sql.Append(oracleColumnDefinition.column_name);
                //    sql.Append(" ");
                //}
                else if (oracleColumn.column_data_type == "BLOB") //RXSPECIALTY schema only
                {
                    sql.Append(" || '|' || ");
                    sql.Append("UTL_RAW.CAST_TO_VARCHAR2(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(")");
                    //BTW - To get VARCHAR2 formatted data back to BLOB,
                    //  you can use utl_raw.cast_to_raw to convert a varchar2 into a raw type (no conversion, just a cast).
                }
                else if (oracleColumn.column_data_type == "DATE")
                {
                    sql.Append(" || '|' || ");
                    sql.Append("Round((");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(" - To_Date('20350101','YYYYMMDD')),6)");
                }
                else
                {
                    //oracleTableDefinition.column_data_type == "RAW", which in Scripts is used as plan characters.
                    //oracleColumnDefinition.column_data_type == "CLOB", which in Scripts is used as plan characters.
                    //NUMBER type data.
                    sql.Append(" || '|' || ");
                    sql.Append("REPLACE(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(", '|', '')");
                }
            }
            
            sql.Append(" AS SQL FROM ");
            sql.Append(oracleSchema.First().table_owner);
            sql.Append(".");
            sql.Append(oracleSchema.First().table_name);

            return sql;
        }

        public static List<MultipurposeString> GetOracleSqlWithFormatAllColumns_ForOdbcTenUp(List<OracleSchema> oracleSchema)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT ");

            foreach (var oracleColumn in oracleSchema)
            {
                if (sql.ToString().Replace("\n", "").Trim() != "SELECT")
                    sql.Append(", ");
                else
                    sql.Append("  ");

                if (oracleColumn.column_data_type.IndexOf("CHAR") > -1)
                {
                    sql.AppendLine(oracleColumn.column_name);
                }
                //else if (oracleColumnDefinition.column_data_type == "CLOB") //NOTE: Scripts uses plain text in these fields.
                //{
                //    sql.Append("dbms_lob.substr(");
                //    sql.Append(oracleColumnDefinition.column_name);
                //    sql.Append(", 64000, 1) AS ");
                //    sql.Append(oracleColumnDefinition.column_name);
                //    sql.Append(" ");
                //}
                else if (oracleColumn.column_data_type == "BLOB") //RXSPECIALTY schema only
                {
                    /*
                    sql.Append("A141780.base64encode(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(") AS ");
                    sql.AppendLine(oracleColumn.column_name);
                    //BTW - To get VARCHAR2 formatted data back to BLOB,
                    //  you can use utl_raw.cast_to_raw to convert a varchar2 into a raw type (no conversion, just a cast).
                    */
                    sql.Append("CASE WHEN ");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(" IS NULL THEN NULL ELSE ");

                    sql.Append("'SCRIPTS-");
                    sql.Append(oracleColumn.table_owner);
                    sql.Append("-");
                    sql.Append(oracleColumn.table_name);
                    sql.Append("-");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(".' || rownum || ");
                    sql.Append("'.tif' END AS ");
                    sql.AppendLine(oracleColumn.column_name);

                }
                else if (oracleColumn.column_data_type == "DATE")
                {
                    sql.Append("Round((");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(" - To_Date('20350101','YYYYMMDD')),6)");
                    sql.Append(" AS ");
                    sql.AppendLine(oracleColumn.column_name);
                }
                else if (oracleColumn.column_data_type == "T_NUMBER_TABLE") //"RX.SMARTOPCONFIG" table only
                {
                    sql.AppendLine("(SELECT LISTAGG(COLUMN_VALUE, ',')");
                    sql.AppendLine("   WITHIN GROUP(ORDER BY COLUMN_VALUE)");
                    sql.Append("   FROM TABLE(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(")) AS ");
                    sql.AppendLine(oracleColumn.column_name);
                }
                else
                {
                    //oracleTableDefinition.column_data_type == "RAW", which in Scripts is used as plan characters.
                    //oracleColumnDefinition.column_data_type == "CLOB", which in Scripts is used as plan characters.
                    //NUMBER type data.
                    sql.AppendLine(oracleColumn.column_name);
                }
            }

            sql.Append(" FROM ");
            sql.Append(oracleSchema.First().table_owner);
            sql.Append(".");
            sql.Append(oracleSchema.First().table_name);

            //sql.AppendLine(" WHERE rownum <= 10");

            if (oracleSchema.Where(r => r.column_data_type == "BLOB").Count() > 0)
            {
                //Setting ORDER BY will make rownum values consistent on reruns while Scripts is in a static state (no more updates) during the archiving process.

                if (oracleSchema.First().table_name == "VOB_STAGING")
                    sql.AppendLine(" ORDER BY CASE_ID");

                if (oracleSchema.First().table_name == "VOB")
                    sql.AppendLine(" ORDER BY VOB_ID");

                if (oracleSchema.First().table_name == "JANSSEN_TRIAGE_STAGING")
                    sql.AppendLine(" ORDER BY RECORD_DATE_ID");

                if (oracleSchema.First().table_name == "JANSSEN_TRIAGE")
                    sql.AppendLine(" ORDER BY RECORD_DATE_ID, LASH_PATIENT_ID");

            }

            List<MultipurposeString> sqlList = new List<MultipurposeString>();
            sqlList.Add(new MultipurposeString(sql.ToString()));
            return sqlList;
        }

        public static List<MultipurposeString> GetOracleSqlToDownloadImages(List<OracleSchema> oracleSchema)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT ");

            foreach (var oracleColumn in oracleSchema)
            {
                if (oracleColumn.column_data_type == "BLOB") //RXSPECIALTY schema only
                {
                    /*
                    sql.Append("A141780.base64encode(");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(") AS ");
                    sql.AppendLine(oracleColumn.column_name);
                    //BTW - To get VARCHAR2 formatted data back to BLOB,
                    //  you can use utl_raw.cast_to_raw to convert a varchar2 into a raw type (no conversion, just a cast).
                    */
                    sql.Append("  CASE WHEN ");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(" IS NULL THEN NULL ELSE ");

                    sql.Append("'SCRIPTS-");
                    sql.Append(oracleColumn.table_owner);
                    sql.Append("-");
                    sql.Append(oracleColumn.table_name);
                    sql.Append("-");
                    sql.Append(oracleColumn.column_name);
                    sql.Append(".' || rownum || ");
                    sql.AppendLine("'.tif' END AS IMAGE_FILE_NAME");
                    sql.Append(", ");
                    sql.AppendLine(oracleColumn.column_name);
                }
            }

            sql.Append(" FROM ");
            sql.Append(oracleSchema.First().table_owner);
            sql.Append(".");
            sql.Append(oracleSchema.First().table_name);

            //sql.AppendLine(" WHERE rownum <= 10");

            if (oracleSchema.Where(r => r.column_data_type == "BLOB").Count() > 0)
            {
                //Setting ORDER BY will make rownum values consistent on reruns while Scripts is in a static state (no more updates) during the archiving process.

                if (oracleSchema.First().table_name == "VOB_STAGING")
                    sql.AppendLine(" ORDER BY CASE_ID");

                if (oracleSchema.First().table_name == "VOB")
                    sql.AppendLine(" ORDER BY VOB_ID");

                if (oracleSchema.First().table_name == "JANSSEN_TRIAGE_STAGING")
                    sql.AppendLine(" ORDER BY RECORD_DATE_ID");

                if (oracleSchema.First().table_name == "JANSSEN_TRIAGE")
                    sql.AppendLine(" ORDER BY RECORD_DATE_ID, LASH_PATIENT_ID");

            }

            List<MultipurposeString> sqlList = new List<MultipurposeString>();
            sqlList.Add(new MultipurposeString(sql.ToString()));
            return sqlList;
        }

        public static List<MultipurposeString> GetOracleSqlForRowCount(List<OracleSchema> oracleSchema)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT '");
            sql.Append(oracleSchema.First().table_owner);
            sql.Append("' AS TABLE_OWNER, '");
            sql.Append(oracleSchema.First().table_name);
            sql.Append("' AS TABLE_NAME, To_Char(sysdate,'YYYYMMDD HH24:MI:SS') AS DATE_QUERIED, Count(*) AS ROW_COUNT FROM ");
            sql.Append(oracleSchema.First().table_owner);
            sql.Append(".");
            sql.Append(oracleSchema.First().table_name);

            List<MultipurposeString> sqlList = new List<MultipurposeString>();
            sqlList.Add(new MultipurposeString(sql.ToString()));
            return sqlList;
        }
    }
}
