using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Domain.Entities;
using DentalHub.Domain.Factories;
using DentalHub.Domain.DomainExceptions;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.CaseTypes
{
    public class CaseTypeService : ICaseTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CaseTypeService> _logger;

        public CaseTypeService(IUnitOfWork unitOfWork, ILogger<CaseTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CaseTypeDto>> GetCaseTypeByIdAsync(Guid id)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseType, CaseTypeDto>(
                    ct => ct.Id == id,
                    ct => new CaseTypeDto
                    {
                        Id = ct.Id,
                        Name = ct.Name,
                        Description = ct.Description
                    }
                );

                var caseType = await _unitOfWork.CaseTypes.GetByIdAsync(spec);

                if (caseType == null)
                {
                    return Result<CaseTypeDto>.Failure("Case type not found", 404);
                }

                return Result<CaseTypeDto>.Success(caseType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting case type by ID: {Id}", id);
                return Result<CaseTypeDto>.Failure("Error retrieving case type", 500);
            }
        }

        public async Task<Result<PagedResult<CaseTypeDto>>> GetAllCaseTypesAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseType, CaseTypeDto>(
                    ct => string.IsNullOrEmpty(search) || ct.Name.Contains(search) || ct.Description.Contains(search),
                    ct => new CaseTypeDto
                    {
                        Id = ct.Id,
                        Name = ct.Name,
                        Description = ct.Description
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderBy(ct => ct.Name);

				var caseTypesList = await _unitOfWork.CaseTypes.GetAllAsync(spec);
				var totalCount = await _unitOfWork.CaseTypes.CountAsync(spec);

				var pagedResult = PaginationFactory<CaseTypeDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: caseTypesList
				);

				return Result<PagedResult<CaseTypeDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all case types");
                return Result<PagedResult<CaseTypeDto>>.Failure("Error retrieving case types", 500);
            }
        }

        public async Task<Result<CaseTypeDto>> CreateCaseTypeAsync(CreateCaseTypeDto dto)
        {
            try
            {
                // Check if name exists
                var existing = await _unitOfWork.CaseTypes.GetByIdAsync(
                    new BaseSpecification<CaseType>(ct => ct.Name == dto.Name));

                if (existing != null)
                {
                    return Result<CaseTypeDto>.Failure("Case type with this name already exists");
                }

                var caseType = CaseTypeFactory.Create(dto.Name, dto.Description);

                await _unitOfWork.CaseTypes.AddAsync(caseType);
                await _unitOfWork.SaveChangesAsync();

                return await GetCaseTypeByIdAsync(caseType.Id);
            }
            catch (DomainException ex)
            {
                return Result<CaseTypeDto>.Failure(ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case type");
                return Result<CaseTypeDto>.Failure("Error creating case type", 500);
            }
        }

        public async Task<Result<CaseTypeDto>> UpdateCaseTypeAsync(UpdateCaseTypeDto dto)
        {
            try
            {
                var caseType = await _unitOfWork.CaseTypes.GetByIdAsync(dto.Id);

                if (caseType == null)
                {
                    return Result<CaseTypeDto>.Failure("Case type not found", 404);
                }

                if (!string.IsNullOrEmpty(dto.Name))
                {
                     // Check if name exists (excluding current)
                    var existing = await _unitOfWork.CaseTypes.GetByIdAsync(
                        new BaseSpecification<CaseType>(ct => ct.Name == dto.Name && ct.Id != dto.Id));

                    if (existing != null)
                    {
                        return Result<CaseTypeDto>.Failure("Case type with this name already exists");
                    }
                    caseType.Name = dto.Name;
                }

                if (dto.Description != null) // allow empty description update if passed
                {
                    caseType.Description = dto.Description;
                }

                _unitOfWork.CaseTypes.Update(caseType);
                await _unitOfWork.SaveChangesAsync();

                return await GetCaseTypeByIdAsync(dto.Id);
            }
            catch (DomainException ex)
            {
                return Result<CaseTypeDto>.Failure(ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case type: {Id}", dto.Id);
                return Result<CaseTypeDto>.Failure("Error updating case type", 500);
            }
        }

        public async Task<Result> DeleteCaseTypeAsync(Guid id)
        {
            try
            {
                var caseType = await _unitOfWork.CaseTypes.GetByIdAsync(id);

                if (caseType == null)
                {
                    return Result.Failure("Case type not found", 404);
                }

                
                _unitOfWork.CaseTypes.Remove(caseType);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting case type: {Id}", id);

                return Result.Failure("Error deleting case type", 500);
            }
        }
    }
}
