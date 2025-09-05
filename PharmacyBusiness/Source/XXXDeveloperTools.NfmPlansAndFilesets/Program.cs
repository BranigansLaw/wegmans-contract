using XXXDeveloperTools.NfmPlansAndFilesets;

internal class Program
{
    private static int Main(string[] args)
    {
        FileMoveTracker fileMoveTracker = new FileMoveTracker(Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"));
        return fileMoveTracker.Run();
    }
}

