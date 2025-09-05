using System.Xml.Serialization;
namespace Wegmans.POS.DataHub.ACETransactionModel
{
    [XmlRoot(ElementName = "TransactionRecord_10")]
	public class TransactionRecord10
	{

		[XmlElement(ElementName = "StringType")]
		public int StringType { get; set; }

		[XmlElement(ElementName = "Override")]
		public int Override { get; set; }

		[XmlElement(ElementName = "Reason")]
		public string Reason { get; set; }

		[XmlElement(ElementName = "Index")]
		public int Index { get; set; }

		[XmlElement(ElementName = "Initials")]
		public string Initials { get; set; }

		public string ReasonName
		{
			get
			{
				switch (Reason)
				{
					case "01": return "Exceed price override limit";
					case "02": return "Fall short of minimum price limit";
					case "03": return "Exceed negative entry limit";
					case "04": return "Fall short of minimum misc. item limit";
					case "05": return "Exceed item/misc. limit";
					case "06": return "Cancel item not sold in transaction";
					case "07": return "Accept coupon for item not sold";
					case "08": return "Enter coupon/refund with Department Key";
					case "09": return "Exceed weight override limit";
					case "10": return "Multiply coupon value > item value";
					case "11": return "Item not for sale";
					case "12": return "EMV Security - No AID tendered with non-EMV card";
					case "13": return "Date of birth bypass";
					case "14": return "Exceed volume override limit";
					case "15": return "Exceed discount rate limit";
					case "16": return "Exceed discount amount limit";
					case "17": return "Exceed tax exemption limit";
					case "18": return "Bypass tax exemption ID";
					case "19": return "Invalid Alpha customer ID";
					case "20": return "Bypass tender franking";
					case "21": return "Exceed negative entry limit for trans";
					case "22": return "Exceed negative total limit for trans";
					case "23": return "EMV Security - No AID tendered with EMV card";
					case "24": return "Exceed tender type amount limit";
					case "25": return "Exceed tender type change limit";
					case "26": return "Bypass tender verification";
					case "27": return "Exceed tender verification rejection";
					case "28": return "Perform delayed override";
					case "29": return "Bypass tender verification access failure";
					case "30": return "Item void in negative transaction";
					case "31": return "Item not on scale correctly";
					case "32": return "Accept tender that is not allowed";
					case "33": return "Expired License";
					case "34": return "Keyed Date of Birth";
					case "35": return "Exceed void transaction limit";
					case "36": return "Coupon start date not met";
					case "37": return "Coupon expiration date exceeded";
					case "38": return "Coupon retailer ID not allowed";
					case "39": return "Coupon weight not met";
					case "40": return "Exceed tender exchange limit";
					case "41": return "Bypass tender exchange ranking";
					case "42": return "Exceed tender fee refund limit";
					case "44": return "Special sign-off not authorized";
					case "45": return "Exit special sign-off without password";
					case "46": return "No-sale procedure inside transaction";
					case "47": return "Exceed till exchange limit";
					case "48": return "EMV Security - EMV Chip Error tendered with non-EMV card";
					case "49": return "EMV Security - EMV Chip Error tendered with EMV card";
					case "50": return "Sign-on when operator already active";
					case "51": return "Bypass operator auth. access failure";
					case "52": return "Re-initialize transferred terminal";
					case "55": return "Exceed cashier loan limit";
					case "56": return "Exceed cashier pickup limit";
					case "57": return "Exceed maximum number of suspends allowed";
					case "58": return "Force retrieve is required";
					case "59": return "Force suspend is required";
					case "60": return "No electronic signature";
					case "61": return "Keyed account number";
					case "62": return "SurePOS ACE EPS declined";
					case "63": return "SurePOS ACE EPS limits exceeded";
					case "64": return "GIPC Down";
					case "65": return "Bypass paper error";
					case "66": return "More points needed";
					case "67": return "Keyed customer ID";
					case "68": return "New Customer ID";
					case "69": return "Daily card use limit exceeded";
					case "70": return "Non-sale price verify/change";
					case "71": return "Duplicate prescription";
					case "72": return "Void prescription without a previous sale";
					case "73": return "prescription without a previous sale";
					case "74": return "Prescription information not found";
					case "75": return "Pharmacy application inaccessible";
					case "76": return "Reprint tender receipt";
					case "77": return "Operator age restriction overridden";
					case "78": return "MICR not present";
					case "79": return "Low MICR signal";
					case "80": return "Account number";
					case "81": return "Transit code";
					case "82": return "Check number";
					case "83": return "Foreign transit";
					case "85": return "Exceed rain check override limit";
					case "86": return "Preferred customer not present in transaction";
					case "87": return "Tender Restriction";
					case "88": return "Item Sell By Date Past";
					case "89": return "Immediate quantity restriction";
					default: 
						return Reason;
				}
			}
		}
	}

}