using AutoMapper;
using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.PriceData;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Wegmans.POS.DataHub.TransactionControllerHelper;

public class TransactionControllerHelperImp : ITransactionControllerHelper
{
    private readonly IMapper _mapper;

    public TransactionControllerHelperImp(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<bool> CheckItemSoldByLB(int? itemnumber)
    {
        var appURL = Environment.GetEnvironmentVariable("ItemFunctionAppURL");
        var response = await appURL
            .AppendPathSegments("api", "GetItem", itemnumber)
            .AllowHttpStatus((int)HttpStatusCode.NotFound)
            .WithHeader("x-functions-key", Environment.GetEnvironmentVariable("possalesdatahubfunctionkey"))
            .GetAsync();
        if (response.StatusCode == (int)HttpStatusCode.OK)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <inheritdoc />
    public async Task<PriceDataController.PriceData> GetPriceByItemStore(int? itemnumber, int? storenumber)
    {
        var appURL = Environment.GetEnvironmentVariable("PriceFunctionAppURL");
        var response = await appURL
            .AppendPathSegments("api", "GetPrice", itemnumber, storenumber)
            .AllowHttpStatus((int)HttpStatusCode.NotFound)
            .WithHeader("x-functions-key", Environment.GetEnvironmentVariable("possalesdatahubfunctionkey"))
            .GetAsync();
        if (response.StatusCode == (int)HttpStatusCode.OK)
        {
            return response.ResponseMessage.Content.ReadAsAsync<PriceDataController.PriceData>().Result;
        }
        else
       {
            return null;
       }
    }

    /// <inheritdoc />
    public void UpdateTransactionData(XElement record, Transaction outputTransaction)
    {
        //var items = outputTransaction.Items is null ? new List<Item>() : outputTransaction.Items.ToList();
        //Look at Xelement to look for elements to concat
        //record + Originator + SubType
        record.Name = record.getTransactionRecordName();
        switch (record.Name.LocalName)
        {
            case "TransactionRecord_00":
                outputTransaction = MapTo<TransactionRecord00, Transaction>(record, outputTransaction);
                if (outputTransaction.Reserved2 is null) //Toshiba is going to use this bit to let us know when the transaction is External (i.e. NOT ACE)
                {
                    outputTransaction.IsAceTransaction = true;
                }
                break;
            case "TransactionRecord_01":
                outputTransaction.Items = MapToCollection<TransactionRecord01, AceItem>(record, outputTransaction.Items?.Cast<AceItem>());
                if (outputTransaction.CouponDataEntries is not null)
                {
                    if (outputTransaction.CouponDataEntries.Last().OrdinalNumber == 0)
                    {
                        AssignLastOrdinalNumberToLastCouponDataEntry(outputTransaction);
                    }
                }
                break;
            case "TransactionRecord_02":
                outputTransaction.Items = MapToCollectionItem<TransactionRecord02, AceItem>(record, outputTransaction.Items?.Cast<AceItem>());
                break;
            case "TransactionRecord_03":
                var Discounts = SerializationUtil<TransactionRecord03>.Deserialize(record);
                if (Discounts.Amount != 0)
                {
                    outputTransaction.Discounts = MapToCollection<TransactionRecord03, Discount>(record, outputTransaction.Discounts);
                    outputTransaction.Discounts.Last().Quantity = 1;
                }
                break;
            case "TransactionRecord_04":
                var NegativeDiscounts = SerializationUtil<TransactionRecord04>.Deserialize(record);
                if (NegativeDiscounts.Amount != 0)
                {
                    outputTransaction.Discounts = MapToCollection<TransactionRecord04, Discount>(record, outputTransaction.Discounts);
                    outputTransaction.Discounts.Last().Quantity = -1;
                }
                break;
            case "TransactionRecord_05":
                outputTransaction.TenderExchanges = MapToCollection<TransactionRecord05, TenderExchange>(record, outputTransaction.TenderExchanges);
                break;
            case "TransactionRecord_06":
                outputTransaction.TenderVoids = MapToCollection<TransactionRecord06, TenderVoid>(record, outputTransaction.TenderVoids);
                break;
            case "TransactionRecord_07":
                outputTransaction.TaxData = MapTo<TransactionRecord07, TaxData>(record, outputTransaction.TaxData);
                break;
            case "TransactionRecord_08":
                outputTransaction.TaxRefund = MapTo<TransactionRecord08, TaxRefund>(record, outputTransaction.TaxRefund);
                break;
            case "TransactionRecord_09":
                outputTransaction.TenderExchanges = MapToCollection<TransactionRecord09, TenderExchange>(record, outputTransaction.TenderExchanges);
                break;
            case "TransactionRecord_10":
                outputTransaction.ManagerOverrides = MapToCollection<TransactionRecord10, ManagerOverride>(record, outputTransaction.ManagerOverrides);
                break;
            case "TransactionRecord_11.BD":
                outputTransaction.CouponDataEntries = MapToCollection<TransactionRecord11BD, CouponDataEntry>(record, outputTransaction.CouponDataEntries);
                break;
            case "TransactionRecord_11.DB":
                outputTransaction.UsedTargetedCoupons = MapToCollection<TransactionRecord11DB, UsedTargetedCoupon>(record, outputTransaction.UsedTargetedCoupons);
                break;
            case "TransactionRecord_11.DD":
                if (outputTransaction.CouponDataEntries is not null)
                {
                    if (outputTransaction.CouponDataEntries.Last().OrdinalNumber == outputTransaction.Items.Last().OrdinalNumber)
                    {
                        outputTransaction.CouponDataEntries = MapToCollectionItem<TransactionRecord11DD, CouponDataEntry>(record, outputTransaction.CouponDataEntries);
                    }
                    else
                    {
                        outputTransaction.CouponDataEntries = MapToCollection<TransactionRecord11DD, CouponDataEntry>(record, outputTransaction.CouponDataEntries);
                        AssignLastOrdinalNumberToLastCouponDataEntry(outputTransaction);
                    }
                }
                else
                {
                    outputTransaction.CouponDataEntries = MapToCollection<TransactionRecord11DD, CouponDataEntry>(record, outputTransaction.CouponDataEntries);
                    AssignLastOrdinalNumberToLastCouponDataEntry(outputTransaction);
                }
                break;
            case "TransactionRecord_11.EE":
                outputTransaction.CustomerIdentification = MapTo<TransactionRecord11EE, CustomerIdentification>(record, outputTransaction.CustomerIdentification);
                break;
            case "TransactionRecord_11.FF":
                outputTransaction.CustomerIdentification = MapTo<TransactionRecord11FF, CustomerIdentification>(record, outputTransaction.CustomerIdentification);
                var CustomerInfo = SerializationUtil<TransactionRecord11FF>.Deserialize(record);
                if (outputTransaction.StoreNumber == 9999)
                {
                    if (CustomerInfo.StoreNumber > 0)
                    {
                        outputTransaction.StoreNumber = CustomerInfo.StoreNumber;
                    }
                }
                break;
            case "TransactionRecord_16":
                outputTransaction.PaymentProcessorRequests = MapToCollection<TransactionRecord16, PaymentProcessorRequest>(record, outputTransaction.PaymentProcessorRequests);
                break;
            // case "TransactionRecord_21":
            //     MapTo<TransactionRecord21, StoreClose>(record, destinationLocator: _ => (outputTransaction.StoreClose ??= new StoreClose()));
            //     break;
            case "TransactionRecord_99.0.4":
                if (outputTransaction.TenderExchanges is not null)
                {
                    outputTransaction.TenderExchanges = MapToCollectionItem<TransactionRecord9904, TenderExchange>(record, outputTransaction.TenderExchanges);
                }
                break;
            case "TransactionRecord_99.0.50":
                outputTransaction.Items = MapToCollectionItem<TransactionRecord99050, AceItem>(record, outputTransaction.Items?.Cast<AceItem>());
                //outputTransaction.RefundReasonCodes = MapToCollection<TransactionRecord99050, RefundReasonCode>(record, outputTransaction.RefundReasonCodes);
                break;
            case "TransactionRecord_99.0.96":
                if (outputTransaction.Items is not null)
                {
                    outputTransaction.Items = MapToCollectionItem<TransactionRecord99096, AceItem>(record, outputTransaction.Items.Cast<AceItem>());
                }
                outputTransaction.IsGiftCardSoldTransaction = true;
                break;
            case "TransactionRecord_99.10.4":
                outputTransaction.PharmacyItems = MapToCollection<TransactionRecord99104, PharmacyItem>(record, outputTransaction.PharmacyItems);
                if (outputTransaction.StoreNumber == 9999)
                {
                    if (outputTransaction.PharmacyItems.Last().StoreNumber > 0)
                    {
                        outputTransaction.StoreNumber = outputTransaction.PharmacyItems.Last().StoreNumber;
                    }
                }
                outputTransaction.IsPharmacyTransaction = true;
                break;
            case "TransactionRecord_99.10.11":
                if (outputTransaction.PaymentProcessorRequests is not null)
                {
                    outputTransaction.QualifiedHealthCareProvider = MapTo<TransactionRecord991011, QualifiedHealthCareProvider>(record, outputTransaction.QualifiedHealthCareProvider);
                }
                outputTransaction.IsFsaTransaction = true;
                break;
            case "TransactionRecord_99.10.12":
                //Main CA string
                outputTransaction.CustomArrangements = MapToCollectionForItem<TransactionRecord991012, CustomArrangement>(record, outputTransaction.CustomArrangements, (source, collection) => collection.SingleOrDefault(x => x.UniversalProductCode == source.UniversalProductCode));
                break;
            case "TransactionRecord_99.10.14":
                //Logged for each item in CA
                outputTransaction.CustomArrangements = MapToCollectionForItem<TransactionRecord991014, CustomArrangement>(record, outputTransaction.CustomArrangements, (source, collection) => collection.SingleOrDefault(x => x.UniversalProductCode == source.UniversalProductCode));
                MapToCollection<TransactionRecord991014, AceCustomArrangementItem>(record,
                    get: source => outputTransaction.CustomArrangements.Single(x => x.UniversalProductCode == source.UniversalProductCode).Items?.Cast<AceCustomArrangementItem>(),
                    set: collection => outputTransaction.CustomArrangements.Single(x => x.UniversalProductCode == collection.First().UniversalProductCode).Items = collection);
                break;
            case "TransactionRecord_99.10.26":
                outputTransaction.InstacartQR = MapTo<TransactionRecord991026, InstacartQR>(record, outputTransaction.InstacartQR);
                if (!(new[] { "539186", "544248", "545506", "544318", "544357", "544245", "545037", "545969" }).Contains(outputTransaction.InstacartQR?.BIN))
                {
                    outputTransaction.IsInstacartTransaction = true;
                    var Instacart = SerializationUtil<TransactionRecord991026>.Deserialize(record);
                    if (Instacart.Bypass is not null && Instacart.Bypass.Equals("1"))
                    {
                        outputTransaction.IsInstacartBypassTransaction = true;
                    }
                }
                break;
            case "TransactionRecord_99.10.27":
                if (outputTransaction.Items is not null)
                {
                    outputTransaction.Items = MapToCollectionItem<TransactionRecord991027, AceItem>(record, outputTransaction.Items?.Cast<AceItem>());
                }
                break;
            case "TransactionRecord_99.10.28":
                outputTransaction.Meals2GoQR = MapToCollection<TransactionRecord991028, Meals2GoQR>(record, outputTransaction.Meals2GoQR);
                outputTransaction.IsMeals2GoTransaction = true;
                break;
            case "TransactionRecord_99.10.29":
                outputTransaction.ShopicQR = MapTo<TransactionRecord991029, ShopicQR>(record, outputTransaction.ShopicQR);
                outputTransaction.IsShopicTransaction = true;
                break;
            case "TransactionRecord_99.10.31":
                outputTransaction.AmazonDashCartQR = MapTo<TransactionRecord991031, AmazonDashCartQR>(record, outputTransaction.AmazonDashCartQR);
                outputTransaction.IsAmazonDashCartTransaction = true;
                break;
            case "TransactionRecord_99.11.1":
                //This user string is generated when an item exception is displayed on the POS GUI exception list screen and the operator presses the Add To Transaction button.
                outputTransaction.AddItemButtonPressed = MapTo<TransactionRecord99111, AddItemButtonPressed>(record, outputTransaction.AddItemButtonPressed);
                break;
            case "TransactionRecord_99.11.2":
                //This user string is generated when an item exception is displayed on the POS GUI exception list screen and the operator presses the Void From Transaction button.
                outputTransaction.VoidItemButtonPressed = MapTo<TransactionRecord99112, VoidItemButtonPressed>(record, outputTransaction.VoidItemButtonPressed);
                break;
            case "TransactionRecord_99.11.3":
                //This user string is generated when an item exception is displayed on the POS GUI exception list screen and the operator presses the Item Verified button.
                outputTransaction.VoidItemButtonPressedDuringException = MapTo<TransactionRecord99113, VoidItemButtonPressedDuringException>(record, outputTransaction.VoidItemButtonPressedDuringException);
                break;
            case "TransactionRecord_99.11.5":
                //This user string is generated when an item exception is displayed on the POS GUI exception list screen and the operator presses the Remove From List button.
                outputTransaction.RemoveFromListButtonPressed = MapTo<TransactionRecord99115, RemoveFromListButtonPressed>(record, outputTransaction.RemoveFromListButtonPressed);
                break;
            case "TransactionRecord_99.11.6":
                //This user string is generated when an item exception is displayed on the POS GUI exception list screen and the operator presses the Bypass Audit button.
                outputTransaction.BypassAuditButtonPressed = MapTo<TransactionRecord99116, BypassAuditButtonPressed>(record, outputTransaction.BypassAuditButtonPressed);
                break;
            case "TransactionRecord_99.11.7":
                //This user string is generated when an item was scanned during an audit that was not already in the transaction (for example, it failed an audit).
                outputTransaction.ItemAddedDuringAudit = MapTo<TransactionRecord99117, ItemAddedDuringAudit>(record, outputTransaction.ItemAddedDuringAudit);
                break;
            case "TransactionRecord_99.11.10":
                outputTransaction.MobileTransactionStarted = MapTo<TransactionRecord991110, MobileTransactionStarted>(record, outputTransaction.MobileTransactionStarted);
                outputTransaction.IsFromConsumerMobileApp = true;
                break;
            case "TransactionRecord_99.50.10":
                outputTransaction.InmarCoupons = MapToCollection<TransactionRecord995010, InmarCoupon>(record, outputTransaction.InmarCoupons);
                //This if statement can be removed after AssociatedItemNumber has been fully deprecated
                if (outputTransaction.InmarCoupons.Last().AssociatedItemNumbers.Count() != 0)
                {
                    outputTransaction.InmarCoupons.Last().AssociatedItemNumber = outputTransaction.InmarCoupons.Last().AssociatedItemNumbers.ToList().First();
                }
                AssignLastOrdinalNumberToInmarCoupon(outputTransaction);
                break;
            case "TransactionRecord_99.50.12":
                outputTransaction.InmarCoupons = MapToCollectionItem<TransactionRecord995012, InmarCoupon>(record, outputTransaction.InmarCoupons);
                break;
            case "TransactionRecord_99.50.13":
                outputTransaction = MapTo<TransactionRecord995013, Transaction>(record, outputTransaction);
                break;
            default:
                break;
        }
    }

    /// <Summary>
    ///     MapTo() - generic function that creates mapping between source and destination using automapper, where we are adding to collection
    /// </Summary>
    /// <param name="TSource"> ACE data model class name</param>
    /// <param name="TDestination"> JSON data model class name</param>
    /// <param name="element">  <see cref="XDocument"/> ACE data serialized as XDocument</param>
    /// <param name="collection">  <see cref="ICollection<TDestination>"/> name of the output collection where JSON object is added</param>
    private IEnumerable<TDestination> MapToCollection<TSource, TDestination>(XElement element, IEnumerable<TDestination> collection)
        where TDestination : class
    {
        collection ??= Array.Empty<TDestination>();

        var source = SerializationUtil<TSource>.Deserialize(element);
        var destination = _mapper.Map<TDestination>(source);
        return collection.Append(destination);
    }

    private void MapToCollection<TSource, TDestination>(XElement element, Func<TSource, IEnumerable<TDestination>> get, Action<IEnumerable<TDestination>> set)
        where TDestination : class
    {
        var source = SerializationUtil<TSource>.Deserialize(element);
        var collection = get(source) ?? Array.Empty<TDestination>();
        var destination = _mapper.Map<TDestination>(source);
        set(collection.Append(destination));
    }

    private IEnumerable<TDestination> MapToCollectionItem<TSource, TDestination>(XElement element, IEnumerable<TDestination> collection)
        where TDestination : class
    {
        collection ??= Array.Empty<TDestination>();

        var source = SerializationUtil<TSource>.Deserialize(element);
        TDestination destination = collection.Last();

        _mapper.Map(source, destination);
        return collection;
    }

    private IEnumerable<TDestination> MapToCollectionForItem<TSource, TDestination>(XElement element, IEnumerable<TDestination> collection, Func<TSource, IEnumerable<TDestination>, TDestination> locator)
        where TDestination : class
    {
        collection ??= Array.Empty<TDestination>();

        var source = SerializationUtil<TSource>.Deserialize(element);
        TDestination destination = locator(source, collection);

        if (destination is null)
        {
            destination = _mapper.Map<TDestination>(source);
            collection = collection.Append(destination);
        }
        else
        {
            _mapper.Map(source, destination);
        }

        return collection;
    }

    /// <Summary>
    ///     MapTo() - generic function that creates mapping between source and destination using automapper
    /// </Summary>
    /// <param name="TSource"> ACE data model class name</param>
    /// <param name="TDestination"> JSON data model class name</param>
    /// <param name="element">  <see cref="XDocument"/> ACE data serialized as XDocument</param>
    /// <param name="destinationLocator">  <see cref="Func<TSource, TDestination>"/> destination of where object is being placed in JSON.
    ///              Collection is allowed, could be "discard" variable or a lamba function</param>
    /// <param name="callback">  <see cref="Action<TDestination>"/> callback lamba function that invoked with destination input</param>
    private TDestination MapTo<TSource, TDestination>(XElement element, TDestination destination = null)
        where TDestination : class, new()
    {
        destination ??= new();
        var source = SerializationUtil<TSource>.Deserialize(element);
        return _mapper.Map(source, destination);
    }

    /// <Summary>
    ///     AssignLastOrdinalNumberToLastCouponDataEntry() - function that assigns ordinal number from most recent item to the CouponDataEntry collection
    /// </Summary>
    /// <param name="sourceCollection"> JSON data model class name</param>
    private static void AssignLastOrdinalNumberToLastCouponDataEntry(Transaction sourceCollection)
    {
        var lastItem = sourceCollection.Items.Last();
        var lastCoupon = sourceCollection.CouponDataEntries.Last();
        lastCoupon.OrdinalNumber = lastItem.OrdinalNumber;
        return;
    }

    /// <Summary>
    ///     AssignLastOrdinalNumberToInmarCoupon() - function that assigns ordinal number from most recent item to the CouponDataEntry collection
    /// </Summary>
    /// <param name="sourceCollection"> JSON data model class name</param>
    private static void AssignLastOrdinalNumberToInmarCoupon(Transaction sourceCollection)
    {
        var lastItem = sourceCollection.Items.Last();
        var lastCoupon = sourceCollection.InmarCoupons.Last();
        lastCoupon.OrdinalNumber = lastItem.OrdinalNumber;
        return;
    }
}
