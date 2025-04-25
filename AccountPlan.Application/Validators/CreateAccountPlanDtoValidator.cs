using FluentValidation;
using AccountPlan.Application.DTO;

public class CreateAccountPlanDtoValidator : AbstractValidator<CreateAccountPlanDto>
{
    public CreateAccountPlanDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
    }
}