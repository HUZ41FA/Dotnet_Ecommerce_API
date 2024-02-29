using ECommerce.Domain.Abstractions.IRepository.Application;
using ECommerce.Domain.Abstractions.IUnitOfWork;
using ECommerce.Domain.Models.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using ECommerce.Infrastructure.DataAccess.Repository.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ECommerce.Infrastructure.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly UserManager<SiteUser> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly  ILogger _logger;
        private IDbContextTransaction transaction = null;
        public IUserRepository Users { get; private set; }
        public UnitOfWork(ApplicationDBContext context, ILoggerFactory loggerFactory, UserManager<SiteUser> userManager)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");
            _userManager = userManager;

            Users = new UserRepository(_context, _logger, _userManager);
        }
        public async Task BeginTransactionAsync()
        {
            if (transaction == null)
            {
                transaction = await _context.Database.BeginTransactionAsync();
            }
        }
        public async Task CommitChangesAsync()
        {
            await transaction.CommitAsync();
            Dispose();
        }   
        public async Task RollbackChangesAsync()
        {
            await transaction.RollbackAsync();
            Dispose();
        }
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
