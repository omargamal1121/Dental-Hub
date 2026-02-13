using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using MediatR;

namespace DentalHub.Application.Commands.CaseTypes
{
    public class CreateCaseTypeCommand : IRequest<Result<CaseTypeDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public CreateCaseTypeDto ToDto()
        {
            return new CreateCaseTypeDto
            {
                Name = this.Name,
                Description = this.Description
            };
        }
    }
}
