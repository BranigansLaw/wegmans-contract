using INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper
{
    public class MapperImpTests
    {
        private readonly MapperImp _sut;

        public MapperImpTests()
        {
            _sut = new MapperImp();
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.MapAndOrderToTenTenAdm340BOpportunity(IEnumerable{Adm340BOpportunityRow})"/> returns data mapped to a <see cref="Adm340BOpportunity"/>
        /// </summary>
        [Fact]
        public void MapToTenTenAdm340BOpportunity_Returns_MappedObject()
        {
            // Arrange
            Adm340BOpportunityRow toMap = new()
            {
                AccountId = "93485093485",
                ApprPkg = 234.23M,
                ContractId = "dfg98df0g8",
                ContractName = "skgljslkfjsfj",
                DerivedDrugNdc = "sd90fg8dgf098dg",
                DerivedProcessDate = new DateOnly(2024, 1, 12),
                DerivedRunDate = new DateOnly(2024, 1, 15),
                DerivedStoreNum = 94380,
                DrugName = "skjdksdjgg",
                DrugPackSize = "dfg8dgf90dfg",
                DrugStrength = "dsfg08df09g8dg",
                IsPool = "fgd09fg809dfd",
                NdcWo = "fdklgdfgljkdfg",
                OrderDate = new DateTime(2024, 1, 25),
                OrdPkg = 23.12M,
                WholesalerName = "df9g8d0",
                WholesalerNum = "dfg98d0fg9df"
            };

            // Act
            IEnumerable<Adm340BOpportunity> result = _sut.MapAndOrderToTenTenAdm340BOpportunity([toMap]);

            // Assert
            Adm340BOpportunity mapped = Assert.Single(result);
            Assert.Equal(toMap.AccountId, mapped.AccountId);
            Assert.Equal(toMap.ApprPkg, mapped.ApprPkg);
            Assert.Equal(toMap.ContractId, mapped.ContractId);
            Assert.Equal(toMap.ContractName, mapped.ContractName);
            Assert.Equal(toMap.DerivedProcessDate, mapped.DerivedProcessDate);
            Assert.Equal(toMap.DerivedRunDate, mapped.DerivedRunDate);
            Assert.Equal(toMap.DerivedStoreNum, mapped.DerivedStoreNum);
            Assert.Equal(toMap.DrugName, mapped.DrugName);
            Assert.Equal(toMap.DrugPackSize, mapped.DrugPackSize);
            Assert.Equal(toMap.DrugStrength, mapped.DrugStrength);
            Assert.Equal(toMap.IsPool, mapped.IsPool);
            Assert.Equal(toMap.NdcWo, mapped.NdcWo);
            Assert.Equal(toMap.OrderDate, mapped.OrderDate);
            Assert.Equal(toMap.OrdPkg, mapped.OrdPkg);
            Assert.Equal(toMap.WholesalerName, mapped.WholesalerName);
            Assert.Equal(toMap.WholesalerNum, mapped.WholesalerNum);
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.MapAndOrderToTenTenAdm340BOpportunity(IEnumerable{Adm340BOpportunityRow})"/> returns data mapped to a <see cref="Adm340BOpportunity"/>
        /// </summary>
        [Fact]
        public void MapAdm340BOpportunityRowToTenTenAzureRow_Returns_MappedObject()
        {
            // Arrange
            Adm340BOpportunityRow toMap = new()
            {
                AccountId = "93485093485",
                ApprPkg = 234.23M,
                ContractId = "dfg98df0g8",
                ContractName = "skgljslkfjsfj",
                DerivedDrugNdc = "sd90fg8dgf098dg",
                DerivedProcessDate = new DateOnly(2024, 1, 12),
                DerivedRunDate = new DateOnly(2024, 1, 15),
                DerivedStoreNum = 94380,
                DrugName = "skjdksdjgg",
                DrugPackSize = "dfg8dgf90dfg",
                DrugStrength = "dsfg08df09g8dg",
                IsPool = "fgd09fg809dfd",
                NdcWo = "fdklgdfgljkdfg",
                OrderDate = new DateTime(2024, 1, 25),
                OrdPkg = 23.12M,
                WholesalerName = "df9g8d0",
                WholesalerNum = "dfg98d0fg9df"
            };

            // Act
            IEnumerable<Adm340BOpportunityTenTenRow> result = _sut.MapAdm340BOpportunityRowToTenTenAzureRow([toMap]);

            // Assert
            Adm340BOpportunityTenTenRow mapped = Assert.Single(result);
            Assert.Equal(toMap.AccountId, mapped.AccountId);
            Assert.Equal(toMap.ApprPkg, mapped.ApprPkg);
            Assert.Equal(toMap.ContractId, mapped.ContractId);
            Assert.Equal(toMap.ContractName, mapped.ContractName);
            Assert.Equal(toMap.DerivedProcessDate, mapped.DerivedProcessDate);
            Assert.Equal(toMap.DerivedRunDate, mapped.DerivedRunDate);
            Assert.Equal(toMap.DerivedStoreNum, mapped.DerivedStoreNum);
            Assert.Equal(toMap.DrugName, mapped.DrugName);
            Assert.Equal(toMap.DrugPackSize, mapped.DrugPackSize);
            Assert.Equal(toMap.DrugStrength, mapped.DrugStrength);
            Assert.Equal(toMap.IsPool, mapped.IsPool);
            Assert.Equal(toMap.NdcWo, mapped.NdcWo);
            Assert.Equal(toMap.OrderDate, mapped.OrderDate);
            Assert.Equal(toMap.OrdPkg, mapped.OrdPkg);
            Assert.Equal(toMap.WholesalerName, mapped.WholesalerName);
            Assert.Equal(toMap.WholesalerNum, mapped.WholesalerNum);
        }
    }
}
