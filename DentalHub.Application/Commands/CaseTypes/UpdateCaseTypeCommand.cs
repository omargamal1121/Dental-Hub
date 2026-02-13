using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using MediatR;

namespace DentalHub.Application.Commands.CaseTypes
{
    public class UpdateCaseTypeCommand : IRequest<Result<CaseTypeDto>>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public UpdateCaseTypeDto ToDto()
        {
            return new UpdateCaseTypeDto
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description
            };
        }
    }
}
