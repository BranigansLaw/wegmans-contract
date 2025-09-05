using System.Threading;

namespace Library.TenTenInterface.UploadsToTenTen
{
    public abstract class TenTenEtl<T> where T : class
    {
        public abstract string EtlName { get; }

        public abstract TenTenTableSpecifications TenTenTableSpecs { get; }

        public IEnumerable<T> DataToBeUploadedToTenTen { get; set; }

        public DateOnly RunFor { get; set; }

        public int? RowsActuallyUploaded { get; set; }

        public bool EtlWasVerifiedSuccessful()
        {
            //Some ETLs will have zero rows of data to upload, especially on holidays or weekends.
            return this.DataToBeUploadedToTenTen.Count() == (this.RowsActuallyUploaded ?? 0);
        }

        public TenTenEtl(DateOnly runFor)
        {
            this.DataToBeUploadedToTenTen = new List<T>();
            this.RunFor = runFor;
        }

        //public async Task ExecuteAsync(ITenTenInterface _tenTenDataUpload, CancellationToken c)
        //{
        //    //Upload the data to 1010.
        //    await _tenTenDataUpload.CreateOrReplaceTenTenTableAsync<T>(this, c).ConfigureAwait(false);
        //}
    }
}
