using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.Doctor
{
  public class DoctorQueryHandler :
    IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>,
    IRequestHandler<GetAllDoctorsQuery, Result<List<DoctorDto>>>
{
    private readonly IDoctorService _service;

    public DoctorQueryHandler(IDoctorService service)
    {
        _service = service;
    }

    public Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id);

    public Task<Result<List<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
        => _service.GetAllAsync();
}

}

