using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Universities;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Universities
{
    public class UniversityService : IUniversityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UniversityService> _logger;

        public UniversityService(IUnitOfWork unitOfWork, ILogger<UniversityService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<UniversityLookupDto>>> GetAllUniversitiesLookupAsync()
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<University, UniversityLookupDto>(
                    u => true,
                    u => new UniversityLookupDto
                    {
                        Id = u.Id,
                        Name = u.Name
                    }
                );

                var universities = await _unitOfWork.Universities.GetAllAsync(spec);

                return Result<IEnumerable<UniversityLookupDto>>.Success(universities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting universities lookup");
                return Result<IEnumerable<UniversityLookupDto>>.Failure("Error retrieving universities", 500);
            }
        }
    }
}
