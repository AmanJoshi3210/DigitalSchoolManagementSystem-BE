using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class ExamResultService : IExamResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamResultDto> RecordAsync(RecordExamResultDto request)
        {
            if (!await _unitOfWork.Students.ExistsAsync(s => s.Id == request.StudentId))
                throw new KeyNotFoundException("Student not found.");

            var examSubject = await _unitOfWork.Exams.GetExamSubjectAsync(request.ExamSubjectId)
                ?? throw new KeyNotFoundException("Exam subject not found.");

            if (request.MarksObtained < 0 || request.MarksObtained > examSubject.MaxMarks)
                throw new InvalidOperationException($"Marks obtained must be between 0 and {examSubject.MaxMarks}.");

            var grade = CalculateGrade(request.MarksObtained, examSubject.MaxMarks);
            var existing = await _unitOfWork.ExamResults.GetByStudentAndExamSubjectAsync(request.StudentId, request.ExamSubjectId);

            if (existing is null)
            {
                var result = new ExamResult
                {
                    StudentId = request.StudentId,
                    ExamSubjectId = request.ExamSubjectId,
                    MarksObtained = request.MarksObtained,
                    Grade = grade,
                    Remarks = request.Remarks
                };

                await _unitOfWork.ExamResults.AddAsync(result);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                existing.MarksObtained = request.MarksObtained;
                existing.Grade = grade;
                existing.Remarks = request.Remarks;
                _unitOfWork.ExamResults.Update(existing);
                await _unitOfWork.SaveChangesAsync();
            }

            var saved = await _unitOfWork.ExamResults.GetByStudentAndExamSubjectAsync(request.StudentId, request.ExamSubjectId)
                ?? throw new InvalidOperationException("Failed to load the recorded exam result.");

            return ToDto(saved);
        }

        public async Task<IReadOnlyList<ExamResultDto>> GetByStudentAndExamAsync(int studentId, int examId)
        {
            var results = await _unitOfWork.ExamResults.GetByStudentAndExamAsync(studentId, examId);
            return results.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<ExamResultDto>> GetByStudentAsync(int studentId)
        {
            var results = await _unitOfWork.ExamResults.GetByStudentAsync(studentId);
            return results.Select(ToDto).ToList();
        }

        private static string CalculateGrade(decimal marksObtained, decimal maxMarks)
        {
            if (maxMarks <= 0)
                return "N/A";

            var percentage = marksObtained / maxMarks * 100;

            return percentage switch
            {
                >= 90 => "A+",
                >= 80 => "A",
                >= 70 => "B+",
                >= 60 => "B",
                >= 50 => "C",
                >= 40 => "D",
                _ => "F"
            };
        }

        private static ExamResultDto ToDto(ExamResult result) => new()
        {
            Id = result.Id,
            StudentId = result.StudentId,
            ExamSubjectId = result.ExamSubjectId,
            ExamId = result.ExamSubject.ExamId,
            ExamName = result.ExamSubject.Exam.Name,
            SubjectId = result.ExamSubject.SubjectId,
            SubjectName = result.ExamSubject.Subject.Name,
            MarksObtained = result.MarksObtained,
            MaxMarks = result.ExamSubject.MaxMarks,
            PassingMarks = result.ExamSubject.PassingMarks,
            Grade = result.Grade,
            Passed = result.MarksObtained >= result.ExamSubject.PassingMarks,
            Remarks = result.Remarks
        };
    }
}
