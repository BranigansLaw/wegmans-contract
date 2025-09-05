using Library.InformixInterface.DataModel;

namespace Library.InformixInterface.Mapper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps from <paramref name="r"/> to a new <see cref="CsqAgentRow"/>
        /// </summary>
        CsqAgentRow ParseCsqAgentRow(object[] r);

        /// <summary>
        /// Maps from <paramref name="r"/> to a new <see cref="RecDetailRow"/>
        /// </summary>
        RecDetailRow ParseRecDetailRow(object[] r);

        /// <summary>
        /// Maps from <paramref name="r"/> to a new <see cref="StateDetailRow"/>
        /// </summary>
        StateDetailRow ParseStateDetailRow(object[] r);
    }
}
