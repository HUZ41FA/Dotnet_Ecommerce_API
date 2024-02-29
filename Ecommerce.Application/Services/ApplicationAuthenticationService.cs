using ECommerce.Application.Dtos.Authentication;
using ECommerce.Domain.Models.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerce.Domain.Abstractions.IServices.Application;
using ECommerce.Domain.Abstractions.IRepository.Application;
using ECommerce.Domain.Abstractions.IUnitOfWork;
using System.Web;

namespace ECommerce.Application.Services
{
    public class ApplicationAuthenticationService : IApplicationAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISiteUserService _siteUserService;
        private readonly IEmailService _emailService;
        private readonly JwtConfig _jwtConfig;
        private readonly SignInManager<SiteUser> _signInManager;

        public ApplicationAuthenticationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService, 
            ISiteUserService siteUserService, 
            JwtConfig jwtConfig, 
            SignInManager<SiteUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _siteUserService = siteUserService;
            _emailService = emailService;
            _jwtConfig = jwtConfig;
            _signInManager = signInManager;
        }

        public async Task<ApiResponse> Register(SiteUser model)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                SiteUser existingUser = await _siteUserService.GetByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    response.Messages.Add($"User with email {model.Email} already exists");
                    return response;
                }
                else
                {
                    model.UserName = model.Email;
                    IdentityResult userResult = await _siteUserService.AddAsync(model, model.Password);

                    if (userResult.Succeeded)
                    {
                        string token = await _siteUserService.GenerateEmailConfirmationTokenAsync(model);
                        string link = Helper.GenerateEmailConfirmationLink(token, model.Email);
                        
                        //_emailService.SendMail(new[] { model.Email }, "Email Verification", $"Please follow the below link to confirm your email {link}");
                        
                        response.Messages.Add("Registration Successful");
                        response.Status = true;

                        await _unitOfWork.CommitChangesAsync();

                        return response;
                    }
                    else
                    {
                        await _unitOfWork.RollbackChangesAsync();
                        response.Messages.AddRange(userResult.Errors.Select(e => e.Description));
                        return response;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<ApiResponse> ConfirmEmail(string token, string email)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                SiteUser user = await _siteUserService.GetByEmailAsync(email);

                if(user != null)
                {
                    IdentityResult result = await _siteUserService.ConfirmEmailAsync(user, token);

                    if (result.Succeeded)
                    {
                        await _unitOfWork.CommitChangesAsync();
                        response.Status = true;
                        response.Messages.Add("Email Verified");
                        return response;
                    }
                    else
                    {
                        await _unitOfWork.RollbackChangesAsync();
                        response.Messages.AddRange(result.Errors.Select(e => e.Description));
                        return response;
                    }
                }
                else
                {
                    await _unitOfWork.RollbackChangesAsync();
                    response.Messages.Add("Invalid Email");
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<ApiResponse> Login(string email, string password)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                SiteUser user = await _siteUserService.GetByEmailAsync(email);

                if (user != null && user.TwoFactorEnabled)
                {
                    await Send2FAEmail(user, password);
                    response.Status = true;
                    response.Messages.Add($"OTP sent to email {user.Email}");
                    return response;
                }

                if (user != null && await _siteUserService.CheckPasswordAsync(user!, password))
                {

                    List<Claim> claimList = await GetUserClaims(user); 

                    JwtSecurityToken token = Helper.GenerateJwtToken(claimList, _jwtConfig);
                    UserDto userDto = new UserDto() { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Token = new JwtSecurityTokenHandler().WriteToken(token) };
                    response.Status = true;
                    response.Data = userDto;
                    return response;
                }

                response.Messages.Add("Invalid email or password.");
                return response;
            }
            catch
            {
                throw;
            }
        }
        public async Task<ApiResponse> SendForgetPasswordLink(string email)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                var user = await _siteUserService.GetByEmailAsync(email);
                if (user != null)
                {
                    var token = await _siteUserService.GeneratePasswordResetTokenAsync(user);
                    string link = $"https://localhost:44371/api/authentication/reset-password?token={token}&email={user.Email}";

                    await _emailService.SendMailAsync(new[] { user.Email }, "Reset Password", $"Please click the link below to reset your password: {link}");
                    
                    response.Status = true;
                    return response;
                }

                response.Messages.Add("Invalid email");
                return response;
            }
            catch
            {
                throw;
            }
        }
        public async Task<ApiResponse> ResetPassword(string email, string password, string confirmPassword, string token)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };

                var user = await _siteUserService.GetByEmailAsync(email);

                if (user != null)
                {
                    var result = await _siteUserService.ResetPasswordAsync(user, token, password);

                    if (!result.Succeeded)
                    {
                        response.Messages.AddRange(result.Errors.Select(e => e.Description));
                        return response;
                    }

                    response.Status = true;
                    return response;
                }
                else
                {
                    response.Messages.Add("Invalid email.");
                    return response;
                }
            }
            catch
            {
                throw;
            }
        }
        private async Task Send2FAEmail(SiteUser user, string password)
        {
            try
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, password, false, true);

                var token = _siteUserService.GenerateTwoFactorTokenAsync(user, "Email");
                await _emailService.SendMailAsync(new[] { user.Email }, "OTP Confirmation", $"You requested a verification token: {token}");
            }
            catch
            {
                throw;
            }
        }
        private async Task<List<Claim>> GetUserClaims(SiteUser user)
        {
            try
            {
                List<Claim> claimList = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Sub, user.Email),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
                };

                IEnumerable<string> userRoles = await _siteUserService.GetUserRolesAsync(user);

                if (userRoles != null && userRoles.Count() > 0)
                {
                    claimList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
                }

                return claimList;
            }
            catch
            {
                throw;
            }
        }
    }
}
