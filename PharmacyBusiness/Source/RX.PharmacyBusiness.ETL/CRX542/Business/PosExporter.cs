namespace RX.PharmacyBusiness.ETL.CRX542.Business
{
    using Oracle.ManagedDataAccess.Client;
    using RX.PharmacyBusiness.ETL.CRX542.Core;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.Logging;

    public class PosExporter
    {
        private FileHelper fileHelper { get; set; }
        private OracleHelper oracleHelper { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public PosExporter(ref OracleHelper oracleHelper, string inputLocation, string outputLocation, string archiveLocation, string rejectLocation)
        {
            this.fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);
            this.oracleHelper = oracleHelper; // new OracleHelper();
        }

        /*
        public void ExportPosToScripts(DateTime runDate, IEnumerable<PosRecord> posRecords)
        {
            Log.LogInfo("Upload a total of [{0}] POS records to Scripts Oracle RX schema - TO BE RETIRED!!!", posRecords.Count());

            Log.LogInfo("Purging RX.SALE table");
            OracleParameter[] purgeParams = new OracleParameter[1];
            purgeParams[0] = new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input);
            purgeParams[0].Value = runDate;
            oracleHelper.CallNonQueryAnonymousBlock(
                "SCRIPTS_BATCH",
                @"%BATCH_ROOT%\CRX542\bin\SCRIPTS_POS_DeleteByRunDate.sql",
                purgeParams);

            Log.LogInfo("Upload to RX.SALE table");
            OracleParameter[] uploadParams = new OracleParameter[13];
            uploadParams[0] = new OracleParameter("pSTORE_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[1] = new OracleParameter("pTRANSACTION_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[2] = new OracleParameter("pOPERATOR_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[3] = new OracleParameter("pTERMINAL_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[4] = new OracleParameter("pSALE_DATE", OracleDbType.Date, ParameterDirection.Input);
            uploadParams[5] = new OracleParameter("pRX_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[6] = new OracleParameter("pREFILL_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[7] = new OracleParameter("pPARTIAL_FILL_SEQ_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[8] = new OracleParameter("pTRANSACTION_TYPE", OracleDbType.Char, ParameterDirection.Input);
            uploadParams[9] = new OracleParameter("pRX_PRICE", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[10] = new OracleParameter("pINSURANCE_PAY", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[11] = new OracleParameter("pCUSTOMER_PAY", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[12] = new OracleParameter("pMAIL_FLAG", OracleDbType.Varchar2, ParameterDirection.Input);

            uploadParams[0].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[1].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[2].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[3].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[4].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[5].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[6].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[7].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[8].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[9].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[10].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[11].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[12].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

            int totalUploadedRecordCounter = 0;
            int rowCounterSinceLastCommit = 0;
            int batchCommitSize = 10000;
            List<PosRecord> recordsThisBatch = new List<PosRecord>(batchCommitSize);
            foreach (PosRecord posRecord in posRecords)
            {
                recordsThisBatch.Add(posRecord);
                rowCounterSinceLastCommit++;

                if (batchCommitSize == rowCounterSinceLastCommit ||
                    posRecords.Count() == (rowCounterSinceLastCommit + totalUploadedRecordCounter))
                {
                    //Set Oracle parameter values, then upload that batch of records.
                    uploadParams[0].Value = recordsThisBatch.Select(s => s.Store_Num).ToArray();
                    uploadParams[1].Value = recordsThisBatch.Select(s => s.Transaction_Num).ToArray();
                    uploadParams[2].Value = recordsThisBatch.Select(s => s.Operator).ToArray();
                    uploadParams[3].Value = recordsThisBatch.Select(s => s.Terminal_Num).ToArray();
                    uploadParams[4].Value = recordsThisBatch.Select(s => s.Transaction_Date_Time).ToArray();
                    uploadParams[5].Value = recordsThisBatch.Select(s => s.Rx_Transaction_Num).ToArray();
                    uploadParams[6].Value = recordsThisBatch.Select(s => s.Refill_Num).ToArray();
                    uploadParams[7].Value = recordsThisBatch.Select(s => s.Partial_Fill_Sequence_Num).ToArray();
                    uploadParams[8].Value = recordsThisBatch.Select(s => s.POS_TxType).ToArray();
                    uploadParams[9].Value = recordsThisBatch.Select(s => s.Total_Price).ToArray();
                    uploadParams[10].Value = recordsThisBatch.Select(s => s.Insurance_Amt).ToArray();
                    uploadParams[11].Value = recordsThisBatch.Select(s => s.Copay_Amt).ToArray();
                    uploadParams[12].Value = recordsThisBatch.Select(s => s.Mail_Flag).ToArray();

                    oracleHelper.CallNonQueryAnonymousBlock(
                        "SCRIPTS_BATCH",
                        @"%BATCH_ROOT%\CRX542\bin\SCRIPTS_POS_BulkInsert.sql",
                        uploadParams);

                    //Reset for next bulk insert.
                    totalUploadedRecordCounter += rowCounterSinceLastCommit;
                    recordsThisBatch.Clear();
                    rowCounterSinceLastCommit = 0;
                    Log.LogInfo("With batchCommitSize [{0}], have uploaded [{1}] records so far...", batchCommitSize, totalUploadedRecordCounter);
                }
            }

            if (totalUploadedRecordCounter == posRecords.Count())
                Log.LogInfo("Successfully uploaded [{0}] records to Scripts database, from the set of [{1}] POS records,.", totalUploadedRecordCounter, posRecords.Count());
            else
                throw new Exception(string.Format("Error - [{0}] POS records were uploaded to Scripts database when expecting [{1}] records.", totalUploadedRecordCounter, posRecords.Count()));
        }
        */
        public void ExportPosToMcKesson(DateTime runDate, IEnumerable<PosRecord> posRecords)
        {
            Log.LogInfo("Upload a total of [{0}] POS records to McKesson Oracle WEGMANS schema - TO BE RETIRED!!!", posRecords.Count());

            Log.LogInfo("Purging WEGMANS.SALE and WEGMANS.POSTRANSACTION tables.");
            OracleParameter[] purgeParams = new OracleParameter[1];
            purgeParams[0] = new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input);
            purgeParams[0].Value = runDate;
            oracleHelper.CallNonQueryAnonymousBlock(
                "ENTERPRISE_RX",
                @"%BATCH_ROOT%\CRX542\bin\MCKESSON_POS_DeleteByRunDate.sql",
                purgeParams);

            Log.LogInfo("Upload to WEGMANS.SALE table");
            OracleParameter[] uploadParams = new OracleParameter[12];
            uploadParams[0] = new OracleParameter("pSTORE_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[1] = new OracleParameter("pTRANSACTION_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[2] = new OracleParameter("pOPERATOR_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[3] = new OracleParameter("pTERMINAL_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[4] = new OracleParameter("pSALE_DATE", OracleDbType.Date, ParameterDirection.Input);
            uploadParams[5] = new OracleParameter("pRX_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[6] = new OracleParameter("pREFILL_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[7] = new OracleParameter("pPARTIAL_FILL_SEQ_NUMBER", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[8] = new OracleParameter("pTRANSACTION_TYPE", OracleDbType.Char, ParameterDirection.Input);
            uploadParams[9] = new OracleParameter("pRX_PRICE", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[10] = new OracleParameter("pINSURANCE_PAY", OracleDbType.Decimal, ParameterDirection.Input);
            uploadParams[11] = new OracleParameter("pCUSTOMER_PAY", OracleDbType.Decimal, ParameterDirection.Input);
            //uploadParams[12] = new OracleParameter("pMAIL_FLAG", OracleDbType.Varchar2, ParameterDirection.Input);

            uploadParams[0].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[1].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[2].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[3].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[4].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[5].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[6].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[7].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[8].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[9].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[10].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            uploadParams[11].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            //uploadParams[12].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

            int totalUploadedRecordCounter = 0;
            int rowCounterSinceLastCommit = 0;
            int batchCommitSize = 10000;
            List<PosRecord> recordsThisBatch = new List<PosRecord>(batchCommitSize);
            foreach (PosRecord posRecord in posRecords)
            {
                recordsThisBatch.Add(posRecord);
                rowCounterSinceLastCommit++;

                if (batchCommitSize == rowCounterSinceLastCommit ||
                    posRecords.Count() == (rowCounterSinceLastCommit + totalUploadedRecordCounter))
                {
                    //Set Oracle parameter values, then upload that batch of records.
                    uploadParams[0].Value = recordsThisBatch.Select(s => s.Store_Num).ToArray();
                    uploadParams[1].Value = recordsThisBatch.Select(s => s.Transaction_Num).ToArray();
                    uploadParams[2].Value = recordsThisBatch.Select(s => s.Operator).ToArray();
                    uploadParams[3].Value = recordsThisBatch.Select(s => s.Terminal_Num).ToArray();
                    uploadParams[4].Value = recordsThisBatch.Select(s => s.Transaction_Date_Time).ToArray();
                    uploadParams[5].Value = recordsThisBatch.Select(s => s.Rx_Transaction_Num).ToArray();
                    uploadParams[6].Value = recordsThisBatch.Select(s => s.Refill_Num).ToArray();
                    uploadParams[7].Value = recordsThisBatch.Select(s => s.Partial_Fill_Sequence_Num).ToArray();
                    uploadParams[8].Value = recordsThisBatch.Select(s => s.POS_TxType).ToArray();
                    uploadParams[9].Value = recordsThisBatch.Select(s => s.Total_Price).ToArray();
                    uploadParams[10].Value = recordsThisBatch.Select(s => s.Insurance_Amt).ToArray();
                    uploadParams[11].Value = recordsThisBatch.Select(s => s.Copay_Amt).ToArray();
                    //uploadParams[12].Value = recordsThisBatch.Select(s => s.Mail_Flag).ToArray();

                    oracleHelper.CallNonQueryAnonymousBlock(
                        "ENTERPRISE_RX",
                        @"%BATCH_ROOT%\CRX542\bin\MCKESSON_POS_BulkInsert.sql",
                        uploadParams);

                    //Reset for next bulk insert.
                    totalUploadedRecordCounter += rowCounterSinceLastCommit;
                    recordsThisBatch.Clear();
                    rowCounterSinceLastCommit = 0;
                    Log.LogInfo("With batchCommitSize [{0}], have uploaded [{1}] records so far...", batchCommitSize, totalUploadedRecordCounter);
                }
            }

            if (totalUploadedRecordCounter == posRecords.Count())
                Log.LogInfo("Successfully uploaded [{0}] records to McKesson database WEGMANS.SALE table, from the set of [{1}] POS records,.", totalUploadedRecordCounter, posRecords.Count());
            else
                throw new Exception(string.Format("Error - [{0}] POS records were uploaded to McKesson database when expecting [{1}] records.", totalUploadedRecordCounter, posRecords.Count()));

            Log.LogInfo("Upload only sold POS records to McKesson database WEGMANS.POSTRANSACTION table.");
            OracleParameter[] onlySoldParams = new OracleParameter[1];
            onlySoldParams[0] = new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input);
            onlySoldParams[0].Value = runDate;
            oracleHelper.CallNonQueryAnonymousBlock(
                "ENTERPRISE_RX",
                @"%BATCH_ROOT%\CRX542\bin\MCKESSON_OnlySoldPos_Insert.sql",
                onlySoldParams);     
        }

        public void ExportPosToFileForSFTPto1010data(DateTime runDate, IEnumerable<PosRecord> posRecords)
        {
            Log.LogInfo("Output [{0}] POS records to a file going to 1010data.", posRecords.Count());
            List<PosFor1010dataRecord_SFTP> posFor1010 = new List<PosFor1010dataRecord_SFTP>();
            foreach (var posRecord in posRecords)
            {
                posFor1010.Add(new PosFor1010dataRecord_SFTP(posRecord));
            }
            fileHelper.WriteListToFile<PosFor1010dataRecord_SFTP>(
                posFor1010,
                @"%BATCH_ROOT%\CRX542\Output\POSTransactionERx_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                ",",
                string.Empty,
                true);
        }

        public void ExportPosToFileForTENUPto1010data(DateTime runDate, IEnumerable<PosRecord> posRecords)
        {
            this.ExportPosToFileForTENUPto1010data(runDate, posRecords, string.Empty);
        }

        public void ExportPosToFileForTENUPto1010data(DateTime runDate, IEnumerable<PosRecord> posRecords, string outputFilePrefix)
        {
            Log.LogInfo("Output [{0}] POS records to a file going to 1010data.", posRecords.Count());
            List<PosFor1010dataRecord_TENUP> posFor1010 = new List<PosFor1010dataRecord_TENUP>();
            foreach (var posRecord in posRecords)
            {
                if (outputFilePrefix == "ERXCERT_")
                {
                    if (posRecord.Store_Num == 25)
                    {
                        //The EDW Team cannot process sales from Test Store 275, so
                        //they send us POS for Store 25, and we can to convert that to 275.
                        posRecord.Store_Num = 275;
                    }
                }

                posFor1010.Add(new PosFor1010dataRecord_TENUP(posRecord));
            }
            fileHelper.WriteListToFile<PosFor1010dataRecord_TENUP>(
                posFor1010,
                @"%BATCH_ROOT%\CRX542\Output\" + outputFilePrefix + "POSTransactionERx_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                ",",
                string.Empty,
                true);
        }
    }
}
