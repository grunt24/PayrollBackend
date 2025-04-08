//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using PayrollSystemBackend.InterfaceRepository;
//using payroll_system.Core.Entities;

//namespace PayrollSystemBackend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ShiftController : ControllerBase
//    {
//        private readonly IShiftService _shiftService;

//        public ShiftController(IShiftService shiftService)
//        {
//            _shiftService = shiftService;
//        }

//        [HttpPost]
//        public IActionResult AddShift([FromBody] Shift shift)
//        {
//            if (shift == null)
//            {
//                return BadRequest("Shift data is required.");
//            }

//            try
//            {
//                _shiftService.AddShift(shift);
//                return CreatedAtAction(nameof(GetShiftById), new { id = shift.Id }, shift);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpDelete("{id}")]
//        public IActionResult DeleteShift(int id)
//        {
//            try
//            {
//                _shiftService.DeleteShift(id);
//                return Ok("Shift deleted successfully.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet]
//        public IActionResult GetAllShifts()
//        {
//            try
//            {
//                var shifts = _shiftService.GetAllShifts();
//                return Ok(shifts);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet("{id}")]
//        public IActionResult GetShiftById(int id)
//        {
//            try
//            {
//                var shift = _shiftService.GetShiftById(id);
//                if (shift == null)
//                {
//                    return NotFound("Shift not found.");
//                }

//                return Ok(shift);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPut("{id}")]
//        public IActionResult UpdateShift([FromBody] Shift shift, int id)
//        {
//            if (shift == null || id != shift.Id)
//            {
//                return BadRequest("Invalid shift data.");
//            }

//            try
//            {
//                _shiftService.UpdateShift(shift, id);
//                return Ok("Shift updated successfully.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet("CalculateSalary/{shiftId}")]
//        public IActionResult CalculateTotalSalaryForShift(int shiftId)
//        {
//            try
//            {
//                var salary = _shiftService.CalculateTotalSalaryForShift(shiftId);
//                return Ok(new { TotalSalary = salary });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }
//}
