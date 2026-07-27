using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using studentmanagement.Configurations;
using studentmanagement.Models;

namespace studentmanagement.Services
{
	public class SectionService
	{
		private readonly IMongoCollection<Section> _sectionCollection;

		public SectionService(IOptions<MongoDbSettings> settings)
		{
			var mongoClient = new MongoClient(settings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

			_sectionCollection = mongoDatabase.GetCollection<Section>(
				settings.Value.SectionCollectionName);
		}

		// ==========================
		// Get All Sections
		// ==========================
		public async Task<List<Section>> GetAllAsync()
		{
			return await _sectionCollection.Find(_ => true).ToListAsync();
		}

		// ==========================
		// Create Section
		// ==========================
		public async Task CreateAsync(Section section)
		{
			await _sectionCollection.InsertOneAsync(section);
		}

		// ==========================
		// Get Section By Id
		// ==========================
		public async Task<Section?> GetByIdAsync(string id)
		{
			return await _sectionCollection
				.Find(x => x.Id == id)
				.FirstOrDefaultAsync();
		}

		// ==========================
		// Update Section
		// ==========================
		public async Task UpdateAsync(Section section)
		{
			await _sectionCollection.ReplaceOneAsync(
				x => x.Id == section.Id,
				section);
		}

		// ==========================
		// Delete Section
		// ==========================
		public async Task DeleteAsync(string id)
		{
			await _sectionCollection.DeleteOneAsync(x => x.Id == id);
		}

		// ==========================
		// Search Section
		// ==========================
		public async Task<List<Section>> SearchAsync(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return await GetAllAsync();
			}

			var filter = Builders<Section>.Filter.Regex(
				x => x.SectionName,
				new BsonRegularExpression(keyword, "i"));

			return await _sectionCollection
				.Find(filter)
				.ToListAsync();
		}
	}
}