using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Collections.Generic;

namespace payroll_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        // GET: api/Position
        [HttpGet]
        public ActionResult<IEnumerable<Position>> Get()
        {
            var positions = _positionService.GetAllPositions();
            return Ok(positions);
        }

        // GET api/Position/5
        [HttpGet("{id}")]
        public ActionResult<Position> Get(int id)
        {
            var position = _positionService.GetPositionById(id);
            if (position == null)
            {
                return NotFound($"Position with ID {id} not found.");
            }
            return Ok(position);
        }

        // POST api/Position
        [HttpPost]
        public ActionResult Post([FromBody] Position position)
        {
            if (position == null)
            {
                return BadRequest("Position data is required.");
            }

            var success = _positionService.AddPosition(position);
            if (success)
            {
                return CreatedAtAction(nameof(Get), new { id = position.Id }, position);
            }
            return StatusCode(500, "A problem occurred while handling your request.");
        }

        // PUT api/Position/5
        [HttpPut("{id}")]
        public ActionResult UpdatePosition(int id, [FromBody] Position position)
        {

                var existingPosition = _positionService.GetPositionById(id);
                if (existingPosition == null)
                {
                    return NotFound($"Position with ID {id} not found.");
                }

                // Update the position using the service
                _positionService.UpdatePosition(position, id);
                return NoContent(); // HTTP 204 for successful update with no response body
            
        }


        // DELETE api/Position/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var position = _positionService.GetPositionById(id);
            if (position == null)
            {
                return NotFound($"Position with ID {id} not found.");
            }

            _positionService.DeletePosition(id);
            return NoContent();
        }
    }
}
