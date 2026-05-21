using Microsoft.AspNetCore.Mvc;
using MizanERP.Application.Repositories;

namespace MizanERP.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepository.GetAllAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }
    }
}