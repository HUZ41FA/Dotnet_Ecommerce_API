using ECommerce.Domain.Models.Application;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Abstractions.IServices.Application
{
    public interface IApplicationAuthenticationService
    {
        Task<ApiResponse> Register(SiteUser model);
        Task<ApiResponse> ConfirmEmail(string token, string email);
        Task<ApiResponse> Login(string email, string password);
        Task<ApiResponse> SendForgetPasswordLink(string email);
        Task<ApiResponse> ResetPassword(string email, string password, string confirmPassword, string token);
    }
}
