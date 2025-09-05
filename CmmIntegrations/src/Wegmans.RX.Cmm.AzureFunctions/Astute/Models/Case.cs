using System;
using System.Collections.Generic;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class Case
    {
        private static readonly Dictionary<string, string> ProductNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "57894064001", "TREMFYA 100 MG/ML SYRINGE" },
            { "57894064011", "TREMFYA 100 MG/ML INJECTOR" },
            { "57894007001", "SIMPONI 50 MG/0.5 ML SYRINGE" },
            { "57894007002", "SIMPONI 50 MG/0.5 ML PEN INJECT" },
            { "57894007101", "SIMPONI 100 MG/ML SYRINGE" },
            { "57894007102", "SIMPONI 100 MG/ML PEN INJECTOR" },
            { "57894006002", "STELARA 45 MG/0.5 ML VIAL" },
            { "57894006003", "STELARA 45 MG/0.5 ML SYRINGE" },
            { "57894006103", "STELARA 90 MG/ML SYRINGE" }
        };

        public string Id { get; set; }

        public string ReferralId { get; set; }

        public int AstuteCaseId { get; set; } = -1;

        public int AstuteAddressId { get; set; } = 0;

        public string DiagnosisCode { get; set; }

        public string ProgramName { get; set; }

        public string Ndc { get; set; }

        public string ExceptionReason { get; set; }

        public string EnrollmentPayload { get; set; }

        public string CaseStatus { get; set; }

        public bool? ShipStarterDoseToPatient { get; set; }

        public bool? ShipStarterDoseToProvider { get; set; }
        
        public string AstuteShipFirstDoseToPrescriber
        {
            get
            {
                bool shipToPatient = this.ShipStarterDoseToPatient ?? false;
                bool shipToProvider = this.ShipStarterDoseToProvider ?? false;
                if (shipToPatient && !shipToProvider)
                {
                    return "N";
                }
                else if (!shipToPatient && shipToProvider)
                {
                    return "Y";
                }
                else
                {
                    return null;
                }
            }
        }

        public string AstuteProductName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Ndc) && ProductNameMap.ContainsKey(this.Ndc))
                {
                    return ProductNameMap[this.Ndc];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string BaseProductName
        {
            get
            {
                var baseProductName = string.Empty;
                if (!string.IsNullOrEmpty(this.AstuteProductName) && this.AstuteProductName.Contains(" "))
                {
                    baseProductName = this.AstuteProductName.Substring(0, this.AstuteProductName.IndexOf(" "));
                }
                return baseProductName;
            }
        }

        public string AstuteProgramHeader
        {
            get
            {
                return this.ProgramName.Replace(" ", "");
            }
        }

        public string WorkflowStatus
        {
            get
            {
                return this.ProgramName switch
                {
                    "So Simple" => "Day 0",
                    _ => "New Link - Patient Follow-Up",
                };
            }
        }

        public Provider Provider { get; set; }
    }
}
