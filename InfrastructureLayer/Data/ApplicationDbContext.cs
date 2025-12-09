using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLayer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<Languages> Languages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var dateNow = new DateTime(2025, 1, 1);
            builder.Entity<Languages>().HasData(
                new Languages { Id = 1, Name = "English", Code=LanguageCode.English, CreatedAt= dateNow , UpdatedAt = dateNow},
                new Languages { Id = 2, Name = "Arabic", Code=LanguageCode.Arabic, CreatedAt = dateNow, UpdatedAt = dateNow }
            );
        }
    }
}
