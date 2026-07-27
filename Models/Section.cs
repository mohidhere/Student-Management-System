using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace studentmanagement.Models
{
	public class Section
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[Required(ErrorMessage = "Section Name is required.")]
		[StringLength(20)]
		[BsonElement("SectionName")]
		public string SectionName { get; set; } = string.Empty;

		[BsonElement("Status")]
		public bool Status { get; set; } = true;
	}
}