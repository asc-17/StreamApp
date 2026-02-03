var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<StreamApp.Data.AppDbContext>();
builder.Services.AddSingleton<StreamApp.Services.MediaScannerService>();
builder.Services.AddSingleton<StreamApp.Services.SubtitleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// Run initial scan
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StreamApp.Data.AppDbContext>();
    db.Database.EnsureCreated();
}

var scanner = app.Services.GetRequiredService<StreamApp.Services.MediaScannerService>();
scanner.ScanMedia();

app.Run();
