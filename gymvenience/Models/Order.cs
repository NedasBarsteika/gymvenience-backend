﻿namespace gymvenience_backend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string StripeSessionId { get; set; }   
        public DateTime OrderDate { get; set; }
        public bool IsDelivered { get; set; } = false;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

}
