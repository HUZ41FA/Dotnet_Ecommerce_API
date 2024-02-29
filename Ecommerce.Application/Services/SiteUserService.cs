using ECommerce.Domain.Abstractions.IRepository.Application;
using ECommerce.Domain.Abstractions.IServices.Application;
using ECommerce.Domain.Abstractions.IUnitOfWork;
using ECommerce.Domain.Models.Application;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class SiteUserService : ISiteUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SiteUser> _userManager;    
        public SiteUserService(UserManager<SiteUser> userManager, IUnitOfWork unitOfWork) 
        {
            _userRepository = unitOfWork.Users;
            _userManager = userManager;
        }
        public async Task<IdentityResult> AddAsync(SiteUser user, string password)
        {
            try
            {
                try
                {
                    user.Id = Guid.NewGuid().ToString();
                    user.CreatedAt = DateTime.Now;
                    user.CreatedBy = user.Id;
                    return await _userRepository.AddAsync(user, password);
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<IdentityResult> UpdateAsync(SiteUser user)
        {
            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IdentityResult> DeleteAsync(SiteUser user)
        {
            try
            {
                user.IsDeleted = true;
                return await _userRepository.UpdateAsync(user);
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> CheckPasswordAsync(SiteUser user, string password)
        {
            try
            {
                return await _userManager.CheckPasswordAsync(user, password);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IdentityResult> ConfirmEmailAsync(SiteUser user, string confirmationToken)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                return await _userManager.ConfirmEmailAsync(user, confirmationToken);
            }
            catch
            {
                throw;
            }
        }
        public async Task<SiteUser> GetByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.FindByEmailAsync(email);
            }
            catch 
            {
                throw;
            }
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(SiteUser user)
        {
            try
            {
                return await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GeneratePasswordResetTokenAsync(SiteUser user)
        {
            try
            {
                return await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GenerateTwoFactorTokenAsync(SiteUser user, string tokenProvider = "Email")
        {
            try
            {
                return await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IEnumerable<string>> GetUserRolesAsync(SiteUser user)
        {
            try
            {
                return await _userManager.GetRolesAsync(user);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IdentityResult> ResetPasswordAsync(SiteUser user, string token, string newPassword)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                return await _userManager.ResetPasswordAsync(user, token, newPassword);
            }
            catch
            {
                throw;
            }
        }
    }
}
