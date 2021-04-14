using Application.Features.ExampleFeature.Queries;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.ExampleFeature
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExampleModel, ExampleDto>().ReverseMap();
        }
    }
}
