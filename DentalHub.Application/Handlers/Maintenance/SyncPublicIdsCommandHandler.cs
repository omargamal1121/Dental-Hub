using DentalHub.Application.Commands.Maintenance;
using DentalHub.Application.Common;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Utils;
using DentalHub.Infrastructure.Repository;
using DentalHub.Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DentalHub.Application.Handlers.Maintenance
{
    public class SyncPublicIdsCommandHandler : IRequestHandler<SyncPublicIdsCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SyncPublicIdsCommandHandler> _logger;

        public SyncPublicIdsCommandHandler(IUnitOfWork unitOfWork, ILogger<SyncPublicIdsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(SyncPublicIdsCommand request, CancellationToken ct)
        {
            int updatedCount = 0;
            try
            {
                // Special sync for entities linked to User (Id must match UserId)
                updatedCount += await SyncUserLinkedIds(_unitOfWork.Students);
                updatedCount += await SyncUserLinkedIds(_unitOfWork.Doctors);
                updatedCount += await SyncUserLinkedIds(_unitOfWork.Patients);
                
                // Normal sync for other entities
                updatedCount += await SyncEntityPublicIds(_unitOfWork.Users);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.Admins);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.PatientCases);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.CaseRequests);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.Sessions);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.SessionNotes);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.Medias);
                updatedCount += await SyncEntityPublicIds(_unitOfWork.CaseTypes);

                return Result<string>.Success($"Successfully synced {updatedCount} records.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing PublicIds");
                return Result<string>.Failure("Error syncing PublicIds: " + ex.Message);
            }
        }

        private async Task<int> SyncUserLinkedIds<T>(IMainRepository<T> repository) where T : class
        {
            try
            {
                // We sync all records for these entities to ensure Id == UserId
                var spec = new BaseSpecification<T>();
                var entities = await repository.GetAllAsync(spec);

                if (entities.Count == 0) return 0;

                int currentUpdated = 0;
                foreach (var entity in entities)
                {
                    var idProp = typeof(T).GetProperty("Id");
                    var userIdProp = typeof(T).GetProperty("UserId");
                    var publicIdProp = typeof(T).GetProperty("PublicId");

                    if (idProp != null && userIdProp != null && publicIdProp != null)
                    {
                        var userId = (Guid)userIdProp.GetValue(entity);
                        var currentId = (Guid)idProp.GetValue(entity);
                        var currentPublicId = (string)publicIdProp.GetValue(entity);
                        var expectedPublicId = Base62Converter.Encode(userId);

                        bool needsUpdate = false;
                        if (currentId != userId)
                        {
                            idProp.SetValue(entity, userId);
                            needsUpdate = true;
                        }
                        if (currentPublicId != expectedPublicId)
                        {
                            publicIdProp.SetValue(entity, expectedPublicId);
                            needsUpdate = true;
                        }

                        if (needsUpdate)
                        {
                            repository.Update(entity);
                            await _unitOfWork.SaveChangesAsync();
                            currentUpdated++;
                        }
                    }
                }
                if (currentUpdated > 0)
                {
                    _logger.LogInformation("Successfully synced {Count} {EntityType} records (Id synced to UserId).", currentUpdated, typeof(T).Name);
                }
                return currentUpdated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error syncing User-linked IDs for entity {EntityType}", typeof(T).Name);
                return 0;
            }
        }

        private async Task<int> SyncEntityPublicIds<T>(IMainRepository<T> repository) where T : class
        {
            try 
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, "PublicId");
                var nullCheck = Expression.Equal(property, Expression.Constant(null, typeof(string)));
                var emptyCheck = Expression.Equal(property, Expression.Constant("", typeof(string)));
                var combined = Expression.OrElse(nullCheck, emptyCheck);
                var lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);

                var spec = new BaseSpecification<T>(lambda);
                var entities = await repository.GetAllAsync(spec);

                if (entities.Count == 0) return 0;

                int currentUpdated = 0;
                foreach (var entity in entities)
                {
                    var idProp = typeof(T).GetProperty("Id");
                    var publicIdProp = typeof(T).GetProperty("PublicId");
                    
                    if (idProp != null && publicIdProp != null)
                    {
                        var id = (Guid)idProp.GetValue(entity);
                        var publicId = Base62Converter.Encode(id);
                        publicIdProp.SetValue(entity, publicId);
                        repository.Update(entity);
                        
                        // Save each entity individually to avoid EF batching circular dependency errors with unique indexes
                        await _unitOfWork.SaveChangesAsync();
                        currentUpdated++;
                    }
                }

                if (currentUpdated > 0)
                {
                    _logger.LogInformation("Successfully updated {Count} {EntityType} records.", currentUpdated, typeof(T).Name);
                }
                return currentUpdated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error syncing PublicIds for entity {EntityType}", typeof(T).Name);
                return 0;
            }
        }
    }
}
