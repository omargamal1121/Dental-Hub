using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Factories;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Queries.v2.PatientCase;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.v2.PatientCaseHandler
{
    public class GetAllCasesV2QueryHandler
        : IRequestHandler<GetAllCasesV2Query, Result<PagedResult<PatientCaseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCasesV2QueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> Handle(
            GetAllCasesV2Query request, CancellationToken ct)
        {
            var filter = request.Filter;

            CaseStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<CaseStatus>(filter.Status, ignoreCase: true, out var s))
                    parsedStatus = s;
            }

            var nameFilter = filter.PatientName?.Trim().ToLower();
            var caseTypeFilter = filter.CaseType?.Trim().ToLower();
            var nationalIdFilter = filter.NationalId?.Trim();
            var phoneFilter = filter.PhoneNumber?.Trim();

            var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                criteria: pc =>
                    (parsedStatus == null || pc.Status == parsedStatus) &&
                    (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                    (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                    (filter.Gender == null || pc.Patient.Gender == filter.Gender) &&
                    (string.IsNullOrEmpty(nationalIdFilter) || pc.Patient.NationalId == nationalIdFilter) &&
                    (string.IsNullOrEmpty(phoneFilter) || pc.Patient.Phone == phoneFilter),
                PatientCaseProjections.ToDto
            );

            spec.ApplyPaging(filter.Page, filter.PageSize);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool isDesc = filter.SortDirection?.ToLower() == "desc";
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        if (isDesc) spec.ApplyOrderByDescending(pc => pc.Patient.User.FullName);
                        else spec.ApplyOrderBy(pc => pc.Patient.User.FullName);
                        break;
                    case "age":
                        if (isDesc) spec.ApplyOrderByDescending(pc => pc.Patient.Age);
                        else spec.ApplyOrderBy(pc => pc.Patient.Age);
                        break;
                    case "date":
                    default:
                        if (isDesc) spec.ApplyOrderByDescending(pc => pc.CreateAt);
                        else spec.ApplyOrderBy(pc => pc.CreateAt);
                        break;
                }
            }
            else
            {
                spec.ApplyOrderByDescending(pc => pc.CreateAt);
            }

            var countSpec = new BaseSpecification<PatientCase>(
                pc =>
                    (parsedStatus == null || pc.Status == parsedStatus) &&
                    (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                    (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                    (filter.Gender == null || pc.Patient.Gender == filter.Gender) &&
                    (string.IsNullOrEmpty(nationalIdFilter) || pc.Patient.NationalId == nationalIdFilter) &&
                    (string.IsNullOrEmpty(phoneFilter) || pc.Patient.Phone == phoneFilter));

            var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
            var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpec);

            var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                count: totalCount, page: filter.Page, pageSize: filter.PageSize, data: casesList);

            return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
        }
    }
}
