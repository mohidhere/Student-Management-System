using System.ComponentModel.DataAnnotations;

namespace studentmanagement.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "OTP is required.")]
		public string Otp { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Confirm Password is required.")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}