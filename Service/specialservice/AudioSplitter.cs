using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.specialservice
{
    public class AudioSplitter
    {
        public static void CreateSpeakerWav(
       string sourceWav,
       List<Segment> segments,
       string speakerId,
       string outputWav)
        {
            var speakerSegments = segments
                .Where(s => s.SpeakerId == speakerId)
                .OrderBy(s => s.Offset)
                .ToList();

            if (!speakerSegments.Any())
                throw new Exception($"No segments found for speaker {speakerId}");

            using var reader = new WaveFileReader(sourceWav);
            using var writer = new WaveFileWriter(outputWav, reader.WaveFormat);

            foreach (var seg in speakerSegments)
            {
                long startByte =
                    (long)(seg.Offset.TotalSeconds * reader.WaveFormat.AverageBytesPerSecond);

                long endByte =
                    (long)((seg.Offset + seg.Duration).TotalSeconds *
                           reader.WaveFormat.AverageBytesPerSecond);

                reader.Position = startByte;

                byte[] buffer = new byte[4096];

                while (reader.Position < endByte)
                {
                    int bytesToRead =
                        (int)Math.Min(buffer.Length, endByte - reader.Position);

                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead == 0)
                        break;

                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
