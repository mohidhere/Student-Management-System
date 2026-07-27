using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using studentmanagement.Configurations;
using studentmanagement.Models;

namespace studentmanagement.Services
{
	public class StudentService
	{
		private readonly IMongoCollection<Student> _studentsCollection;

		public StudentService(IOptions<MongoDbSettings> settings)
		{
			var mongoClient = new MongoClient(settings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

			_studentsCollection = mongoDatabase.GetCollection<Student>(
				settings.Value.StudentCollectionName);
		}

		public async Task<List<Student>> GetAllAsync()
		{
			return await _studentsCollection.Find(_ => true).ToListAsync();
		}

		public async Task CreateAsync(Student student)
		{
			// Total students count
			var totalStudents = await _studentsCollection.CountDocumentsAsync(_ => true);

			// Generate Student Code
			student.StudentCode = $"STD-{DateTime.Now.Year}-{(totalStudents + 1):D4}";

			// Generate Public Token
			student.PublicToken = Guid.NewGuid().ToString("N");

			// Registration Date
			student.RegistrationDate = DateTime.Now;

			await _studentsCollection.InsertOneAsync(student);
		}

		public async Task<Student?> GetByIdAsync(string id)
		{
			return await _studentsCollection
				.Find(x => x.Id == id)
				.FirstOrDefaultAsync();
		}

		// ==========================
		// Get Student By Public Token
		// ==========================
		public async Task<Student?> GetByPublicTokenAsync(string token)
		{
			return await _studentsCollection
				.Find(x => x.PublicToken == token)
				.FirstOrDefaultAsync();
		}

		public async Task UpdateAsync(Student student)
		{
			await _studentsCollection.ReplaceOneAsync(
				x => x.Id == student.Id,
				student);
		}

		public async Task DeleteAsync(string id)
		{
			await _studentsCollection.DeleteOneAsync(x => x.Id == id);
		}

		public async Task<bool> IsCourseAssignedAsync(string courseName)
		{
			var count = await _studentsCollection
				.Find(s => s.Course == courseName)
				.CountDocumentsAsync();

			return count > 0;
		}

		// ==========================
		// Search Students
		// ==========================
		public async Task<List<Student>> SearchAsync(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return await GetAllAsync();
			}

			var filter = Builders<Student>.Filter.Or(

				Builders<Student>.Filter.Regex(
					x => x.Name,
					new MongoDB.Bson.BsonRegularExpression(keyword, "i")),

				Builders<Student>.Filter.Regex(
					x => x.Email,
					new MongoDB.Bson.BsonRegularExpression(keyword, "i")),

				Builders<Student>.Filter.Regex(
					x => x.Course,
					new MongoDB.Bson.BsonRegularExpression(keyword, "i"))

			);

			return await _studentsCollection
				.Find(filter)
				.ToListAsync();
		}

		// ==========================
		// Total Male Students
		// ==========================
		public async Task<long> GetMaleCountAsync()
		{
			return await _studentsCollection.CountDocumentsAsync(x => x.Gender == "Male");
		}

		// ==========================
		// Total Female Students
		// ==========================
		public async Task<long> GetFemaleCountAsync()
		{
			return await _studentsCollection.CountDocumentsAsync(x => x.Gender == "Female");
		}

		// ==========================
		// Pending Assignment
		// ==========================
		public async Task<long> GetPendingAssignmentCountAsync()
		{
			return await _studentsCollection.CountDocumentsAsync(x =>
				string.IsNullOrEmpty(x.Batch) ||
				string.IsNullOrEmpty(x.Section));
		}

		// ==========================
		// Recently Registered Students
		// ==========================
		public async Task<List<Student>> GetRecentStudentsAsync()
		{
			return await _studentsCollection
				.Find(_ => true)
				.SortByDescending(x => x.RegistrationDate)
				.Limit(5)
				.ToListAsync();
		}

		// ==========================
		// Students By Batch
		// ==========================
		public async Task<Dictionary<string, int>> GetStudentsByBatchAsync()
		{
			var students = await _studentsCollection.Find(_ => true).ToListAsync();

			return students
				.Where(x => !string.IsNullOrWhiteSpace(x.Batch))
				.GroupBy(x => x.Batch!)
				.ToDictionary(
					g => g.Key,
					g => g.Count());
		}

		// ==========================
		// Get Student By Email
		// ==========================
		public async Task<Student?> GetByEmailAsync(string email)
		{
			return await _studentsCollection
				.Find(x => x.Email == email)
				.FirstOrDefaultAsync();
		}

		// ==========================
		// Student Account Exists
		// ==========================
		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _studentsCollection
				.Find(x => x.Email == email)
				.AnyAsync();
		}

		// ==========================
		// Activate Student Account
		// ==========================
		public async Task ActivateStudentAccountAsync(string email, string hashedPassword)
		{
			var update = Builders<Student>.Update
				.Set(x => x.Password, hashedPassword)
				.Set(x => x.HasAccount, true)
				.Set(x => x.IsActive, true);

			await _studentsCollection.UpdateOneAsync(
				x => x.Email == email,
				update);
		}

		// ==========================
		// Update Student Password
		// ==========================
		public async Task UpdatePasswordAsync(string email, string hashedPassword)
		{
			var update = Builders<Student>.Update
				.Set(x => x.Password, hashedPassword);

			await _studentsCollection.UpdateOneAsync(
				x => x.Email == email,
				update);
		}

		// ==========================
		// Get Active Student
		// ==========================
		public async Task<Student?> GetActiveStudentAsync(string email)
		{
			return await _studentsCollection
				.Find(x => x.Email == email && x.IsActive)
				.FirstOrDefaultAsync();
		}


	}
}