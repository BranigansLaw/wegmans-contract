namespace RX.PharmacyBusiness.ETL.CRX540.Contracts
{
    /// <summary>
    /// Contract for a generic rule
    /// </summary>
    /// <typeparam name="T">Type the rule will be check against</typeparam>
    public interface IRule<T>
    {
        /// <summary>
        /// Gets the failure reason when the rule is not passed
        /// </summary>
        string FailReason { get; }

        /// <summary>
        /// Executes the rule against the entity
        /// </summary>
        /// <param name="entity">Entity that will be checked</param>
        /// <returns>True if rule passes</returns>
        bool IsMetBy(T entity);
    }
}
