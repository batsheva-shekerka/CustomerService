using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class VolumeAnalyzer
    {
        public static TimeSpan GetDuration(string wavPath)
        {
            using var reader = new AudioFileReader(wavPath);
            return reader.TotalTime;
        }
        public static (float avg, float peak) Analyze(string wavPath)
        {
            float sumSquares = 0;
            float peak = 0;
            int sampleCount = 0;

            using var reader = new AudioFileReader(wavPath);

            float[] buffer = new float[reader.WaveFormat.SampleRate];
            int read;

            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                {
                    float sample = Math.Abs(buffer[i]);
                    sumSquares += sample * sample;
                    peak = Math.Max(peak, sample);
                    sampleCount++;
                }
            }

            float rms = (float)Math.Sqrt(sumSquares / sampleCount);
            return (rms, peak);
        }
    }
}
