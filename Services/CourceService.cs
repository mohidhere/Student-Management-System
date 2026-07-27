using Microsoft.Extensions.Options;
using MongoDB.Driver;
using studentmanagement.Configurations;
using studentmanagement.Models;

namespace studentmanagement.Services
{
	public class CourseService
	{
		private readonly IMongoCollection<Course> _courseCollection;

		public CourseService(IOptions<MongoDbSettings> settings)
		{
			var client = new MongoClient(settings.Value.ConnectionString);

			var database = client.GetDatabase(settings.Value.DatabaseName);

			_courseCollection = database.GetCollection<Course>(
				settings.Value.CourseCollectionName);
		}

		public async Task<List<Course>> GetAllAsync()
		{
			return await _courseCollection.Find(_ => true).ToListAsync();
		}

		public async Task CreateAsync(Course course)
		{
			await _courseCollection.InsertOneAsync(course);
		}

		public async Task<Course?> GetByIdAsync(string id)
		{
			return await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task UpdateAsync(Course course)
		{
			await _courseCollection.ReplaceOneAsync(x => x.Id == course.Id, course);
		}

		public async Task DeleteAsync(string id)
		{
			await _courseCollection.DeleteOneAsync(x => x.Id == id);
		}
	}
}