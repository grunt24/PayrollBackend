using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Dto.Leave;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollSystemBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        public async Task<IActionResult> AddLeave([FromBody] LeaveDto leave)
        {
            if (leave.LeaveDates == null || leave.LeaveDates.Count == 0)
                return BadRequest("Leave dates are required.");

            var newLeave = await _leaveService.AddLeaveAsync(leave);
            return Ok(newLeave);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaves()
        {
            var leaves = await _leaveService.GetAllLeavesAsync();
            return Ok(leaves);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveById(int id)
        {
            var leave = await _leaveService.GetLeaveByIdAsync(id);
            if (leave == null) return NotFound();
            return Ok(leave);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetLeavesByEmployeeId(int employeeId)
        {
            var leaves = await _leaveService.GetLeavesByEmployeeIdAsync(employeeId);
            return Ok(leaves);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeLeave(int id)
        {
            var result = await _leaveService.DeleteLeaveAsync(id);
            if (!result)
            {
                return NotFound("Invalid data");
            }

            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeLeave(int id, [FromBody] LeaveDto leave)
        {
            if (leave == null || id != leave.Id)
            {
                return BadRequest("Invalid data");
            }

            var result = await _leaveService.UpdateLeaveAsync(id, leave);
            if (!result)
            {
                return NotFound("Leave not found");
            }

            return NoContent();
        }

    }
}
