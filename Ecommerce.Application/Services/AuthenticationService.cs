using ECommerce.Application.Dtos.Authentication;
using ECommerce.Domain.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Application.Services
{
    public class AuthenticationService
    {
        private readonly UserManager<SiteUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _context;
        private readonly EmailService _emailService;
        private readonly JwtConfig _jwtConfig;
        private readonly SignInManager<SiteUser> _signInManager;

        public AuthenticationService(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext context, 
            EmailService emailService, JwtConfig jwtConfig, SignInManager<SiteUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;
            _jwtConfig = jwtConfig;
            _signInManager = signInManager;
        }

        #region CRUD
        public async Task<IdentityResult> Add(SiteUser model)
        {
            model.Id = Guid.NewGuid().ToString();
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = model.Id;
            return await _userManager.CreateAsync(model);
        }
        #endregion
        public async Task<ApiResponse> Register(SiteUser model)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                SiteUser existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    response.Messages.Add($"User with email {model.Email} already exists");
                    return response;
                }
                else
                {
                    model.UserName = model.Email;
                    IdentityResult userResult = await Add(model);

                    if (userResult.Succeeded)
                    {
                        string token = await _userManager.GenerateEmailConfirmationTokenAsync(model);
                        string link = $"https://localhost:44371/api/authentication/confirm-email?token={token}&email={model.Email}";
                        _emailService.SendMail(new[] { model.Email }, "Email Verification", $"Please follow the below link to confirm your email {link}");
                        response.Messages.Add("Registration Successful");
                        response.Status = true;
                        return response;
                    }
                    else
                    {
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
                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                SiteUser user = await _userManager.FindByEmailAsync(email);

                if(user != null)
                {
                    IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

                    if (result.Succeeded)
                    {
                        response.Status = true;
                        response.Messages.Add("Email Verified");
                        return response;
                    }
                    else
                    {
                        response.Messages.AddRange(result.Errors.Select(e => e.Description));
                        return response;
                    }
                }
                else
                {
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
                SiteUser user = await _userManager.FindByEmailAsync(email);

                if (user != null && user.TwoFactorEnabled)
                {
                    await Send2FAEmail(user, password);
                    response.Status = true;
                    response.Messages.Add($"OTP sent to email {user.Email}");
                    return response;
                }

                if (user != null && await _userManager.CheckPasswordAsync(user!, password))
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
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string link = $"https://localhost:44371/api/authentication/reset-password?token={token}&email={user.Email}";

                    _emailService.SendMail(new[] { user.Email }, "Reset Password", $"Please click the link below to reset your password: {link}");
                    
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

        public async Task<ApiResponse> ResetPassword(ResetPasswordDto model)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

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

                var token = _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                _emailService.SendMail(new[] { user.Email }, "OTP Confirmation", $"You requested a verification token: {token}");
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
                    new Claim("UId", user.Id),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles != null && userRoles.Count > 0)
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
