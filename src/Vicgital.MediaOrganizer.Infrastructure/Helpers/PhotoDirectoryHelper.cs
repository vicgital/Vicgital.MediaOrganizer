using System;
using System.Collections.Generic;
using System.Text;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Domain.Constants;
using Vicgital.MediaOrganizer.Domain.Models;
using Vicgital.MediaOrganizer.Infrastructure.Configuration.Services;

namespace Vicgital.MediaOrganizer.Infrastructure.Helpers
{
    public class PhotoDirectoryHelper(
        IAppConfiguration appConfiguration) : IPhotoDirectoryHelper
    {
        private readonly List<string> allowedPhotoExtensions = [.. appConfiguration.GetValue(ConfigurationKeys.AllowedPhotoExtensions).Split(",")];
        public async Task<List<MediaFileInfo>> GetPhotoFilesAsync(string folderPath)
        {
            var photos = Directory.EnumerateFiles(folderPath)
                .Where(f => allowedPhotoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToList();

            List<MediaFileInfo> result = [];

            foreach (var photo in photos)
            {
                FileInfo fileInfo = new(photo);
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
