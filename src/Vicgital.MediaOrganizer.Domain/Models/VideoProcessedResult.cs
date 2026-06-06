namespace Vicgital.MediaOrganizer.Domain.Models
{
    public class VideoProcessedResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public double ElapsedTimeSeconds { get; set; }
        public long NewSize { get; set; }
        public double DurationSeconds { get; set; }
    }
}
