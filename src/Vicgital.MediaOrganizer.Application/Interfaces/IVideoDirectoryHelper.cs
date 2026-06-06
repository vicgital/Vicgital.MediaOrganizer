using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IVideoDirectoryHelper
    {
        Task<List<VideoFileInfo>> GetVideoFilesAsync(string folderPath);
    }
}
