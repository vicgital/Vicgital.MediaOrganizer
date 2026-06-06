using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Domain.Constants;
using Vicgital.MediaOrganizer.Domain.Models;
using Vicgital.MediaOrganizer.Infrastructure.Configuration.Services;

namespace Vicgital.MediaOrganizer.Infrastructure.Processors
{
    public class VideoProcessor(
        ILogger<VideoProcessor> logger,
        IAppConfiguration appConfiguration,
        IProcessExecutor processExecutor) : IVideoProcessor
    {
        private readonly ILogger<VideoProcessor> _logger = logger;
        private readonly IProcessExecutor _processExecutor = processExecutor;
        private readonly string _ffmpegArgs = appConfiguration.GetValue(ConfigurationKeys.FfmpegArgs);
        private readonly string _ffmpegPath = appConfiguration.GetValue(ConfigurationKeys.FfmpegPath, "ffmpeg");
        private readonly string _ffprobePath = appConfiguration.GetValue(ConfigurationKeys.FfprobePath, "ffprobe");

        public VideoProcessedResult ProcessVideo(string videoInputPath, string videoOutputPath)
        {
            var videoDuration = GetVideoDuration(videoInputPath);
            _logger.LogInformation("---------");
            _logger.LogInformation("Processing video {videoInputPath}, Video Duration: {videoDuration} seconds", videoInputPath, videoDuration);

            string args = $"-y -i \"{videoInputPath}\" {_ffmpegArgs} \"{videoOutputPath}\"";
            var sw = Stopwatch.StartNew();
            VideoProcessedResult result = new()
            {
                DurationSeconds = videoDuration
            };

            try
            {
                var processResult = _processExecutor.Run(_ffmpegPath, args);

                if (processResult.ExitCode == 0 && File.Exists(videoOutputPath))
                {
                    result.Success = true;
                    _logger.LogInformation("✓ FFmpeg succeeded. Output saved to {videoOutputPath} | Elapsed Time: {elapsedTime} seconds", videoOutputPath, sw.Elapsed.TotalSeconds);
                }
                else
                {
                    result.ErrorMessage = processResult.StandardError;
                    _logger.LogWarning("✗ FFmpeg failed (exit {ExitCode}). Original left in place. Elapsed Time: {elapsedTime} seconds", processResult.ExitCode, sw.Elapsed.TotalSeconds);
                    _logger.LogWarning("FFmpeg Log: {log}", processResult.StandardError);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error processing video {videoInputPath}", videoInputPath);
            }
            finally
            {
                sw.Stop();
                result.ElapsedTimeSeconds = sw.Elapsed.TotalSeconds;
            }

            return result;
        }

        public double GetVideoDuration(string videoFilePath)
        {
            string args = $"-v error -show_entries format=duration -of csv=p=0 \"{videoFilePath}\"";
            var processResult = _processExecutor.Run(_ffprobePath, args);

            return double.TryParse(processResult.StandardOutput.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double duration)
                ? duration : 0;
        }
    }
}
