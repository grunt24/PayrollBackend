using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AllowanceController : ControllerBase
    {
        private readonly IAllowanceService _additionalEarningService;

        public AllowanceController(IAllowanceService additionalEarningService)
        {
            _additionalEarningService = additionalEarningService;
        }

        // Get all additional earnings
        [HttpGet]
        public IActionResult Get()
        {
            var additionalEarnings = _additionalEarningService.GetAll();
            return Ok(additionalEarnings);
        }

        // Add a new additional earning
        [HttpPost]
        public IActionResult Add(Allowance additionalEarning)
        {
            if (additionalEarning == null)
            {
                return BadRequest("Invalid data.");
            }

            bool result = _additionalEarningService.AddAdditionalEarning(additionalEarning);
            if (result)
            {
                return Ok("Additional Earning added successfully.");
            }
            else
            {
                return BadRequest("Error adding Additional Earning.");
            }
        }

        // Delete an additional earning by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteAdditionalEarning(int id)
        {
            try
            {
                _additionalEarningService.DeleteAdditionalEarning(id);
                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting Additional Earning: {ex.Message}");
            }
        }

        // Update an existing additional earning
        [HttpPut("{id}")]
        public IActionResult UpdateAdditionalEarning(Allowance earning, int id)
        {
            var existingEarning = _additionalEarningService.GetAll().FirstOrDefault(e => e.Id == id);
            if (existingEarning == null)
            {
                return NotFound("Additional Earning not found.");
            }

            _additionalEarningService.UpdateAdditionalEarning(earning, id);
            return Ok("Updated successfully.");
        }
    }
}
