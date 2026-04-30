using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DentalHub.Application.Common;
using DentalHub.Application.Interfaces;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Configurations;
using DentalHub.Infrastructure.UnitOfWork;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DentalHub.Infrastructure.Services
{
    public class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly Cloudinary _cloudinary;

        private int MaxFileSize => _configuration.GetValue<int>("Security:FileUpload:MaxFileSizeMB", 10) * 1024 * 1024;

        private string[] AllowedContentTypes => _configuration.GetSection("Security:FileUpload:AllowedContentTypes").Get<string[]>()
            ?? new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

        private string[] AllowedExtensions => _configuration.GetSection("Security:FileUpload:AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private readonly byte[][] _fileSignatures = {
            new byte[] { 0xFF, 0xD8, 0xFF },      // JPEG
            new byte[] { 0x89, 0x50, 0x4E, 0x47 }, // PNG
            new byte[] { 0x47, 0x49, 0x46, 0x38 }, // GIF
            new byte[] { 0x52, 0x49, 0x46, 0x46 }, // WEBP
        };

        public MediaService(
            IOptions<CloudinarySettings> config,
            IBackgroundJobClient backgroundJobClient,
            ILogger<MediaService> logger,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        #region Interface Implementation

        public async Task<Result<ImageUploadDto>> UploadImageAsync(IFormFile file)
        {
            var result = await SaveToCloudinaryAsync(file, "General");
            if (!result.IsSuccess || result.Data == null)
            {
                return Result<ImageUploadDto>.Failure(result.Message ?? "Failed to upload image");
            }

            return Result<ImageUploadDto>.Success(new ImageUploadDto
            {
                Url = result.Data.SecureUrl.ToString(),
                PublicId = result.Data.PublicId
            });
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return false;

            try
            {
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary: {PublicId}", publicId);
                return false;
            }
        }

        #endregion

        #region Domain-Specific Methods

        public async Task<Result<Media>> SavePatientMediaAsync(IFormFile file, Guid patientId)
            => await SaveMediaAndLinkToEntityAsync(file, "PatientPhotos", patientId, "Patient");

        public async Task<Result<Media>> SaveSessionMediaAsync(IFormFile file, Guid sessionId)
            => await SaveMediaAndLinkToEntityAsync(file, "SessionPhotos", sessionId, "Session");

        public async Task<Result<Media>> SaveNoteMediaAsync(IFormFile file, Guid noteId)
            => await SaveMediaAndLinkToEntityAsync(file, "NotePhotos", noteId, "Note");

        public async Task<Result<Media>> SaveCaseTypeMediaAsync(IFormFile file, Guid caseTypeId)
            => await SaveMediaAndLinkToEntityAsync(file, "CaseTypePhotos", caseTypeId, "CaseType");

        public async Task<Result<Media>> SaveCaseMediaAsync(IFormFile file, Guid caseId)
            => await SaveMediaAndLinkToEntityAsync(file, "CasePhotos", caseId, "Case");

        #endregion

        #region Private Helpers

        private async Task<Result<Media>> SaveMediaAndLinkToEntityAsync(IFormFile file, string folderName, Guid entityId, string entityType)
        {
            _logger.LogInformation("Uploading media for {EntityType} with ID {EntityId}", entityType, entityId);

            var validationResult = ValidateFile(file);
            if (!validationResult.IsSuccess) return Result<Media>.Failure(validationResult.Errors ?? new List<string> { "Validation failed" });

            try
            {
                var uploadResult = await SaveToCloudinaryAsync(file, folderName);
                if (!uploadResult.IsSuccess || uploadResult.Data == null)
                    return Result<Media>.Failure(uploadResult.Message ?? "Upload failed");

                var media = new Media
                {
                    MediaUrl = uploadResult.Data.SecureUrl.ToString(),
                    CloudinaryPublicId = uploadResult.Data.PublicId,

                    CreateAt = DateTime.UtcNow
                };

                // Link to entity
                AssignMediaToEntity(media, entityType, entityId);

                await _unitOfWork.Medias.AddAsync(media);
                await _unitOfWork.SaveChangesAsync();

                return Result<Media>.Success(media, "Media uploaded and linked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving media for {EntityType} {EntityId}", entityType, entityId);
                return Result<Media>.Failure($"Internal error: {ex.Message}");
            }
        }

        private async Task<Result<ImageUploadResult>> SaveToCloudinaryAsync(IFormFile file, string folderName)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"DentalHub/{folderName}",
                    PublicId = Guid.NewGuid().ToString(),
                    Overwrite = false
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null) return Result<ImageUploadResult>.Failure(result.Error.Message);

                return Result<ImageUploadResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cloudinary upload error");
                return Result<ImageUploadResult>.Failure("Cloudinary upload failed");
            }
        }

        private void AssignMediaToEntity(Media media, string entityType, Guid entityId)
        {
            switch (entityType.ToLower())
            {
                case "patient":
                    media.PatientId = entityId;
                    break;
                case "session":
                    media.SessionId = entityId;
                    break;
                case "note":
                    media.SessionNoteId = entityId;
                    break;
                case "casetype":
                    media.CaseTypeId = entityId;
                    break;
                case "case":
                    media.PatientCaseId = entityId;
                    break;
                default:
                    _logger.LogWarning("Unknown entity type for media assignment: {EntityType}", entityType);
                    break;
            }
        }

        private Result ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return Result.Failure("File is empty");

            if (file.Length > MaxFileSize)
                return Result.Failure($"File size exceeds limit ({MaxFileSize / (1024 * 1024)}MB)");

            if (!AllowedContentTypes.Contains(file.ContentType.ToLower()))
                return Result.Failure($"Invalid content type: {file.ContentType}");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
                return Result.Failure($"Invalid file extension: {extension}");

            using var stream = file.OpenReadStream();
            if (!IsValidFileSignature(stream))
                return Result.Failure("Invalid file signature (file might be corrupted or spoofed)");

            return Result.Success();
        }

        private bool IsValidFileSignature(Stream fileStream)
        {
            try
            {
                using var reader = new BinaryReader(fileStream, System.Text.Encoding.UTF8, true);
                var headerBytes = reader.ReadBytes(8);
                return _fileSignatures.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig));
            }
            catch
            {
                return false;
            }
        }

        #endregion

        // Background job method for cleanup (can be used by others)
        [AutomaticRetry(Attempts = 3)]
        public async Task DeleteFromCloudinaryAsync(string publicId)
        {
            await DeleteImageAsync(publicId);
        }
    }
}
