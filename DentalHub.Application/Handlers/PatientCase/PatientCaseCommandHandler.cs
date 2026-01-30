using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.PatientCase
{
 public class PatientCaseCommandHandler :
    IRequestHandler<CreatePatientCaseCommand, Result<Guid>>,
    IRequestHandler<UpdatePatientCaseCommand, Result>,
    IRequestHandler<DeletePatientCaseCommand, Result>
{
    private readonly IPatientCaseService _service;

    public PatientCaseCommandHandler(IPatientCaseService service)
    {
        _service = service;
    }

    public Task<Result<Guid>> Handle(CreatePatientCaseCommand request, CancellationToken ct)
        => _service.CreateAsync(request);

    public Task<Result> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
        => _service.UpdateAsync(request);

    public Task<Result> Handle(DeletePatientCaseCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id);
}

}

