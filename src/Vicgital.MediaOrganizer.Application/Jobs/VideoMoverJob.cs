using Microsoft.Extensions.Logging;
using Vicgital.MediaOrganizer.Application.Interfaces;

namespace Vicgital.MediaOrganizer.Application.Jobs
{
    public class VideoMoverJob(
        ILogger<VideoMoverJob> logger,
        IVideoDirectoryHelper directoryHelper,
        IFileSystemService fileSystem
        ) : IJob
    {

        private readonly ILogger<VideoMoverJob> _logger = logger;
        private readonly IVideoDirectoryHelper _directoryHelper = directoryHelper;
        private readonly IFileSystemService _fileSystem = fileSystem;


        public async Task<bool> Start(string folderPath)
        {
            try
            {
                _logger.LogInformation("Moving Videos");
                
                var videoFiles = await _directoryHelper.GetVideoFilesAsync(folderPath);

                _logger.LogInformation("Found {count} video files in folder", videoFiles.Count);

                string newFolder = Path.Combine(folderPath, "Videos");
                _fileSystem.EnsureDirectoryExists(newFolder);

                foreach (var video in videoFiles)
                {
                    string newFilePath = Path.Combine(newFolder, video.FileName);
                    _logger.LogInformation("Moving {fileName} to Videos folder", video.FileName);
                    _fileSystem.MoveFile(video.FilePath, newFilePath, true);
                }

                return true;



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred");
                return false;
            }
        }
    }
}
