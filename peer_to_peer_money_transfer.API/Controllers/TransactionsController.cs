using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using peer_to_peer_money_transfer.DAL.Dtos.Responses;
using peer_to_peer_money_transfer.DAL.Entities;

namespace peer_to_peer_money_transfer.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [EnableCors("AllowOrigin")]
    [Route("CashMingle/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsServices _transactionsServices;
        public TransactionsController(ITransactionsServices transactionsServices)
        {
            _transactionsServices = transactionsServices;
        }

        [HttpPut("transfer-money")]
        public async Task<ActionResult<Response>> TransferAsync(TransferRequest transfer)
        {
            var model = await _transactionsServices.TransferMoneyAsync(transfer);
            if (model.Success == false)
            {
                return BadRequest(new { error = model.Data });
            }
            return Ok(new { result = model.Data });
        }

        [HttpGet("get-transaction-details")]
        public async Task<ActionResult<TransactionHistory>> TransactionDetailsAsync(long transactionId)
        {
            var model = await _transactionsServices.GetTransactionDetailsAsync(transactionId);
            return Ok(model);
        }

        [HttpGet("get-transaction-histories")]
        public async Task<ActionResult<IEnumerable<TransactionHistory>>> UserTransactionHistoryAsync(string accountNumber)
        {
            var model = await _transactionsServices.GetTransactionHistoriesAsync(accountNumber);
            return Ok(model);
        }
    }
}
