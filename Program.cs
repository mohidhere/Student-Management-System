using studentmanagement.Configurations;
using studentmanagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
	builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<StudentService>();
builder.Services.AddSingleton<CourseService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<OtpService>();
builder.Services.AddSingleton<studentmanagement.Services.BatchService>();
builder.Services.AddSingleton<studentmanagement.Services.SectionService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(5);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.Configure<AdminLoginSettings>(
builder.Configuration.GetSection("AdminLogin"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();