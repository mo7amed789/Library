using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple_LBApi.Services.Interfaces;
using System.Security.Claims;

namespace Simple_LBApi.Controllers
{
    [ApiController]
    [Route("api/fines")]
    [Authorize]
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FinesController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyUnpaid()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fines = await _fineService.GetMyUnpaidAsync(userId);
            return Ok(fines.Select(f => new
            {
                f.Id,
                f.Amount,
                f.IsPaid,
                f.CreatedAt,
                f.LoanId
            }));
        }

        [HttpPost("{fineId:int}/pay")]
        public async Task<IActionResult> Pay(int fineId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            await _fineService.MarkPaidAsync(fineId, userId, isAdmin);
            return NoContent();
        }
    }
}
