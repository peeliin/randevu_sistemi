using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Session (Admin login için)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AdminAuthService>();

var app = builder.Build();

// Development'ta detaylı hata sayfası (debug için)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Eğer Identity / auth kullanmıyorsan bile sıralama doğru kalsın
app.UseAuthentication(); // (Şimdilik zarar vermez; ileride admin auth ekleyeceksen şart)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
