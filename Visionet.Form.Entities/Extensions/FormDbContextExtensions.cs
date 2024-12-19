using Microsoft.EntityFrameworkCore;
using Visionet.Form.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FormDbContextExtensions
    {
        public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<FormDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseOpenIddict();
            });
        }
    }
}
