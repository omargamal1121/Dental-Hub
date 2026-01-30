using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.Patient
{
public class PatientQueryHandler :
    IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>,
    IRequestHandler<GetAllPatientsQuery, Result<List<PatientDto>>>
{
    private readonly IPatientService _service;

    public PatientQueryHandler(IPatientService service)
    {
        _service = service;
    }

    public Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.UserId);

    public Task<Result<List<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken ct)
        => _service.GetAllAsync();
}

}

