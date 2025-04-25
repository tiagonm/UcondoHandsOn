using AccountPlan.Application.DTO;
using AccountPlan.Application.Validators;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountPlansController(IAccountPlanService service, NotificationContext notification) : ControllerBase
{    

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountPlanDto dto)
    {
        var result = await service.CreateAsync(dto);

        if (notification.HasNotifications)
            return BadRequest(notification.Notifications);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("suggest/{parentId}")]
    public async Task<IActionResult> SuggestNextCode(Guid parentId)
    {
        var code = await service.SuggestNextCodeAsync(parentId);

        if (notification.HasNotifications)
            return BadRequest(notification.Notifications);

        return Ok(code);
    }
}