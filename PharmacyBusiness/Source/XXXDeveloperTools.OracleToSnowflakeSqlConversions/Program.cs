using XXXDeveloperTools.OracleToSnowflakeSqlConversions;

internal class Program
{
    private static void Main(string[] args)
    {
        //TODO: Change this to your local directory where the Snowflake SQL files will be saved.
        string snowflakeSqlDirectory = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\SnowflakeSql_Draft02\");

        if (Directory.Exists(snowflakeSqlDirectory) == false)
            Directory.CreateDirectory(snowflakeSqlDirectory);

        OracleToSnowflakeSqlConverter converterTool = new OracleToSnowflakeSqlConverter(false);

        //TODO: Change these paths to your local directories where source Oracle SQL files are located.

        /* Keep for Chris M. 
        string oracleSqlRootDirectory = @"C:\SourceControl\Pharmacy\PharmacyBusiness\Source\RX.PharmacyBusiness.ETL\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = @"C:\SourceControl\Pharmacy\PharmacyBusiness\Source\Library.McKessonDWInterface\SQL\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = @"C:\SourceControl\Pharmacy\Specialty\source\RX.Specialty.ETL\RXS622\SQL\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = @"C:\SourceControl\Pharmacy\Specialty\source\RX.Specialty.ETL\RXS652\SQL\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = @"C:\SourceControl\Pharmacy\Specialty\source\RX.Specialty.ETL\RXS642\SQL\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        */

        //Keep for Mike B.
        
        string oracleSqlRootDirectory = "C:\\Users\\430139\\Source\\Repos\\PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = "C:\\Users\\430139\\Source\\Repos\\PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = "C:\\Users\\430139\\source\\repos\\Specialty\\source\\RX.Specialty.ETL\\RXS622\\SQL\\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = "C:\\Users\\430139\\source\\repos\\Specialty\\source\\RX.Specialty.ETL\\RXS652\\SQL\\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        oracleSqlRootDirectory = "C:\\Users\\430139\\source\\repos\\Specialty\\source\\RX.Specialty.ETL\\RXS642\\SQL\\";
        converterTool.ProcessSqlFiles(oracleSqlRootDirectory, snowflakeSqlDirectory);
        

        //Remember, this is a temporary tool to help with our migration to Snowflake.  It is not a permanent solution or job to be deployed.

    }
}