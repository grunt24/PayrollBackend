using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PayrollSystemBackend.Core.Dto.Employee;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Text;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDeductionsController : ControllerBase
    {
        private readonly IEmployeeDeduction _employeeDeductionService;

        public EmployeeDeductionsController(IEmployeeDeduction employeeDeductionService)
        {
            _employeeDeductionService = employeeDeductionService;
        }

        // GET: api/EmployeeDeductions
        [HttpGet]
        public IActionResult GetAll()
        {
            var deductions = _employeeDeductionService.GetAllEmployeeDeductions();
            return Ok(deductions);
        }
        [HttpGet("employee/{employeeId}")]
        public IActionResult GetEmployeeDeductionByEmployeeId(int employeeId)
        {
            var employeeDeductions = _employeeDeductionService.GetEmployeeDeductionByEmployeeId(employeeId);

            if (employeeDeductions == null)
            {
                return NotFound($"The employee ID {employeeId} not found");
            }

            return Ok(employeeDeductions);
        }

        // GET: api/EmployeeDeductions/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var deduction = _employeeDeductionService.GetEmployeeDeductionById(id);
            if (deduction == null)
            {
                return NotFound($"Employee Deduction with ID {id} not found.");
            }

            return Ok(deduction);
        }

        // POST: api/EmployeeDeductions
        [HttpPost]
        public IActionResult Add([FromBody] EmployeeDeduction deduction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = _employeeDeductionService.AddEmployeeDeduction(deduction);
            if (success)
            {
                return CreatedAtAction(nameof(GetById), new { id = deduction.Id }, deduction);
            }

            return BadRequest("Failed to add Employee Deduction.");
        }

        // PUT: api/EmployeeDeductions/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] EmployeeDeductionRequest deduction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _employeeDeductionService.UpdateEmployeeDeduction(deduction, id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("employee/{employeeId}/deduction/{deductionId}")]
        public IActionResult UpdateDeductionById(int employeeId, int deductionId, [FromBody] EmployeeDeduction updatedDeduction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _employeeDeductionService.UpdateEmployeeDeductionById(employeeId, deductionId, updatedDeduction);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // DELETE: api/EmployeeDeductions/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deduction = _employeeDeductionService.GetEmployeeDeductionById(id);
            if (deduction == null)
            {
                return NotFound($"Employee Deduction with ID {id} not found.");
            }

            _employeeDeductionService.DeleteEmployeeDeduction(id);
            return NoContent();
        }
    }
}
