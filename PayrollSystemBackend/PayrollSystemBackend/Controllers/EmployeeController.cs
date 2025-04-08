using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Entities;
using payroll_system.Core.Services;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }
        // Get all employees
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Retrieve all employees, including related data
                var employeeResponse = _employeeService.GetAllEmployee();

                // If no employees are found, return a 404 (Not Found)
                if (employeeResponse == null || !employeeResponse.Any())
                {
                    return NotFound("No employees found.");
                }

                // Return a 200 (OK) status with the list of employees
                return Ok(employeeResponse);
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting (logging should be implemented)
                _logger.LogError($"Error retrieving employees: {ex.Message}", ex);

                // Return a 500 (Internal Server Error) with a detailed message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Add a new employee
        [HttpPost]
        public IActionResult Add([FromBody] Employee data)
        {
            if (data == null)
            {
                return BadRequest(new { message = "Employee data is required." });
            }

            var errorMessage = _employeeService.AddEmployee(data);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }
        [HttpPost]
        [Route("add-deductions")]
        public IActionResult AddEmployeeDeductions([FromBody] List<EmployeeDeduction> deductions)
        {
            if (deductions == null || !deductions.Any())
                return BadRequest("The deductions field is required.");

            try
            {
                _employeeService.AddEmployeeDeductions(deductions);
                return Ok("Deductions added successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("add-allowance")]
        public IActionResult AddEmployeeAdditionalEarnings([FromBody] List<Allowance> earnings)
        {
            try
            {
                var result = _employeeService.AddEmployeeAllowances(earnings);
                if (result)
                {
                    return Ok("Additional earnings have been successfully added or updated.");
                }

                return BadRequest("Failed to add additional earnings.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete an employee
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                bool result = _employeeService.DeleteEmployee(id);
                if (result)
                {
                    return Ok("Deleted successfully.");
                }
                else
                {
                    return NotFound("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting employee: {ex.Message}");
            }
        }

        //// Update an employee
        //[HttpPut("{id}")]
        //public IActionResult UpdateEmployee([FromBody] Employee employee, int id)
        //{
        //    if (employee == null || id != employee.Id)
        //    {
        //        return BadRequest("Employee data is invalid.");
        //    }

        //    try
        //    {
        //        bool result = _employeeService.UpdateEmployee(employee, id);
        //        if (result)
        //        {
        //            return Ok("Updated successfully.");
        //        }
        //        else
        //        {
        //            return NotFound("Employee not found.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error updating employee: {ex.Message}");
        //    }
        //}

    }
}
