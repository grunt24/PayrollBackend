using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Services;
using PayrollSystemBackend.Core.Dto.Schedule;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class EmployeeScheduleController : ControllerBase
    {
        private readonly IEmployeeScheduleService _employeeScheduleService;

        public EmployeeScheduleController(IEmployeeScheduleService employeeScheduleService)
        {
            this._employeeScheduleService = employeeScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSchedules([FromBody] EmployeeScheduleRequestDto request)
        {
            if (request.Schedules == null || request.Schedules.Count == 0)
            {
                return BadRequest(new { message = "No schedules provided" });
            }

            var result = await _employeeScheduleService.AddEmployeeSchedule(request);
            if (result)
                return Ok(new { message = "Schedules added successfully" });

            return BadRequest(new { message = "No new schedules were added (duplicates detected)" });
        }

        [HttpGet]
        public IActionResult GetAllSchedule()
        {
            var employeeSchedules = _employeeScheduleService.GetEmployeeSchedules();
            return Ok(employeeSchedules);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeScheduleService.DeleteAllSchedulesAsync(id);
            if (!result)
            {
                return NotFound("Not Found");
            }

            return Ok("Deleted Successfully!");

        }
        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeSchedule([FromBody] EmployeeScheduleRequestDto request)
        {
            try
            {
                var result = await _employeeScheduleService.UpdateEmployeeSchedule(request);
                if (!result) return BadRequest("Schedule was not updated.");
                return Ok("Schedule updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
