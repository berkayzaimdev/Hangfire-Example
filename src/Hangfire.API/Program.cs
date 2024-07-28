using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseDefaultTypeSerializer()
          .UseSqlServerStorage("Server=(localdb)\\MSSQLLocalDb;Database=HangfireDb;Trusted_Connection=True;", new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),  // Komutlarýn maksimum süre aþýmý
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),  // Ýþlerin görünmezlik süresi
              QueuePollInterval = TimeSpan.FromSeconds(10),  // Kuyruðun kontrol edilme sýklýðý
              UseRecommendedIsolationLevel = true,  // Önerilen izolasyon seviyesini kullan
              DisableGlobalLocks = true  // Global kilitleri devre dýþý býrak
          });
}); // hangfire'ý konfigüre ediyoruz
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHangfireDashboard();

app.Run();
