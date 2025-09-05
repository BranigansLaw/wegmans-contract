using Library.LibraryUtilities.Extensions;
using Library.McKessonCPSInterface.DataModel;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Library.McKessonCPSInterface.DataSetMapper
{
    public class DataSetMapperImp : IDataSetMapper
    {
        private readonly ILogger<DataSetMapperImp> _logger;

        public DataSetMapperImp(
            ILogger<DataSetMapperImp> logger
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IEnumerable<ConversationFactRow> MapConversationFact(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<ConversationFactRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new ConversationFactRow
                {
                    MessageKey = double.Parse(row.NonNullField<string>(0, rowNumber)),
                    ConversationText = row.NonNullField<string>(1, rowNumber),
                    SentDate = row.NonNullField<DateTime>(2, rowNumber),
                    Sender = row.NonNullField<string>(3, rowNumber),
                    CpsConversationNumber = double.Parse(row.NonNullField<string>(4, rowNumber)),
                    DueDate = row.NonNullField<DateTime>(5, rowNumber),
                    FollowUpResponse = row.NonNullField<string>(6, rowNumber),
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<ImmunizationQuestionnaireRow> MapImmunizationQuestionnaires(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<ImmunizationQuestionnaireRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new ImmunizationQuestionnaireRow
                {
                    StoreNbr = row.NonNullField<string>(0, rowNumber),
                    PatientNum = row.NonNullField<int>(1, rowNumber),
                    RxNbr = row.NonNullField<string>(2, rowNumber),
                    RefillNbr = row.NonNullField<int>(3, rowNumber),
                    KeyDesc = row.NonNullField<string>(4, rowNumber),
                    KeyValue = row.NonNullField<string>(5, rowNumber),
                    CreateDate = row.NonNullField<DateTime>(6, rowNumber)
                });

                rowNumber++;
            }

            return result;
        }
    }
}
