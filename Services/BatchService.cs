using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using studentmanagement.Configurations;
using studentmanagement.Models;

namespace studentmanagement.Services
{
	public class BatchService
	{
		private readonly IMongoCollection<Batch> _batchCollection;

		public BatchService(IOptions<MongoDbSettings> settings)
		{
			var mongoClient = new MongoClient(settings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

			_batchCollection = mongoDatabase.GetCollection<Batch>(
				settings.Value.BatchCollectionName);
		}

		// ==========================
		// Get All Batches
		// ==========================
		public async Task<List<Batch>> GetAllAsync()
		{
			return await _batchCollection.Find(_ => true).ToListAsync();
		}

		// ==========================
		// Create Batch
		// ==========================
		public async Task CreateAsync(Batch batch)
		{
			await _batchCollection.InsertOneAsync(batch);
		}

		// ==========================
		// Get Batch By Id
		// ==========================
		public async Task<Batch?> GetByIdAsync(string id)
		{
			return await _batchCollection
				.Find(x => x.Id == id)
				.FirstOrDefaultAsync();
		}

		// ==========================
		// Update Batch
		// ==========================
		public async Task UpdateAsync(Batch batch)
		{
			await _batchCollection.ReplaceOneAsync(
				x => x.Id == batch.Id,
				batch);
		}

		// ==========================
		// Delete Batch
		// ==========================
		public async Task DeleteAsync(string id)
		{
			await _batchCollection.DeleteOneAsync(x => x.Id == id);
		}

		// ==========================
		// Search Batch
		// ==========================
		public async Task<List<Batch>> SearchAsync(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return await GetAllAsync();
			}

			var filter = Builders<Batch>.Filter.Or(

				Builders<Batch>.Filter.Regex(
					x => x.BatchName,
					new BsonRegularExpression(keyword, "i")),

				Builders<Batch>.Filter.Regex(
					x => x.Shift,
					new BsonRegularExpression(keyword, "i"))

			);

			return await _batchCollection
				.Find(filter)
				.ToListAsync();
		}

		// ==========================
		// Check Batch Exists
		// ==========================
		public async Task<bool> ExistsAsync(string batchName)
		{
			return await _batchCollection
				.Find(x => x.BatchName == batchName)
				.AnyAsync();
		}
	}
}