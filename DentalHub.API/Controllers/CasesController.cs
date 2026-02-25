using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : BaseController
    {
        private readonly IMediator _mediator;

        public CasesController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all patient cases with optional search (patient name / case-type name),
        /// optional status filter, and pagination.
        /// </summary>
        /// <param name="search">Partial match on patient full-name or case-type name.</param>
        /// <param name="status">Filter by status: Pending | InProgress | Completed | Cancelled | UnderReview</param>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 10).</param>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetAll(
            [FromQuery] string? search   = null,
            [FromQuery] string? status   = null,
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllCasesQuery(search, status, page, pageSize));
            return HandleResult(result);
        }


      

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromForm] CreatePatientCaseCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var result = await _mediator.Send(new DeletePatientCaseCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientCaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientCaseDto>>> GetById(string id)
        {
            var result = await _mediator.Send(new GetPatientCaseByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetByPatientId(string patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPatientCasesByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(string     id, [FromBody] UpdatePatientCaseCommand command)
        {
             if (id != command.Id)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
