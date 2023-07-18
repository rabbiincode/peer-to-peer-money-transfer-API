using Microsoft.AspNetCore.Authorization;
using Response = peer_to_peer_money_transfer.DAL.Dtos.Responses.Response;
using Microsoft.AspNetCore.Mvc;
using PayStack.Net;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace peer_to_peer_money_transfer.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [EnableCors("AllowOrigin")]
    [Route("CashMingle/[controller]")]

    public class FundController : ControllerBase
    {
        IFundingService _fundingService;
        IHttpContextAccessor _contextAccessor;

        public FundController(IFundingService fundingService, IHttpContextAccessor contextAccessor)
        {
            _fundingService = fundingService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("make-deposit")]
        [SwaggerOperation(Summary = "Funds account using paystack")]
        public ActionResult<TransactionInitializeResponse> Deposit(DepositRequest depositRequest)
        {
            var response = _fundingService.MakePayment(depositRequest);
            return Ok(response);
        }

        [HttpPost("verify-payment")]
        [SwaggerOperation(Summary = "Verifying deposits using paystack")]
        public async Task<ActionResult<Response>> Verify(string reference)
        {
            string? userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return new Response { Success = false, Data = "User not found" };
            }
            var response = await _fundingService.FundAccount(userId,reference);
            return Ok(response);
        }

        [HttpPost("card-deposit")]
        public async Task<ActionResult<Response>> CardDepositAsync(CardDepositRequest depositRequest)
        {
            var response = await _fundingService.CardDepositAsync(depositRequest);
            if (response.Success == false) return BadRequest(new { error = response.Data });
            return Ok(new { result = response.Data });
         }

        [HttpPost("withdrawal")]
        public async Task<ActionResult<Response>> WithdrawalAsync(WithdrawalRequest withdrawal)
        {
            var response = await _fundingService.WithdrawAsync(withdrawal);
            if (response.Success == false) return BadRequest(new { error = response.Data });
            return Ok(new { result = response.Data });
        }

        [HttpPost("purchase-airtime")]
        public async Task<ActionResult<Response>> AirtimeAsync(PurchaseAirtime airtime)
        {
            var response = await _fundingService.PurchaseAirtimeAsync(airtime);
            if (response.Success == false) return BadRequest(new { error = response.Data });
            return Ok(new { result = response.Data });
        }
    }
}
