using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.specialservice
{
    public class AudioDurationService
    {
        public static TimeSpan GetDuration(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{filePath}\"",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardError.ReadToEnd();
            process.WaitForExit();

            var match = Regex.Match(output, @"Duration:\s(\d+):(\d+):(\d+\.\d+)");
            if (!match.Success)
                throw new Exception("Duration not found");

            return new TimeSpan(
                0,
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                (int)double.Parse(match.Groups[3].Value)
            );
        }
    }
}
