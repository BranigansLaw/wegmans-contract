namespace INN.JobRunner
{
    public static class ControlM
    {
        /// <summary>
        /// Used to get an executable run by Control-M's current directory. Other methods will not work
        /// </summary>
        public static string GetCurrentExecutableDirectory()
        {
            return AppContext.BaseDirectory;
        }
    }
}
