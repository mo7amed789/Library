using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple_LBApi.Common;
using Simple_LBApi.DTOs;
using Simple_LBApi.Services.Interfaces;
using System.Security.Claims;

namespace Simple_LBApi.Controllers
{
    [ApiController]
    [Route("api/loans")]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _service;

        public LoansController(ILoanService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(id, out var userId))
            {
                throw new ApiException("Invalid user context", StatusCodes.Status401Unauthorized);
            }

            return userId;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> Borrow(BorrowDto dto)
        {
            await _service.BorrowAsync(GetUserId(), dto);
            return NoContent();
        }

        [HttpPost("{loanId:int}/return")]
        public async Task<IActionResult> Return(int loanId)
        {
            await _service.ReturnAsync(loanId);
            return NoContent();
        }

        [HttpPost("{loanId:int}/renew")]
        public async Task<IActionResult> Renew(int loanId)
        {
            await _service.RenewAsync(loanId);
            return NoContent();
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyLoans()
        {
            return Ok(await _service.GetUserLoans(GetUserId()));
        }
    }
}
