namespace gymvenience_backend.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string CoverImageUrl { get; set; }
    }

    public class UpdateProductDto : CreateProductDto { }

}
