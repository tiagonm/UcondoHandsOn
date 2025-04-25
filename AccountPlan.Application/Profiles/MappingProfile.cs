using AccountPlan.Application.DTO;
using AccountPlan.Domain.Entities;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AccountPlanEntity, AccountPlanDto>().ReverseMap();
        CreateMap<CreateAccountPlanDto, AccountPlanEntity>();
    }
}