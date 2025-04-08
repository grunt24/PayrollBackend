using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EducationalDegreeController : ControllerBase
    {
        private readonly IEducationalDegreeService _educationalDegreeService;

        public EducationalDegreeController(IEducationalDegreeService educationalDegreeService)
        {
            _educationalDegreeService = educationalDegreeService;
        }

        // Get all educational degrees
        [HttpGet]
        public IActionResult Get()
        {
            var degrees = _educationalDegreeService.GetAll();
            return Ok(degrees);
        }

        // Add a new educational degree
        [HttpPost]
        public IActionResult Add(EducationalDegree data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            bool result = _educationalDegreeService.AddEducationalDegree(data);
            if (result)
            {
                return Ok("Educational Degree added successfully.");
            }
            else
            {
                return BadRequest("Error adding educational degree.");
            }
        }

        // Delete an educational degree by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteEducationalDegree(int id)
        {
            try
            {
                _educationalDegreeService.DeleteEducationalDegree(id);
                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting educational degree: {ex.Message}");
            }
        }

        // Update an existing educational degree
        [HttpPut("{id}")]
        public IActionResult UpdateEducationalDegree(EducationalDegree degree, int id)
        {
            var existingDegree = _educationalDegreeService.GetById(id);
            if (existingDegree == null)
            {
                return NotFound("Educational Degree not found.");
            }

            _educationalDegreeService.UpdateEducationalDegree(degree, id);
            return Ok("Updated successfully.");
        }

        // Get an educational degree by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var degree = _educationalDegreeService.GetById(id);
            if (degree == null)
            {
                return NotFound("Educational Degree not found.");
            }

            return Ok(degree);
        }

        // Get the count of employees per educational degree
        [HttpGet("employee-count")]
        public IActionResult GetEmployeeCountPerEducationalDegree()
        {
            try
            {
                var result = _educationalDegreeService.GetEmployeeCountPerEducationalDegree();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving employee counts: {ex.Message}");
            }
        }

    }
}
