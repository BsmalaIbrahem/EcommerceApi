using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<ContactMessage> _repository;

        public ContactService(IRepository<ContactMessage> repository)
        {
            _repository = repository;
        }

        public async Task SendMessage(ContactRequest request)
        {
            var message = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Subject = request.Subject,
                Message = request.Message
            };

            await _repository.AddAsync(message);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetAllMessages()
        {
            return await _repository.GetAllAsync(orderBy: q => q.OrderByDescending(m => m.CreatedAt));
        }
    }
}
