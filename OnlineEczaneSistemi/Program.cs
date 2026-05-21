using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik doğrulama
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";
		options.LogoutPath = "/Account/Logout";
		options.AccessDeniedPath = "/Account/AccessDenied";
	});

builder.Services.AddAuthorization(); // ← Build'den ÖNCE olmalı
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed: Admin kullanıcısı oluştur
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.EnsureDeleted();
	db.Database.EnsureCreated();

	if (!db.Users.Any(u => u.Role == "Admin"))
	{
		var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<OnlineEczaneSistemi.Models.User>();
		var admin = new OnlineEczaneSistemi.Models.User
		{
			FullName = "Admin",
			Email = "admin@eczane.com",
			Role = "Admin",
			IsActive = true
		};
		admin.Password = hasher.HashPassword(admin, "Admin123!");
		db.Users.Add(admin);
		db.SaveChanges();
	}
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Prometheus: HTTP metrics (UseRouting'den sonra, UseAuthentication'dan önce)
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

// Prometheus metrics endpoint: /metrics
app.MapMetrics();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();