using AutoMapper;
using gymvenience_backend.DTOs;
using gymvenience_backend.Models;

namespace gymvenience_backend.MapperProfiles
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, ProductDetailedView>();
            CreateMap<Product, ProductListView>();
            CreateMap<Order, OrderListView>();
        }

    }
}
