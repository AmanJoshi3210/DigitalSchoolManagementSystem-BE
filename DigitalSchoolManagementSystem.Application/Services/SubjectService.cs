using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<SubjectDto>> GetAllAsync(string? grade)
        {
            var subjects = string.IsNullOrWhiteSpace(grade)
                ? await _unitOfWork.Subjects.FindAsync(s => s.IsActive)
                : await _unitOfWork.Subjects.GetByGradeAsync(grade);

            return subjects.Select(ToDto).ToList();
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            return subject is null ? null : ToDto(subject);
        }

        public async Task<SubjectDto> CreateAsync(CreateSubjectDto request)
        {
            if (await _unitOfWork.Subjects.ExistsForGradeAsync(request.Grade, request.Code))
                throw new InvalidOperationException("A subject with this code already exists for this grade.");

            var subject = new Subject
            {
                Name = request.Name,
                Code = request.Code,
                Grade = request.Grade,
                Description = request.Description
            };

            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(subject);
        }

        public async Task<SubjectDto> UpdateAsync(int id, UpdateSubjectDto request)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Subject not found.");

            if ((subject.Grade != request.Grade || subject.Code != request.Code)
                && await _unitOfWork.Subjects.ExistsForGradeAsync(request.Grade, request.Code))
                throw new InvalidOperationException("A subject with this code already exists for this grade.");

            subject.Name = request.Name;
            subject.Code = request.Code;
            subject.Grade = request.Grade;
            subject.Description = request.Description;

            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(subject);
        }

        public async Task DeleteAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Subject not found.");

            subject.IsActive = false;
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveChangesAsync();
        }

        private static SubjectDto ToDto(Subject subject) => new()
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Grade = subject.Grade,
            Description = subject.Description,
            IsActive = subject.IsActive
        };
    }
}
