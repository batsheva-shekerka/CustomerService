using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Service.Services
{
    public class FolderWatcherWorker : BackgroundService
    {
        private readonly string _rootPath = @"C:\Calls";
        private readonly string _inboxPath = @"C:\Calls\Incoming";
        private readonly string _processedPath = @"C:\Calls\Done";
        private readonly IServiceProvider _serviceProvider;

        public FolderWatcherWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Directory.CreateDirectory(_inboxPath);
            Directory.CreateDirectory(_processedPath);
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        // סריקת כל תיקיות המשנה בתוך תיקיית השיחות הראשית
        //        var operatorFolders = Directory.GetDirectories(_rootPath);

        //        foreach (var folderPath in operatorFolders)
        //        {
        //            // שם התיקייה הוא ה-ID של הטלפנית
        //            if (int.TryParse(Path.GetFileName(folderPath), out int operatorId))
        //            {
        //                var wavFiles = Directory.GetFiles(folderPath, "*.wav");
        //                foreach (var file in wavFiles)
        //                {
        //                    using (var scope = _serviceProvider.CreateScope())
        //                    {
        //                        var analysisService = scope.ServiceProvider.GetRequiredService<CallAnalysisService>();
        //                        // שולחים גם את ה-OperatorId לניתוח
        //                        //await analysisService.ProcessFullCallChain(file, operatorId);
        //                    }

        //                    // העברה לתיקיית 'בוצע' בתוך תיקיית הטלפנית
        //                    MoveToProcessed(file, folderPath);
        //                }
        //            }
        //        }
        //        await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        //    }
        //}


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // לוג לבדיקה שהשירות התחיל
            Console.WriteLine("Worker started at: " + DateTime.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var companyService = scope.ServiceProvider.GetRequiredService<CompanyService>();
                    var companies = await companyService.GetAllAsync(); // שליפת החברות מה-DB

                    foreach (var company in companies)
                    {
                        if (!Directory.Exists(company.AudioFolderRoute)) continue;

                        var operatorFolders = Directory.GetDirectories(company.AudioFolderRoute);
                        foreach (var folderPath in operatorFolders)
                        {
                            await ProcessOperatorFolder(folderPath, company.CompanyId, scope);
                        }
                    }
                }

                // חישוב זמן עד לריצה הבאה (למשל ב-24:00)
                await Task.Delay(CalculateDelayUntilNextRun(), stoppingToken);
            }
        }
        private void MoveToProcessed(string file, string folder)
        {
            string doneDir = Path.Combine(folder, "Processed");
            Directory.CreateDirectory(doneDir);
            string dest = Path.Combine(doneDir, Path.GetFileName(file));
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(file, dest);
        }


        private async Task ProcessOperatorFolder(string folderPath, int companyId, IServiceScope scope)
        {
            if (int.TryParse(Path.GetFileName(folderPath), out int operatorId))
            {
                var wavFiles = Directory.GetFiles(folderPath, "*.wav");
                var analysisService = scope.ServiceProvider.GetRequiredService<CallAnalysisService>();

                foreach (var file in wavFiles)
                {
                try
                {
                    await analysisService.ProcessFullCallChain(file, operatorId);
                    MoveToProcessed(file, folderPath);
                }
                catch (Exception ex)
                {
                    // מדפיס שגיאה אבל ממשיך לקובץ הבא!
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                }
                }
            }
        }

        private TimeSpan CalculateDelayUntilNextRun()
        {
            // חישוב זמן עד ל-2 בלילה למשל
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(1).AddHours(2);
            return nextRun - now;
        }
    }
}
