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
        //private readonly string _rootPath = @"C:\Calls";
        //private readonly string _inboxPath = @"C:\Calls\Incoming";
        //private readonly string _processedPath = @"C:\Calls\Done";
        private readonly IServiceProvider _serviceProvider;

        public FolderWatcherWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //Directory.CreateDirectory(_inboxPath);
            //Directory.CreateDirectory(_processedPath);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Worker started at: " + DateTime.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var companyService = scope.ServiceProvider.GetRequiredService<CompanyService>();
                    var companies = await companyService.GetAllAsync(); // companies from-DB

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

                // culculate the next run time
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
            string operatorEmail = Path.GetFileName(folderPath);

            var operatorService = scope.ServiceProvider.GetRequiredService<OperatorService>();

            int? opId = await operatorService.GetIdByEmailAsync(operatorEmail);

            if (opId == null)
            {
                Console.WriteLine($"Warning: No operator found for email {operatorEmail}");
                return;
            }

            var wavFiles = Directory.GetFiles(folderPath, "*.wav");
            var analysisService = scope.ServiceProvider.GetRequiredService<CallAnalysisService>();

            foreach (var file in wavFiles)
            {
                try
                {
                    await analysisService.ProcessFullCallChain(file, opId.Value);
                    MoveToProcessed(file, folderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
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
