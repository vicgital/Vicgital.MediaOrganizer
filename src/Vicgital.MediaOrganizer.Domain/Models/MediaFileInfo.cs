namespace Vicgital.MediaOrganizer.Domain.Models
{
    public class MediaFileInfo
    {
        public required string FilePath { get; init; }
        public required string FileName { get; init; }
        public required string Extension { get; init; }
        public long Size { get; init; }
    }
}
