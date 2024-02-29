using ECommerce.Domain.Abstractions.IRepository.Application;
using ECommerce.Domain.Models.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.DataAccess.Repository.Application
{
    public class UserRepository : GenericRepository<SiteUser>, IUserRepository
    {
        private readonly UserManager<SiteUser> _userManager;
        public UserRepository(ApplicationDBContext context, ILogger logger, UserManager<SiteUser> userManager) : base(context, logger)
        {
            _userManager = userManager;
        }
        public async Task<IdentityResult> AddAsync(SiteUser model, string password)
        {
            try
            {
                model.Id = Guid.NewGuid().ToString();
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = model.Id;
                return await _userManager.CreateAsync(model, password);
            }
            catch
            {
                throw;
            }
        }
        public override async Task<IEnumerable<SiteUser>> GetAll()
        {
            try
            {
                return await _userManager.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User Repository Error", typeof(UserRepository));
                return new List<SiteUser>();
            }
        }
        public async Task<SiteUser> FindByEmailAsync(string email)
        {
            try
            {
                SiteUser user = await _userManager.FindByEmailAsync(email);
                return user;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<IdentityResult> UpdateAsync(SiteUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
