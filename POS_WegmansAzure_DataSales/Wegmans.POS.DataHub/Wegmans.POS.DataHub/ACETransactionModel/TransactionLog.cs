using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.TranslationEnumModel;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.ACETransactionModel
{
    [XmlRoot(ElementName = "Indicat0")]
    public class Indicat0
    {

        [XmlElement(ElementName = "Bit0")]
        public bool Bit0 { get; set; }

        [XmlElement(ElementName = "Bit1")]
        public bool Bit1 { get; set; }

        [XmlElement(ElementName = "Bit2")]
        public bool Bit2 { get; set; }

        [XmlElement(ElementName = "Bit3")]
        public bool Bit3 { get; set; }

        [XmlElement(ElementName = "Bit4")]
        public bool Bit4 { get; set; }

        [XmlElement(ElementName = "Bit5")]
        public bool Bit5 { get; set; }

        [XmlElement(ElementName = "Bit6")]
        public bool Bit6 { get; set; }

        [XmlElement(ElementName = "Bit7")]
        public bool Bit7 { get; set; }
    }

    [XmlRoot(ElementName = "LogFlags")]
    public class LogFlags
    {

        [XmlElement(ElementName = "Bit0")]
        public bool Bit0 { get; set; }

        [XmlElement(ElementName = "Bit1")]
        public bool Bit1 { get; set; }

        [XmlElement(ElementName = "Bit2")]
        public bool Bit2 { get; set; }

        [XmlElement(ElementName = "Bit3")]
        public bool Bit3 { get; set; }

        [XmlElement(ElementName = "Bit4")]
        public bool Bit4 { get; set; }

        [XmlElement(ElementName = "Bit5")]
        public bool Bit5 { get; set; }

        [XmlElement(ElementName = "Bit6")]
        public bool Bit6 { get; set; }

        [XmlElement(ElementName = "Bit7")]
        public bool Bit7 { get; set; }

        [XmlElement(ElementName = "Bit8")]
        public bool Bit8 { get; set; }

        [XmlElement(ElementName = "Bit9")]
        public bool Bit9 { get; set; }

        [XmlElement(ElementName = "Bit10")]
        public bool Bit10 { get; set; }

        [XmlElement(ElementName = "Bit11")]
        public bool Bit11 { get; set; }

        [XmlElement(ElementName = "Bit12")]
        public bool Bit12 { get; set; }

        [XmlElement(ElementName = "Bit13")]
        public bool Bit13 { get; set; }

        [XmlElement(ElementName = "Bit14")]
        public bool Bit14 { get; set; }

        [XmlElement(ElementName = "Bit15")]
        public bool Bit15 { get; set; }
    }

    [XmlRoot(ElementName = "Indicat1")]
    public class Indicat1
    {

        [XmlElement(ElementName = "Bit0")]
        public bool Bit0 { get; set; }

        [XmlElement(ElementName = "Bit1")]
        public bool Bit1 { get; set; }

        [XmlElement(ElementName = "Bit2")]
        public bool Bit2 { get; set; }

        [XmlElement(ElementName = "Bit3")]
        public bool Bit3 { get; set; }

        [XmlElement(ElementName = "Bit4")]
        public bool Bit4 { get; set; }

        [XmlElement(ElementName = "Bit5")]
        public bool Bit5 { get; set; }

        [XmlElement(ElementName = "Bit6")]
        public bool Bit6 { get; set; }

        [XmlElement(ElementName = "Bit7")]
        public bool Bit7 { get; set; }

        [XmlElement(ElementName = "Bit8")]
        public bool Bit8 { get; set; }

        [XmlElement(ElementName = "Bit9")]
        public bool Bit9 { get; set; }

        [XmlElement(ElementName = "Bit10")]
        public bool Bit10 { get; set; }

        [XmlElement(ElementName = "Bit11")]
        public bool Bit11 { get; set; }

        [XmlElement(ElementName = "Bit12")]
        public bool Bit12 { get; set; }

        [XmlElement(ElementName = "Bit13")]
        public bool Bit13 { get; set; }

        [XmlElement(ElementName = "Bit14")]
        public bool Bit14 { get; set; }

        [XmlElement(ElementName = "Bit15")]
        public bool Bit15 { get; set; }

        [XmlElement(ElementName = "Bit16")]
        public bool Bit16 { get; set; }

        [XmlElement(ElementName = "Bit17")]
        public bool Bit17 { get; set; }

        [XmlElement(ElementName = "Bit18")]
        public bool Bit18 { get; set; }

        [XmlElement(ElementName = "Bit19")]
        public bool Bit19 { get; set; }

        [XmlElement(ElementName = "Bit20")]
        public bool Bit20 { get; set; }

        [XmlElement(ElementName = "Bit21")]
        public bool Bit21 { get; set; }

        [XmlElement(ElementName = "Bit22")]
        public bool Bit22 { get; set; }

        [XmlElement(ElementName = "Bit23")]
        public bool Bit23 { get; set; }

        [XmlElement(ElementName = "Bit24")]
        public bool Bit24 { get; set; }

        [XmlElement(ElementName = "Bit25")]
        public bool Bit25 { get; set; }

        [XmlElement(ElementName = "Bit26")]
        public bool Bit26 { get; set; }

        [XmlElement(ElementName = "Bit27")]
        public bool Bit27 { get; set; }

        [XmlElement(ElementName = "Bit28")]
        public bool Bit28 { get; set; }

        [XmlElement(ElementName = "Bit29")]
        public bool Bit29 { get; set; }

        [XmlElement(ElementName = "Bit30")]
        public bool Bit30 { get; set; }

        [XmlElement(ElementName = "Bit31")]
        public bool Bit31 { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_00")]
    public class TransactionRecord00
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }

        [XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public string DateTimeString { get; set; }

        [XmlIgnore()]
        public DateTimeOffset Timestamp
        {
            get { return DateTime.ParseExact(DateTimeString, "yyMMddHHmm", CultureInfo.InvariantCulture).ToEasternStandardTime(); }
        }

        public TransactionType? TransactionTypeEnum
        {
            get{
                if(int.TryParse(TransactionType, out var outvar))
                {
                    return (TransactionType?)outvar;
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlElement(ElementName = "TransactionType")]
        public string TransactionType { get; set; }



        [XmlElement(ElementName = "NumberOfStrings")]
        public string NumberOfStrings { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "GrossPositive")]
        public decimal GrossPositive { get; set; }

        [XmlElement(ElementName = "GrossNegative")]
        public decimal GrossNegative { get; set; }

        [XmlElement(ElementName = "RingTime")]
        public string RingSeconds { get; set; }

        [XmlElement(ElementName = "TenderTime")]
        public string TenderSeconds { get; set; }

        [XmlElement(ElementName = "SpecialTime")]
        public string SpecialTime { get; set; }

        [XmlElement(ElementName = "InactiveTime")]
        public string InactiveSeconds { get; set; }

        [XmlElement(ElementName = "Indicat1")]
        public Indicat1 Indicat1 { get; set; }

        [XmlElement(ElementName = "AltPrice")]
        public string AltPrice { get; set; }
        public String00AltPriceEnum AltPrice00Enum
        {
            get{
                if(int.TryParse(AltPrice, out var number))
                {
				    return (String00AltPriceEnum)number; 
                }
                else
                {
                    return String00AltPriceEnum.Unknown;
                }
            }
        }


        [XmlElement(ElementName = "VoidTrc")]
        public string VoidTrc { get; set; }

        public String00VoidTransactionEnum? VoidTransaction00Enum
        {
            get{
                if(int.TryParse(VoidTrc, out var number))
                {
				    return (String00VoidTransactionEnum)number; 
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlElement(ElementName = "UserValue")]
        public string UserValue { get; set; }
    }

    [XmlRoot(ElementName = "Indicat2")]
    public class Indicat2
    {

        [XmlElement(ElementName = "Bit0")]
        public bool Bit0 { get; set; }

        [XmlElement(ElementName = "Bit1")]
        public bool Bit1 { get; set; }

        [XmlElement(ElementName = "Bit2")]
        public bool Bit2 { get; set; }

        [XmlElement(ElementName = "Bit3")]
        public bool Bit3 { get; set; }

        [XmlElement(ElementName = "Bit4")]
        public bool Bit4 { get; set; }

        [XmlElement(ElementName = "Bit5")]
        public bool Bit5 { get; set; }

        [XmlElement(ElementName = "Bit6")]
        public bool Bit6 { get; set; }

        [XmlElement(ElementName = "Bit7")]
        public bool Bit7 { get; set; }

        [XmlElement(ElementName = "Bit8")]
        public bool Bit8 { get; set; }

        [XmlElement(ElementName = "Bit9")]
        public bool Bit9 { get; set; }

        [XmlElement(ElementName = "Bit10")]
        public bool Bit10 { get; set; }

        [XmlElement(ElementName = "Bit11")]
        public bool Bit11 { get; set; }

        [XmlElement(ElementName = "Bit12")]
        public bool Bit12 { get; set; }

        [XmlElement(ElementName = "Bit13")]
        public bool Bit13 { get; set; }

        [XmlElement(ElementName = "Bit14")]
        public bool Bit14 { get; set; }

        [XmlElement(ElementName = "Bit15")]
        public bool Bit15 { get; set; }
    }

    [XmlRoot(ElementName = "Indicat4")]
    public class Indicat4
    {

        [XmlElement(ElementName = "Bit0")]
        public bool Bit0 { get; set; }

        [XmlElement(ElementName = "Bit1")]
        public bool Bit1 { get; set; }

        [XmlElement(ElementName = "Bit2")]
        public bool Bit2 { get; set; }

        [XmlElement(ElementName = "Bit3")]
        public bool Bit3 { get; set; }

        [XmlElement(ElementName = "Bit4")]
        public bool Bit4 { get; set; }

        [XmlElement(ElementName = "Bit5")]
        public bool Bit5 { get; set; }

        [XmlElement(ElementName = "Bit6")]
        public bool Bit6 { get; set; }

        [XmlElement(ElementName = "Bit7")]
        public bool Bit7 { get; set; }

        [XmlElement(ElementName = "Bit8")]
        public bool Bit8 { get; set; }

        [XmlElement(ElementName = "Bit9")]
        public bool Bit9 { get; set; }

        [XmlElement(ElementName = "Bit10")]
        public bool Bit10 { get; set; }

        [XmlElement(ElementName = "Bit11")]
        public bool Bit11 { get; set; }

        [XmlElement(ElementName = "Bit12")]
        public bool Bit12 { get; set; }

        [XmlElement(ElementName = "Bit13")]
        public bool Bit13 { get; set; }

        [XmlElement(ElementName = "Bit14")]
        public bool Bit14 { get; set; }

        [XmlElement(ElementName = "Bit15")]
        public bool Bit15 { get; set; }

        [XmlElement(ElementName = "Bit16")]
        public bool Bit16 { get; set; }

        [XmlElement(ElementName = "Bit17")]
        public bool Bit17 { get; set; }

        [XmlElement(ElementName = "Bit18")]
        public bool Bit18 { get; set; }

        [XmlElement(ElementName = "Bit19")]
        public bool Bit19 { get; set; }

        [XmlElement(ElementName = "Bit20")]
        public bool Bit20 { get; set; }

        [XmlElement(ElementName = "Bit21")]
        public bool Bit21 { get; set; }

        [XmlElement(ElementName = "Bit22")]
        public bool Bit22 { get; set; }

        [XmlElement(ElementName = "Bit23")]
        public bool Bit23 { get; set; }

        [XmlElement(ElementName = "Bit24")]
        public bool Bit24 { get; set; }

        [XmlElement(ElementName = "Bit25")]
        public bool Bit25 { get; set; }

        [XmlElement(ElementName = "Bit26")]
        public bool Bit26 { get; set; }

        [XmlElement(ElementName = "Bit27")]
        public bool Bit27 { get; set; }

        [XmlElement(ElementName = "Bit28")]
        public bool Bit28 { get; set; }

        [XmlElement(ElementName = "Bit29")]
        public bool Bit29 { get; set; }

        [XmlElement(ElementName = "Bit30")]
        public bool Bit30 { get; set; }

        [XmlElement(ElementName = "Bit31")]
        public bool Bit31 { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_01")]
    public class TransactionRecord01
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; } //not needed

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "ItemCode")]
        public string UniversalProductCode { get; set; }

        [XmlElement(ElementName = "ExtendedPrice")]
        public decimal ExtendedPrice { get; set; }

        [XmlElement(ElementName = "DepartmentNumber")]
        public string DepartmentNumber { get; set; }

        [XmlElement(ElementName = "CouponFamilyNumber")]
        public string CouponFamilyNumber { get; set; }

        [XmlElement(ElementName = "Indicat0")]
        public Indicat1 Indicat0 { get; set; }

        [XmlElement(ElementName = "Indicat1")]
        public Indicat1 Indicat1 { get; set; }

        [XmlElement(ElementName = "Indicat2")]
        public Indicat2 Indicat2 { get; set; }

        [XmlElement(ElementName = "Indicat3")]
        public string Indicat3 { get; set; }

        public String01ItemTypeEnum ItemType01Enum
        {
            get{
                if(int.TryParse(Indicat3.Substring(0, 1), out var number))
                {
				    return (String01ItemTypeEnum)number; 
                }
                else
                {
                    return String01ItemTypeEnum.NotAvailable;
                }
            }
        }

        public String01OperatorEntryMethod OperatorEntryMethod01Enum
        {
            get{
                if(int.TryParse(Indicat3.Substring(1, 1), out var number))
                {
				    return (String01OperatorEntryMethod)number; 
                }
                else
                {
                    return String01OperatorEntryMethod.NotAvailable;
                }
            }
        }
       

        [XmlElement(ElementName = "Indicat4")]
        public Indicat4 Indicat4 { get; set; }

        [XmlElement(ElementName = "OrdinalNumber")]
        public int OrdinalNumber { get; set; }

        [XmlElement(ElementName = "VoidedOrdinalNumber")]
        public int VoidedOrdinalNumber { get; set; }
        
    }

    [XmlRoot(ElementName = "TransactionRecord_02")]
    public class TransactionRecord02
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "MultiPricingGroup")]
        public string MultiPricingGroup { get; set; }

        [XmlElement(ElementName = "DealQuantity")]
        public string DealQuantity { get; set; }

        [XmlElement(ElementName = "PricingMethod")]
        public string PricingMethod { get; set; }

        [XmlElement(ElementName = "SaleQuantity")]
        public int SaleQuantity { get; set; }

        [XmlElement(ElementName = "SalePrice")]
        public decimal SalePrice { get; set; }

        [XmlElement(ElementName = "QtyOrWgtOrVol")]
        public string QuantityOrWeightOrVolume { get; set; }

        [XmlElement(ElementName = "Indicat1")]
        public Indicat1 Indicat1 { get; set; }

        [XmlElement(ElementName = "UserValue")]
        public string UserValue { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_03")]
    public class TransactionRecord03
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "DiscountGroup")]
        public string GroupId { get; set; }

        [XmlElement(ElementName = "DiscountRate")]
        public decimal Percent { get; set; }

        [XmlElement(ElementName = "DiscountAmount")]
        public decimal Amount { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_04")]
    public class TransactionRecord04
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "DiscountGroup")]
        public string GroupId { get; set; }

        [XmlElement(ElementName = "DiscountRate")]
        public decimal Percent { get; set; }

        [XmlElement(ElementName = "DiscountAmount")]
        public decimal Amount { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.0.11")]
    public class TransactionRecord99011
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "BarCodeType")]
        public int BarCodeType { get; set; }

        [XmlElement(ElementName = "BarCodeData")]
        public double BarCodeData { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.0.50")]
    public class TransactionRecord99050
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "RefundReason")]
        public int RefundReason { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_99.0.96")]
    public class TransactionRecord99096
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Plan")]
        public string Plan { get; set; }

        [XmlElement(ElementName = "AccountType")]
        public int AccountType { get; set; }

        [XmlElement(ElementName = "RequestType")]
        public int RequestType { get; set; }

        [XmlElement(ElementName = "Amount")]
        public int Amount { get; set; }

        [XmlElement(ElementName = "Sequence")]
        public string Sequence { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public string DateTime { get; set; }

        [XmlElement(ElementName = "AccountNumber")]
        public string AccountNumber { get; set; }

        [XmlElement(ElementName = "EntryMethod")]
        public string EntryMethod { get; set; }

        [XmlElement(ElementName = "ApprovalCode")]
        public string ApprovalCode { get; set; }

        [XmlElement(ElementName = "ApprovalCodeSource")]
        public string ApprovalCodeSource { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Flags")]
        public int Flags { get; set; }

        [XmlElement(ElementName = "LotteryItemRRN")]
        public string LotteryItemRRN { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_05")]
    public class TransactionRecord05
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TenderEntrySequenceNumber")]
        public int TenderEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public string TenderType { get; set; }

        [XmlElement(ElementName = "TenderAmount")]
        public decimal TenderAmount { get; set; }

        [XmlElement(ElementName = "TenderCashingFee")]
        public decimal TenderCashingFee { get; set; }

        [XmlElement(ElementName = "CustomerAccount")]
        public string CustomerAccount { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_06")]
    public class TransactionRecord06
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TenderEntrySequenceNumber")]
        public int TenderEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public string TenderType { get; set; }

        [XmlElement(ElementName = "TenderAmount")]
        public decimal TenderAmount { get; set; }

        [XmlElement(ElementName = "TenderCashingFee")]
        public decimal TenderCashingFee { get; set; }

        [XmlElement(ElementName = "CustomerAccount")]
        public string LoyaltyNumber { get; set; }

        [XmlElement(ElementName = "Status")]
        public int Status { get; set; }
    }

     [XmlRoot(ElementName = "TransactionRecord_09")]

    public class TransactionRecord09
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public string TenderType { get; set; }

        [XmlElement(ElementName = "TenderAmount")]
        public decimal TenderAmount { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_16")]
    public class TransactionRecord16
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "HostID")]
        public string HostId { get; set; }

        [XmlElement(ElementName = "TotalsType")]
        public int TotalsType { get; set; }
        /// <summary>
        ///  Returns the Totals Type value based on the string value
        /// </summary>
		public String16TotalTypeEnum TotalType16Enum
        {
            get{
                return (String16TotalTypeEnum)TotalsType;
            }
        }

        [XmlElement(ElementName = "TotalAmount")]
        public decimal TotalAmount { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public string DateTimeString16 { get; set; }

        [XmlIgnore()]
        public DateTimeOffset Timestamp
        {
            get { return DateTime.ParseExact(DateTimeString16, "yyMMddHHmmss", CultureInfo.InvariantCulture).ToEasternStandardTime(); }
        }

        [XmlElement(ElementName = "ActionCode")]
        public string ActionCode { get; set; }
		/// <summary>
        ///  Returns the Action Code value based on the string value
        /// </summary>
        public string getActionCode()
        {
            switch (ActionCode)
            {
                case "A00":
                    return "Approved";
				case "C1":
                    return "CallForAuth";
				case "D94":
                    return "DuplicateTransaction";
				case "I104":
                    return "InvalidTender";
                default:
                    return ActionCode;
            }
        }

        [XmlElement(ElementName = "RRN")]
        public string RelativeRecordNumber { get; set; }

        [XmlElement(ElementName = "Sequence")]
        public int Sequence { get; set; }

        [XmlElement(ElementName = "MessageType")]
        public int MessageType { get; set; }
        		/// <summary>
        ///  Returns the Message Type value based on the string value
        /// </summary>
		public String16MessageTypeEnum MessageType16Enum
        {
            get{
				return (String16MessageTypeEnum)MessageType; 
            }
        }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }


        [XmlElement(ElementName = "ReasonCode")]
        public int EpsFailureReason { get; set; }
		/// <summary>
        ///  Returns the Reason Code value based on the int value
        /// </summary>
		public String16ReasonCodeEnum ReasonCode16Enum
        {
            get{
				return (String16ReasonCodeEnum)EpsFailureReason; 
            }
        }

        [XmlElement(ElementName = "ExpirationDate")]
        public int ExpirationDate { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public int TenderType { get; set; }
        /// <summary>
        ///  Returns the Entry Method value based on the string value
        /// </summary>
		public String16TenderTypeEnum TenderType16Enum
        {
            get{
				return (String16TenderTypeEnum)TenderType; 
            }
        }

        [XmlElement(ElementName = "CashBack")]
        public decimal CashBack { get; set; }

        [XmlElement(ElementName = "Account")]
        public string Account16 { get; set; }

        [XmlIgnore()]
        public string Account
        {
            get { return Account16.Substring(Account16.Length - 4); }
        }

        [XmlElement(ElementName = "EntryMethod")]
        public string EntryMethod { get; set; }
        /// <summary>
        ///  Returns the Entry Method value based on the string value
        /// </summary>
        public string getEntryMethod()
        {
            switch (EntryMethod)
            {
                case "C":
                    return "OCR";
				case "H":
                    return "SwipedTrack1";
				case "D":
                    return "SwipedTrack2";
				case "E":
                    return "SwipedTrack1";
				case "R":
                    return "RawSwipedTrackData";
				case "F":
                    return "ContactlessTrack2";
				case "G":
                    return "ContactlessTrack1";
				case "K":
                    return "KeyedCheck";
				case "T":
                    return "KeyedTrack";
				case "Y":
                    return "KeyedLicense";
				case "M":
                    return "MicrData";
				case "S":
                    return "Scanner";
				case "L":
                    return "EmvFallbackSwiped";
				case "I":
                    return "EmvFallbackKeyed";
				case "V":
                    return "EmvContactChip";
				case "W":
                    return "EmvContactless";
                default:
                    return EntryMethod;
            }
        }

        private string _ID1;
        [XmlElement(ElementName = "ID1")]
        public string TrackTwo 
        { 
            get { return _ID1; }

            set { _ID1 = value.Trim(); }
            
        }

        [XmlElement(ElementName = "ApprovalCode")]
        public string ApprovalCode { get; set; }

        [XmlElement(ElementName = "RCDescription")]
        public string ResultCodeDescription { get; set; }

        [XmlElement(ElementName = "ApprovalCodeSource")]
        public string ApprovalCodeSource { get; set; }
        /// <summary>
        ///  Returns the Approval Code Source value based on the string value
        /// </summary>
        public string getApprovalCodeSource()
        {
            switch (ApprovalCodeSource)
            {
                case "A":
                    return "CalledForAuthorization";
				case "E":
                    return "FromSurePosAceEpsApplication";
				case "H":
                    return "FromHost";
				case "W":
                    return "FromWorkstation";
                default:
                    return ApprovalCodeSource;
            }
        }

        [XmlElement(ElementName = "OriginalHostResponseCode")]
        public int OriginalHostResponseCode { get; set; }


        [XmlElement(ElementName = "HostResponseIndicator")]
        public int HostResponseIndicator { get; set; }
        /// <summary>
        ///  Returns the Host Response Indicator value based on the string value
        /// </summary>
        public string getHostResponseIndicator()
        {
            switch (HostResponseIndicator)
            {
                case 1:  return "HostIsOnline";
                default: return "HostIsOffline";
            }
        }

        [XmlElement(ElementName = "Flags1")]
        public int Flags1 { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.0.3")]
    public class TransactionRecord9903
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TenderEntrySequenceNumber")]
        public int TenderEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "SignatureLength")]
        public int SignatureLength { get; set; }

        [XmlElement(ElementName = "SignatureData")]
        public string SignatureData { get; set; }

        [XmlElement(ElementName = "Reserved")]
        public int Reserved { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.0.4")]
    public class TransactionRecord9904
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "TenderEntrySequenceNumber")]
        public int TenderEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "SignatureSource")]
        public string SignatureSource { get; set; }
        /// <summary>
        ///  Returns the Signature Type value based on the string value
        /// </summary>
        public string getSignatureSource()
        {
            switch (SignatureSource)
            {
                case "F":
                    return "Financial";
				case "P":
                    return "Pharmacy";
                default:
                    return SignatureSource;
            }
        }

        [XmlElement(ElementName = "SignatureFormat")]
        public string SignatureFormat { get; set; }

        private string _StoreName;
        [XmlElement(ElementName = "StoreName")]
        public string SignatureName
        {
            get { return _StoreName; }

            set { _StoreName = value.Trim(); }

        }

    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.11")]
    public class TransactionRecord991011
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public string DateTime { get; set; }

        [XmlElement(ElementName = "TerminalNumber")]
        public int TerminalNumber { get; set; }

        [XmlElement(ElementName = "TransactionNumber")]
        public int TransactionNumber { get; set; }

        [XmlElement(ElementName = "QhcpSubtotal")]
        public decimal QhcpSubtotal { get; set; }

        [XmlElement(ElementName = "QhcpDiscountTotal")]
        public decimal QhcpDiscountTotal { get; set; }

        [XmlElement(ElementName = "QhcpSalesTax")]
        public decimal QhcpSalesTax { get; set; }

        [XmlElement(ElementName = "QhcpTotal")]
        public decimal QhcpTotal { get; set; }

        [XmlElement(ElementName = "TransactionSubtotal")]
        public decimal TransactionSubtotal { get; set; }

        [XmlElement(ElementName = "TransactionDiscountTotal")]
        public decimal TransactionDiscountTotal { get; set; }

        [XmlElement(ElementName = "TransactionSalesTax")]
        public decimal TransactionSalesTax { get; set; }

        [XmlElement(ElementName = "TransactionTotal")]
        public decimal TransactionTotal { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.12")]
    public class TransactionRecord991012
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "CAUPC")]
        public string UniversalProductCode { get; set; }
		
		[XmlElement(ElementName = "Void")]
        public bool Void { get; set; }
		
		[XmlElement(ElementName = "ServiceDeptUPC")]
        public string ServiceDeptUPC { get; set; }
		
		[XmlElement(ElementName = "ServiceDept")]
        public string ServiceDept { get; set; }
		
		[XmlElement(ElementName = "NumItems")]
        public string NumberOfItems { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.14")]
    public class TransactionRecord991014
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "CAUPC")]
        public string UniversalProductCode { get; set; }
		
		[XmlElement(ElementName = "Item")]
        public string ItemNumber { get; set; }
		
		[XmlElement(ElementName = "Quantity")]
        public string Quantity { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.26")]
    public class TransactionRecord991026
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "QRCodeInTransaction")]
        public int QRCodeInTransaction { get; set; }

		[XmlElement(ElementName = "BIN")]
        public string BIN { get; set; }

        [XmlElement(ElementName = "Last4Digits")]
        public string LastFour { get; set; }

        [XmlElement(ElementName = "ICBypass")]
        public string Bypass { get; set; }
        [XmlElement(ElementName = "ICBypassOrderID")]
        public string BypassOrderId { get; set; }
        [XmlElement(ElementName = "ICBypassDeliveryID")]
        public string BypassDeliveryId { get; set; }
        [XmlElement(ElementName = "ShopperType")]
        public string ShopperType { get; set; }
        [XmlElement(ElementName = "OrderingPlatform")]
        public string OrderingPlatform { get; set;}
    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.28")]
    public class TransactionRecord991028
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "IDField")]
        public string Id { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.29")]
    public class TransactionRecord991029
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "IDField")]
        public string Id { get; set; }

    }

	[XmlRoot(ElementName = "TransactionRecord_99.10.31")]
    public class TransactionRecord991031
    {
         
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "IDField")]
        public string Id { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_11.DD")]
    public class TransactionRecord11DD
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Unused")]
        public int Unused { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public string SubStringType { get; set; }

        [XmlElement(ElementName = "LogFlags")]
        public LogFlags LogFlags { get; set; }

        [XmlElement(ElementName = "CampaignNumber")]
        public string CampaignId { get; set; }

        [XmlElement(ElementName = "ManufacturerNumber")]
        public string ManufacturerId { get; set; }

        [XmlElement(ElementName = "PromotionCode")]
        public string PromotionCode { get; set; }

        [XmlElement(ElementName = "AssociatedItemCode")]
        public string AssociatedItemId { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_11.EE")]
    public class TransactionRecord11EE
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public string SubStringType { get; set; }

        [XmlElement(ElementName = "CustomerAccountID")]
        public string CustomerAccountID { get; set; }

        [XmlElement(ElementName = "Points")]
        public int Points { get; set; }

        [XmlElement(ElementName = "CouponAmount")]
        public int CouponAmount { get; set; }

        [XmlElement(ElementName = "CouponCount")]
        public int CouponCount { get; set; }

        [XmlElement(ElementName = "MessageCount")]
        public int MessageCount { get; set; }

        [XmlElement(ElementName = "TransferredTransactionCount")]
        public int TransferredTransactionCount { get; set; }

        [XmlElement(ElementName = "TransferredTransactionAmount")]
        public int TransferredTransactionAmount { get; set; }

        [XmlElement(ElementName = "BonusPoints")]
        public int BonusPoints { get; set; }

        [XmlElement(ElementName = "RedeemedPoints")]
        public int RedeemedPoints { get; set; }

        [XmlElement(ElementName = "EntryMethod")]
        public int EntryMethod { get; set; }
        public string getEntryMethod()
            => EntryMethod switch
            {
                0 => "Internal",
                1 => "Keyboard",
                2 => "OCR",
                3 => "Keytag",
                4 => "Wand",
                5 => "MsrScCardSwipe",
                6 => "Micr",
                int n when (n >= 1000 && n <= 1999) => "ReservedToshiba",
                int n when (n >= 2000 && n <= 2999) => "PhoneNumberEntry",
                _ => "not available",
                
            };
    }

    [XmlRoot(ElementName = "TransactionRecord_07")]
    public class TransactionRecord07
    {

        [XmlElement(ElementName = "StringType")]
        public decimal StringType { get; set; }

        [XmlElement(ElementName = "TaxBAmount")]
        public decimal TaxBAmount { get; set; }

        [XmlElement(ElementName = "TaxBSales")]
        public decimal TaxBSales { get; set; }

        [XmlElement(ElementName = "TaxAAmount")]
        public decimal TaxAAmount { get; set; }

        [XmlElement(ElementName = "TaxCAmount")]
        public decimal TaxCAmount { get; set; }

        [XmlElement(ElementName = "TaxASales")]
        public decimal TaxASales { get; set; }

        [XmlElement(ElementName = "TaxCSales")]
        public decimal TaxCSales { get; set; }

        [XmlElement(ElementName = "TaxDAmount")]
        public decimal TaxDAmount { get; set; }

        [XmlElement(ElementName = "TaxDSales")]
        public decimal TaxDSales { get; set; }

        [XmlElement(ElementName = "TaxEAmount")]
        public decimal TaxEAmount { get; set; }

        [XmlElement(ElementName = "TaxESales")]
        public decimal TaxESales { get; set; }
		
		[XmlElement(ElementName = "TaxGAmount")]
        public decimal TaxGAmount { get; set; }

        [XmlElement(ElementName = "TaxGSales")]
        public decimal TaxGSales { get; set; }
		
		[XmlElement(ElementName = "TaxHAmount")]
        public decimal TaxHAmount { get; set; }

        [XmlElement(ElementName = "TaxHSales")]
        public decimal TaxHSales { get; set; }

        [XmlElement(ElementName = "TaxFAmount")]
        public decimal TaxFAmount { get; set; }

        [XmlElement(ElementName = "TaxFSales")]
        public decimal TaxFSales { get; set; }

        public decimal FinalAmount
        {
            get { return TaxAAmount + TaxASales + TaxBAmount + TaxBSales + TaxCAmount + TaxCSales + TaxDSales + TaxDAmount + 
                            TaxESales + TaxEAmount + TaxFSales + TaxFAmount + TaxGSales + TaxGAmount + TaxHSales + TaxHAmount;}
        }

        public decimal SubTotalAmount 
        {
            get { return  TaxASales +  TaxBSales +  TaxCSales + TaxDSales + TaxESales + TaxFSales + TaxGSales + TaxHSales; }
        }
    }
    [XmlRoot(ElementName = "TransactionRecord_99.0.0")]
    public class TransactionRecord9900
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Date")]
        public string DateString { get; set; }

        [XmlIgnore()]
        public DateTime Date
        {
            get { return DateTime.ParseExact(DateString, "yyMMddHHmm", CultureInfo.InvariantCulture); }
        }

        [XmlElement(ElementName = "EntryMethod")]
        public int EntryMethod { get; set; }

        [XmlElement(ElementName = "ShortDate")]
        public int ShortDate { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_11.555352")]
    public class TransactionRecord11555352
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public int SubStringType { get; set; }

        [XmlElement(ElementName = "UserData1")]
        public int UserData1 { get; set; }

        [XmlElement(ElementName = "UserData2")]
        public int UserData2 { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_11.BD")]
    public class TransactionRecord11BD
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "ItemEntrySequenceNumber")]
        public int ItemEntrySequenceNumber { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public string SubStringType { get; set; }

        [XmlElement(ElementName = "Indicator")]
        public string Indicator { get; set; }

        [XmlElement(ElementName = "ItemCode")]
        public string ItemNumber { get; set; }

        [XmlElement(ElementName = "UnitPrice")]
        public decimal UnitPrice { get; set; }

        [XmlElement(ElementName = "DepartmentNumber")]
        public int DepartmentNumber { get; set; }

        [XmlElement(ElementName = "CouponItemCode")]
        public long CouponItemCode { get; set; }

        [XmlElement(ElementName = "CouponPrice")]
        public decimal CouponPrice { get; set; }

        [XmlElement(ElementName = "CouponDepartment")]
        public int CouponDepartment { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_11.DB")]
    public class TransactionRecord11DB
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Identifier")]
        public int Identifier { get; set; }

        [XmlElement(ElementName = "CustomerAccountID")]
        public string LoyaltyNumber { get; set; }

        [XmlElement(ElementName = "TargetedCoupon")]
        public int TargetedCoupon { get; set; }

    }


    [XmlRoot(ElementName = "TransactionRecord_11.CC")]
    public class TransactionRecord11CC
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public string SubStringType { get; set; }

        [XmlElement(ElementName = "ManufacturerCouponAmount")]
        public int ManufacturerCouponAmount { get; set; }

        [XmlElement(ElementName = "StoreCouponAmount")]
        public int StoreCouponAmount { get; set; }

        [XmlElement(ElementName = "ManufacturerCouponCount")]
        public int ManufacturerCouponCount { get; set; }

        [XmlElement(ElementName = "StoreCouponCount")]
        public int StoreCouponCount { get; set; }

        [XmlElement(ElementName = "Unused")]
        public int Unused { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_11.FF")]
    public class TransactionRecord11FF
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "SubStringType")]
        public string SubStringType { get; set; }

        [XmlElement(ElementName = "CustomerAccountID")]
        public string LoyaltyNumber { get; set; }

        [XmlElement(ElementName = "StoreNumber")]
        public int StoreNumber { get; set; }

        [XmlElement(ElementName = "EntryMethod")]
        public int EntryMethod { get; set; }
        public string getEntryMethod()
            => EntryMethod switch
            {
                0 => "Internal",
                1 => "Keyboard",
                2 => "OCR",
                3 => "Keytag",
                4 => "Wand",
                5 => "MsrScCardSwipe",
                6 => "Micr",
                int n when (n >= 1000 && n <= 1999) => "ReservedToshiba",
                int n when (n >= 2000 && n <= 2999) => "PhoneNumberEntry",
                _ => "not available",
                
            };

        [XmlElement(ElementName = "Unused")]
        public int Unused { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.10.2")]
    public class TransactionRecord99102
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

		[XmlElement(ElementName="CustomerNumberAsEntered")] 
		public string CustomerNumberAsEntered { get; set; } 
	}

	[XmlRoot(ElementName="TransactionRecord_99.10.4")]
	public class TransactionRecord99104 { 

		[XmlElement(ElementName="StringType")] 
		public int StringType { get; set; } 

		[XmlElement(ElementName="Originator")] 
		public int Originator { get; set; } 

		[XmlElement(ElementName="SubType")] 
		public int SubType { get; set; } 

		[XmlElement(ElementName="RxTransactionNumber")] 
		public string RxTransactionNumber { get; set; } 

		[XmlElement(ElementName="CustomerEsignatureType")] 
		public string CustomerEsignatureType { get; set; } 

		[XmlElement(ElementName="HostIndicator")] 
		public string HostIndicator { get; set; }

		[XmlElement(ElementName="PharmacyItemSequenceNumber")] 
		public string PharmacyItemSequenceNumber { get; set; } 

		[XmlElement(ElementName = "DateTime")]
		public string DateTimeString { get; set; }

		[XmlIgnore()]
		public DateTimeOffset Timestamp
        {
			get { return DateTime.ParseExact(DateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToEasternStandardTime(); }
		}

		[XmlElement(ElementName="TerminalNumber")] 
		public string TerminalId { get; set; } 

		[XmlElement(ElementName="TransactionNumber")] 
		public string TransactionId { get; set; }
		
		[XmlElement(ElementName="OperatorID")] 
		public string Operator { get; set; } 

		[XmlElement(ElementName="eSigType")] 
		public string ElectronicSignatureType { get; set; }

		[XmlElement(ElementName="eSigName")] 
		public string ElectronicSignatureName { get; set; } 

		[XmlElement(ElementName="HostResponseCode")] 
		public string HostResponseCode { get; set; }

		[XmlElement(ElementName="TotalAmount")] 
		public decimal TotalAmount { get; set; }

		[XmlElement(ElementName="InsuranceAmount")] 
		public decimal InsuranceAmount { get; set; } 

		[XmlElement(ElementName="NetDue")] 
		public decimal NetDue { get; set; }

		[XmlElement(ElementName="Barcode")] 
		public string Barcode { get; set; }

		[XmlElement(ElementName="RefillNumber")] 
		public string RefillNumber { get; set; } 

		[XmlElement(ElementName="PartialFillSequenceNumber")] 
		public string PartialFillSequenceNumber { get; set; }

		[XmlElement(ElementName="StoreNumber")] 
		public int StoreNumber { get; set; } 
	}

    [XmlRoot(ElementName = "TransactionRecord_99.10.27")]
    public class TransactionRecord991027
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "MSINumber")]
        public int ItemNumber { get; set; }

    }

	[XmlRoot(ElementName="TransactionRecord_99.10.96")]
	public class TransactionRecord991096 { 

		[XmlElement(ElementName="StringType")] 
		public int StringType { get; set; } 

		[XmlElement(ElementName="Originator")] 
		public int Originator { get; set; } 

		[XmlElement(ElementName="SubType")] 
		public int SubType { get; set; } 

		[XmlElement(ElementName="Plan")] 
		public string Plan { get; set; } 

		[XmlElement(ElementName="AccountType")] 
		public int AccountType { get; set; } 

		[XmlElement(ElementName="RequestType")] 
		public int RequestType { get; set; }

		[XmlElement(ElementName="Amount")] 
		public int Amount { get; set; } 

		[XmlElement(ElementName="Sequence")] 
		public string Sequence { get; set; }

		[XmlElement(ElementName = "DateTime")]
		public string DateTimeString { get; set; }

		[XmlIgnore()]
		public DateTimeOffset Timestamp
		{
			get { return DateTime.ParseExact(DateTimeString, "yyMMddHHmmss", CultureInfo.InvariantCulture); }
		}

		[XmlElement(ElementName="AccountNumber")] 
		public string LoyaltyNumber { get; set; } 

		[XmlElement(ElementName="EntryMethod")] 
		public int EntryMethod { get; set; }
		
		[XmlElement(ElementName="UserField")] 
		public string UserField { get; set; } 

		[XmlElement(ElementName="Track1")] 
		public string Track1 { get; set; }

		[XmlElement(ElementName="Track2")] 
		public string Track2 { get; set; } 

		[XmlElement(ElementName="Track3")] 
		public string Track3 { get; set; }

		[XmlElement(ElementName="ApprovalCode")] 
		public int ApprovalCode { get; set; }

		[XmlElement(ElementName="ApprovalCodeSource")] 
		public string ApprovalCodeSource { get; set; } 

		[XmlElement(ElementName="ResponseCode")] 
		public int ResponseCode { get; set; }

		[XmlElement(ElementName="Flags")] 
		public string Flags { get; set; }

		[XmlElement(ElementName="PromotionServiceID")] 
		public string PromotionServiceID { get; set; } 

		[XmlElement(ElementName="PromotionServiceGiftCardID")] 
		public string PromotionServiceGiftCardID { get; set; }

		[XmlElement(ElementName="LotteryItemRRN")] 
		public string LotteryItemRRN { get; set; } 

		[XmlElement(ElementName="LotteryPlayerNumber")] 
		public string LotteryPlayerNumber { get; set; }
	}

   [XmlRoot(ElementName = "TransactionRecord_99.11.1")]
    public class TransactionRecord99111
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ExceptionType")]
        public string ExceptionType { get; set; }
		
		[XmlElement(ElementName = "ExceptionCode")]
        public string ExceptionCode { get; set; }
		
		[XmlElement(ElementName = "ItemCode")]
        public string ItemCode { get; set; }
		
		[XmlElement(ElementName = "Weight")]
        public double CurrentScaleWeight { get; set; }
		
		[XmlElement(ElementName = "Age")]
        public int MinimumAgeRestriction { get; set; }
    }
   
   [XmlRoot(ElementName = "TransactionRecord_99.11.2")]
    public class TransactionRecord99112
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ExceptionType")]
        public string ExceptionType { get; set; }
		
		[XmlElement(ElementName = "ExceptionCode")]
        public string ExceptionCode { get; set; }
		
		[XmlElement(ElementName = "ItemCode")]
        public string ItemCode { get; set; }
		
		[XmlElement(ElementName = "Weight")]
        public double CurrentScaleWeight { get; set; }
		
		[XmlElement(ElementName = "Age")]
        public int MinimumAgeRestriction { get; set; }
    }
   
   [XmlRoot(ElementName = "TransactionRecord_99.11.3")]
    public class TransactionRecord99113
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get ; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ExceptionType")]
        public string ExceptionType { get; set; }
		
		[XmlElement(ElementName = "ExceptionCode")]
        public string ExceptionCode { get; set; }
		
		[XmlElement(ElementName = "ItemCode")]
        public string UniversalProductCode { get; set; }
		
		[XmlElement(ElementName = "Weight")]
        public double CurrentScaleWeight { get; set; }
		
		[XmlElement(ElementName = "Age")]
        public int MinimumAgeRestriction { get; set; }
    }
   
   [XmlRoot(ElementName = "TransactionRecord_99.11.5")]
    public class TransactionRecord99115
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ExceptionType")]
        public string ExceptionType { get; set; }
		
		[XmlElement(ElementName = "ExceptionCode")]
        public string ExceptionCode { get; set; }
		
		[XmlElement(ElementName = "ItemCode")]
        public string UniversalProductCode { get; set; }
		
		[XmlElement(ElementName = "Weight")]
        public double CurrentScaleWeight { get; set; }
		
		[XmlElement(ElementName = "Age")]
        public int MinimumAgeRestriction { get; set; }
    }

   [XmlRoot(ElementName = "TransactionRecord_99.11.6")]
    public class TransactionRecord99116
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ExceptionType")]
        public string ExceptionType { get; set; }
		
		[XmlElement(ElementName = "ExceptionCode")]
        public string ExceptionCode { get; set; }
		
		[XmlElement(ElementName = "ItemCode")]
        public string ItemCode { get; set; }
		
		[XmlElement(ElementName = "Weight")]
        public double CurrentScaleWeight { get; set; }
		
		[XmlElement(ElementName = "Age")]
        public int MinimumAgeRestriction { get; set; }
    }



   [XmlRoot(ElementName = "TransactionRecord_99.11.7")]
    public class TransactionRecord99117
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "Operator")]
        public string Operator { get; set; }
		
        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }
		
		[XmlElement(ElementName = "TransactionNumber")]
        public string TransactionId { get; set; }

		[XmlElement(ElementName = "ItemCode")]
        public string ItemNumber { get; set; }
    }



    [XmlRoot(ElementName = "TransactionRecord_99.11.10")]
    public class TransactionRecord991110
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "TerminalNumber")]
        public string TerminalId { get; set; }

        [XmlElement(ElementName = "CustomerNum")]
        public string LoyaltyNumber { get; set; }

    }

    [XmlRoot(ElementName = "TransactionRecord_99.50.10")]
    public class TransactionRecord995010
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "InmarCouponIdentifier")]
        public int InmarCouponIdentifier { get; set; }

        [XmlElement(ElementName = "NumberOfAssociatedItems")]
        public int NumberOfAssociatedItems { get; set; }

        [XmlElement(ElementName = "AssociatedItemCode")]
        public string[] AssociatedItemNumbers { get; set; }
    }

    [XmlRoot(ElementName = "TransactionRecord_99.50.12")]
    public class TransactionRecord995012
    {
        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "InmarCouponDescription")]
        public string InmarCouponShortDescription { get; set; }
        
    }


    [XmlRoot(ElementName = "TransactionRecord_99.50.13")]
    public class TransactionRecord995013
    {

        [XmlElement(ElementName = "StringType")]
        public int StringType { get; set; }

        [XmlElement(ElementName = "Originator")]
        public int Originator { get; set; }

        [XmlElement(ElementName = "SubType")]
        public int SubType { get; set; }

        [XmlElement(ElementName = "QueryResult")]
        public int QueryResult { get; set; }

        public string getQueryResultMeaning()
        {
            switch (QueryResult)
            {
                case 1: return "HostOffline";
                case 2: return "DifOffline";
                default:
                    return "NotApplicable";
            }

        }
    }


    [XmlRoot(ElementName = "TransactionRecord_08")]
    public class TransactionRecord08
    {

        [XmlElement(ElementName = "StringType")]
        public decimal StringType { get; set; }

        [XmlElement(ElementName = "TaxBAmount")]
        public decimal TaxBAmount { get; set; }

        [XmlElement(ElementName = "TaxBSales")]
        public decimal TaxBSales { get; set; }

        [XmlElement(ElementName = "TaxAAmount")]
        public decimal TaxAAmount { get; set; }

        [XmlElement(ElementName = "TaxCAmount")]
        public decimal TaxCAmount { get; set; }

        [XmlElement(ElementName = "TaxASales")]
        public decimal TaxASales { get; set; }

        [XmlElement(ElementName = "TaxCSales")]
        public decimal TaxCSales { get; set; }

        [XmlElement(ElementName = "TaxDAmount")]
        public decimal TaxDAmount { get; set; }

        [XmlElement(ElementName = "TaxDSales")]
        public decimal TaxDSales { get; set; }

        [XmlElement(ElementName = "TaxEAmount")]
        public decimal TaxEAmount { get; set; }

        [XmlElement(ElementName = "TaxESales")]
        public decimal TaxESales { get; set; }
		
		[XmlElement(ElementName = "TaxGAmount")]
        public decimal TaxGAmount { get; set; }

        [XmlElement(ElementName = "TaxGSales")]
        public decimal TaxGSales { get; set; }
		
		[XmlElement(ElementName = "TaxHAmount")]
        public decimal TaxHAmount { get; set; }

        [XmlElement(ElementName = "TaxHSales")]
        public decimal TaxHSales { get; set; }

        [XmlElement(ElementName = "TaxFAmount")]
        public decimal TaxFAmount { get; set; }

        [XmlElement(ElementName = "TaxFSales")]
        public decimal TaxFSales { get; set; }

        public decimal FinalAmount
        {
            get { return TaxAAmount + TaxASales + TaxBAmount + TaxBSales + TaxCAmount + TaxCSales + TaxDSales + TaxDAmount + 
                            TaxESales + TaxEAmount + TaxFSales + TaxFAmount + TaxGSales + TaxGAmount + TaxHSales + TaxHAmount;}
        }

        public decimal SubTotalAmount 
        {
            get { return  TaxASales +  TaxBSales +  TaxCSales + TaxDSales + TaxESales + TaxFSales + TaxGSales + TaxHSales; }
        }
    }


    [XmlRoot(ElementName = "Transaction")]
    public class HubTransaction
    {
        [XmlElement(ElementName = "TransactionRecord_00")]
        public List<TransactionRecord00> TransactionRecord00 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_01")]
        public List<TransactionRecord01> TransactionRecord01 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_02")]
        public List<TransactionRecord02> TransactionRecord02 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_11.555352")]
        public TransactionRecord11555352 TransactionRecord11555352 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_05")]
		public List<TransactionRecord05> TransactionRecord05 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_06")]
		public List<TransactionRecord06> TransactionRecord06 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_07")]
		public TransactionRecord07 TransactionRecord07 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_08")]
		public TransactionRecord08 TransactionRecord08 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_09")]
		public List<TransactionRecord09> TransactionRecord09 { get; set; }
		
		[XmlElement(ElementName = "TransactionRecord_10")]
		public List<TransactionRecord10> TransactionRecord10 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_11.BD")]
		public TransactionRecord11BD TransactionRecord11BD { get; set; }

		[XmlElement(ElementName = "TransactionRecord_11.CC")]
		public TransactionRecord11CC TransactionRecord11CC { get; set; }

		[XmlElement(ElementName = "TransactionRecord_11.DD")]
		public List<TransactionRecord11DD> TransactionRecord11DD { get; set; }

		[XmlElement(ElementName = "TransactionRecord_11.EE")]
		public List<TransactionRecord11EE> TransactionRecord11EE { get; set; }

		[XmlElement(ElementName = "TransactionRecord_11.FF")]
		public TransactionRecord11FF TransactionRecord11FF { get; set; }

		[XmlElement(ElementName = "TransactionRecord_16")]
		public List<TransactionRecord16> TransactionRecord16 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_21")]
		public TransactionRecord21 TransactionRecord21 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.0.0")]
		public List<TransactionRecord9900> TransactionRecord9900 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.0.3")]
		public TransactionRecord9903 TransactionRecord9903 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.0.4")]
		public List<TransactionRecord9904> TransactionRecord9904 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.0.11")]
		public List<TransactionRecord99011> TransactionRecord99011 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.0.50")]
        public List<TransactionRecord99050> TransactionRecord99050 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.10.2")]
		public List<TransactionRecord99102> TransactionRecord99102 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.10.4")]
		public List<TransactionRecord99104> TransactionRecord99104 { get; set; }
        
        [XmlElement(ElementName = "TransactionRecord_99.10.12")]
		public List<TransactionRecord991012> TransactionRecord991012 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.10.14")]
		public List<TransactionRecord991014> TransactionRecord991014 { get; set; }

		[XmlElement(ElementName = "TransactionRecord_99.10.26")]
		public TransactionRecord991026 TransactionRecord991026 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.10.27")]
		public TransactionRecord991027 TransactionRecord991027 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.10.28")]
        public List<TransactionRecord991028> TransactionRecord991028 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.10.29")]
        public TransactionRecord991029 TransactionRecord991029 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.10.96")]
		public List<TransactionRecord991096> TransactionRecord991096 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.1")]
        public TransactionRecord99111 TransactionRecord99111 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.2")]
        public TransactionRecord99112 TransactionRecord99112 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.3")]
        public TransactionRecord99113 TransactionRecord99113 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.5")]
        public TransactionRecord99115 TransactionRecord99115 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.6")]
        public TransactionRecord99116 TransactionRecord99116 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.11.7")]
        public TransactionRecord99117 TransactionRecord99117 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.50.10")]
        public TransactionRecord995010 TransactionRecord995010 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.50.12")]
        public TransactionRecord995012 TransactionRecord995012 { get; set; }

        [XmlElement(ElementName = "TransactionRecord_99.50.13")]
        public TransactionRecord995013 TransactionRecord995013 { get; set; }

	}

    [XmlRoot(ElementName = "TransactionLog")]
    public class TransactionLog
    {

        [XmlElement(ElementName = "Transaction")]
        public IEnumerable<HubTransaction> Transaction { get; set; }
    }

}