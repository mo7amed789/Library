using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple_LBApi.Domain.Enities;
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
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> Borrow(BorrowDto dto)
        {
            await _service.BorrowAsync(GetUserId(), dto);
            return Ok();
        }

        [HttpPost("{loanId}/return")]
        public async Task<IActionResult> Return(int loanId)
        {
            await _service.ReturnAsync(loanId);
            return Ok();
        }

        [HttpPost("{loanId}/renew")]
        public async Task<IActionResult> Renew(int loanId)
        {
            await _service.RenewAsync(loanId);
            return Ok();
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyLoans()
        {
            return Ok(await _service.GetUserLoans(GetUserId()));
        }
    }
}
