using System;
using System.Globalization;
using System.Xml.Serialization;
using Wegmans.POS.DataHub.Util;
namespace Wegmans.POS.DataHub.ACETransactionModel
{
    [XmlRoot(ElementName = "TransactionRecord")]
	public class TransactionRecord21
	{

		[XmlElement(ElementName = "StringType")]
		public int StringType { get; set; }

		[XmlElement(ElementName = "DateTime")]
		public string DateTimeString { get; set; }

		[XmlIgnore()]
		public DateTimeOffset Timestamp
		{
			get { return DateTime.ParseExact(DateTimeString, "yyMMddHHmm", CultureInfo.InvariantCulture).ToEasternStandardTime(); }
        }

		[XmlElement(ElementName = "Indicat1")]
		public string CloseType { get; set; }

		[XmlElement(ElementName = "Indicat2")]
		public Indicat2 Indicat2 { get; set; }

		[XmlElement(ElementName = "AutoPickup")]
		public string AutoPickup { get; set; }

		[XmlElement(ElementName = "CloseProcedure")]
		public int CloseProcedure { get; set; }
	}

}