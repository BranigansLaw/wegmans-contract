using AstutePatientService;

namespace Wegmans.RX.Orbita.Astute.Extensions
{
    public static class AstutePatientServiceExtensions
    {
        public static AddressResponseFormat CreateDefaultAddressResponseFormat(this AddressResponseFormat addressResponseFormat)
        {
            addressResponseFormat.AddressList = new AddressListFormat
            {
                Address = new AddressFormat()
                {
                    AllAttributes = TrueFalseType.Item1,
                    PhoneList = new PhoneFormat[]
                        {
                            new PhoneFormat()
                            {
                                AllAttributes = TrueFalseType.Item1
                            }
                        }
                }
            };

            return addressResponseFormat;
        }
    }
}
