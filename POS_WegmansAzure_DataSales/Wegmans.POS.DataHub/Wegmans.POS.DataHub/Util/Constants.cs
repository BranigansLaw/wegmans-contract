namespace Wegmans.POS.DataHub.Util
{
    public static class Constants
    {
        /// <summary>
        /// If true when running locally in debug, is false in release mode (ie. when deployed)
        /// </summary>
        public const bool RunOnStartup =
#if DEBUG
    true
#else
    false
#endif
;
    }
}
