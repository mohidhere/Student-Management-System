using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using studentmanagement.Configurations;

namespace studentmanagement.Services
{
	public class EmailService
	{
		private readonly EmailSettings _settings;

		public EmailService(IOptions<EmailSettings> options)
		{
			_settings = options.Value;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var email = new MimeMessage();

			email.From.Add(MailboxAddress.Parse(_settings.Email));
			email.To.Add(MailboxAddress.Parse(toEmail));

			email.Subject = subject;

			email.Body = new TextPart("html")
			{
				Text = body
			};

			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);

			await smtp.AuthenticateAsync(_settings.Email, _settings.Password);

			await smtp.SendAsync(email);

			await smtp.DisconnectAsync(true);
		}
	}
}