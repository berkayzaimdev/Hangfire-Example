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
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),  // Komutlar�n maksimum s�re a��m�
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),  // ��lerin g�r�nmezlik s�resi
              QueuePollInterval = TimeSpan.FromSeconds(10),  // Kuyru�un kontrol edilme s�kl���
              UseRecommendedIsolationLevel = true,  // �nerilen izolasyon seviyesini kullan
              DisableGlobalLocks = true  // Global kilitleri devre d��� b�rak
          });
}); // hangfire'� konfig�re ediyoruz
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
