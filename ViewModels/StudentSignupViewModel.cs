using System.ComponentModel.DataAnnotations;

namespace studentmanagement.ViewModels
{
	public class StudentSignupViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		public string Email { get; set; } = string.Empty;

		public string Otp { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}