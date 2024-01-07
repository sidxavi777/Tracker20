// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Models;
//using Database.Migrations;

namespace tracker.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IServiceProvider _serviceProvider;
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _emailSender = emailSender; 
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by.");
                var _smtp = _serviceProvider.GetRequiredService<SMTP>();
                _smtp.To = Input.Email;
                _smtp.Body = $"Please click the link to reset the password of your account: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>";

                _smtp.Subject = "Tracker Account confirmation";

                MailMessage mail = new MailMessage
                {
                    Subject = _smtp.Subject,
                    Body = _smtp.Body,
                    From = new MailAddress(_smtp.SenderAddress, _smtp.SenderDisplayName),
                    IsBodyHtml = _smtp.IsBodyHTML

                };
                mail.To.Add(_smtp.To);

                NetworkCredential credential = new NetworkCredential(_smtp.UserName, _smtp.Password);

                SmtpClient client = new SmtpClient
                {
                    Host = _smtp.Host,
                    Port = _smtp.Port,
                    EnableSsl = _smtp.EnableSSL,
                    UseDefaultCredentials = _smtp.UseDefaultCredentials,
                    Credentials = credential
                };
                mail.BodyEncoding = Encoding.Default;
                await client.SendMailAsync(mail);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
