using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : BaseController
    {
        private readonly IMediator _mediator;

        public DoctorsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateDoctorCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDoctorCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DoctorDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDoctorByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<DoctorDto>>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllDoctorsQuery());
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateDoctorCommand command)
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
