using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Options;
using OneUniBackend.Configuration;
using OneUniBackend.DTOs.Profile;
using OneUniBackend.Entities;
using OneUniBackend.Enums;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Models;

namespace OneUniBackend.Services;

public class OneProfileService : IOneProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly CloudinarySettings _storageSettings;
    private readonly ILogger<OneProfileService> _logger;

    public OneProfileService(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<CloudinarySettings> storageSettings,
        ILogger<OneProfileService> logger)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _storageSettings = storageSettings.Value;
        _logger = logger;
    }

    public async Task<OneProfileResponseDto> GetStudentProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await EnsureStudentUserAsync(userId, cancellationToken);

        var profile = await _unitOfWork.StudentProfiles.GetByUserIdAsync(userId, cancellationToken);
        var records = await _unitOfWork.EducationalRecords.GetByUserAsync(userId, cancellationToken);
        var documents = await _unitOfWork.Documents.GetByUserAsync(userId, cancellationToken);

        return await BuildResponse(profile, records, documents, cancellationToken);
    }

    public async Task<(OneProfileResponseDto Response, IReadOnlyList<DocumentUploadResponseDto> Uploads)> UpsertStudentProfileAsync(Guid userId, OneProfileUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateDocuments(request.Documents);

        await EnsureStudentUserAsync(userId, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var existingProfile = await _unitOfWork.StudentProfiles.GetByUserIdAsync(userId, cancellationToken);
            if (existingProfile == null)
            {
                throw new InvalidOperationException("Basic student profile not found for this user.");
            }

            var updatedProfile = await UpsertProfileAsync(userId, existingProfile, request.StudentProfile, cancellationToken);

            var existingRecords = await _unitOfWork.EducationalRecords.GetByUserAsync(userId, cancellationToken);
            var recordsById = existingRecords.Where(r => r.RecordId != Guid.Empty).ToDictionary(r => r.RecordId, r => r);
            var updatedRecords = await UpsertEducationalRecordsAsync(userId, request.EducationalRecords, recordsById, cancellationToken);
            var recordsForResponse = existingRecords
                .Where(r => !updatedRecords.ContainsKey(r.RecordId))
                .Concat(updatedRecords.Values)
                .ToList();

            foreach (var kvp in updatedRecords)
            {
                recordsById[kvp.Key] = kvp.Value;
            }

            var uploads = await UpsertDocumentsAsync(userId, request.Documents, recordsById, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var response = await BuildResponse(updatedProfile, recordsForResponse, uploads.Select(u => u.Document), cancellationToken);

            var uploadDtos = uploads.Select(u => new DocumentUploadResponseDto
            {
                DocumentId = u.Document.DocumentId,
                DocumentType = u.Document.DocumentType,
                EducationalRecordId = u.Document.EducationalRecordId,
                UploadUrl = u.UploadResult.UploadUrl,
                ExpiresAt = u.UploadResult.ExpiresAt,
                Folder = u.UploadResult.Folder,
                ObjectKey = u.UploadResult.ObjectKey,
                Signature = u.UploadResult.Signature,
                Timestamp = u.UploadResult.Timestamp,
                ApiKey = u.UploadResult.ApiKey,
                CloudName = u.UploadResult.CloudName,
                ResourceType = u.UploadResult.ResourceType,
                PublicId = u.UploadResult.PublicId
            }).ToList();

            return (response, uploadDtos);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to upsert student profile for user {UserId}", userId);
            throw;
        }
    }

    private async Task EnsureStudentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.Role != UserRole.student)
        {
            throw new InvalidOperationException("Only students can update their profile.");
        }
    }

    private async Task<StudentProfile> UpsertProfileAsync(Guid userId, StudentProfile? profile, StudentProfilePayloadDto dto, CancellationToken cancellationToken)
    {
        var target = profile ?? new StudentProfile
        {
            ProfileId = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        target.FullName = dto.FullName;
        target.DateOfBirth = dto.DateOfBirth;
        target.Gender = dto.Gender;
        target.IdDocumentType = dto.IdDocumentType;
        target.Cnic = dto.Cnic;
        target.PassportNumber = dto.PassportNumber;
        target.NicopNumber = dto.NicopNumber;
        target.GuardianName = dto.GuardianName;
        target.FatherName = dto.FatherName;
        target.GuardianRelation = dto.GuardianRelation;
        target.GuardianPhone = dto.GuardianPhone;
        target.GuardianCnic = dto.GuardianCnic;
        target.GuardianCity = dto.GuardianCity;
        target.GuardianAddress = dto.GuardianAddress;
        target.City = dto.City;
        target.Address = dto.Address;
        target.Phone = dto.Phone;
        target.ProfilePictureUrl = dto.ProfilePictureUrl;
        target.ScholarshipPriority = dto.ScholarshipPriority;
        target.HostelPriority = dto.HostelPriority;
        target.GuardianIncome = dto.GuardianIncome;
        target.PreferredAdmissionCity = dto.PreferredAdmissionCity;
        target.HouseholdIncome = dto.HouseholdIncome;
        target.IsHafizQuran = dto.IsHafizQuran;
        target.IsOrphan = dto.IsOrphan;
        target.Disability = ToJsonDocument(dto.Disability);
        target.Sports = ToJsonDocument(dto.Sports);
        target.UpdatedAt = DateTime.UtcNow;

        if (profile == null)
        {
            await _unitOfWork.StudentProfiles.AddAsync(target, cancellationToken);
        }
        else
        {
            await _unitOfWork.StudentProfiles.UpdateAsync(target, cancellationToken);
        }

        return target;
    }

    private async Task<Dictionary<Guid, EducationalRecord>> UpsertEducationalRecordsAsync(
        Guid userId,
        IEnumerable<EducationalRecordDto> requestedRecords,
        IDictionary<Guid, EducationalRecord> existing,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, EducationalRecord>();

        foreach (var recordDto in requestedRecords)
        {
            EducationalRecord? record = null;
            if (recordDto.RecordId.HasValue && existing.TryGetValue(recordDto.RecordId.Value, out var existingRecord))
            {
                record = existingRecord;
            }

            if (record == null)
            {
                var newRecordId = recordDto.RecordId ?? Guid.NewGuid();
                record = new EducationalRecord
                {
                    RecordId = newRecordId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.EducationalRecords.AddAsync(record, cancellationToken);
            }

            record.EducationType = recordDto.EducationType;
            record.InstitutionName = recordDto.InstitutionName;
            record.BoardUniversity = recordDto.BoardUniversity;
            record.RollNumber = recordDto.RollNumber;
            record.TotalMarks = recordDto.TotalMarks;
            record.ObtainedMarks = recordDto.ObtainedMarks;
            record.Percentage = recordDto.Percentage;
            record.Grade = recordDto.Grade;
            record.YearOfCompletion = recordDto.YearOfCompletion;
            record.IsResultAwaited = recordDto.IsResultAwaited;
            record.UpdatedAt = DateTime.UtcNow;

            result[record.RecordId] = record;

            if (recordDto.RecordId.HasValue)
            {
                await _unitOfWork.EducationalRecords.UpdateAsync(record, cancellationToken);
            }
        }

        return result;
    }

    private async Task<List<(Document Document, PresignedUploadResult UploadResult)>> UpsertDocumentsAsync(
        Guid userId,
        IEnumerable<DocumentUploadRequestDto> documentRequests,
        IDictionary<Guid, EducationalRecord> recordsById,
        CancellationToken cancellationToken)
    {
        var uploads = new List<(Document, PresignedUploadResult)>();

        foreach (var docRequest in documentRequests)
        {
            if (docRequest.EducationalRecordId.HasValue && !recordsById.ContainsKey(docRequest.EducationalRecordId.Value))
            {
                throw new InvalidOperationException("Invalid educational_record_id for document request.");
            }

            var existingDoc = await _unitOfWork.Documents.GetByTypeForRecordAsync(
                docRequest.EducationalRecordId,
                userId,
                docRequest.DocumentType,
                cancellationToken);

            var document = existingDoc ?? new Document
            {
                DocumentId = Guid.NewGuid(),
                UserId = userId,
                VerificationStatus = Enums.VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var objectKey = BuildObjectKey(userId, docRequest);
            var folder = _storageSettings.Folder;

            document.DocumentName = docRequest.FileName;
            document.DocumentType = docRequest.DocumentType;
            document.EducationalRecordId = docRequest.EducationalRecordId;
            document.FilePath = $"{folder}/{objectKey}";
            document.ObjectKey = objectKey;
            document.Bucket = folder; // Repurposed for Cloudinary folder
            document.StorageProvider = "Cloudinary";
            document.Checksum = docRequest.Checksum;
            document.FileSize = (int?)Math.Min(docRequest.FileSize, int.MaxValue);
            document.MimeType = docRequest.ContentType;
            document.Metadata = ToJsonDocument(docRequest.Metadata);
            document.UpdatedAt = DateTime.UtcNow;

            if (existingDoc == null)
            {
                await _unitOfWork.Documents.AddAsync(document, cancellationToken);
            }
            else
            {
                await _unitOfWork.Documents.UpdateAsync(document, cancellationToken);
            }

            var uploadResult = await _storageService.GenerateUploadUrlAsync(
                new StorageObjectRequest(folder, objectKey, docRequest.ContentType, docRequest.FileSize),
                cancellationToken);

            uploads.Add((document, uploadResult));
        }

        return uploads;
    }

    private string BuildObjectKey(Guid userId, DocumentUploadRequestDto request)
    {
        var extension = Path.GetExtension(request.FileName);
        var sanitizedType = request.DocumentType.ToString().ToLowerInvariant();
        return $"students/{userId}/{sanitizedType}/{Guid.NewGuid()}{extension}";
    }

    private void ValidateDocuments(IEnumerable<DocumentUploadRequestDto> documents)
    {
        foreach (var doc in documents)
        {
            if (doc.FileSize > _storageSettings.MaxUploadBytes)
            {
                throw new InvalidOperationException($"File {doc.FileName} exceeds the maximum allowed size.");
            }

            if (!_storageSettings.AllowedContentTypes.Any(ct => string.Equals(ct, doc.ContentType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Content type {doc.ContentType} is not allowed.");
            }
        }
    }

    private async Task<OneProfileResponseDto> BuildResponse(
        StudentProfile? profile,
        IEnumerable<EducationalRecord> records,
        IEnumerable<Document> documents,
        CancellationToken cancellationToken)
    {
        var profileDto = profile is null
            ? new StudentProfilePayloadDto()
            : new StudentProfilePayloadDto
            {
                ProfileId = profile.ProfileId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                IdDocumentType = profile.IdDocumentType,
                Cnic = profile.Cnic,
                PassportNumber = profile.PassportNumber,
                NicopNumber = profile.NicopNumber,
                GuardianName = profile.GuardianName,
                FatherName = profile.FatherName,
                GuardianRelation = profile.GuardianRelation,
                GuardianPhone = profile.GuardianPhone,
                GuardianCnic = profile.GuardianCnic,
                GuardianCity = profile.GuardianCity,
                GuardianAddress = profile.GuardianAddress,
                City = profile.City,
                Address = profile.Address,
                Phone = profile.Phone,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                ScholarshipPriority = profile.ScholarshipPriority,
                HostelPriority = profile.HostelPriority,
                GuardianIncome = profile.GuardianIncome,
                PreferredAdmissionCity = profile.PreferredAdmissionCity,
                HouseholdIncome = profile.HouseholdIncome,
                IsHafizQuran = profile.IsHafizQuran,
                IsOrphan = profile.IsOrphan,
                Disability = profile.Disability?.RootElement,
                Sports = profile.Sports?.RootElement
            };

        var recordDtos = records.Select(r => new EducationalRecordDto
        {
            RecordId = r.RecordId,
            EducationType = r.EducationType,
            InstitutionName = r.InstitutionName,
            BoardUniversity = r.BoardUniversity,
            RollNumber = r.RollNumber,
            TotalMarks = r.TotalMarks,
            ObtainedMarks = r.ObtainedMarks,
            Percentage = r.Percentage,
            Grade = r.Grade,
            YearOfCompletion = r.YearOfCompletion,
            IsResultAwaited = r.IsResultAwaited
        }).ToList();

        var documentDtos = new List<DocumentDescriptorDto>();
        foreach (var document in documents)
        {
            // document.Bucket is repurposed as folder for Cloudinary
            var downloadUrl = (!string.IsNullOrWhiteSpace(document.Bucket) && !string.IsNullOrWhiteSpace(document.ObjectKey))
                ? await _storageService.GenerateDownloadUrlAsync(
                    new StorageObjectReference(document.Bucket!, document.ObjectKey!),
                    TimeSpan.FromMinutes(_storageSettings.PresignExpiryMinutes),
                    cancellationToken)
                : null;

            documentDtos.Add(new DocumentDescriptorDto
            {
                DocumentId = document.DocumentId,
                DocumentType = document.DocumentType,
                EducationalRecordId = document.EducationalRecordId,
                FileName = document.DocumentName,
                DownloadUrl = downloadUrl,
                ObjectKey = document.ObjectKey,
                Bucket = document.Bucket,
                MimeType = document.MimeType,
                FileSize = document.FileSize,
                VerificationStatus = document.VerificationStatus,
                StorageProvider = document.StorageProvider
            });
        }

        return new OneProfileResponseDto
        {
            StudentProfile = profileDto,
            EducationalRecords = recordDtos,
            Documents = documentDtos
        };
    }

    private static JsonDocument? ToJsonDocument(JsonElement? element)
    {
        if (element == null)
        {
            return null;
        }

        return JsonDocument.Parse(element.Value.GetRawText());
    }
}

