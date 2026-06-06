using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IPhotoDirectoryHelper
    {
        Task<List<MediaFileInfo>> GetPhotoFilesAsync(string folderPath);
    }
}
