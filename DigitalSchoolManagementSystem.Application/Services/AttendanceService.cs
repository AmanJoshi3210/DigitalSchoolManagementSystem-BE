using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AttendanceDto> MarkAsync(MarkAttendanceDto request)
        {
            if (!await _unitOfWork.Students.ExistsAsync(s => s.Id == request.StudentId))
                throw new KeyNotFoundException("Student not found.");

            var attendance = await _unitOfWork.Attendances.GetByStudentAndDateAsync(request.StudentId, request.Date);

            if (attendance is null)
            {
                attendance = new Attendance
                {
                    StudentId = request.StudentId,
                    Date = request.Date.Date,
                    Status = request.Status,
                    Remarks = request.Remarks
                };
                await _unitOfWork.Attendances.AddAsync(attendance);
            }
            else
            {
                attendance.Status = request.Status;
                attendance.Remarks = request.Remarks;
                _unitOfWork.Attendances.Update(attendance);
            }

            await _unitOfWork.SaveChangesAsync();
            return ToDto(attendance);
        }

        public async Task<IReadOnlyList<AttendanceDto>> GetByStudentAsync(int studentId, DateTime? from, DateTime? to)
        {
            var records = await _unitOfWork.Attendances.GetByStudentAsync(studentId, from, to);
            return records.Select(ToDto).ToList();
        }

        public async Task<AttendanceSummaryDto> GetSummaryAsync(int studentId, DateTime? from, DateTime? to)
        {
            var records = await _unitOfWork.Attendances.GetByStudentAsync(studentId, from, to);

            var present = records.Count(a => a.Status == AttendanceStatus.Present);
            var absent = records.Count(a => a.Status == AttendanceStatus.Absent);
            var late = records.Count(a => a.Status == AttendanceStatus.Late);
            var half = records.Count(a => a.Status == AttendanceStatus.HalfDay);
            var excused = records.Count(a => a.Status == AttendanceStatus.Excused);

            var countedDays = records.Count - excused;
            var attendedDays = present + late + (half * 0.5m);
            var percentage = countedDays > 0 ? Math.Round(attendedDays / countedDays * 100, 2) : 0m;

            return new AttendanceSummaryDto
            {
                StudentId = studentId,
                From = from,
                To = to,
                TotalDays = records.Count,
                PresentDays = present,
                AbsentDays = absent,
                LateDays = late,
                HalfDays = half,
                ExcusedDays = excused,
                AttendancePercentage = percentage
            };
        }

        private static AttendanceDto ToDto(Attendance attendance) => new()
        {
            Id = attendance.Id,
            StudentId = attendance.StudentId,
            Date = attendance.Date,
            Status = attendance.Status,
            Remarks = attendance.Remarks
        };
    }
}
