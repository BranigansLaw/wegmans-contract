namespace Library.SnowflakeInterface.Data
{
    public class SelectSureScriptsMedicalHistoryRow
    {
        public required long Recordsequencenumber { get; set; }

        public required long? Participantpatientid { get; set; }

        public required string? Patientlastname { get; set; }

        public required string? Patientfirstname { get; set; }

        public required string? Patientmiddlename { get; set; }

        public required string? Patientprefix { get; set; }

        public required string? Patientsufix { get; set; }

        public required DateTime? Patientdateofbirth { get; set; }

        public required string? Patientgender { get; set; }

        public required string? Patientaddress1 { get; set; }

        public required string? Patientcity { get; set; }

        public required string? Patientstate { get; set; }

        public required string? Patientzipcode { get; set; }

        public required string? Ncpdpid { get; set; }

        public required string? Chainsiteid { get; set; }

        public required string? Pharmacyname { get; set; }

        public required string? Facilityaddress1 { get; set; }

        public required string? Facilitycity { get; set; }

        public required string? Facilitystate { get; set; }

        public required string? Facilityzipcode { get; set; }

        public required string? Facilityphonenumber { get; set; }

        public required string? Fprimarycareproviderlastname { get; set; }

        public required string? Primarycareproviderfirstname { get; set; }

        public required string? Primarycareprovideraddress1 { get; set; }

        public required string? Primarycareprovidercity { get; set; }

        public required string? Primarycareproviderstate { get; set; }

        public required string? Primarycareproviderzipcode { get; set; }

        public required string? Primarycareproviderareacode { get; set; }

        public required string? Primarycareproviderphonenumber { get; set; }

        public required string? Prescriptionnumber { get; set; }

        public required long? Fillnumber { get; set; }

        public required string? Ndcnumberdispensed { get; set; }

        public required string? Medicationname { get; set; }

        public required decimal? Quantityprescribed { get; set; }

        public required decimal? Quantitydispensed { get; set; }

        public required long? Dayssupply { get; set; }

        public required string? Sigtext { get; set; }

        public required long? Datewritten { get; set; }

        public required long? Datefilled { get; set; }

        public required long? Datepickedupdispensed { get; set; }

        public required decimal? Refillsoriginallyauthorized { get; set; }

        public required decimal? Refillsremaining { get; set; }

        public required long LogicFillfactkey { get; set; }

        public required long? LogicPdpatientkey { get; set; }

        public required string? LogicPatientaddressusage { get; set; }

        public required DateTime? LogicPatientaddresscreatedate { get; set; }

        public required long? LogicPresphonekey { get; set; }

        public required string? LogicPresphonestatus { get; set; }

        public required string? LogicPresphonesourcecode { get; set; }

        public required DateTime? LogicPresphonehlevel { get; set; }
    }
}
