using Microsoft.Extensions.Logging;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Application.Jobs
{
    public class VideoResizerJob(
        ILogger<VideoResizerJob> logger,
        IVideoProcessor videoProcessor,
        IVideoDirectoryHelper directoryHelper,
        IFileSystemService fileSystem,
        IReportWriter reportWriter) : IJob
    {
        private readonly ILogger<VideoResizerJob> _logger = logger;
        private readonly IVideoProcessor _videoProcessor = videoProcessor;
        private readonly IVideoDirectoryHelper _directoryHelper = directoryHelper;
        private readonly IFileSystemService _fileSystem = fileSystem;
        private readonly IReportWriter _reportWriter = reportWriter;

        public async Task<bool> Start(string folderPath)
        {
            try
            {
                _logger.LogInformation("Resizing videos in folder..");

                string resizedDir = Path.Combine(folderPath, "Videos");
                string rawDir = Path.Combine(folderPath, "Raw_Videos");

                _logger.LogInformation("Resized Videos FolderPath: {resizedDir}", resizedDir);
                _logger.LogInformation("Raw Videos FolderPath: {rawDir}", rawDir);

                _fileSystem.EnsureDirectoryExists(resizedDir);
                _fileSystem.EnsureDirectoryExists(rawDir);

                var videos = await _directoryHelper.GetVideoFilesAsync(folderPath);

                if (videos.Count == 0)
                {
                    _logger.LogWarning("No video files were found");
                    return false;
                }
                
                _logger.LogInformation("START processing video files..");

                List<VideoResizedResult> results = new List<VideoResizedResult>();

                foreach (var video in videos.OrderBy(e => e.Size))
                {
                    string fileName = Path.GetFileName(video.FilePath);
                    string outputPath = Path.Combine(resizedDir, fileName);

                    var result = _videoProcessor.ProcessVideo(video.FilePath, outputPath);


                    if (result.Success)
                    {
                        string rawPath = Path.Combine(rawDir, fileName);
                        _fileSystem.MoveFile(video.FilePath, rawPath);
                        result.NewSize = _fileSystem.GetFileSize(outputPath);
                    }
                    else
                    {
                        if (_fileSystem.FileExists(outputPath))
                            _fileSystem.DeleteFile(outputPath);
                    }

                    results.Add(new VideoResizedResult
                    {
                        VideoFileInfo = video,
                        VideoProcessedResult = result
                    });
                }

                await _reportWriter.WriteResultsAsync(folderPath, results);

                _logger.LogInformation("Job ended at {Time}", DateTimeOffset.Now);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred");
                _logger.LogInformation("Job ended at {Time}", DateTimeOffset.Now);
                return false;
            }
        }
    }
}
