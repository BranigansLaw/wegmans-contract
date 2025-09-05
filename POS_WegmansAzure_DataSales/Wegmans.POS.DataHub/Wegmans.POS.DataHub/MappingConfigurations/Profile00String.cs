using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class Profile00String : Profile
    {
        public Profile00String()
        {
            CreateMap<TransactionRecord00, Transaction>()
                .ForMember(dest => dest.TerminalType, opt => opt.MapFrom(src => src.TerminalId.ConvertToTerminalType()))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionTypeEnum))
                .ForMember(dest => dest.TransactionTypeId, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(dest => dest.Reserved1, opt => opt.MapFrom(src => src.Indicat1.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.TOF_Recovered, opt => opt.MapFrom(src => src.Indicat1.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.TransactionCopiedAfterClose, opt => opt.MapFrom(src => src.Indicat1.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.CVBPaper, opt => opt.MapFrom(src => src.Indicat1.Bit3.ToNullableBoolean()))
                .ForMember(dest => dest.Reserved2, opt => opt.MapFrom(src => src.Indicat1.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.SelfCheckoutTerminal, opt => opt.MapFrom(src => src.Indicat1.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.DigitalReceiptCreated, opt => opt.MapFrom(src => src.Indicat1.Bit6.ToNullableBoolean()))
                .ForMember(dest => dest.PaperReceiptSuppressed, opt => opt.MapFrom(src => src.Indicat1.Bit7.ToNullableBoolean()))
                .ForMember(dest => dest.EatInTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit8.ToNullableBoolean()))
                .ForMember(dest => dest.PreferredCustomer, opt => opt.MapFrom(src => src.Indicat1.Bit9.ToNullableBoolean()))
                .ForMember(dest => dest.GrossPositiveIsNegative, opt => opt.MapFrom(src => src.Indicat1.Bit10.ToNullableBoolean()))
                .ForMember(dest => dest.GrossNegativeIsNegative, opt => opt.MapFrom(src => src.Indicat1.Bit11.ToNullableBoolean()))
                .ForMember(dest => dest.AdditionalRecordsExist, opt => opt.MapFrom(src => src.Indicat1.Bit12.ToNullableBoolean()))
                .ForMember(dest => dest.NotFirstRecord, opt => opt.MapFrom(src => src.Indicat1.Bit13.ToNullableBoolean()))
                .ForMember(dest => dest.TillChangeSignoffRecord, opt => opt.MapFrom(src => src.Indicat1.Bit14.ToNullableBoolean()))
                .ForMember(dest => dest.TermInitialized, opt => opt.MapFrom(src => src.Indicat1.Bit15.ToNullableBoolean()))
                .ForMember(dest => dest.RollbackPriceItem, opt => opt.MapFrom(src => src.Indicat1.Bit16.ToNullableBoolean()))
                .ForMember(dest => dest.Reserved3, opt => opt.MapFrom(src => src.Indicat1.Bit17.ToNullableBoolean()))
                .ForMember(dest => dest.TransactionTransferred, opt => opt.MapFrom(src => src.Indicat1.Bit18.ToNullableBoolean()))
                .ForMember(dest => dest.TransactionMonitored, opt => opt.MapFrom(src => src.Indicat1.Bit19.ToNullableBoolean()))
                .ForMember(dest => dest.TenderRemoval, opt => opt.MapFrom(src => src.Indicat1.Bit20.ToNullableBoolean()))
                .ForMember(dest => dest.TillContentsReportBeforeThisTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit21.ToNullableBoolean()))
                .ForMember(dest => dest.TillExchangedBeforeThisTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit22.ToNullableBoolean()))
                .ForMember(dest => dest.TenderVerificationBeforeTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit23.ToNullableBoolean()))
                .ForMember(dest => dest.NewPasswordUsed, opt => opt.MapFrom(src => src.Indicat1.Bit24.ToNullableBoolean()))
                .ForMember(dest => dest.OperatorSignonPriorToTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit25.ToNullableBoolean()))
                .ForMember(dest => dest.TenderRejectedinTransaction, opt => opt.MapFrom(src => src.Indicat1.Bit26.ToNullableBoolean()))
                .ForMember(dest => dest.SignoffIsFalse, opt => opt.MapFrom(src => src.Indicat1.Bit27.ToNullableBoolean()))
                .ForMember(dest => dest.DataEntry, opt => opt.MapFrom(src => src.Indicat1.Bit28.ToNullableBoolean()))
                .ForMember(dest => dest.OpenDrawer, opt => opt.MapFrom(src => src.Indicat1.Bit29.ToNullableBoolean()))
                .ForMember(dest => dest.SpecialSignoff, opt => opt.MapFrom(src => src.Indicat1.Bit30.ToNullableBoolean()))
                .ForMember(dest => dest.TerminalAccountability, opt => opt.MapFrom(src => src.Indicat1.Bit31.ToNullableBoolean()))

                .ForMember(dest => dest.GrossPositive, opt => opt.MapFrom(src => src.GrossPositive.Divide()))
                .ForMember(dest => dest.GrossNegative, opt => opt.MapFrom(src => src.GrossNegative.Divide()))

                .ForMember(dest => dest.RingSeconds, opt => opt.MapFrom(src => src.RingSeconds.getConvertedSeconds()))
                .ForMember(dest => dest.TenderSeconds, opt => opt.MapFrom(src => src.TenderSeconds.getConvertedSeconds()))
                .ForMember(dest => dest.InactiveSeconds, opt => opt.MapFrom(src => src.InactiveSeconds.getConvertedSeconds()))
                .ForMember(dest => dest.SpecialTime, opt => opt.MapFrom(src => src.SpecialTime.getConvertedSeconds()))

                .ForMember(dest => dest.AltPrice, opt => opt.MapFrom(src => src.AltPrice00Enum))
                .ForMember(dest => dest.VoidTrc, opt => opt.MapFrom(src => src.VoidTransaction00Enum))

            ;
        }
    }
}