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
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        
        public SalesController(ISalesService salesService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
        }

        /// <summary>
        /// Create a new sales order
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> Create([FromBody] CreateSalesOrderDto dto)
        {
            try
            {
                var id = await _salesService.CreateSalesOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Ship goods for a sales order
        /// </summary>
        [HttpPost("{id}/ship")]
        [Authorize(Roles = "Admin,Warehouse,Manager")]
        public async Task<IActionResult> Ship(Guid id)
        {
            try
            {
                await _salesService.ShipGoodsAsync(id);
                return Ok(new { message = "Goods shipped successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales order by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { id });
        }
    }
}