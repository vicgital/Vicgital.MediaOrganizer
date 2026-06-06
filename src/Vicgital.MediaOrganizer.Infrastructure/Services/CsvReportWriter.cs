using Microsoft.Extensions.Logging;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Infrastructure.Services
{
    public class CsvReportWriter(ILogger<CsvReportWriter> logger) : IReportWriter
    {
        private readonly ILogger<CsvReportWriter> _logger = logger;

        public async Task WriteResultsAsync(string folderPath, IReadOnlyList<VideoResizedResult> results)
        {
            string csvPath = Path.Combine(folderPath, $"video_resize_results_{DateTimeOffset.Now.ToFileTime()}.csv");
            await using var writer = new StreamWriter(csvPath);
            await writer.WriteLineAsync("FileName,Extension,DurationSeconds,Size,NewSize,% Compressed,Success,ElapsedTimeSeconds");

            foreach (var r in results)
            {
                string fileName = r.VideoFileInfo.FileName.Replace("\"", "\"\"");
                string extension = r.VideoFileInfo.Extension.Replace("\"", "\"\"");
                double percentCompressed = r.VideoFileInfo.Size > 0
                    ? (1 - (double)r.VideoProcessedResult.NewSize / r.VideoFileInfo.Size) * 100
                    : 0;
                string fileSizeString = ConvertFriendlySize(r.VideoFileInfo.Size);
                string newSizeString = ConvertFriendlySize(r.VideoProcessedResult.NewSize);

                await writer.WriteLineAsync(
                    $"\"{fileName}\",\"{extension}\",{r.VideoProcessedResult.DurationSeconds},{fileSizeString},{newSizeString},{percentCompressed}%,{r.VideoProcessedResult.Success},{r.VideoProcessedResult.ElapsedTimeSeconds}");
            }

            _logger.LogInformation("Results written to {csvPath}", csvPath);
        }

        private static string ConvertFriendlySize(long size)
        {
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
