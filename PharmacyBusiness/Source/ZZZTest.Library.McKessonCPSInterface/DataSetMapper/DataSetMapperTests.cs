using Library.McKessonCPSInterface.DataModel;
using Library.McKessonCPSInterface.DataSetMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System.Data;

namespace ZZZTest.Library.McKessonCPSInterface.DataSetMapper
{
    public class DataSetMapperTests
    {
        private readonly ILogger<DataSetMapperImp> _loggerMock = Substitute.For<ILogger<DataSetMapperImp>>();

        private readonly DataSetMapperImp _sut;

        public DataSetMapperTests()
        {
            _sut = new DataSetMapperImp(_loggerMock);
        }

        [Fact]
        public void MapOmnisysClaim_MapsFields_ToReturnedList()
        {
            // Arrange
            string messageKey = "14497545.0";
            string convoText = "Test";
            string sentDate = "2023-03-15T00:00:00";
            string sender = "174841";
            string convoNum = "300032067.0";
            string dueDate = "2023-03-21T00:00:00";
            string followUpResponse = "Custom";
            string dataSetJson = string.Format("{{\"Table\":[{{\"message_key\":\"{0}\", \"convo_text\":\"{1}\", \"sent_date\":\"{2}\",\"sender\":\"{3}\", \"convo_num\":\"{4}\", \"due_date\":\"{5}\",\"followup_resp\":\"{6}\"}}]}}", messageKey, convoText, sentDate, sender, convoNum, dueDate, followUpResponse);
            DataSet input = JsonConvert.DeserializeObject<DataSet>(dataSetJson) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<ConversationFactRow> result = _sut.MapConversationFact(input);

            // Assert
            ConversationFactRow singleResult = Assert.Single(result);
            Assert.Equal(double.Parse(messageKey), singleResult.MessageKey);
            Assert.Equal(convoText, singleResult.ConversationText);
            Assert.Equal(DateTimeOffset.Parse(sentDate), singleResult.SentDate);
            Assert.Equal(sender, singleResult.Sender);
            Assert.Equal(double.Parse(convoNum), singleResult.CpsConversationNumber);
            Assert.Equal(DateTimeOffset.Parse(dueDate), singleResult.DueDate);
            Assert.Equal(followUpResponse, singleResult.FollowUpResponse);
        }
    }
}
