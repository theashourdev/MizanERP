using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MizanERP.Application;
using MizanERP.Application.DTOs;
using MizanERP.Application.Services;
using System;
using System.Threading.Tasks;

namespace MizanERP.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        /// <summary>
        /// Stock in - Receive goods into inventory
        /// </summary>
        [HttpPost("stockin")]
        [Authorize(Roles = "Admin,Warehouse,Manager")]
        public async Task<IActionResult> StockIn([FromBody] InventoryTransactionDto dto)
        {
            try
            {
                await _inventoryService.StockInAsync(dto);
                return Ok(new { message = "Goods received successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Stock out - Issue goods from inventory
        /// </summary>
        [HttpPost("stockout")]
        [Authorize(Roles = "Admin,Warehouse,Manager")]
        public async Task<IActionResult> StockOut([FromBody] InventoryTransactionDto dto)
        {
            try
            {
                await _inventoryService.StockOutAsync(dto);
                return Ok(new { message = "Goods issued successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Adjust inventory quantities
        /// </summary>
        [HttpPost("adjust")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Adjust([FromBody] InventoryTransactionDto dto)
        {
            try
            {
                await _inventoryService.AdjustAsync(dto);
                return Ok(new { message = "Inventory adjusted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}