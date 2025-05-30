using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Dto.Payroll;
using PayrollSystemBackend.ServiceRepository;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        private readonly IPayrollService _payrollService;

        public DashboardController(DashboardService dashboardService, IPayrollService payrollService)
        {
            _dashboardService = dashboardService;
            _payrollService = payrollService;
        }

        [HttpGet("CountOrganization")]
        public IActionResult Get()
        {
            var allCount = _dashboardService.GetAll();
            return Ok(allCount);
        }

        [HttpGet("DepartmentCount")]
        public IActionResult GetDepartmentsWithEmployeeCount()
        {
            var allCount = _dashboardService.GetDepartmentsWithEmployeeCount();
            return Ok(allCount);
        }
        [HttpGet("PositionCount")]
        public IActionResult GetPositionsWithEmployeeCount()
        {
            var allCount = _dashboardService.GetPositionsWithEmployeeCount();
            return Ok(allCount);
        }
        [HttpGet("GetPayrollSummary")]
        public IActionResult GetTotalGrossIncome()
        {
            var totalSummary = _payrollService.GetPayrollSummary();
            return Ok(new
            {
                TotalNetSalary = totalSummary.TotalNetSalary,
                TotalDeductions = totalSummary.TotalDeductions,
                TotalGrossIncome = totalSummary.TotalGrossIncome
            });
        }

        [HttpGet("PayrollStatusCount")]
        public async Task<ActionResult<PayrollStatusCountDto>> GetPayrollStatusCOunt()
        {
            return await _payrollService.GetPayrollStatusCount();
        }

    }
}
