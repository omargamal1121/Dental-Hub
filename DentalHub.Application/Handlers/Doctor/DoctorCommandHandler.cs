using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.Doctor
{
    public class DoctorCommandHandler :
    IRequestHandler<CreateDoctorCommand, Result<Guid>>,
    IRequestHandler<UpdateDoctorCommand, Result>,
    IRequestHandler<DeleteDoctorCommand, Result>
{
    private readonly IDoctorService _service;

    public DoctorCommandHandler(IDoctorService service)
    {
        _service = service;
    }

    public Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken ct)
        => _service.CreateAsync(request);

    public Task<Result> Handle(UpdateDoctorCommand request, CancellationToken ct)
        => _service.UpdateAsync(request);

    public Task<Result> Handle(DeleteDoctorCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id);
}

}

