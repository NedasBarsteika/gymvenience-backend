namespace gymvenience_backend.DTOs
{
    public class AddProductDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveProductDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}