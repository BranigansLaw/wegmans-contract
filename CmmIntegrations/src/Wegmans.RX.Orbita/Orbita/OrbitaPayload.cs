using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Orbita.Orbita
{
    public static class OrbitaPayload
    {
        /// <summary>
        /// Indents and adds line breaks etc to make it pretty for printing/viewing
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonPrettify(string json)
        {
            var jDoc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    #region JSON Classes
    public class OrbitaJsonPayload
    {
        public OrbitaSystem System { get; set; } = new OrbitaSystem();

        public List<OrbitaPatient> Patients { get; set; }
    }

    public class OrbitaSystem
    {
        public string ApiVersion { get; set; } = "0.2";

        public string RequestDate { get; set; } = DateTime.UtcNow.ToString();
    }

    public class OrbitaPatient
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DOB { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string TransactionID { get; set; }

        public string Result { get; set; }
    }
    #endregion JSON Classes
}
