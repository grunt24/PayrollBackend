using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_system.Core.Services;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.Payroll;
using PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly IEmployeeService _employeeService;

        public PayrollController(IPayrollService payrollService, IEmployeeService employeeService)
        {
            _payrollService = payrollService;
            _employeeService = employeeService;
        }

        // ✅ GET PAYROLL DATA FOR VIEWING

        [HttpGet("by-department")]
        public async Task<IActionResult> GetPayrollByDepartment()
        {
            var payrolls = await _payrollService.GetPayrollsByDepartment();
            return Ok(payrolls);
        }


        [HttpGet("by-position")]
        public async Task<IActionResult> GetPayrollByPosition()
        {
            var payrolls = await _payrollService.GetPayrollsByPosition();
            return Ok(payrolls);
        }

        // ✅ EXPORT PAYROLL DATA AS CSV

        [HttpGet("export/by-department/csv")]
        public async Task<IActionResult> ExportPayrollByDepartmentToCsv()
        {
            var payrolls = await _payrollService.GetPayrollsByDepartment();
            var csvContent = GenerateCsvForDepartmentPayrolls(payrolls);

            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "Payroll_By_Department.csv");
        }

        [HttpGet("export/by-position/csv")]
        public async Task<IActionResult> ExportPayrollByPositionToCsv()
        {
            var payrolls = await _payrollService.GetPayrollsByPosition();
            var csvContent = GenerateCsvForPositionPayrolls(payrolls);

            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "Payroll_By_Position.csv");
        }

        // ✅ CSV GENERATION HELPERS

        private string GenerateCsvForDepartmentPayrolls(IEnumerable<DepartmentPayrollDto> payrolls)
        {
            var sb = new StringBuilder();

            // CSV header row
            sb.AppendLine("ID Number,Full Name,Payroll Period,Gross Salary,Net Salary,Total Deductions," +
                          "SSS EE Share,SSS ER Share,PhilHealth EE Share,PhilHealth ER Share," +
                          "Pag-IBIG EE Share,Pag-IBIG ER Share,Total EE Contributions,Total ER Contributions," +
                          "Total Contribution,Others");

            payrolls.SelectMany(dept => dept.Payrolls).ToList().ForEach(payroll =>
            {
                var others = payroll.EmployeeDeductions.Any()
                    ? string.Join(", ", payroll.EmployeeDeductions.Select(d => $"{d.EmployeeDeductionName}: {d.Amount}"))
                    : "N/A";

                sb.AppendLine($"{payroll.IdNumber},\"{payroll.FullName}\"," +
                              $"\"{payroll.PayrollStartDate} - {payroll.PayrollEndDate}\"," +
                              $"\"{payroll.GrossSalary}\",\"{payroll.NetSalary}\",\"{payroll.TotalDeductions}\"," +
                              $"\"{payroll.SssEmployeeShare}\",\"{payroll.SssEmployerShare}\"," +
                              $"\"{payroll.PhilHealthEmployeeShare}\",\"{payroll.PhilHealthEmployerShare}\"," +
                              $"\"{payroll.PagibigEmployeeShare}\",\"{payroll.PagibigEmployerShare}\"," +
                              $"\"{payroll.TotalEmployeeContributions}\",\"{payroll.TotalEmployerContributions}\"," +
                              $"\"{payroll.TotalContribution}\",\"{others}\"");
            });

            return sb.ToString();
        }





        private string GenerateCsvForPositionPayrolls(IEnumerable<PositionPayrollDto> payrolls)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Position,ID Number,Full Name,Start Date,End Date,Gross Salary,Net Salary,Total Deductions");

            foreach (var position in payrolls)
            {
                foreach (var payroll in position.Payrolls)
                {
                    sb.AppendLine($"{position.PositionName}," +
                        $"{payroll.IdNumber},{payroll.FullName}," +
                        $"{payroll.PayrollStartDate},{payroll.PayrollEndDate}," +
                        $"{payroll.GrossSalary},{payroll.NetSalary},{payroll.TotalDeductions}");
                }
            }

            return sb.ToString();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PayrollAddRequestDto requestDto)
        {
            if (requestDto == null || requestDto.IdNumbers == null || !requestDto.IdNumbers.Any())
            {
                return BadRequest(new { message = "Invalid request data. Employee ID(s) are required." });
            }

            var results = await _payrollService.GeneratePayroll(requestDto);

            if (results == null || !results.Any())
            {
                return StatusCode(500, new { message = "Payroll generation failed." });
            }

            var failedResults = results.Where(r => !r.Success).ToList();
            if (failedResults.Any())
            {
                return BadRequest(new
                {
                    message = "Some payrolls failed to generate.",
                    errors = failedResults.Select(r => r.Error?.Message).ToList()
                });
            }

            return Ok(new { message = "Payroll generated successfully!" });
        }

        [HttpGet("department")]
        public async Task<IActionResult> GetPayrollsByDepartment()
        {
            var payrolls = await _payrollService.GetPayrollsByDepartment();
            return Ok(payrolls);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllPayrolls()
        {
            try
            {
                var payrolls = _payrollService.GetAllPayrolls();
                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to retrieve payroll records: {ex.Message}" });
            }
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdatePayrollStatus([FromBody] PayrollStatusRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid Request Data");
            }   

            var updatePayrollStatus = await _payrollService.UpdatePayrollStatusAsync(request);
            return Ok(updatePayrollStatus);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayroll(int id)
        {
            var result = await _payrollService.DeletePayroll(id);
            if (!result)
            {
                return NotFound("Payroll not found.");
            }

            return Ok("Deleted Successfully");
        }

        [HttpGet("export/by-payroll/{payrollId}/csv")]
        public async Task<IActionResult> ExportPayrollByEmployeeToCsv(int payrollId)
        {
            var payrolls = await _payrollService.GetPayslipByPayrollId(payrollId);

            if (payrolls == null || !payrolls.Any())
            {
                return NotFound(new { message = "No payroll records found for this employee." });
            }

            var csvContent = GenerateCsvForEmployeePayrolls(payrolls);

            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", $"Payroll_Employee_{payrollId}.csv");
        }

        // ✅ CSV GENERATION FOR EMPLOYEE PAYROLL
        private string GenerateCsvForEmployeePayrolls(IEnumerable<PayslipDto> payrolls)
        {
            var sb = new StringBuilder();

            // CSV header row
            sb.AppendLine("ID Number,Full Name,Payroll Period,Gross Salary,Net Salary,Total Deductions," +
                          "SSS EE Share,SSS ER Share,PhilHealth EE Share,PhilHealth ER Share," +
                          "Pag-IBIG EE Share,Pag-IBIG ER Share,Total EE Contributions,Total ER Contributions," +
                          "Total Contribution,Others");

            foreach (var payroll in payrolls)
            {
                var others = payroll.EmployeeDeductions.Any()
                    ? string.Join(", ", payroll.EmployeeDeductions.Select(d => $"{d.EmployeeDeductionName}: {d.Amount}"))
                    : "N/A";

                sb.AppendLine($"{payroll.IdNumber},\"{payroll.FullName}\"," +
                              $"\"{payroll.PayrollStartDate} - {payroll.PayrollEndDate}\"," +
                              $"\"{payroll.GrossSalary}\",\"{payroll.NetSalary}\",\"{payroll.TotalDeductions}\"," +
                              $"\"{payroll.SssEmployeeShare}\",\"{payroll.SssEmployerShare}\"," +
                              $"\"{payroll.PhilHealthEmployeeShare}\",\"{payroll.PhilHealthEmployerShare}\"," +
                              $"\"{payroll.PagibigEmployeeShare}\",\"{payroll.PagibigEmployerShare}\"," +
                              $"\"{payroll.TotalEmployeeContributions}\",\"{payroll.TotalEmployerContributions}\"," +
                              $"\"{payroll.TotalContribution}\",\"{others}\"");
            }

            return sb.ToString();
        }


    }
}