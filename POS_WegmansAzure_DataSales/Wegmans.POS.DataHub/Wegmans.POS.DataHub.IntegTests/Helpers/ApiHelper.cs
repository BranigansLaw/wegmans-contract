using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Wegmans.POS.DataHub.IntegTests.Helpers
{
    public static class ApiHelper
    {
#if RELEASE
        public static string env = "test";
#else
        //Running locally
        public static string env = "local";
#endif

        static Dictionary<string, string> BlobStorage_URL = new Dictionary<string, string>()
        {
            {"test", "https://possalesdatahubtest.blob.core.windows.net/" },
            {"local", "http://127.0.0.1:10000/devstoreaccount1/" }
        };

        public static string GetBlobStorageAddress() => BlobStorage_URL[env];

    }
}
