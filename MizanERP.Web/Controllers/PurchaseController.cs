using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using MizanERP.Application;
using MizanERP.Application.DTOs;
using MizanERP.Application.Exceptions;
using MizanERP.Application.Services;
using MizanERP.Domain.Entities;

namespace MizanERP.Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPurchaseService _purchaseService;
        private readonly INotyfService _notifyService;
        public PurchaseController(IUnitOfWork unitOfWork, IPurchaseService purchaseService, INotyfService notifyService)
        {
            _unitOfWork = unitOfWork;
            _purchaseService = purchaseService;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _unitOfWork.PurchaseOrders.GetAllAsync();
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var txns = await _unitOfWork.CapitalTransactions.GetAllAsync();

            ViewBag.Suppliers = suppliers.ToDictionary(s => s.Id, s => s.Name);
            // Map domain User.UserName as display name
            ViewBag.Users = users.ToDictionary(u => u.Id, u => u.UserName ?? string.Empty);

            // Only create mapping for transactions that are linked to a purchase (non-null PurchaseOrderId)
            var linkedTxns = txns.Where(t => t.PurchaseOrderId.HasValue);


            var txnMap = linkedTxns
                .GroupBy(t => t.PurchaseOrderId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => new PurchaseTransactionInfo
                    {
                        LatestTransaction = g.OrderByDescending(x => x.Date).FirstOrDefault(),
                        TotalAmount = g.Sum(x => x.Amount),
                        Count = g.Count()
                    });

            ViewBag.CapitalTxns = txnMap; // Dictionary<Guid, CapitalTransaction>

            return View(purchases);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var products = await _unitOfWork.Products.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var txns = await _unitOfWork.CapitalTransactions.GetAllAsync();
            var nonDeleted = txns.Where(t => !t.IsDeleted);

            decimal currentBalance = nonDeleted
                .OrderByDescending(t => t.Date)
                .FirstOrDefault()?.Amount ?? 0m;

            ViewBag.Suppliers = suppliers;
            ViewBag.Products = products;
            ViewBag.Users = users;
            ViewBag.CurrentBalance = currentBalance;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto model)
        {
            // 1. Fetch data early to establish baseline variables
            var txns = await _unitOfWork.CapitalTransactions.GetAllAsync();
            decimal currentBalance = txns.Where(t => !t.IsDeleted).OrderByDescending(t => t.Date).FirstOrDefault()?.Amount ?? 0m;
            decimal totalCost = model.Lines?.Sum(l => l.Quantity * l.Price) ?? 0m;

            try
            {
                // 2. Validate sufficient capital
                if (currentBalance - totalCost < 0)
                {
                    throw new BusinessException("Insufficient capital for this purchase.");
                }

                // 3. Proceed to creation
                var id = await _purchaseService.CreatePurchaseOrderAsync(model); // placeholder kept for tooling
                _notifyService.Success("Purchase saved successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                // Handled business rule failures gracefully
                ModelState.AddModelError(string.Empty, ex.Message);
                _notifyService.Warning(ex.Message);

                await PopulateViewBagAsync(currentBalance);
                return View(model);
            }
            catch (Exception ex)
            {
                // Handle unexpected system errors (log 'ex' if you have a logger)
                _notifyService.Error("An error occurred while processing the purchase.");
                Console.WriteLine(ex); // Replace with proper logging
                await PopulateViewBagAsync(currentBalance);
                return View(model);
            }
        }

        // Helper method to eliminate massive code duplication
        private async Task PopulateViewBagAsync(decimal currentBalance)
        {
            ViewBag.Suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Users = await _unitOfWork.Users.GetAllAsync();
            ViewBag.CurrentBalance = currentBalance;
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var purchase = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
            if (purchase == null) return NotFound();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(purchase.SupplierId);
            ViewBag.SupplierName = supplier?.Name;

            var txns = await _unitOfWork.CapitalTransactions.GetAllAsync();
            var cap = txns.Where(t => t.PurchaseOrderId == purchase.Id).OrderByDescending(t => t.Date).FirstOrDefault();
            ViewBag.CapitalTransaction = cap;

            var users = await _unitOfWork.Users.GetAllAsync();
            ViewBag.Users = users.ToDictionary(u => u.Id, u => u.UserName ?? string.Empty);

            // Product lookup: pass full product objects so view can render code/unit/name
            var products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.ProductMap = products.ToDictionary(p => p.Id, p => p);

            return View(purchase);
        }

        // Temporary placeholder to satisfy edit tooling; actual method exists earlier in file.
        private async Task<Guid> _purchase_service_rename(CreatePurchaseOrderDto model)
        {
            return await _purchaseService.CreatePurchaseOrderAsync(model);
        }
    }

    public class PurchaseTransactionInfo
    {
        public CapitalTransaction? LatestTransaction { get; set; }

        public decimal TotalAmount { get; set; }

        public int Count { get; set; }
    }
}
