using XXXDeveloperTools.CompareOutputFiles;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Two arguments must be provided: new job name, run date. For example [INN701 07/11/2024].");
            return 99;
        }

        string newJobName = args[0];
        DateTime runDate = DateTime.Parse(args[1]);

        CompareDerivedDataRunner compareDerivedDataRunner = new CompareDerivedDataRunner(newJobName, runDate);
        return compareDerivedDataRunner.Run();
    }
}

