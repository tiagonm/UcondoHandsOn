namespace AccountPlan.Application.DTO;

public class CreateAccountPlanDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AcceptsLaunches { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}