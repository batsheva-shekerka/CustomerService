using AutoMapper;
using Common.Dto;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Operator, OperatorDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Call, CallDto>().ReverseMap();

        }
    }
}
