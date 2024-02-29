using ECommerce.Domain.Models.Application;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Abstractions.IRepository.Application
{
    public interface IUserRepository : IGenericRepository<SiteUser>
    {
        Task<SiteUser> FindByEmailAsync(string email);
        Task<IdentityResult> UpdateAsync(SiteUser siteUser);
        Task<IdentityResult> AddAsync(SiteUser siteUser, string password);
    }
}
