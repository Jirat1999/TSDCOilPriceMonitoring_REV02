namespace TSDCOilPriceMonitoring_REV02.Models
{
    public class cmlFuelSummary
    {
        public string tStationName { get; set; }
        public string tFuelName { get; set; }
        public decimal cAvgPrice { get; set; }
        public decimal cMinPrice { get; set; }
        public decimal cMaxPrice { get; set; }
        public int nTotalRecords { get; set; }
    }
}
