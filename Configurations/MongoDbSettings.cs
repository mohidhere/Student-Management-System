namespace studentmanagement.Configurations
{
	public class MongoDbSettings
	{
		public string ConnectionString { get; set; } = null!;
		public string DatabaseName { get; set; } = null!;
		public string StudentCollectionName { get; set; } = null!;

		public string CourseCollectionName { get; set; } = null!;

		public string BatchCollectionName { get; set; } = null!;

		public string SectionCollectionName { get; set; } = null!;
	}
}