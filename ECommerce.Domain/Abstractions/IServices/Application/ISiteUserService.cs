using ECommerce.Domain.Models.Application;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Abstractions.IServices.Application
{
    public interface ISiteUserService
    {
        Task<IdentityResult> AddAsync(SiteUser user, string password);
        Task<IdentityResult> UpdateAsync(SiteUser user);
        Task<IdentityResult> DeleteAsync(SiteUser user);
        Task<SiteUser> GetByEmailAsync(string email);
        Task<IEnumerable<string>> GetUserRolesAsync(SiteUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(SiteUser user);
        Task<IdentityResult> ConfirmEmailAsync(SiteUser user, string confirmationToken);
        Task<bool> CheckPasswordAsync(SiteUser user, string password);
        Task<string> GeneratePasswordResetTokenAsync(SiteUser user);
        Task<IdentityResult> ResetPasswordAsync(SiteUser user, string token, string newPassword);
        Task<string> GenerateTwoFactorTokenAsync(SiteUser user, string tokenProvider);

    }
}
