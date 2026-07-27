using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace studentmanagement.Models
{
	public class Student
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonElement("StudentCode")]
		[ValidateNever]
		public string StudentCode { get; set; } = string.Empty;

		// ==========================
		// Personal Information
		// ==========================

		[Required(ErrorMessage = "Full Name is required.")]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters are allowed.")]
		[BsonElement("Name")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Father Name is required.")]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters are allowed.")]
		[BsonElement("FatherName")]
		public string FatherName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		[BsonElement("Email")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Phone Number is required.")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be 11 digits.")]
		[BsonElement("PhoneNumber")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Gender is required.")]
		[BsonElement("Gender")]
		public string Gender { get; set; } = string.Empty;

		[Required(ErrorMessage = "Date of Birth is required.")]
		[DataType(DataType.Date)]
		[BsonElement("DateOfBirth")]
		public DateTime DateOfBirth { get; set; }

		[BsonElement("Age")]
		public int Age { get; set; }

		// ==========================
		// Academic Information
		// ==========================

		[Required(ErrorMessage = "Course is required.")]
		[BsonElement("Course")]
		public string Course { get; set; } = string.Empty;


		[BsonElement("Batch")]
		public string? Batch { get; set; }


		[BsonElement("Section")]
		public string? Section { get; set; }

		// Batch & Section baad me add karenge

		// ==========================
		// Address Information
		// ==========================

		[Required(ErrorMessage = "Address is required.")]
		[StringLength(200)]
		[BsonElement("Address")]
		public string Address { get; set; } = string.Empty;

		[Required(ErrorMessage = "City is required.")]
		[StringLength(50)]
		[BsonElement("City")]
		public string City { get; set; } = string.Empty;

		// ==========================
		// Profile
		// ==========================

		[BsonElement("ProfileImage")]
		public string? ProfileImage { get; set; }

		[BsonElement("PublicToken")]
		public string PublicToken { get; set; } = string.Empty;

		[BsonElement("RegistrationDate")]
		public DateTime RegistrationDate { get; set; } = DateTime.Now;

		// ==========================
		// Authentication
		// ==========================


		[BsonElement("Role")]
		public string Role { get; set; } = "Student";

		[BsonElement("IsActive")]
		public bool IsActive { get; set; } = false;

		[BsonElement("HasAccount")]
		public bool HasAccount { get; set; } = false;

		[BsonElement("Password")]
		public string Password { get; set; } = string.Empty;

		// ==========================
		// Not Stored In MongoDB
		// ==========================

		[BsonIgnore]
		[NotMapped]
		public bool OtpVerified { get; set; }

		[BsonIgnore]
		[NotMapped]
		public IFormFile? Photo { get; set; }
	}
}