using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Entities.Contributions;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PagibigContributionController : ControllerBase
    {
        private readonly IPagibigContributionService _service;

        public PagibigContributionController(IPagibigContributionService service)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllContributions()
        {
            var contributions = await _service.GetAllAsync();
            return Ok(contributions);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetContribution(int id)
        {
            var contribution = await _service.GetByIdAsync(id);
            if (contribution == null)
                return NotFound(new { Message = "Contribution not found." });

            return Ok(contribution);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContribution([FromBody] PagibigContribution contribution)
        {
            await _service.CreateAsync(contribution);
            return Ok(new { Message = "Pagibig contribution created successfully." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateContribution(int id, [FromBody] PagibigContribution contribution)
        {
            contribution.Id = id;
            var isUpdated = await _service.UpdateAsync(contribution);

            if (!isUpdated)
                return NotFound(new { Message = "Contribution not found." });

            return Ok(new { Message = "Pagibig contribution updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteContribution(int id)
        {
            var isDeleted = await _service.DeleteAsync(id);
            if (!isDeleted)
                return NotFound(new { Message = "Contribution not found." });

            return Ok(new { Message = "Pagibig contribution deleted successfully." });
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllContributions()
        {
            var isDeleted = await _service.DeleteAllAsync();
            if (!isDeleted)
                return NotFound(new { Message = "No records found to delete." });

            return Ok(new { Message = "All Pagibig contributions deleted successfully." });
        }
    }
}
