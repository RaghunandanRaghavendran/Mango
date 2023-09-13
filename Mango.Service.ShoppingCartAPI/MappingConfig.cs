using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTOs;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartDTO, Cart>().ReverseMap();
                config.CreateMap<CartDetailsDTO, CartDetails>().ReverseMap();
                config.CreateMap<UpsertCartDTO,Cart>().ReverseMap();
                config.CreateMap<UpsertCartDetailsDTO, CartDetails>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
