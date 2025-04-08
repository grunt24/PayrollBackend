using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademicAwardController : ControllerBase
    {
        private readonly IAcademicAwardService _academicAwardService;

        public AcademicAwardController(IAcademicAwardService academicAwardService)
        {
            _academicAwardService = academicAwardService;
        }

        // Get all academic awards
        [HttpGet]
        public IActionResult Get()
        {
            var awards = _academicAwardService.GetAll();
            return Ok(awards);
        }

        // Add a new academic award
        [HttpPost]
        public IActionResult Add(AcademicAward data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            _academicAwardService.AddAcademicAward(data);
            return Ok("Academic Award added successfully.");
        }

        // Delete an academic award by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteAcademicAward(int id)
        {
            try
            {
                _academicAwardService.DeleteAcademicAward(id);
                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update an existing academic award
        [HttpPut("{id}")]
        public IActionResult UpdateAcademicAward(AcademicAward award, int id)
        {
            var existingAward = _academicAwardService.GetById(id);
            if (existingAward == null)
            {
                return NotFound("Academic Award not found.");
            }

            _academicAwardService.UpdateAcademicAward(award, id);
            return Ok("Updated successfully.");
        }

        // Get an academic award by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var award = _academicAwardService.GetById(id);
            if (award == null)
            {
                return NotFound("Academic Award not found.");
            }

            return Ok(award);
        }
    }
}
