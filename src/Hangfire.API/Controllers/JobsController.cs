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
               * Hemen s�raya al�n�p, i�lenecek task'ler i�in
               * H�zl� ve k�sa i�lemler i�in kullan�labilir
            */

            BackgroundJob.Enqueue(() => WriteDebugMessage("Fire-and-forget job �al��t�"));
            return Ok("Fire-and-forget job ayarland�");
        }

        [HttpGet("delayed")]
        public IActionResult Delayed()
        {
            /*
               * Belirli bir s�reden sonra i�lenecek task'ler i�in
               * Zamanlanm�� i�lemler (�rne�in, belirli bir s�re sonra bir e-posta g�nderme)
            */

            BackgroundJob.Schedule(() => WriteDebugMessage("Delayed job �al��t�"), TimeSpan.FromSeconds(10));
            return Ok("Delayed job, 10 saniye sonra �al��mak �zere ayarland�.");
        }

        [HttpGet("recurring")]
        public IActionResult Recurring()
        {
            /*
               * Periyodik olarak �al��mas� gereken task'ler
               * �rn: her g�n ba��nda kullan�c�n�n do�um g�n� m�, de�il mi kontrol et
            */

            RecurringJob.AddOrUpdate("recurring-job", () => WriteDebugMessage("Recurring job �al��t�"), Cron.Minutely);
            return Ok("Recurring job, her dakika ba��nda �al��mak �zere ayarland�.");
        }

        [HttpGet("continuations")]
        public IActionResult Continuations()
        {
            /*
               * Belirli bir task �al��t�ktan sonra �al��mas� gereken task'ler i�in
               * Ard���k taskler i�in kullan��l�d�r
            */

            var parentJobId = BackgroundJob.Enqueue(() => WriteDebugMessage("Parent job �al��t�"));
            BackgroundJob.ContinueJobWith(parentJobId, () => WriteDebugMessage("Continuation job, parent job'dan sonra �al��t�"));
            return Ok("Continuation job ayarland�");
        }

        public void WriteDebugMessage(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
