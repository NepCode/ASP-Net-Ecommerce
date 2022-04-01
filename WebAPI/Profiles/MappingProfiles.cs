﻿using AutoMapper;
using Core.Models;
using WebAPI.DTO.Address;
using WebAPI.DTO.Product;

namespace WebAPI.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductReadDTO>()
                .ForMember(p => p.CategoryName, x => x.MapFrom(a => a.Category.Name))
                .ForMember(p => p.BrandName, x => x.MapFrom(a => a.Brand.Name));
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            CreateMap<Address, AddressReadDTO>().ReverseMap();

        }

    }
}
