using Microsoft.AspNetCore.Mvc;
using MizanERP.Application; // added for IUnitOfWork
using MizanERP.Application.DTOs;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupplierController(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork)
        {
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return View(suppliers);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new CreateSupplierDto());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Address? address = null;
            if (model.Address != null && !string.IsNullOrWhiteSpace(model.Address.Line1))
            {
                address = new Address(
                    model.Address.Line1,
                    model.Address.City,
                    model.Address.State,
                    model.Address.PostalCode,
                    model.Address.Country,
                    model.Address.Line2
                );
            }

            var supplier = new Supplier(Guid.NewGuid(), model.Name, address, model.ContactInfo);
            await _supplierRepository.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return NotFound();

            var dto = new SupplierEditDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactInfo = supplier.ContactInfo,
                Address = supplier.Address != null ? new AddressDto
                {
                    Line1 = supplier.Address.Line1,
                    Line2 = supplier.Address.Line2,
                    City = supplier.Address.City,
                    State = supplier.Address.State,
                    PostalCode = supplier.Address.PostalCode,
                    Country = supplier.Address.Country
                } : new AddressDto()
            };

            return View(dto);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierEditDto model)
        {
            if (model.Id == Guid.Empty) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = await _supplierRepository.GetByIdAsync(model.Id);
            if (existing == null) return NotFound();

            Address? address = null;
            if (model.Address != null && !string.IsNullOrWhiteSpace(model.Address.Line1))
            {
                address = new Address(
                    model.Address.Line1,
                    model.Address.City,
                    model.Address.State,
                    model.Address.PostalCode,
                    model.Address.Country,
                    model.Address.Line2
                );
            }

            // Use domain update methods on the tracked entity
            existing.UpdateName(model.Name);
            existing.UpdateContactInfo(model.ContactInfo);
            existing.UpdateAddress(address);

            await _supplierRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Delete (soft delete) - AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return Json(new { success = false, message = "Not found" });

            await _supplierRepository.DeleteAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}