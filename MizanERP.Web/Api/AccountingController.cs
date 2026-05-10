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
    public class AccountingController : ControllerBase
    {
        private readonly IAccountingService _accountingService;
        
        public AccountingController(IAccountingService accountingService)
        {
            _accountingService = accountingService ?? throw new ArgumentNullException(nameof(accountingService));
        }

        /// <summary>
        /// Generate journal entries for a transaction
        /// </summary>
        [HttpPost("journal-entries")]
        [Authorize(Roles = "Admin,Accountant,Manager")]
        public async Task<IActionResult> GenerateJournalEntries([FromBody] AccountingTransactionDto dto)
        {
            try
            {
                await _accountingService.GenerateJournalEntriesAsync(dto);
                return Ok(new { message = "Journal entries generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Post accounting entries to ledger
        /// </summary>
        [HttpPost("post")]
        [Authorize(Roles = "Admin,Accountant,Manager")]
        public async Task<IActionResult> PostEntries()
        {
            try
            {
                await _accountingService.PostEntriesToLedgerAsync();
                return Ok(new { message = "Entries posted to ledger successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Generate trial balance report
        /// </summary>
        [HttpGet("trial-balance")]
        [Authorize(Roles = "Admin,Accountant,Manager")]
        public async Task<IActionResult> GetTrialBalance()
        {
            try
            {
                var report = await _accountingService.GenerateTrialBalanceAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}