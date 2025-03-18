using gymvenience_backend.Common;
using gymvenience_backend.Models;

namespace gymvenience_backend.DTOs
{
    public class PurchaseListView
    {
        public string Id { get; set; }
        public List<Purchase> PurchasedProducts { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
