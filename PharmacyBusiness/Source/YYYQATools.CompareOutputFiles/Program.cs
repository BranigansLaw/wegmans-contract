using YYYQATools.CompareOutputFiles;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 2 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
        {
            Console.WriteLine("Two arguments must be provided: new file name and path, and old file. For example [\"%BATCH_ROOT%\\ArchiveForQA\\SnowflakeDataFiles\\Wegmans_SOLDDATE_REQ_20240801.TXT\" \"%BATCH_ROOT%\\ArchiveForQA\\InterfaceEngine\\Wegmans_SOLDDATE_REQ_20240801.TXT\"].");
            return 99;
        }

        CompareRunner compareRunner = new CompareRunner(args[0], args[1]);
        return compareRunner.Run();
    }
}
