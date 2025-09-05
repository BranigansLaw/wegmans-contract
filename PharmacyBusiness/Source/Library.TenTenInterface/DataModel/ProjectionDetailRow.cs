namespace Library.TenTenInterface.DataModel;

public class ProjectionDetailRow
{
    public string? StoreNumber { get; set; }
    public string? RxNumber { get; set; }
    public string? RefillNumber { get; set; }
    public string? PartialFillNumber { get; set; }
    public string? TransactionNumber { get; set; }
    public string? StoreGenericSales { get; set; }
    public string? StoreGenericCost { get; set; }
    public string? StoreGenericCount { get; set; }
    public string? StoreBrandSales { get; set; }
    public string? StoreBrandCost { get; set; }
    public string? StoreBrandCount { get; set; }
    public string? CfGenericSales { get; set; }
    public string? CfGenericCost { get; set; }
    public string? CfGenericCount { get; set; }
    public string? CfBrandSales { get; set; }
    public string? CfBrandCost { get; set; }
    public string? CfBrandCount { get; set; }
    public string? Discount { get; set; }
    public string? BillIndicator { get; set; }
    public string? RefundPrice { get; set; }
    public string? RefundYouPay { get; set; }
    public string? DataFileSource { get; set; }
    public string? SoldDate { get; set; }
    public string? RxId { get; set; }

    /// <summary>
    /// The order of parameters in this constructor is the same order as in the 1010data query results.
    /// Also, the parameter names are spelled exactly as the column headers in the 1010data query results to make mapping easier.
    /// WARNING: Do NOT change the order of these parameters, it is used in the TenTen CSV column export
    /// </summary>
    /// <param name="store_num"></param>
    /// <param name="rx_num"></param>
    /// <param name="refill_num"></param>
    /// <param name="part_seq_num"></param>
    /// <param name="tx_num"></param>
    /// <param name="store_gen_sales"></param>
    /// <param name="store_gen_cost"></param>
    /// <param name="store_gen_count"></param>
    /// <param name="store_brand_sales"></param>
    /// <param name="store_brand_cost"></param>
    /// <param name="store_brand_count"></param>
    /// <param name="cf_gen_sales"></param>
    /// <param name="cf_gen_cost"></param>
    /// <param name="cf_gen_count"></param>
    /// <param name="cf_brand_sales"></param>
    /// <param name="cf_brand_cost"></param>
    /// <param name="cf_brand_count"></param>
    /// <param name="discount"></param>
    /// <param name="bill_ind"></param>
    /// <param name="refund_price"></param>
    /// <param name="refund_youpay"></param>
    /// <param name="datafilesrc"></param>
    /// <param name="date"></param>
    /// <param name="rxid"></param>
    public ProjectionDetailRow(
        string? store_num,
        string? rx_num,
        string? refill_num,
        string? part_seq_num,
        string? tx_num,
        string? store_gen_sales,
        string? store_gen_cost,
        string? store_gen_count,
        string? store_brand_sales,
        string? store_brand_cost,
        string? store_brand_count,
        string? cf_gen_sales,
        string? cf_gen_cost,
        string? cf_gen_count,
        string? cf_brand_sales,
        string? cf_brand_cost,
        string? cf_brand_count,
        string? discount,
        string? bill_ind,
        string? refund_price,
        string? refund_youpay,
        string? datafilesrc,
        string? date,
        string? rxid)
    {
        StoreNumber = store_num;
        RxNumber = rx_num;
        RefillNumber = refill_num;
        PartialFillNumber = part_seq_num;
        TransactionNumber = tx_num;
        StoreGenericSales = store_gen_sales;
        StoreGenericCost = store_gen_cost;
        StoreGenericCount = store_gen_count;
        StoreBrandSales = store_brand_sales;
        StoreBrandCost = store_brand_cost;
        StoreBrandCount = store_brand_count;
        CfGenericSales = cf_gen_sales;
        CfGenericCost = cf_gen_cost;
        CfGenericCount = cf_gen_count;
        CfBrandSales = cf_brand_sales;
        CfBrandCost = cf_brand_cost;
        CfBrandCount = cf_brand_count;
        Discount = discount;
        BillIndicator = bill_ind;
        RefundPrice = refund_price;
        RefundYouPay = refund_youpay;
        DataFileSource = datafilesrc;
        SoldDate = date;
        RxId = rxid;
    }
}
