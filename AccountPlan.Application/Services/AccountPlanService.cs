using AccountPlan.Application.DTO;
using AccountPlan.Application.Validators;
using AccountPlan.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using uCondo.AccountPlan.Infrastructure;

public class AccountPlanService(NotificationContext notification, AppDbContext context, IMapper mapper) : IAccountPlanService
{  

    public async Task<IEnumerable<AccountPlanDto>> GetAllAsync()
    {
        var entities = await context.AccountPlans
            .AsNoTracking()
            .ToListAsync();
        return mapper.Map<IEnumerable<AccountPlanDto>>(entities);
    }

    public async Task<AccountPlanDto?> GetByIdAsync(Guid id)
    {
        var entity = await context.AccountPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        return entity == null ? null : mapper.Map<AccountPlanDto>(entity);
    }

    public async Task<AccountPlanDto> CreateAsync(CreateAccountPlanDto dto)
    {
        if (context.AccountPlans.Any(a => a.Code == dto.Code))
            notification.AddNotification("Código já existente");

        if (dto.ParentId.HasValue)
        {
            var parent = await context.AccountPlans.FindAsync(dto.ParentId.Value);
            if (parent == null)
                notification.AddNotification("Conta pai não encontrada");

            if (parent != null)
            {
                if (parent.AcceptsLaunches)
                    notification.AddNotification("Conta pai não pode aceitar lançamentos se for pai");

                if (parent.Type != dto.Type)
                    notification.AddNotification("Conta deve ser do mesmo tipo do pai");
            }
        }

        if (notification.HasNotifications) return null;

        var entity = mapper.Map<AccountPlanEntity>(dto);
        entity.Id = Guid.NewGuid();
        context.AccountPlans.Add(entity);
        await context.SaveChangesAsync();
        return mapper.Map<AccountPlanDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.AccountPlans.FindAsync(id);
        if (entity == null) return;
        context.AccountPlans.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<string?> SuggestNextCodeAsync(Guid parentId)
    {
        var parent = await context.AccountPlans.FirstOrDefaultAsync(p => p.Id == parentId);
        if (parent == null)
        {
            notification.AddNotification("Conta pai não encontrada");
            return null;
        }

        return await FindAvailableNextCode(parent.Code);
    }

    private async Task<string?> FindAvailableNextCode(string parentCode)
    {
        var parentDotCount = parentCode.Count(c => c == '.');

        var allChildren = await context.AccountPlans
            .Where(c => c.Code.StartsWith($"{parentCode}."))
            .ToListAsync();

        var directChildren = allChildren
            .Where(c => c.Code.StartsWith(parentCode + ".") &&
                   c.Code.Count(co => co == '.') == parentDotCount + 1 &&
                   !c.Code.Substring(parentCode.Length + 1).Contains('.'))            
            .Select(c =>
            {
                var last = c.Code.Split('.').Last();
                return int.TryParse(last, out var num) ? num : 0;
            })
            .ToList();


        int next = directChildren.Any() ? directChildren.Max() + 1 : 1;

        if (next <= 999)
        {
            return $"{parentCode}.{next}";
        }

        var parentParts = parentCode.Split('.').ToList();
        if (parentParts.Count == 1)
        {
            var rawSiblings = await context.AccountPlans
             .Where(c => c.Code.StartsWith(parentParts[0] + "."))
             .ToListAsync();

            var siblings = rawSiblings
                .Select(c =>
                {
                    var parts = c.Code.Split('.');
                    return parts.Length > 1 && int.TryParse(parts[1], out var num) ? num : 0;
                })
                .ToList();



            int siblingNext = siblings.Any() ? siblings.Max() + 1 : 1;
            if (siblingNext > 999)
                return null;

            return $"{parentParts[0]}.{siblingNext}";
        }

        parentParts.RemoveAt(parentParts.Count - 1);
        var newParentCode = string.Join('.', parentParts);
        return await FindAvailableNextCode(newParentCode);
    }



}