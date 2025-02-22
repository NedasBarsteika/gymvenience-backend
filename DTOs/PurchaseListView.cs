using gymvenience_backend.Common;

namespace gymvenience_backend.DTOs
{
    public class PurchaseListView
    {
        public string Id { get; set; }
        public ProductListView PurchasedProduct { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
