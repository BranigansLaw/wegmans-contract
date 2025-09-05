# McKesson Interface Architecture

This document outlines a proposed architecture for libraries that interface with the McKesson database (CPS and DW).

## Goals

1. **Encapsulation**: The library should encapsulate what type of data source is used so that if the database is moved (ie. to Snowflake), the library can be changed without having to change the jobs that use it.
2. **Ease of Configuration**: The method called should know what configuration (ie. SQL file) is used just by the type of data or method that is called.
3. **Abstraction**: The method used to connect should be shared and as little code as possible should be duplicated.
4. **Ease of Unit Testing**: Class interfaces will be designed in such a way to allow for easy unit testing.

## Proposed Architecture

Methods describing the data being requested at their return types will be created in `IMcKessonDWInterface` and `IMcKessonCPSInterface`. These methods will handle the interface between the SQL layer and mapping the `DataRow` returns to their correct return types.

All database specific code will be encapsulated in their own interface (ex. `IMcKessonDWOracleInterface` and `IMcKessonCPSMsSqlInterface`).

Additionally, a separate `IMapper` interface class will handle mapping from `DataRow`s to the respective return types.

The distribution of responsibility will allow for easy unit testing. The `McKessonInterfaceImp` tests will focus on making use the correct SQL files and parameters are passed to the correct interface methods (either a query or stored procedure), while the `MapperImp` tests will focus on mapping `DataRow`s to the correct return types.

### Example

*Job.cs*
```csharp
public class GummyBearJob : PharmacyCommandBase
{
    private readonly IMcKessonDWInterface _mckessonDWInterface;
    private readonly ITenTenInterface _tenTenInterface;
    private readonly ILogger<TenTenImportCallRecordDetailFromCisco> _logger;

    public TenTenImportCallRecordDetailFromCisco(
        IMcKessonDWInterface mckessonDWInterface,
        ITenTenInterface tenTenInterface,
        ILogger<TenTenImportCallRecordDetailFromCisco> logger,
        IGenericHelper genericHelper,
        ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
    {
        _mckessonDWInterface = mckessonDWInterface ?? throw new ArgumentNullException(nameof(mckessonDWInterface));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command("tenten-import-gummy-bear-purchases-from-mckesson", Description = "Import gummy bear puchases from McKesson and send to TenTen. Control-M job INN599")]
    public async Task RunAsync(RunForParameter runFor)
    {
        IEnumerable<GummyBearPurchase> gummyBearPurchases =
            await _mckessonDWInterface.GetGummyBearPurchasesAsync(Color.Red, runFor.RunAsDate);

        // Map the returned data and send to TenTen
    }
}
```

*McKessonDWInterfaceImp.cs*
```csharp
public class McKessonDWInterfaceImp : IMcKessonDWInterface
{
    private IMcKessonDWOracleInterface _oracleInterface;
    private IMapper _mapper;

    public McKessonDWInterfaceImp(IMcKessonDWOracleInterface oracleInterface, IMapper mapper)
    {
        _oracleInterface = oracleInterface;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GummyBearPurchase>> GetGummyBearPurchasesAsync(Color gummyBearColor, DataOnly runAs)
    {
        OracleParameter[] oracleParams = new OracleParameter[2];
        oracleParams[0] = new OracleParameter("GummyBearColor", OracleDbType.String, ParameterDirection.Input);
        oracleParams[0].Value = gummyBearColor;
        oracleParams[1] = new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input);
        oracleParams[1].Value = runAs;

        DataSet gummyBearPurchases = await _oracleInterface.OracleQueryAsync(
            sqlFile: "GetGummyBearPurchases.sql",
            queryParams: oracleParams
        );

        return _mapper.MapGummyBearDataSet(drugs);
    }
}
```

*McKessonDWOracleInterfaceImp.cs*
```csharp
public class McKessonDWOracleInterfaceImp : IMcKessonDWOracleInterface
{
    public McKessonDWOracleInterfaceImp(McKessonDWConfig options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _fetchSizeMultiplier = options.FetchSizeMultiplier;
        _oracleDatabaseConnection = options.OracleDatabaseConnection;
        OracleConfiguration.OracleDataSources.Add(options.TnsName, options.TnsDescriptor);
        OracleConfiguration.SelfTuning = options.SelfTuning;
        OracleConfiguration.BindByName = options.BindByName;
        OracleConfiguration.CommandTimeout = options.CommandTimeout;
        OracleConfiguration.FetchSize = options.FetchSize;
        OracleConfiguration.DisableOOB = options.DisableOOB;
    }

    public async Task<DataSet> OracleQueryAsync(string sqlFile, OracleParameter[] queryParams)
    {
        // Proof of Concept code for Oracle connections goes here
    }

    // ... other methods for Oracle connections like Stored Procs, Functions, etc ...
}
```

*MapperImp.cs*
```csharp
public class MapperImp : IMapper
{
    public IEnumerable<GummyBearPurchase> MapGummyBearDataSet(DataSet gummyBearPurchases)
    {
        List<GummyBearPurchase> gummyBearPurchases = new List<GummyBearPurchase>();

        foreach (DataRow row in gummyBearPurchases.Tables[0].Rows)
        {
            gummyBearPurchases.Add(new GummyBearPurchase
            {
                GummyBearColor = row["GummyBearColor"].ToString(),
                GummyBearPurchaseDate = Convert.ToDateTime(row["GummyBearPurchaseDate"]),
                GummyBearPurchaseAmount = Convert.ToInt32(row["GummyBearPurchaseAmount"])
            });
        }

        return gummyBearPurchases;
    }
}
```

*McKessonDWInterfaceImpTests.cs*
```csharp
public class McKessonDWInterfaceImpTests
{
    private readonly Mock<IMcKessonDWOracleInterface> _mockOracleInterface = Substitute.For<IMcKessonDWOracleInterface>();
    private readonly Mock<IMapper> _mockMapper = Substitute.For<IMapper>();

    private readonly McKessonDWInterfaceImp _sut;

    public class McKessonDWInterfaceImpTests()
    {
        _sut = new McKessonDWInterfaceImp(_mockOracleInterface, _mockMapper);
    }

    [Test]
    public async Task GetGummyBearPurchasesAsync_WhenCalled_ShouldCallOracleQueryAsync()
    {
        // Arrange
        Color gummyBearColor = Color.Red;
        DataOnly runAs = new DataOnly(DateTime.Now);
        DataSet expectedDataSet = new DataSet();
        List<GummyBearPurchase> expectedMappedData = new List<GummyBearPurchase>();
        _mockOracleInterface.OracleQueryAsync(
            "GetGummyBearPurchases.sql", 
            It.Is<OracleParameter[]>(/* Check params match inputs */)
        ).Returns(expectedDataSet);
        _mockMapper.MapGummyBearDataSet(expectedDataSet).Returns(expectedMappedData);

        // Act
        var result = await _mcKessonDWInterface.GetGummyBearPurchasesAsync(gummyBearColor, runAs);

        // Assert
        _mockOracleInterface.Received(1).OracleQueryAsync("GetGummyBearPurchases.sql", It.Is<OracleParameter[]>(/* Check params match inputs */));
        _mockOracleInterface.Received(1).MapGummyBearDataSet(expectedDataSet);
        Assert.AreEqual(expectedMappedData, result);
    }
}
```

*MapperImpTests.cs*
```csharp
public class MapperImpTests
{
    private readonly IMapper _sut;

    public class MapperImpTests()
    {
        _sut = new MapperImp();
    }

    [Theory]
    [InlineData("Red", 2024, 5, 14)]
    // ... More data tests
    public void MapGummyBearDataSet_WhenCalled_ShouldMapDataSetToGummyBearPurchases(string color, int year, int month, int day)
    {
        // Arrange
        DataSet gummyBearPurchases = new DataSet();
        DataTable gummyBearPurchasesTable = new DataTable();
        gummyBearPurchasesTable.Columns.Add("GummyBearColor", typeof(string));
        gummyBearPurchasesTable.Columns.Add("GummyBearPurchaseDate", typeof(DateTime));
        gummyBearPurchasesTable.Columns.Add("GummyBearPurchaseAmount", typeof(int));
        gummyBearPurchasesTable.Rows.Add(color, new DateTime(year, month, day));
        gummyBearPurchases.Tables.Add(gummyBearPurchasesTable);

        // Act
        var result = _sut.MapGummyBearDataSet(gummyBearPurchases);

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Red", result.First().GummyBearColor);
        Assert.AreEqual(DateTime.Now, result.First().GummyBearPurchaseDate);
    }
}
```