using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace Mango.Services.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<ApplicationDbContext> _dboptions;

        public EmailService(DbContextOptions<ApplicationDbContext> dboptions)
        {
            _dboptions = dboptions;
        }

        public async Task EmailAndLog(ShoppingCartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDTO.Cart.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDTO?.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDTO.Cart.Email);
        }

        public async Task LogOrderPlaced(OrderPlacedEmailNotificationDTO orderDTO)
        {
            string message = "New Order placed. <br/> Order ID :" + orderDTO.OrderId;
            await LogAndEmail(message, "fastrobot@greatIndianKithchen.com");
        }

        private async Task<bool> LogAndEmail(string message, string? email)
        {
            try
            {
                EmailLog emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new ApplicationDbContext(_dboptions);
                await _db.EmailLogs.AddAsync(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
