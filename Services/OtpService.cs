using Microsoft.AspNetCore.Http;

namespace studentmanagement.Services
{
	public class OtpService
	{
		private readonly EmailService _emailService;

		public OtpService(EmailService emailService)
		{
			_emailService = emailService;
		}

		private string GenerateOtp()
		{
			Random random = new Random();
			return random.Next(100000, 999999).ToString();
		}

		public async Task<bool> SendOtpAsync(HttpContext httpContext, string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			string otp = GenerateOtp();

			httpContext.Session.SetString("OTP", otp);
			httpContext.Session.SetString("Email", email);

			await _emailService.SendEmailAsync(
				email,
				"Student Management Email Verification",
				$@"
                <h2>Email Verification</h2>
                <p>Your verification code is:</p>
                <h1>{otp}</h1>
                <p>This code will expire in 5 minutes.</p>");

			return true;
		}

		public bool VerifyOtp(HttpContext httpContext, string email, string otp)
		{
			var savedOtp = httpContext.Session.GetString("OTP");
			var savedEmail = httpContext.Session.GetString("Email");

			if (savedOtp == otp && savedEmail == email)
			{
				httpContext.Session.Remove("OTP");
				httpContext.Session.Remove("Email");

				return true;
			}

			return false;
		}
	}
}