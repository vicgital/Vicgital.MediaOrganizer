using Microsoft.Extensions.Logging;
using Vicgital.MediaOrganizer.Application.Interfaces;

namespace Vicgital.MediaOrganizer.Application.Jobs
{
    public class PhotoMoverJob(
        ILogger<PhotoMoverJob> logger,
        IPhotoDirectoryHelper directoryHelper,
        IFileSystemService fileSystem
        ) : IJob
    {
        public async Task<bool> Start(string folderPath)
        {
            try
            {
                logger.LogInformation("Moving Photos");

                var photoFiles = await directoryHelper.GetPhotoFilesAsync(folderPath);
                logger.LogInformation("Found {count} photo files in folder", photoFiles.Count);
                string newFolder = Path.Combine(folderPath, "Photos");
                fileSystem.EnsureDirectoryExists(newFolder);

                foreach (var photo in photoFiles)
                {
                    string newFilePath = Path.Combine(newFolder, photo.FileName);
                    logger.LogInformation("Moving {fileName} to Photos folder", photo.FileName);
                    fileSystem.MoveFile(photo.FilePath, newFilePath, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has occurred");
                return false;
            }
        }
    }
}
