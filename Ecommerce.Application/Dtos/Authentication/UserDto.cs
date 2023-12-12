using System;
namespace ECommerce.Application.Dtos.Authentication
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Email { get; set; }   
        public string Token { get; set; }
    }
}
