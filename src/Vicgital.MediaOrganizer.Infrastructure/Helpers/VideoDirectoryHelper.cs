using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Domain.Constants;
using Vicgital.MediaOrganizer.Domain.Models;
using Vicgital.MediaOrganizer.Infrastructure.Configuration.Services;

namespace Vicgital.MediaOrganizer.Infrastructure.Helpers
{
    public class VideoDirectoryHelper(
        IAppConfiguration appConfiguration
        ) : IVideoDirectoryHelper
    {
        private readonly List<string> allowedVideoExtensions = [.. appConfiguration.GetValue(ConfigurationKeys.AllowedVideoExtensions).Split(",")];

        public async Task<List<MediaFileInfo>> GetVideoFilesAsync(string folderPath)
        {
            var videos = Directory.EnumerateFiles(folderPath)
                .Where(f => allowedVideoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToList();

            List<MediaFileInfo> result = [];

            foreach (var video in videos)
            {
                FileInfo fileInfo = new(video);
                result.Add(new MediaFileInfo
                {
                    Extension = fileInfo.Extension,
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    Size = fileInfo.Length
                });
            }

            return result;
        }        
    }
}
