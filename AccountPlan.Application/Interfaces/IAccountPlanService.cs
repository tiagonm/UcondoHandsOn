using AccountPlan.Application.DTO;

public interface IAccountPlanService
{
    Task<IEnumerable<AccountPlanDto>> GetAllAsync();
    Task<AccountPlanDto?> GetByIdAsync(Guid id);
    Task<AccountPlanDto> CreateAsync(CreateAccountPlanDto dto);
    Task DeleteAsync(Guid id);
    Task<string> SuggestNextCodeAsync(Guid parentId);
}