namespace Vicgital.MediaOrganizer.Domain.Exceptions
{
    public class VideoFilePathMissingException : Exception
    {
        public VideoFilePathMissingException()
            : base("A video file was encountered with a missing or empty file path.") { }

        public VideoFilePathMissingException(string message)
            : base(message) { }
    }
}
