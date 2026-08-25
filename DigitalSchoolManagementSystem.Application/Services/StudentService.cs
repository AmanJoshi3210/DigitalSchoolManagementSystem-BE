using DigitalSchoolManagementSystem.Application.DTOs.Students;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorageService;

        public StudentService(IUnitOfWork unitOfWork, IDocumentService documentService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _documentService = documentService;
            _fileStorageService= fileStorageService;
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(id);
            return student is null ? null : ToDto(student);
        }

        public async Task<StudentDto?> GetByUserIdAsync(int userId)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
            return student is null ? null : ToDto(student);
        }

        public async Task<IReadOnlyList<StudentDto>> GetAllAsync()
        {
            var students = await _unitOfWork.Students.GetAllWithDetailsAsync();
            return students.Select(ToDto).ToList();
        }

        public async Task<StudentDto> UpdateAsync(int id, UpdateStudentDto request)
        {
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Student not found.");

            student.RollNumber = request.RollNumber;
            student.Grade = request.Grade;
            student.Section = request.Section;
            student.GuardianName = request.GuardianName;
            student.GuardianPhoneNumber = request.GuardianPhoneNumber;
            student.BloodGroup = request.BloodGroup;
            student.EducationLevel = request.EducationLevel;

            student.User.FirstName = request.FirstName;
            student.User.LastName = request.LastName;
            student.User.PhoneNumber = request.PhoneNumber;
            student.User.Address = request.Address;
            student.User.DateOfBirth = request.DateOfBirth;
            student.User.Gender = request.Gender;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Users.Update(student.User);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(student);
        }

        public async Task<StudentDto> UpdateOwnProfileAsync(int userId, UpdateOwnProfileDto request)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Student not found.");

            student.User.FirstName = request.FirstName;
            student.User.LastName = request.LastName;
            student.User.PhoneNumber = request.PhoneNumber;
            student.User.Address = request.Address;
            student.User.DateOfBirth = request.DateOfBirth;
            student.User.Gender = request.Gender;
            student.EducationLevel = request.EducationLevel;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Users.Update(student.User);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(student);
        }

        private static readonly HashSet<string> AllowedProfileImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp", "image/gif"
        };

        public async Task<StoredFileResult> UpdateAddOwnProfileImageAsync(IFormFile File, int userId, CancellationToken cancellationToken = default)
        {
            if (File is null || File.Length == 0)
                throw new ArgumentException("Please choose an image file to upload.");

            if (!AllowedProfileImageContentTypes.Contains(File.ContentType))
                throw new ArgumentException("Profile photo must be a JPEG, PNG, WEBP or GIF image.");

            var student = await _unitOfWork.Students.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Student not found.");

            // Replacing an existing photo: soft-delete the previous ProfilePhoto document so it
            // no longer shows up in the review queue or the student's document list.
            if (student.User.DocumentId.HasValue)
            {
                await _documentService.DeleteAsync(student.User.DocumentId.Value, userId, false);
            }

            var document = await _documentService.UploadAsync(
                userId,
                File,
                DocumentType.ProfilePhoto,
                "Photo",
                cancellationToken);

            StoredFileResult? storedFileResult = null;

            if (document.FileStorageId.HasValue)
            {
                // pass the non-nullable int
                storedFileResult = await _fileStorageService.GetFileAsync(document.FileStorageId.Value, cancellationToken);
            }

            // Update student's user with document info if needed
            if (storedFileResult != null)
            {
                student.User.ProfileImageUrl = storedFileResult.Url;
            }
            else if (!string.IsNullOrEmpty(document.FileName))
            {
                student.User.ProfileImageUrl = document.FileName;
            }

            student.User.DocumentId = document.Id;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Users.Update(student.User);
            await _unitOfWork.SaveChangesAsync();

            // Return the retrieved stored file info or a fallback constructed from the document DTO
            return storedFileResult ?? new StoredFileResult
            {
                Url = student.User.ProfileImageUrl ?? string.Empty,
                FileName = document.FileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize
            };
        }

        public async Task<EducationStatusDto?> GetEducationStatusByIdAsync(int studentId)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            return student is null ? null : ToEducationStatusDto(student);
        }

        public async Task<EducationStatusDto?> GetEducationStatusByUserIdAsync(int userId)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
            return student is null ? null : ToEducationStatusDto(student);
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Student not found.");

            student.IsActive = false;
            student.User.IsActive = false;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Users.Update(student.User);
            await _unitOfWork.SaveChangesAsync();
        }

        private static EducationStatusDto ToEducationStatusDto(Student student) => new()
        {
            StudentId = student.Id,
            AdmissionNumber = student.AdmissionNumber,
            RollNumber = student.RollNumber,
            Grade = student.Grade,
            Section = student.Section,
            AdmissionDate = student.AdmissionDate,
            Status = student.Status,
            IsActive = student.IsActive
        };

        private static StudentDto ToDto(Student student) => new()
        {
            Id = student.Id,
            UserId = student.UserId,
            Username = student.User.Username,
            Email = student.User.Email,
            FirstName = student.User.FirstName,
            LastName = student.User.LastName,
            PhoneNumber = student.User.PhoneNumber,
            Address = student.User.Address,
            DateOfBirth = student.User.DateOfBirth,
            Gender = student.User.Gender,
            ProfileImageUrl = student.User.ProfileImageUrl,
            AdmissionNumber = student.AdmissionNumber,
            RollNumber = student.RollNumber,
            Grade = student.Grade,
            Section = student.Section,
            AdmissionDate = student.AdmissionDate,
            GuardianName = student.GuardianName,
            GuardianPhoneNumber = student.GuardianPhoneNumber,
            BloodGroup = student.BloodGroup,
            EducationLevel = student.EducationLevel,
            IsActive = student.IsActive
        };
    }
}
