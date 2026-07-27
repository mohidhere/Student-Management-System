using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace studentmanagement.Models
{
    public class Batch
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Batch Name is required.")]
        [StringLength(50)]
        [BsonElement("BatchName")]
        public string BatchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shift is required.")]
        [BsonElement("Shift")]
        public string Shift { get; set; } = string.Empty;

        [BsonElement("Status")]
        public bool Status { get; set; } = true;
    }
}
