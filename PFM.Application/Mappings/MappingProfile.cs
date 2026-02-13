using AutoMapper;
using PFM.Application.Dtos;
using PFM.Domain.Entities;
using PFM.Domain.Models;

namespace PFM.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<Transaction, TransactionDto>()
                .ForMember(d => d.Mcc, opt => opt.MapFrom(s => (int?)s.Mcc));

            CreateMap<TransactionSplit, TransactionSplitDto>();

            CreateMap<AutoCategorizeResult, AutoCategorizeResultDto>();

            CreateMap<TransactionSplitDto, TransactionSplit>();
        }
    }
}
