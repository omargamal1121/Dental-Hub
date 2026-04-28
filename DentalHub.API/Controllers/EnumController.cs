using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers
{
    public class EnumController:ControllerBase
    {

  [HttpGet("case-status")]
        public IActionResult GetCaseStatus()
        {
            var result = Enum.GetValues(typeof(CaseStatus))
                .Cast<CaseStatus>()
                .Select(e => new
                {
                    Name = e.ToString(),
                    Value = (int)e
                });

            return Ok(result);
        }



        [HttpGet("request-status")]
        public IActionResult GetRequestStatus()
        {
            var result = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>()
                .Select(e => new
                {
                    Name = e.ToString(),
                    Value = (int)e
                });

            return Ok(result);
        }


        [HttpGet("session-status")]
        public IActionResult GetSessionStatus()
        {
            var result = Enum.GetValues(typeof(SessionStatus))
                .Cast<SessionStatus>()
                .Select(e => new
                {
                    Name = e.ToString(),
                    Value = (int)e
                });

            return Ok(result);
        }


        [HttpGet("enums/{enumName}")]
        public IActionResult GetEnum(string enumName)
        {
            var assembly = typeof(SessionStatus).Assembly;

            var enumType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsEnum && t.Name.Equals(enumName, StringComparison.OrdinalIgnoreCase));

            if (enumType == null)
                return NotFound("Enum not found");

            var result = Enum.GetValues(enumType)
                .Cast<object>()
                .Select(e => new
                {
                    Name = e.ToString(),
                    Value = Convert.ToInt32(e)
                });

            return Ok(result);
        }

        [HttpGet("gender")]
        public IActionResult GetGender()
        {
            var result = Enum.GetValues(typeof(Gender))
                .Cast<Gender>()
                .Select(e => new
                {
                    Name = e.ToString(),
                    Value = (int)e
                });

            return Ok(result);
        }

        [HttpGet("cities")]
        public IActionResult GetCities()
        {
            var cities = Enum.GetValues(typeof(City))
                .Cast<City>()
                .Select(c => new
                {
                    Value = (int)c,
                    Name = c.ToString()
                });

            return Ok(cities);
        }

    }
}
