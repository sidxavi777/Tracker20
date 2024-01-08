using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Models;

namespace tracker
{
	public class EmailSender:IEmailSender
	{
		private readonly IServiceProvider _serviceProvider;
		public EmailSender(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var _smtp = _serviceProvider.GetRequiredService<SMTP>();

			_smtp.To = email;
			_smtp.Body = htmlMessage;
			_smtp.Subject = subject;

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
		}
	}
}
