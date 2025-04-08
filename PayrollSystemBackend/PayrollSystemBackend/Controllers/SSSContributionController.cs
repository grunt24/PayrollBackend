using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Entities.Contributions;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSSContributionController : ControllerBase
    {
        private readonly ISSSContributionService _service;

        public SSSContributionController(ISSSContributionService service)
        {
            _service = service;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllContributions()
        {
            var contributions = await _service.GetAllAsync();
            return Ok(contributions);
        }

        [HttpGet("GetBySalary")]
        public async Task<IActionResult> GetContributionBySalary([FromQuery] decimal salary)
        {
            var contribution = await _service.GetBySalaryAsync(salary);
            if (contribution == null) return NotFound("No contribution found for the given salary.");

            return Ok(new
            {
                contribution.Year,
                SalaryRange = $"₱{contribution.MinCompensation:N2} to ₱{contribution.MaxCompensation:N2}",
                EmployerContribution = new
                {
                    RegularSS = contribution.EmployerSS,
                    EC = contribution.EmployerEC,
                    MPF = contribution.EmployerMPF,
                    Total = contribution.TotalEmployerContribution
                },
                EmployeeContribution = new
                {
                    RegularSS = contribution.EmployeeSS,
                    MPF = contribution.EmployeeMPF,
                    Total = contribution.TotalEmployeeContribution
                },
                TotalContribution = contribution.TotalContribution
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded." });
            }

            try
            {
                var isSuccess = await _service.ImportFromExcelAsync(file);
                if (!isSuccess)
                {
                    return BadRequest(new { Message = "Failed to process the file." });
                }

                return Ok(new { Message = "File uploaded and data imported successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the file.", Error = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAllContributions()
        {
            var isDeleted = await _service.DeleteAllAsync();
            if (!isDeleted)
            {
                return NotFound(new { Message = "No records found to delete." });
            }

            return Ok(new { Message = "All SSS contributions deleted successfully." });
        }
    }
}
