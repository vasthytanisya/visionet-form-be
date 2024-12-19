using Visionet.Form.Entities.Extensions;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visionet.Form.Entities;

namespace Visionet.Form.Entities
{
    public class FormDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
    {
        /// <summary>
        /// https://www.postgresql.org/docs/current/pgtrgm.html
        /// </summary>
        private const string PgTrigramExtension = "pg_trgm";

        public FormDbContext(DbContextOptions<FormDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension(PgTrigramExtension);

            // Guid max length
            builder.Entity<OpenIddictEntityFrameworkCoreApplication>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreScope>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreToken>().Property(Q => Q.Id).HasMaxLength(36);

            foreach (var relationship in builder.Model.GetEntityTypes().Where(Q => !Q.IsOwned()).SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }
        }
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    }
}
