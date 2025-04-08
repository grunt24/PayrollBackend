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
    public class BenefitController : ControllerBase
    {
        private readonly IBenefitService _benefitService;

        public BenefitController(IBenefitService benefitService)
        {
            _benefitService = benefitService;
        }

        // Get all benefits
        [HttpGet]
        public IActionResult Get()
        {
            var benefits = _benefitService.GetAll();
            return Ok(benefits);
        }

        // Add a new benefit
        [HttpPost]
        public IActionResult Add(Benefit data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            bool result = _benefitService.AddBenefit(data);
            if (result)
            {
                return Ok("Benefit added successfully.");
            }
            else
            {
                return BadRequest("Error adding benefit.");
            }
        }

        // Delete a benefit by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteBenefit(int id)
        {
            try
            {
                _benefitService.DeleteBenefit(id);
                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting benefit: {ex.Message}");
            }
        }

        // Update an existing benefit
        [HttpPut("{id}")]
        public IActionResult UpdateBenefit(Benefit benefit, int id)
        {
            var existingBenefit = _benefitService.GetById(id);
            if (existingBenefit == null)
            {
                return NotFound("Benefit not found.");
            }

            _benefitService.UpdateBenefit(benefit, id);
            return Ok("Updated successfully.");
        }

        // Get a benefit by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var benefit = _benefitService.GetById(id);
            if (benefit == null)
            {
                return NotFound("Benefit not found.");
            }

            return Ok(benefit);
        }
    }
}
