using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Visionet.Form.Entities.MigrationDesigners
{
    internal class DesignTimeFormDbContextFactory : IDesignTimeDbContextFactory<FormDbContext>
    {
        public FormDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FormDbContext>();

            optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=visionet_form;Username=postgres;Password=0000");
            optionsBuilder.UseOpenIddict();

            return new FormDbContext(optionsBuilder.Options);
        }
    }
}
