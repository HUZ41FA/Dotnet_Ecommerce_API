using ECommerce.Domain.Models.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.DataAccess.ApplicationDbContext
{
    public class ApplicationDBContext : IdentityDbContext<SiteUser>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }
        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
    }
}
