using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IReportWriter
    {
        Task WriteResultsAsync(string folderPath, IReadOnlyList<VideoResizedResult> results);
    }
}
