using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Entities;

namespace peer_to_peer_money_transfer.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, User")]
    [EnableCors("AllowOrigin")]
    [Route("CashMingle/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _admin;

        public AdminController(IAdminServices admin)
        {
            _admin = admin;
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _admin.GetAll());
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-all-customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            return Ok(await _admin.GetAllCustomers());
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("get-all-customer-by-user-id")]
        public async Task<IActionResult> GetAllCustomersByUserId(string userId)
        {
            var user = await _admin.GetCustomerByUserId(userId);
            if (user == null) return NotFound(new { error = "user does not exists" });
            return Ok(user);

        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-all-customers-by-category")]
        public async Task<IActionResult> GetAllCustomersByCategory(string category)
        {
            return Ok(await _admin.GetAllCustomersByCategory(category));
        }

        [AllowAnonymous]
        [HttpGet("get-customer-by-email-or-userName")]
        public async Task<IActionResult> GetCustomerByEmailAddressOrUserName(string EmailAddressOrUserName)
        {
            var user = await _admin.GetCustomerByEmailAddressOrUserName(EmailAddressOrUserName);
            if (user == null) return NotFound(new { error = "user does not exists" });
            return Ok(user);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("get-customer-by-email-or-userName-all")]
        public async Task<IActionResult> GetCustomerByEmailAddressOrUserNameAll(string EmailAddressOrUserName)
        {
            var user = await _admin.GetCustomerByEmailAddressOrUserNameAll(EmailAddressOrUserName);
            if (user == null) return NotFound(new { error = "user does not exists" });
            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-customer-by-accountNumber")]
        public async Task<IActionResult> GetCustomerByAccountNumber(string accountNumber)
        {
            var user = await _admin.GetCustomerByAccountNumber(accountNumber);
            if (user == null) return NotFound(new { error = "user does not exists" });
            return Ok(user);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("get-all-transactions")]
        public async Task<IActionResult> GetAllTransaction()
        {
            return Ok(await _admin.GetAllTransactions());
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-transaction-by-id")]
        public async Task<IActionResult> GetTransactionById(long id)
        {
            var transaction = await _admin.GetTransactionById(id);
            if (transaction == null) return NotFound(new { error = "transaction does not exists" });
            return Ok(transaction);
        }

        [HttpPatch("edit-customer-details")]
        public async Task<IActionResult> EditCustomerDetails(string userName, [FromBody] JsonPatchDocument<ApplicationUser> user)
        {
            var edited = await _admin.EditCustomerDetails(userName, user);
            if (edited == null) return NotFound(new { error = "user does not exists" });
            return Ok(new { result = "Edited Successfully" });
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost("deactivate-customer")]
        public async Task<IActionResult> DeactivateCustomer(string userName)
        {
            var deactivated = await _admin.DeactivateCustomer(userName);
            if (deactivated == null) return NotFound(new { error = "user does not exists" });
            return Ok(new { result = "User Deactivated Successfully" });
        }

        [HttpPost("delete-customer")]
        public async Task<IActionResult> SoftDelete(string userName)
        {
            var deleted = await _admin.SoftDelete(userName);
            if (deleted == null) return NotFound(new { error = "user does not exists" });
            return Ok(new { result = "User Deleted Successfully" });
        }


        [Authorize(Policy = "SuperAdmin")]
        [HttpDelete("delete/delete-customer")]
        public async Task<IActionResult> DeleteCustomer(string userName)
        {
            var deleted = await _admin.DeleteCustomer(userName);
            if (deleted == null) return NotFound(new { error = "user does not exists" });
            return Ok(new { result = "User Deleted Successfully"});
        }
    }
}
