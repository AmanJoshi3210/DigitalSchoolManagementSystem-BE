using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ExamDto>> GetAllAsync(string? grade, string? academicYear)
        {
            var exams = await _unitOfWork.Exams.SearchAsync(grade, academicYear);
            return exams.Select(e => ToDto(e, e.ExamSubjects)).ToList();
        }

        public async Task<ExamDto?> GetByIdAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithSubjectsAsync(id);
            return exam is null ? null : ToDto(exam, exam.ExamSubjects);
        }

        public async Task<ExamDto> CreateAsync(CreateExamDto request)
        {
            var exam = new Exam
            {
                Name = request.Name,
                Grade = request.Grade,
                AcademicYear = request.AcademicYear,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(exam, exam.ExamSubjects);
        }

        public async Task<ExamDto> UpdateAsync(int id, UpdateExamDto request)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Exam not found.");

            exam.Name = request.Name;
            exam.Grade = request.Grade;
            exam.AcademicYear = request.AcademicYear;
            exam.StartDate = request.StartDate;
            exam.EndDate = request.EndDate;

            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(exam, exam.ExamSubjects);
        }

        public async Task DeleteAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Exam not found.");

            exam.IsActive = false;
            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamSubjectDto> AddSubjectAsync(int examId, AddExamSubjectDto request)
        {
            if (!await _unitOfWork.Exams.ExistsAsync(e => e.Id == examId))
                throw new KeyNotFoundException("Exam not found.");

            if (!await _unitOfWork.Subjects.ExistsAsync(s => s.Id == request.SubjectId))
                throw new KeyNotFoundException("Subject not found.");

            if (await _unitOfWork.Exams.SubjectAlreadyScheduledAsync(examId, request.SubjectId))
                throw new InvalidOperationException("This subject is already scheduled for the exam.");

            var examSubject = new ExamSubject
            {
                ExamId = examId,
                SubjectId = request.SubjectId,
                ExamDate = request.ExamDate,
                MaxMarks = request.MaxMarks,
                PassingMarks = request.PassingMarks
            };

            await _unitOfWork.ExamSubjects.AddAsync(examSubject);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Exams.GetExamSubjectAsync(examSubject.Id)
                ?? throw new InvalidOperationException("Failed to load the created exam subject.");

            return ToExamSubjectDto(created);
        }

        public async Task<ExamSubjectDto> UpdateSubjectAsync(int examId, int examSubjectId, UpdateExamSubjectDto request)
        {
            var examSubject = await _unitOfWork.Exams.GetExamSubjectAsync(examSubjectId);
            if (examSubject is null || examSubject.ExamId != examId)
                throw new KeyNotFoundException("Exam subject not found.");

            examSubject.ExamDate = request.ExamDate;
            examSubject.MaxMarks = request.MaxMarks;
            examSubject.PassingMarks = request.PassingMarks;

            _unitOfWork.ExamSubjects.Update(examSubject);
            await _unitOfWork.SaveChangesAsync();

            return ToExamSubjectDto(examSubject);
        }

        public async Task RemoveSubjectAsync(int examId, int examSubjectId)
        {
            var examSubject = await _unitOfWork.Exams.GetExamSubjectAsync(examSubjectId);
            if (examSubject is null || examSubject.ExamId != examId)
                throw new KeyNotFoundException("Exam subject not found.");

            _unitOfWork.ExamSubjects.Remove(examSubject);
            await _unitOfWork.SaveChangesAsync();
        }

        private static ExamDto ToDto(Exam exam, IEnumerable<ExamSubject> examSubjects) => new()
        {
            Id = exam.Id,
            Name = exam.Name,
            Grade = exam.Grade,
            AcademicYear = exam.AcademicYear,
            StartDate = exam.StartDate,
            EndDate = exam.EndDate,
            Subjects = examSubjects.Select(ToExamSubjectDto).ToList()
        };

        private static ExamSubjectDto ToExamSubjectDto(ExamSubject examSubject) => new()
        {
            Id = examSubject.Id,
            ExamId = examSubject.ExamId,
            SubjectId = examSubject.SubjectId,
            SubjectName = examSubject.Subject.Name,
            ExamDate = examSubject.ExamDate,
            MaxMarks = examSubject.MaxMarks,
            PassingMarks = examSubject.PassingMarks
        };
    }
}
