using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalSchoolManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IStudentAuthService, StudentAuthService>();
            services.AddScoped<IStaffAuthService, StaffAuthService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamResultService, ExamResultService>();
            services.AddScoped<IStudentAcademicsService, StudentAcademicsService>();

            return services;
        }
    }
}
