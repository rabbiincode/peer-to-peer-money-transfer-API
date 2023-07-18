using Microsoft.AspNetCore.Mvc;
using peer_to_peer_money_transfer.BLL.Models;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.Shared.EmailConfiguration;
using peer_to_peer_money_transfer.Shared.SmsConfiguration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace peer_to_peer_money_transfer.API.Controllers
{
    public partial class AccountController : ControllerBase
    {
        private static string otpToken;

        [HttpGet("get-verify-phone-no-token")]
        public async Task<IActionResult> GetVerifyPhoneNumberTokenAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest(new { error = "User does not exist" });
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            if (token == null)
            {
                return BadRequest(new { error = "Could not send verification token...please try again" });
            }
            var model = new SmsModel() { ReceiversPhoneNumber = user.PhoneNumber, MessageBody = token };

            await _sendSms.SendSmsAsync(model);
            return Ok(new { result = "Please check your phone for your verification code" });
        }

        [HttpPost("verify-phone-no")]
        public async Task<IActionResult> VerifyPhoneNumberAsync(string userName, string token)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(new { error = "User does not exist" });
            }

            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);
            if (result == null)
            {
                return BadRequest(new { error = "Verification failed...please try again" });
            }
            return Ok(new { result = "Phone number verification successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            string pattern = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";
            Regex regex = new(pattern);

            _logger.LogInformation($"Login Attempt for {login.EmailAddressOrUserName}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isEmail = regex.IsMatch(login.EmailAddressOrUserName);

            var existingUser = isEmail == true ? await _userManager.FindByEmailAsync(login.EmailAddressOrUserName)
                               : await _userManager.FindByNameAsync(login.EmailAddressOrUserName);

            if (existingUser == null)
            {
                return BadRequest(new { error = "User does not exist...register to continue" });
            }

            if (existingUser.EmailConfirmed)
            {
                existingUser.Activated = true;
                await _admin.ResetCount(existingUser.UserName);
            }

            /*if (!existingUser.Activated)
            {
                await VerifyMailAsync(existingUser);
                return BadRequest(new
                {
                    error = "Email Address not verified, please verify your Email Address to proceed",
                    Message = "A confirmation link has been sent to you. Please verify your Email Address to proceed"
                });
            }*/

            if (existingUser.TwoFactorEnabled)
            {
                var lockAccount = await LockAccount(existingUser);

                if (lockAccount)
                {
                    return BadRequest(new
                    {
                        error = "Password trial limit exceeded, please check the link sent to your Mail to verify your identity",
                        Message = "A confirmation link has been sent to your mail. Please verify to proceed"
                    });
                }

                var signIn = await _userManager.CheckPasswordAsync(existingUser, login.Password);

                if (!signIn)
                {
                    await _admin.AccessFailedCount(existingUser.UserName);
                    return Unauthorized(new { error = "Incorrect Password, please enter your correct password" });
                }
                return await SendOTP(existingUser.UserName);
            }

            // Deactivate user account if incorrect password is entered more than 3 times
            return await PasswordVerification(existingUser.UserName, login.Password);
        }

        [HttpPost("password-verification")]
        public async Task<IActionResult> PasswordVerification(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var lockAccount = await LockAccount(user);

            if (lockAccount)
            {
                return BadRequest(new
                {
                    error = "Password trial limit exceeded, please check the link sent to your Mail to verify your identity",
                    Message = "A confirmation link has been sent to your mail. Please verify to proceed"
                });
            }

            var signIn = await _userManager.CheckPasswordAsync(user, password);

            if (!signIn)
            {
                await _admin.AccessFailedCount(userName);
                return Unauthorized(new { error = "Incorrect Password, please enter your correct password" });
            }

            await _admin.ResetCount(userName);
            return Accepted(new { result = "Login successful", Token = await _jwtConfig.GenerateJwtToken(user) });
        }

        private async Task<bool> LockAccount(ApplicationUser user)
        {
            if (user.AccessFailedCount >= 4)
            {
                await _admin.LockCustomer(user.UserName);
                await VerifyMailAsync(user);

                return true;
            }

            return false;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            otpToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // send token to Email
            await SendMailAsync(user.Email, "CashMingle - OTP", $"OTP - {otpToken}");

            // send token to SMS
            // var model = new SmsModel() { ReceiversPhoneNumber = user.PhoneNumber, MessageBody = otpToken };
            // await _sendSms.SendSmsAsync(model);

            return Ok(new { Message = "An OTP has been sent to your email, input the OTP to continue" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP(VerifyOtpModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            /*var result = await _signInManager.TwoFactorSignInAsync("Email", model.Token, false, false);

            if (result == null)
            {
                return BadRequest(new { error = "OTP verification failed" });
            }*/
            if (model.Token != otpToken)
            {
                return BadRequest(new { error = "Incorrect OTP" });
            }

            otpToken = "";
            return Accepted(new { result = "OTP verification successful", Token = await _jwtConfig.GenerateJwtToken(user) });
        }

        [HttpPost("get-forgotten-password-reset-token")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest(new { error = "Email does not exist...register to continue" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (token == null)
            {
                return BadRequest(new { error = "Password reset link failed...please try again" });
            }

            var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Account", new { token }, Request.Scheme);
            await SendMailAsync(user.Email, "CashMingle- ResetPassword", $"Click the link to get the token for your password reset - {forgotPasswordLink}");

            return Ok(new { result = "Password reset link sent...please check your mail" });
        }

        [HttpPost("reset-forgotten-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            var reset = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

            if (!reset.Succeeded)
            {
                return BadRequest(new { error = "Password reset failed" });
            }

            return Ok(new { result = "Password reset successfull" });
        }

        [HttpPost("send-text")]
        public async Task<IActionResult> SendText(SmsModel model)
        {
            await _sendSms.SendSmsAsync(model);

            return Ok(new { result = "sms sent successfully" });

        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendMailAsync(string recipientEmailAddress, string subject, string message)
        {
            var mail = new Message(new string[] { $"{recipientEmailAddress}" }, $"{subject}", $"{message}");
            await _emailSender.SendEmailAsync(mail);
            return Ok(new { result = "Email sent Successfully" });
        }
    }
}
