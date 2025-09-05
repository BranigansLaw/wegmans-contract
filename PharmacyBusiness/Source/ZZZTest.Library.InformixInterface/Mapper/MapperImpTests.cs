using Library.InformixInterface.DataModel;
using Library.InformixInterface.Mapper;

namespace ZZZTest.Library.InformixInterface.Mapper
{
    public class MapperImpTests
    {
        private readonly MapperImp _sut;

        public MapperImpTests()
        {
            _sut = new MapperImp();
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.ParseStateDetailRow"/> parses all fields correctly
        /// </summary>
        [Fact]
        public void ParseStateDetailRow_MapsFields_Correctly()
        {
            // Arrange
            object?[] input = ["John Doe", "vndtalentbridgeADE", "5008592", "2024-02-01T11:12:26.361", "Logged-in", "", 0, null];

            // Act
            StateDetailRow mapped = _sut.ParseStateDetailRow((object[])input);

            // Assert
            Assert.Equal("John Doe", mapped.agent_name);
            Assert.Equal("vndtalentbridgeADE", mapped.agent_login_id);
            Assert.Equal("5008592", mapped.agent_extension);
            Assert.Equal(DateTime.Parse("2024-02-01T11:12:26.361"), mapped.transition_time);
            Assert.Equal("Logged-in", mapped.agent_state);
            Assert.Equal("", mapped.reason_code);
            Assert.Equal(0, mapped.duration);
            Assert.Null(mapped.latestsynchedtime);
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.ParseRecDetailRow"/> parses all fields correctly
        /// </summary>
        [Fact]
        public void ParseRecDetailRow_MapsFields_Correctly()
        {
            // Arrange
            object?[] input = ["John Doe", "vndtalentbridgeYXA", "5001642", null, "2023-01-06T21:41:24.521", "2022-02-01T21:51:41.361", 251, "5005216", "351521679", "RX Janssen Select Queue", null, "RX Janssen Select", 180, 0, 0, "Inbound ACD, Transfer-in", null];

            // Act
            RecDetailRow mapped = _sut.ParseRecDetailRow((object[])input);

            // Assert
            Assert.Equal("John Doe", mapped.agent_name);
            Assert.Equal("vndtalentbridgeYXA", mapped.agent_login_id);
            Assert.Equal("5001642", mapped.agent_extension);
            Assert.Null(mapped.agent_nonipcc_extn);
            Assert.Equal(DateTime.Parse("2023-01-06T21:41:24.521"), mapped.call_start_time);
            Assert.Equal(DateTime.Parse("2022-02-01T21:51:41.361"), mapped.call_end_time);
            Assert.Equal(251, mapped.call_duration);
            Assert.Equal("5005216", mapped.called_number);
            Assert.Equal("351521679", mapped.call_ani);
            Assert.Equal("RX Janssen Select Queue", mapped.call_routed_csq);
            Assert.Null(mapped.other_csqs);
            Assert.Equal("RX Janssen Select", mapped.call_skills);
            Assert.Equal(180, mapped.talk_time);
            Assert.Equal(0, mapped.hold_time);
            Assert.Equal(0, mapped.wrapup_time);
            Assert.Equal("Inbound ACD, Transfer-in", mapped.type_call);
            Assert.Null(mapped.latestsynchedtime);
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.ParseCsqAgentRow"/> parses all fields correctly
        /// </summary>
        [Fact]
        public void ParseCsqAgentRow_MapsFields_Correctly()
        {
            // Arrange
            object?[] input = [1, 175000405952.0, 0, "1-175000405952-0", "2020-01-06T11:44:25", "2024-04-02T12:26:03", 2, "052512", "5005261", "5005162", "Managed Care", 1, "Managed Care Queue*", 2, "John Doe", 2, 75, 0, null];

            // Act
            CsqAgentRow mapped = _sut.ParseCsqAgentRow((object[])input);

            // Assert
            Assert.Equal((short) 1, mapped.node_id);
            Assert.Equal(175000405952.0M, mapped.session_id);
            Assert.Equal((short) 0, mapped.sequence_num);
            Assert.Equal("1-175000405952-0", mapped.session_id_seq);
            Assert.Equal(DateTime.Parse("2020-01-06T11:44:25"), mapped.start_time);
            Assert.Equal(DateTime.Parse("2024-04-02T12:26:03"), mapped.end_time);
            Assert.Equal((short) 2, mapped.contact_disposition);
            Assert.Equal("052512", mapped.originator_dn);
            Assert.Equal("5005261", mapped.destination_dn);
            Assert.Equal("5005162", mapped.called_number);
            Assert.Equal("Managed Care", mapped.application_name);
            Assert.Equal((short) 1, mapped.qindex);
            Assert.Equal("Managed Care Queue*", mapped.csq_names);
            Assert.Equal((short) 2, mapped.queue_time);
            Assert.Equal("John Doe", mapped.agent_name);
            Assert.Equal((short) 2, mapped.ring_time);
            Assert.Equal(75, mapped.talk_time);
            Assert.Equal((short) 0, mapped.work_time);
            Assert.Null(mapped.latestsynchedtime);
        }
    }
}
