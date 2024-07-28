using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hangfire.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        [HttpGet("fire-and-forget")]
        public IActionResult FireAndForget()
        {
            /*
               * Hemen sýraya alýnýp, iþlenecek task'ler için
               * Hýzlý ve kýsa iþlemler için kullanýlabilir
            */

            BackgroundJob.Enqueue(() => WriteDebugMessage("Fire-and-forget job çalýþtý"));
            return Ok("Fire-and-forget job ayarlandý");
        }

        [HttpGet("delayed")]
        public IActionResult Delayed()
        {
            /*
               * Belirli bir süreden sonra iþlenecek task'ler için
               * Zamanlanmýþ iþlemler (örneðin, belirli bir süre sonra bir e-posta gönderme)
            */

            BackgroundJob.Schedule(() => WriteDebugMessage("Delayed job çalýþtý"), TimeSpan.FromSeconds(10));
            return Ok("Delayed job, 10 saniye sonra çalýþmak üzere ayarlandý.");
        }

        [HttpGet("recurring")]
        public IActionResult Recurring()
        {
            /*
               * Periyodik olarak çalýþmasý gereken task'ler
               * Örn: her gün baþýnda kullanýcýnýn doðum günü mü, deðil mi kontrol et
            */

            RecurringJob.AddOrUpdate("recurring-job", () => WriteDebugMessage("Recurring job çalýþtý"), Cron.Minutely);
            return Ok("Recurring job, her dakika baþýnda çalýþmak üzere ayarlandý.");
        }

        [HttpGet("continuations")]
        public IActionResult Continuations()
        {
            /*
               * Belirli bir task çalýþtýktan sonra çalýþmasý gereken task'ler için
               * Ardýþýk taskler için kullanýþlýdýr
            */

            var parentJobId = BackgroundJob.Enqueue(() => WriteDebugMessage("Parent job çalýþtý"));
            BackgroundJob.ContinueJobWith(parentJobId, () => WriteDebugMessage("Continuation job, parent job'dan sonra çalýþtý"));
            return Ok("Continuation job ayarlandý");
        }

        public void WriteDebugMessage(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
