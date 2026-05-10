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
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;
        
        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService ?? throw new System.ArgumentNullException(nameof(productionService));
        }

        /// <summary>
        /// Create a new production order
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Production")]
        public async Task<IActionResult> Create([FromBody] CreateProductionOrderDto dto)
        {
            try
            {
                var id = await _productionService.CreateProductionOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Produce finished goods
        /// </summary>
        [HttpPost("{id}/produce")]
        [Authorize(Roles = "Admin,Production,Manager")]
        public async Task<IActionResult> Produce(Guid id)
        {
            try
            {
                await _productionService.ProduceFinishedGoodsAsync(id);
                return Ok(new { message = "Production completed successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get production order by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { id });
        }
    }
}