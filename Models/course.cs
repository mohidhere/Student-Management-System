using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace studentmanagement.Models
{
	public class Course
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[Required(ErrorMessage = "Course Name is required")]
		[Display(Name = "Course Name")]
		public string CourseName { get; set; } = string.Empty;
	}
}