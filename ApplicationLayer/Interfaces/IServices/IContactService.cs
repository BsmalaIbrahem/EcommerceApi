using ApplicationLayer.DTOs;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IContactService
    {
        Task SendMessage(ContactRequest request);
        Task<IEnumerable<ContactMessage>> GetAllMessages(); // للأدمن داشبورد
    }
}
