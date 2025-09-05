namespace Library.LibraryUtilities.GetNow
{
    /// <summary>
    /// Small interface to wrap DateTimeOffset.Now for unit testing purposes
    /// </summary>
    public interface IGetNow
    {
        /// <summary>
        /// Returns <see cref="DateTimeOffset.Now"/>
        /// </summary>
        DateTimeOffset GetNow();

        /// <summary>
        /// Return <see cref="DateTimeOffset.Now"/> in EST timezone
        /// </summary>
        DateTimeOffset GetNowEasternStandardTime();
    }
}
