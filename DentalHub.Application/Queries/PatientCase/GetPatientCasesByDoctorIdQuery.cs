using System;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
    public record GetPatientCasesByDoctorIdQuery(Guid DoctorId, string? Status, int Page = 1, int PageSize = 10) : IRequest<Result<PagedResult<PatientCaseDto>>>;
}
