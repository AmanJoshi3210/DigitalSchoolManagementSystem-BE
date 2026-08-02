using DigitalSchoolManagementSystem.Application.DTOs.Students;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            student.User.FirstName = request.FirstName;
            student.User.LastName = request.LastName;
            student.User.PhoneNumber = request.PhoneNumber;
            student.User.Address = request.Address;
            student.User.DateOfBirth = request.DateOfBirth;
            student.User.Gender = request.Gender;
            student.User.ProfileImageUrl = request.ProfileImageUrl;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Users.Update(student.User);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(student);
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
            IsActive = student.IsActive
        };
    }
}
