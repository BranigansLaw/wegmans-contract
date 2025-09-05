using Library.TenTenInterface.Exceptions;
using Library.TenTenInterface.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Library.TenTenInterface.TenTenResponseParser
{
    public class TenTenResponseParserImp : ITenTenResponseParser
    {
        private readonly ILogger<TenTenResponseParserImp> _logger;

        public TenTenResponseParserImp(ILogger<TenTenResponseParserImp> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T ParseTenTenResponse<T>(HttpResponseMessage res) where T : class, ITenTenResponse
        {
            _logger.LogDebug($"Status code returned from TenTen: {res.StatusCode}");

            res.EnsureSuccessStatusCode();

            T toReturn = ParseXmlResponseFromStream<T>(res.Content.ReadAsStream());

            HandleReturnCode(toReturn);

            return toReturn;
        }

        private T ParseXmlResponseFromStream<T>(Stream s) where T : class
        {
            XmlSerializer serializer = new(typeof(T));

            T? toReturn = (T?)serializer.Deserialize(s);

            if (toReturn is null)
            {
                string xml;
                using (var reader = new StreamReader(s))
                {
                    xml = reader.ReadToEnd();
                }

                throw new TenTenXmlSerializeFailedException(xml, typeof(T));
            }

            return toReturn;
        }

        private void HandleReturnCode(ITenTenResponse tenTenResponse)
        {
            switch (tenTenResponse.ResponseCode)
            {
                case 0:
                    return;
                case 1:
                    if (tenTenResponse.Message != null && tenTenResponse.Message.Contains("requires special permissions", StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new SpecialPermissionsRequiredException(tenTenResponse.Message);
                    }
                    else if (tenTenResponse.Message != null && tenTenResponse.Message.Contains("already exists AS A DIR", StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new TenTenDirectoryAlreadyExistsException(tenTenResponse.Message);
                    }
                    else if (tenTenResponse.Message != null && tenTenResponse.Message.Contains("Requested columns not found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new RequestedColumnsNotFoundException(tenTenResponse.Message);
                    }
                    else
                    {
                        throw new TimeoutException(tenTenResponse.Message);
                    }
                case 4:
                    throw new InvalidUsernameOrPasswordException(tenTenResponse.Message);
                case 5:
                    throw new TooManyActiveLoginsException(tenTenResponse.Message);
                case 19:
                    throw new TenTenDirectoryAlreadyExistsException(tenTenResponse.Message);
                case 25:
                    throw new MaterializeQueryFailedException(tenTenResponse.Message);
                case 35:
                    throw new DuplicateSessionException(tenTenResponse.Message);
                case 38:
                    throw new SystemBusyException(tenTenResponse.Message);
                case 45:
                    throw new LoginThrottlingException(tenTenResponse.Message);
                default:
                    throw new UnknownReturnCodeException(tenTenResponse.ResponseCode, tenTenResponse.Message);
            }
        }
    }
}
