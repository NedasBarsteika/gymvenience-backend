using gymvenience_backend.Common;

namespace gymvenience_backend.Models
{
    public class Purchase
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public List<Product> PurchasedProducts { get; set; }
        public ProductType TypeOfProduct { get; set; }

        public double TotalPrice
        {
            get
            {
                double totalSum = 0;

                foreach (Product product in PurchasedProducts) 
                { 
                    totalSum += product.Price; 
                }

                //// Discount
                //if (DaysToReserve > Constants.SecondLevelDayLimit)
                //{
                //    totalSum = ((100 - Constants.SecondLevelDiscountPercentage) / (decimal)100) * totalSum;
                //}
                //else if (DaysToReserve > Constants.FirstLevelDayLimit)
                //{
                //    totalSum = ((100 - Constants.FirstLevelDiscountPercentage) / (decimal)100) * totalSum;
                //}

                totalSum += Constants.DeliveryFee;

                return totalSum;
            }
        }

        public Purchase(string id, string ownerId, List<Product> purchasedProducts, ProductType typeOfProduct)
        {
            Id = id;
            OwnerId = ownerId;
            PurchasedProducts = purchasedProducts;
            TypeOfProduct = typeOfProduct;
        }

        public Purchase()
        {
        }
    }
}
