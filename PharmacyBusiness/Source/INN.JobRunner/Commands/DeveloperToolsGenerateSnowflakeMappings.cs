using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities.SqlFileReader;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.SnowflakeInterface.SnowflakeDbConnectionFactory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snowflake.Data.Client;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace INN.JobRunner.Commands
{
    /// <summary>
    /// This method is used to automatically generate Snowflake mappings and should only be run developers. It can be deleted once all Snowflake SQL files are mapped
    /// </summary>
    public class DeveloperToolsGenerateSnowflakeMappings : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly ISnowflakeDbConnectionFactory _snowflakeDbConnectionFactory;
        private readonly ISqlFileReader _sqlFileReader;
        private readonly IOptions<SnowflakeConfig> _options;
        private readonly ILogger<ScriptsWorkloadBalancing> _logger;

        private static readonly Dictionary<string, string> types = new Dictionary<string, string>()
        {
            { "System.String", "string" },
            { "System.SByte", "sbyte"},
            { "System.Byte", "byte" },
            { "System.Int16", "short" },
            { "System.UInt16", "ushort" },
            { "System.Int32", "int" },
            { "System.UInt32", "uint" },
            { "System.Int64", "long" },
            { "System.UInt64", "ulong" },
            { "System.Char", "char" },
            { "System.Single", "float" },
            { "System.Double", "double" },
            { "System.Boolean", "bool" },
            { "System.Decimal", "decimal" },
            { "System.Void", "void" },
            { "System.Object", "object" },
            { "System.DateTime", "DateTime" },
        };

        public DeveloperToolsGenerateSnowflakeMappings(
            ISnowflakeInterface snowflakeInterface,
            ISnowflakeDbConnectionFactory snowflakeDbConnectionFactory,
            ISqlFileReader sqlFileReader,
            IOptions<SnowflakeConfig> options,
            ILogger<ScriptsWorkloadBalancing> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _snowflakeDbConnectionFactory = snowflakeDbConnectionFactory ?? throw new ArgumentNullException(nameof(snowflakeDbConnectionFactory));
            _sqlFileReader = sqlFileReader ?? throw new ArgumentNullException(nameof(sqlFileReader));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "generate-snowflake-datamappings",
            Description = "Generates Snowflake data mappings"
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            Console.WriteLine("Running Snowflake DataMappings Command");

            //            ICollection<string> commands = [];
            //            ICollection<string> methods = [];
            //            foreach (string file in Directory.GetFiles("../../../../Library.SnowflakeInterface/QueryConfigurations"))
            //            {
            //                string className = new FileInfo(file).Name.Replace(".cs", "");

            //                commands.Add($"await Run{className}Async();");
            //                methods.Add(@"        private async Task Run" + className + @"Async()
            //        {
            //            await GenerateDataFields(new " + className + @"
            //            {
            //                RunDate = new DateOnly(2024, 9, 18),
            //            });
            //        }
            //");
            //            }

            //            foreach (string method in methods)
            //            {
            //                Console.WriteLine(method);
            //            }

            //            foreach (string command in commands)
            //            {
            //                Console.WriteLine(command);
            //            }

            await RunWorkersCompMonthlyQueryAsync();
            await RunAlternativePaymentsQueryAsync();
            await RunAtebMasterQueryAsync();
            await RunCCPaymentsQueryAsync();
            await RunCustomerQueryAsync();
            await RunDailyDrugQueryAsync();
            await RunDeceasedQueryAsync();
            await RunDurConflictQueryAsync();
            await RunETLCheckOracleDWQueryAsync();
            await RunETLInfoForLoggingOracleDWQueryAsync();
            await RunExtendedDrugFileQueryAsync();
            await RunFdsPharmaciesQueryAsync();
            await RunFdsPrescriptionsQueryAsync();
            await RunFillFactQueryAsync();
            await RunGetClaimsQueryAsync();
            await RunGetEnterpriseRxDataByPatientNumQueryAsync();
            await RunGetFillFactsQueryAsync();
            await RunGetIVROutboundCallsQueryAsync();
            await RunGetPOAuditQueryAsync();
            await RunGetPOFactQueryAsync();
            await RunGetSmartOrderMinMaxQueryAsync();
            await RunGetSpecialtyDispensesQueryAsync();
            await RunGettHipaaClaimElementsQueryAsync();
            await RunHpOnePharmaciesQueryAsync();
            await RunInvAdjustmentQueryAsync();
            await RunMcKessonDWNaloxoneQueryAsync();
            await RunMcKessonOracleDWQueryAsync();
            await RunMightBeRefundTransactionsQueryAsync();
            await RunMightBeSoldTransactionsQueryAsync();
            await RunOmnisysClaimQueryAsync();
            await RunPatientAddressesQueryAsync();
            await RunPatientsQueryAsync();
            await RunPetPtNumsQueryAsync();
            await RunPrescriberAddressQueryAsync();
            await RunPrescriberQueryAsync();
            await RunRxReadyQueryAsync();
            await RunRxTransferQueryAsync();
            await RunSelectERxUnauthorizedQueryAsync();
            await RunSelectNewTagPatientGroupsQueryAsync();
            await RunSelectRxErpQueryAsync();
            await RunSelectSoldDetailQueryAsync();
            await RunSelectStoreInventoryHistoryQueryAsync();
            await RunSelectSureScriptsMedicalHistoryQueryAsync();
            await RunSelectSureScriptsPhysicianNotificationLettersQueryAsync();
            await RunSelectThirdPartyClaimsBaseQueryAsync();
            await RunSelectThirdPartyClaimsLookupAcquisitionCostQueryAsync();
            await RunSelectTurnaroundTimeQueryAsync();
            await RunSelectWorkloadBalanceQueryAsync();
            await RunSuperDuperClaimQueryAsync();
            await RunSuperWorkflowStepsQueryAsync();
            await RunSupplierPriceDrugFileQueryAsync();
            await RunTagPatientGroupsQueryAsync();
            await RunTEMPWORKFLOWSTEPQueryAsync();
            await RunVeriFonePaymentsQueryAsync();
            await RunWEG08601For198QueryAsync();
            await RunWEG08601QueryAsync();
            await RunWegmansHPOnePrescriptionsQueryAsync();
        }

        private async Task GenerateDataFields<T>(AbstractQueryConfiguration<T> queryConfiguration) where T : class
        {
            try
            {
                CancellationToken c = CancellationToken.None;
                ICollection<T> toReturn = [];
                string sql = await _sqlFileReader.GetSqlFileContentsAsync(queryConfiguration.QueryFilePath, c).ConfigureAwait(false);

                sql = sql.Replace("{DW}", _options.Value.SnowflakeDWDatabase);
                sql = sql.Replace("{AUD}", _options.Value.SnowflakeAUDDatabase);

                using (SnowflakeDbConnection conn = await _snowflakeDbConnectionFactory.CreateAsync(c).ConfigureAwait(false))
                {
                    try
                    {
                        await conn.OpenAsync(c).ConfigureAwait(false);

                        DbCommand cmd = conn.CreateCommand();
                        cmd.CommandText = sql;
                        queryConfiguration.AddParameters(cmd, (l) => { });
                        DbDataReader reader = await cmd.ExecuteReaderAsync(c).ConfigureAwait(false);

                        Console.WriteLine($"reader.HasRows: {(reader.HasRows ? "true" : "false")}");

                        string dataFileProperties = "";
                        string mapperLines = "";
                        string mockedReaderSetupLines = "";
                        string mockedDataObjectValuesLines = "";
                        string mockedDataPropertyAssignmentLines = "";
                        string mockedNullableDataObjectValuesLines = "";
                        string mockedNullableDataPropertyAssignmentLines = "";

                        int index = 0;
                        foreach (DbColumn column in await reader.GetColumnSchemaAsync())
                        {
                            if (column.DataType == null)
                            {
                                throw new Exception($"{column.ColumnName} had a null DataType");
                            }

                            if (column.DataType.FullName == null)
                            {
                                throw new Exception($"{column.ColumnName} had a null DataType.FullName");
                            }

                            string columnType = types[column.DataType.FullName];
                            bool nullable = column.AllowDBNull == true;
                            string propertyCodeName = ToCamelCase(column.ColumnName);

                            // Data File Properties Generation
                            dataFileProperties += $@"{(index > 0 ? "\n\n" : "")}        public required {columnType}{(nullable ? "?" : "")} {propertyCodeName} {{ get; set; }}";

                            // Mapping File Lines Generation
                            if (columnType == "string")
                            {
                                mapperLines += $@"{(index > 0 ? "\n" : "")}                {propertyCodeName} = reader.GetStringByIndex({index}),";
                            }
                            else
                            {
                                mapperLines += $@"{(index > 0 ? "\n" : "")}                {propertyCodeName} = reader.Get{(nullable ? "Nullable" : "")}ValueByIndex<{columnType}>({index}),";
                            }

                            // Unit Test Mocked Reader Setup Lines
                            string prefix = $@"{(index > 0 ? "\n" : "")}        ";
                            if (columnType == "string")
                            {
                                mockedReaderSetupLines += $@"{prefix}mockedReader.GetString({index}).Returns(readerIndex[{index}]);";
                            }
                            else if (nullable)
                            {
                                mockedReaderSetupLines += $@"{prefix}Util.SetupNullableReturn(mockedReader, readerIndex, {index});";
                            }
                            else
                            {
                                mockedReaderSetupLines += $@"{prefix}mockedReader.GetFieldValue<{columnType}>({index}).Returns(readerIndex[{index}]);";
                            }

                            // Unit Test Mocked Data Lines
                            string testValue = $"\"{Random.Shared.Next(10000, 99999)}\"";
                            prefix = $@"{(index > 0 ? "\n" : "")}                    ";
                            if (columnType == "decimal")
                            {
                                testValue = $"{Random.Shared.Next(10000, 99999)}M";
                            }
                            else if (columnType == "int")
                            {
                                testValue = $"{Random.Shared.Next(10000, 99999)}M";
                            }
                            else if (columnType == "long")
                            {
                                testValue = ((long)(Random.Shared.NextDouble() * long.MaxValue)).ToString();
                            }
                            else if (columnType == "DateTime")
                            {
                                testValue = $"new DateTime({Random.Shared.Next(2010, 2050)}, {Random.Shared.Next(1, 12)}, {Random.Shared.Next(1, 25)})";
                            }

                            string nullableTestValue = testValue;
                            if (nullable)
                            {
                                nullableTestValue = "null";
                            }
                            mockedDataObjectValuesLines += $@"{prefix}{testValue},";
                            mockedDataPropertyAssignmentLines += $@"{prefix}{propertyCodeName} = {testValue},";
                            mockedNullableDataObjectValuesLines += $@"{prefix}{nullableTestValue},";
                            mockedNullableDataPropertyAssignmentLines += $@"{prefix}{propertyCodeName} = {nullableTestValue},";

                            index++;
                        }

                        // Read the query file and extract the class it's mapping to
                        string queryFilePath = $"../../../../Library.SnowflakeInterface/QueryConfigurations/{queryConfiguration.GetType().Name}.cs";
                        string queryTestFileClassName = $"{queryConfiguration.GetType().Name}Tests";
                        string queryTestFilePath = $"../../../../ZZZTest.Library.SnowflakeInterface/QueryConfigurations/{queryTestFileClassName}.cs";
                        string queryFileContents = await File.ReadAllTextAsync(queryFilePath);
                        string dataFileClassName = Regex.Match(queryFileContents, @"AbstractQueryConfiguration<([A-Za-z0-9]+)>").Groups[1].Value;
                        string existingMapperFunction = Regex.Match(queryFileContents, @"public override [A-Za-z0-9]+ MapFromDataReaderToType\(DbDataReader reader, Action<string> logDebug\)\s+{[A-Za-z0-9\(\)\""\';\s{}=,\.\<\>]+\s{8}}").Value;

                        // Insert a line into the mapper for this file
                        string dataFilePath = $"../../../../Library.SnowflakeInterface/Data/{dataFileClassName}.cs";
                        string newQueryFileContents = queryFileContents.Replace(existingMapperFunction, $@"public override {dataFileClassName} MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {{
            logDebug(""Creating {dataFileClassName}"");
            return new {dataFileClassName} {{
{mapperLines}
            }};
        }}");

                        // Insert a line into the properties for the data file this maps to
                        string newDataFileContents = $@"namespace Library.SnowflakeInterface.Data
{{
    public class {dataFileClassName}
    {{
{dataFileProperties}
    }}
}}
";

                        string newUnitTestFileContents = $@"using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class {queryTestFileClassName}
{{
    /// <summary>
    /// Tests that <see cref=""{queryConfiguration.GetType().Name}""/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof({queryTestFileClassName}.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], {dataFileClassName}) testParameters)
    {{
        // Arrange
        (object[] readerIndex, {dataFileClassName} expectedResult) = testParameters;
        {queryConfiguration.GetType().Name}Query query = new()
        {{
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        }};

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
{mockedReaderSetupLines}

        // Act
        {dataFileClassName} res = query.MapFromDataReaderToType(mockedReader, s => {{ }});

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }}

    public class MappingTests : TheoryData<(object[], {dataFileClassName})>
    {{
        public MappingTests()
        {{
            AddRow((
                new object[] {{
{mockedDataObjectValuesLines}
                }},
                new {dataFileClassName}
                {{
{mockedDataPropertyAssignmentLines}
                }}
            ));

            AddRow((
                new object[] {{
{mockedNullableDataObjectValuesLines}
                }},
                new {dataFileClassName}
                {{
{mockedNullableDataPropertyAssignmentLines}
                }}
            ));
        }}
    }}
}}
";
                        // Figure out the data file path so it can be overwritten

                        // Rewrite the files
                        await File.WriteAllTextAsync(queryFilePath, newQueryFileContents);
                        await File.WriteAllTextAsync(dataFilePath, newDataFileContents);
                        await File.WriteAllTextAsync(queryTestFilePath, newUnitTestFileContents);

                        //int counter = 0;
                        //try
                        //{
                        //    while (await reader.ReadAsync(c).ConfigureAwait(false))
                        //    {
                        //        toReturn.Add(queryConfiguration.MapFromDataReaderToType(reader, l => Console.WriteLine(l)));
                        //        counter++;
                        //    }
                        //}
                        //catch (InvalidMappingException e)
                        //{
                        //    throw new RowMapperFailedException(counter, e);
                        //}
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to run {queryConfiguration.GetType()} with error {e.Message}");
            }
        }

        private string ToCamelCase(string prop)
        {
            if (prop.Contains("_"))
            {
                string[] parts = prop.Split('_');
                parts = parts.Select(x => x.Trim()).Select(x => x.Substring(0, 1).ToUpperInvariant() + x.Substring(1).ToLowerInvariant()).ToArray();

                return string.Join("", parts);
            }
            else if (prop.All(c => char.IsNumber(c) || char.IsUpper(c)))
            {
                return prop.Substring(0, 1).ToUpperInvariant() + prop.Substring(1).ToLowerInvariant();
            }

            return prop;
        }

        private async Task RunAlternativePaymentsQueryAsync()
        {
            await GenerateDataFields(new AlternativePaymentsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunAtebMasterQueryAsync()
        {
            await GenerateDataFields(new AtebMasterQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunCCPaymentsQueryAsync()
        {
            await GenerateDataFields(new CCPaymentsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunCustomerQueryAsync()
        {
            await GenerateDataFields(new CustomerQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
                FirstNameContains = "sfjkghdfkghdg",
                Limit = 100,
            });
        }

        private async Task RunDailyDrugQueryAsync()
        {
            await GenerateDataFields(new DailyDrugQuery
            {
            });
        }

        private async Task RunDeceasedQueryAsync()
        {
            await GenerateDataFields(new DeceasedQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunDurConflictQueryAsync()
        {
            await GenerateDataFields(new DurConflictQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunETLCheckOracleDWQueryAsync()
        {
            await GenerateDataFields(new ETLCheckOracleDWQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunETLInfoForLoggingOracleDWQueryAsync()
        {
            await GenerateDataFields(new ETLInfoForLoggingOracleDWQuery
            {
            });
        }

        private async Task RunExtendedDrugFileQueryAsync()
        {
            await GenerateDataFields(new ExtendedDrugFileQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunFdsPharmaciesQueryAsync()
        {
            await GenerateDataFields(new FdsPharmaciesQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunFdsPrescriptionsQueryAsync()
        {
            await GenerateDataFields(new FdsPrescriptionsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunFillFactQueryAsync()
        {
            await GenerateDataFields(new FillFactQuery
            {
                DispensedItemExpirationDate = new DateOnly(2024, 9, 18),
                LocalTransactionDate = new DateTime(2024, 9, 18),
                ReadyDateKey = 5,
                RxNumber = "dfkjdk",
                TotalPricePaid = 15.6M,
            });
        }

        private async Task RunGetClaimsQueryAsync()
        {
            await GenerateDataFields(new GetClaimsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGetEnterpriseRxDataByPatientNumQueryAsync()
        {
            await GenerateDataFields(new GetEnterpriseRxDataByPatientNumQuery
            {
                PatientNumList = "dfgjdfkghdgf",
            });
        }

        private async Task RunGetFillFactsQueryAsync()
        {
            await GenerateDataFields(new GetFillFactsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGetIVROutboundCallsQueryAsync()
        {
            await GenerateDataFields(new GetIVROutboundCallsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGetPOAuditQueryAsync()
        {
            await GenerateDataFields(new GetPOAuditQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGetPOFactQueryAsync()
        {
            await GenerateDataFields(new GetPOFactQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGetSmartOrderMinMaxQueryAsync()
        {
            await GenerateDataFields(new GetSmartOrderMinMaxQuery
            {
            });
        }

        private async Task RunGetSpecialtyDispensesQueryAsync()
        {
            await GenerateDataFields(new GetSpecialtyDispensesQuery
            {
                StartDate = new DateOnly(2024, 7, 1),
                EndDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunGettHipaaClaimElementsQueryAsync()
        {
            await GenerateDataFields(new GettHipaaClaimElementsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunHpOnePharmaciesQueryAsync()
        {
            await GenerateDataFields(new HpOnePharmaciesQuery
            {
            });
        }

        private async Task RunInvAdjustmentQueryAsync()
        {
            await GenerateDataFields(new InvAdjustmentQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunMcKessonDWNaloxoneQueryAsync()
        {
            await GenerateDataFields(new McKessonDWNaloxoneQuery
            {
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 9, 18),
                NdcCsv = "dfkjghdk",
                PrescriberNPicsV = "sfkjghdfgh",
                ReportingEmail = "sjkfghdkh@sgkjhjkgg.con",
                ReportingName = "kjsfghdkjhgf",
                ReportingPhone = "skjghdkjghd",
            });
        }

        private async Task RunMcKessonOracleDWQueryAsync()
        {
            await GenerateDataFields(new McKessonImmunizationsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunMightBeRefundTransactionsQueryAsync()
        {
            await GenerateDataFields(new MightBeRefundTransactionsQuery
            {
                QueryDateDateType = "kdlfgdklgjd",
                QueryDateString = "2024-08-08",
            });
        }

        private async Task RunMightBeSoldTransactionsQueryAsync()
        {
            await GenerateDataFields(new MightBeSoldTransactionsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunOmnisysClaimQueryAsync()
        {
            await GenerateDataFields(new OmnisysClaimQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunPatientAddressesQueryAsync()
        {
            await GenerateDataFields(new PatientAddressesQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunPatientsQueryAsync()
        {
            await GenerateDataFields(new PatientsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunPetPtNumsQueryAsync()
        {
            await GenerateDataFields(new PetPtNumsQuery
            {
            });
        }

        private async Task RunPrescriberAddressQueryAsync()
        {
            await GenerateDataFields(new PrescriberAddressQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunPrescriberQueryAsync()
        {
            await GenerateDataFields(new PrescriberQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunRxReadyQueryAsync()
        {
            await GenerateDataFields(new RxReadyQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunRxTransferQueryAsync()
        {
            await GenerateDataFields(new RxTransferQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectERxUnauthorizedQueryAsync()
        {
            await GenerateDataFields(new SelectERxUnauthorizedQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectNewTagPatientGroupsQueryAsync()
        {
            await GenerateDataFields(new SelectNewTagPatientGroupsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectRxErpQueryAsync()
        {
            await GenerateDataFields(new SelectRxErpQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectSoldDetailQueryAsync()
        {
            await GenerateDataFields(new SelectSoldDetailQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectStoreInventoryHistoryQueryAsync()
        {
            await GenerateDataFields(new SelectStoreInventoryHistoryQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectSureScriptsMedicalHistoryQueryAsync()
        {
            await GenerateDataFields(new SelectSureScriptsMedicalHistoryQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectSureScriptsPhysicianNotificationLettersQueryAsync()
        {
            await GenerateDataFields(new SelectSureScriptsPhysicianNotificationLettersQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectThirdPartyClaimsBaseQueryAsync()
        {
            await GenerateDataFields(new SelectThirdPartyClaimsBaseQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSelectThirdPartyClaimsLookupAcquisitionCostQueryAsync()
        {
            await GenerateDataFields(new SelectThirdPartyClaimsLookupAcquisitionCostQuery
            {
                FillStateKey = "skjhskjghf",
                RxFillSequence = "kdsjfhkjdgf",
                RxRecordNumber = "jskghdkjhg"
            });
        }

        private async Task RunSelectTurnaroundTimeQueryAsync()
        {
            await GenerateDataFields(new SelectTurnaroundTimeQuery
            {
                StartDate = new DateOnly(2024, 7, 18),
                EndDate = new DateOnly(2024, 9, 18),
                TatTarget = "kdfgjdkjfgh",
            });
        }

        private async Task RunSelectWorkloadBalanceQueryAsync()
        {
            await GenerateDataFields(new SelectWorkloadBalanceQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSuperDuperClaimQueryAsync()
        {
            await GenerateDataFields(new SuperDuperClaimQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSuperWorkflowStepsQueryAsync()
        {
            await GenerateDataFields(new SuperWorkflowStepsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunSupplierPriceDrugFileQueryAsync()
        {
            await GenerateDataFields(new SupplierPriceDrugFileQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunTagPatientGroupsQueryAsync()
        {
            await GenerateDataFields(new TagPatientGroupsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunTEMPWORKFLOWSTEPQueryAsync()
        {
            await GenerateDataFields(new TempWorkflowStepQuery
            {
            });
        }

        private async Task RunVeriFonePaymentsQueryAsync()
        {
            await GenerateDataFields(new VeriFonePaymentsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunWEG08601For198QueryAsync()
        {
            await GenerateDataFields(new WEG08601For198Query
            {
            });
        }

        private async Task RunWEG08601QueryAsync()
        {
            await GenerateDataFields(new WEG08601Query
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunWegmansHPOnePrescriptionsQueryAsync()
        {
            await GenerateDataFields(new WegmansHPOnePrescriptionsQuery
            {
                RunDate = new DateOnly(2024, 9, 18),
            });
        }

        private async Task RunWorkersCompMonthlyQueryAsync()
        {
            await GenerateDataFields(new WorkersCompMonthlyQuery 
            {
                RunDate = new DateOnly(2024, 11, 26),
            });
        }
    }
}
