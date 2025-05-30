using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Globalization;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CalendarController(ICalendarService _calendarService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllHolidays()
        {
            var holidays = await _calendarService.GetHolidays();
            return Ok(holidays);
        }

        [HttpPost]
        public IActionResult AddHoliday([FromBody] BCASCalendar data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = _calendarService.AddHoliday(data);

            if (result == "Holiday added successfully!")
            {
                return Ok(result);
            }

            return BadRequest("Something went wrong, please try again!.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            try
            {
                await _calendarService.DeleteHoliday(id);
                return Ok($"Holiday with id: {id}, deleted succesfully");
            }
            catch (Exception ex)
            {

                return BadRequest($"Error deleting holidayLL {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoliday(int id, [FromBody]BCASCalendar calendar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var holiday = await _calendarService.GetById(id);
                if (holiday == null)
                {
                    return NotFound($"Holiday with id: {id}, is not found");
                }

                await _calendarService.UpdateHoliday(calendar, id);
                return Ok($"Holiday Updated Successfully");
            }
            catch (Exception ex)
            {

                return BadRequest($"Error updating holiday: {ex.Message}");
            }
        }
    }
}
