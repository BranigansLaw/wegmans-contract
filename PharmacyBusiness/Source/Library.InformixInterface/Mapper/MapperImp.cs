using Library.InformixInterface.DataModel;
using Library.LibraryUtilities.Extensions;

namespace Library.InformixInterface.Mapper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public CsqAgentRow ParseCsqAgentRow(object[] r)
        {
            return new CsqAgentRow
            {
                node_id = r[0].ToShort(),
                session_id = r[1].ToDecimal(),
                sequence_num = r[2].ToShort(),
                session_id_seq = r[3].ToNullableString(),
                start_time = r[4].ToDateTime(),
                end_time = r[5].ToDateTime(),
                contact_disposition = r[6].ToShort(),
                originator_dn = r[7].ToNullableString(),
                destination_dn = r[8].ToNullableString(),
                called_number = r[9].ToNullableString(),
                application_name = r[10].ToNullableString(),
                qindex = r[11].ToShort(),
                csq_names = r[12].ToNullableString(),
                queue_time = r[13].ToShort(),
                agent_name = r[14].ToNullableString(),
                ring_time = r[15].ToShort(),
                talk_time = r[16].ToInt(),
                work_time = r[17].ToShort(),
                latestsynchedtime = r[18].ToDateTime(),
            };
        }

        /// <inheritdoc />
        public RecDetailRow ParseRecDetailRow(object[] r)
        {
            return new RecDetailRow
            {
                agent_name = r[0].ToNullableString(),
                agent_login_id = r[1].ToNullableString(),
                agent_extension = r[2].ToNullableString(),
                agent_nonipcc_extn = r[3].ToNullableString(),
                call_start_time = r[4].ToDateTime(),
                call_end_time = r[5].ToDateTime(),
                call_duration = r[6].ToInt(),
                called_number = r[7].ToNullableString(),
                call_ani = r[8].ToNullableString(),
                call_routed_csq = r[9].ToNullableString(),
                other_csqs = r[10].ToNullableString(),
                call_skills = r[11].ToNullableString(),
                talk_time = r[12].ToInt(),
                hold_time = r[13].ToInt(),
                wrapup_time = r[14].ToInt(),
                type_call = r[15].ToNullableString(),
                latestsynchedtime = r[16].ToDateTime(),
            };
        }

        /// <inheritdoc />
        public StateDetailRow ParseStateDetailRow(object[] r)
        {
            return new StateDetailRow
            {
                agent_name = r[0].ToNullableString(),
                agent_login_id = r[1].ToNullableString(),
                agent_extension = r[2].ToNullableString(),
                transition_time = r[3].ToDateTime(),
                agent_state = r[4].ToNullableString(),
                reason_code = r[5].ToNullableString(),
                duration = r[6].ToInt(),
                latestsynchedtime = r[7].ToDateTime(),
            };
        }
    }
}
