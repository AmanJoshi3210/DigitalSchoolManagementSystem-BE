using CloudinaryDotNet;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Options;
using DigitalSchoolManagementSystem.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DigitalSchoolManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

            // Cloudinary wraps an HttpClient internally, so a single long-lived instance is
            // the efficient choice — avoid rebuilding it (and its connections) per request.
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account) { Api = { Secure = true } };
            });

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
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IDocumentService, DocumentService>();

            return services;
        }
    }
}
