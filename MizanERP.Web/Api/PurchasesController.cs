using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MizanERP.Application;
using MizanERP.Application.DTOs;
using MizanERP.Application.Services;
using System.Threading.Tasks;

namespace MizanERP.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        
        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
        }

        /// <summary>
        /// Create a new purchase order
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            try
            {
                var id = await _purchaseService.CreatePurchaseOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Receive goods for a purchase order
        /// </summary>
        [HttpPost("{id}/receive")]
        [Authorize(Roles = "Admin,Warehouse,Manager")]
        public async Task<IActionResult> Receive(Guid id)
        {
            try
            {
                await _purchaseService.ReceiveGoodsAsync(id);
                return Ok(new { message = "Goods received successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get purchase order by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { id });
        }
    }
}