using Asp.Versioning;
using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/patients")]
    public class PatientsController : BaseController
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService) : base()
        {
            _patientService = patientService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreatePatientCommand command)
        {
            var result = await _patientService.CreatePatientAsync(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id)
        {
            var result = await _patientService.GetPatientByIdAsync(id);
            return HandleResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> GetAll(
            [FromQuery] FilterPatientDto filter,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.GetAllPatientsAsync(filter, pageNumber, pageSize);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            if (id != dto.PublicId)
            {
                return CreateErrorResponse<PatientDto>("Id mismatch", 400);
            }
            var result = await _patientService.UpdatePatientAsync(dto);
            return HandleResult(result);
        }
    }
}


