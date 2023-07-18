using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using peer_to_peer_money_transfer.BLL.Infrastructure;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.DAL.Enums;
using peer_to_peer_money_transfer.Shared.DataTransferObject;
using peer_to_peer_money_transfer.Shared.Interfaces;

namespace peer_to_peer_money_transfer.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [EnableCors("AllowOrigin")]
    [Route("CashMingle/[controller]")]
    public partial class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GenerateAccountNumber _accountNumber;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IJwtConfig _jwtConfig;
        private readonly IEmailSender _emailSender;
        private readonly ISendSms _sendSms;
        private readonly IAdminServices _admin;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, GenerateAccountNumber accountNumber,
                                 IMapper mapper, ILogger<AccountController> logger, IJwtConfig jwtConfig,  IEmailSender emailSender, ISendSms sendSms, IAdminServices admin)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountNumber = accountNumber;
            _mapper = mapper;
            _logger = logger;
            _jwtConfig = jwtConfig;
            _emailSender = emailSender;
            _sendSms = sendSms;
            _admin = admin;
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDTO admin)
        {
            _logger.LogInformation($"Registration Attempt for {admin.Email}");

            var userExists = await _userManager.FindByEmailAsync(admin.Email);
            var userNameExists = await _userManager.FindByNameAsync(admin.UserName);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userExists != null)
            {
                return BadRequest(new { error = "Email Already Exist" });
            }

            if (userNameExists != null)
            {
                return BadRequest(new { error = "Username Already Exist" });
            }

            var user = _mapper.Map<ApplicationUser>(admin);
            user.UserType = UserTypeExtension.GetStringValue(UserType.Admin);

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = "Admin Registration Failed" });
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            await VerifyMailAsync(user);

            return Accepted(new { result = "Registration Successful",
                                  Message = "A confirmation link has been sent to you. Please verify your Email Address to proceed",
                                  Token = await _jwtConfig.GenerateJwtToken(user) } );
        }

        [HttpPost("register/individual")]
        public async Task<IActionResult> RegisterIndividual([FromBody] RegisterIndividualDTO register)
        {
            _logger.LogInformation($"Registration Attempt for {register.Email}");

            var userExists = await _userManager.FindByEmailAsync(register.Email);
            var userNameExists = await _userManager.FindByNameAsync(register.UserName);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userExists != null)
            {
                return BadRequest(new { error = "Email Already Exist" });
            }

            if (userNameExists != null)
            {
                return BadRequest(new { error = "Username Already Exist" });
            }

            var user = _mapper.Map<ApplicationUser>(register);
            user.UserType = UserTypeExtension.GetStringValue(UserType.Individual);
            user.AccountNumber = await _accountNumber.GenerateAccount();

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = "User Registration Failed" });
            }

            await _userManager.AddToRoleAsync(user, "User");
            await VerifyMailAsync(user);

            return Accepted(new { result = "Registration Successful",
                                  Message = "A confirmation link has been sent to you. Please verify your Email Address to proceed",
                                  Token = await _jwtConfig.GenerateJwtToken(user) });
        }

        [HttpPost("register/business")]
        public async Task<IActionResult> RegisterBusiness([FromBody] RegisterBusinessDTO business)
        {
            _logger.LogInformation($"Registration Attempt for {business.Email}");

            var userExists = await _userManager.FindByEmailAsync(business.Email);
            var userNameExists = await _userManager.FindByNameAsync(business.UserName);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userExists != null)
            {
                return BadRequest(new { error = "Email Already Exist" });
            }

            if (userNameExists != null)
            {
                return BadRequest(new { error = "Username Already Exist" });
            }

            var user = _mapper.Map<ApplicationUser>(business);
            user.UserType = UserTypeExtension.GetStringValue(UserType.Corporate);
            user.AccountNumber = await _accountNumber.GenerateAccount();

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = "User Registration Failed" });
            }

            await _userManager.AddToRoleAsync(user, "User");
            await VerifyMailAsync(user);

            return Accepted(new { result = "Registration Successful",
                                  Message = "A confirmation link has been sent to you. Please verify your Email Address to proceed",
                                  Token = await _jwtConfig.GenerateJwtToken(user) });
        }

        [HttpGet("get-confirm-email-token")]
        private async Task VerifyMailAsync(ApplicationUser user)
        {
            //Add Token to verify email
            var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            await SendMailAsync(user.Email, "CashMingle - Verify Email Address", $"Click the link to verify your email {confirmationLink}");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest(new { error = "User does not exist...register to continue" });
            }

            var confirmEmail = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirmEmail.Succeeded)
            {
                return BadRequest(new { error = "Email verification failed" });
            }

            return Ok(new { result = "Email verification successfull" });
        }
    }
}
