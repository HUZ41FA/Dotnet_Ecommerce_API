using AutoMapper;
using ECommerce.Application.Dtos.Authentication;
using ECommerce.Application.Services;
using ECommerce.Domain.Application;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return StatusCode(StatusCodes.Status200OK, new { Message = "It is wortking fine"});
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };
                if(Helper.ConfirmPassword(model.Password, model.ConfirmPassword))
                {
                    SiteUser user = new() { FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, Password = model.Password };
                    response = await _authenticationService.Register(user);

                    if (response.Status)
                    {
                        return StatusCode(StatusCodes.Status201Created, response);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, response);
                    }
                }
                else
                {
                    response.Messages.Add("Passwords do not match");
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }
            }
            catch
            {
                throw;
            }
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([Required]string token, [Required]string email)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };

                response = await  _authenticationService.ConfirmEmail(token, email);

                if (response.Status)
                {
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                ApiResponse response = new() { Status = false, Messages = new List<string>() };

                response = await _authenticationService.Login(model.Email, model.Password);

                if (response.Status)
                {
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            catch
            {
                throw;
            }
        }
    }
}
