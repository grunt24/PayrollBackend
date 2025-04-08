using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

[Route("api/[controller]")]
[ApiController]
public class PhilhealthContributionController : ControllerBase
{
    private readonly IPhilhealthContributionService service;

    public PhilhealthContributionController(IPhilhealthContributionService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPhilhealthContribution(PhilHealthContribution philHealthContribution)
    {
        var result = await service.CreateAsync(philHealthContribution);

        if (!result)
            return BadRequest("Invalid Data");

        return result ? Ok("Record successfully added!") : BadRequest("Failed to add record.");
    }

    [HttpGet]
    public async Task<IActionResult> GetPhilhealthTable()
    {
        var result = await service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("GetContribution")]
    public async Task<IActionResult> GetContributionWithUserSalary([FromQuery] decimal salary)
    {
        var result = await service.GetContributionWithUserSalary(salary);
        return Ok(result);
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool isDeleted = await service.DeleteAsync(id);

            if (!isDeleted)
                return NotFound("Record not found.");

            return Ok("Deleted Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error deleting: { ex.Message}");
        }
    }
}
