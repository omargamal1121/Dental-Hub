using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.Patient
{
public class PatientCommandHandler :
    IRequestHandler<CreatePatientCommand, Result<Guid>>,
    IRequestHandler<UpdatePatientCommand, Result>,
    IRequestHandler<DeletePatientCommand, Result>
{
    private readonly IPatientService _service;

    public PatientCommandHandler(IPatientService service)
    {
        _service = service;
    }

    public Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken ct)
        => _service.CreateAsync(request);

    public Task<Result> Handle(UpdatePatientCommand request, CancellationToken ct)
        => _service.UpdateAsync(request);

    public Task<Result> Handle(DeletePatientCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.UserId);
}

}

