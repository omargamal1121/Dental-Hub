using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.PatientCase
{
  public class PatientCaseQueryHandler :
    IRequestHandler<GetPatientCaseByIdQuery, Result<PatientCaseDto>>,
    IRequestHandler<GetPatientCasesByPatientIdQuery, Result<List<PatientCaseDto>>>
{
    private readonly IPatientCaseService _service;

    public PatientCaseQueryHandler(IPatientCaseService service)
    {
        _service = service;
    }

    public Task<Result<PatientCaseDto>> Handle(GetPatientCaseByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id);

    public Task<Result<List<PatientCaseDto>>> Handle(GetPatientCasesByPatientIdQuery request, CancellationToken ct)
        => _service.GetByPatientIdAsync(request.PatientId);
}

}

