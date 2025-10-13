using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Route("api/Customer/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BookController : Controller
    {
        
       //
    }
}
