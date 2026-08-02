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

            return services;
        }
    }
}
