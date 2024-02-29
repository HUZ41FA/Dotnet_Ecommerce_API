using ECommerce.Utilities.Helper;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Abstractions.IServices.Application
{
    public interface IEmailService
    {
        Task SendMailAsync(string[] emailList, string subject, string content);
    }
}
