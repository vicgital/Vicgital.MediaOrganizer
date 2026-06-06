namespace Vicgital.MediaOrganizer.Domain.Models
{
    public class VideoResizedResult
    {
        public required VideoFileInfo VideoFileInfo { get; init; }
        public required VideoProcessedResult VideoProcessedResult { get; init; }
    }
}
