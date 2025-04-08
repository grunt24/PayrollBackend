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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // Get all departments
        [HttpGet]
        public IActionResult Get()
        {
            var departments = _departmentService.GetAll();
            return Ok(departments);
        }

        // Add a new department
        [HttpPost]
        public IActionResult Add(Department data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            bool result = _departmentService.AddDepartment(data);
            if (result)
            {
                return Ok("Department added successfully.");
            }
            else
            {
                return BadRequest("Error adding department.");
            }
        }

        // Delete a department by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                _departmentService.DeleteDepartment(id);
                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting department: {ex.Message}");
            }
        }

        // Update an existing department
        [HttpPut("{id}")]
        public IActionResult UpdateDepartment(Department department, int id)
        {
            var existingDepartment = _departmentService.GetById(id);
            if (existingDepartment == null)
            {
                return NotFound("Department not found.");
            }

            _departmentService.UpdateDepartment(department, id);
            return Ok("Updated successfully.");
        }

        // Get a department by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var department = _departmentService.GetById(id);
            if (department == null)
            {
                return NotFound("Department not found.");
            }

            return Ok(department);
        }
    }
}
