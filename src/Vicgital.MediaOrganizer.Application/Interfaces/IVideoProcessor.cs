using Vicgital.MediaOrganizer.Domain.Models;

namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IVideoProcessor
    {
        double GetVideoDuration(string videoFilePath);
        VideoProcessedResult ProcessVideo(string videoInputPath, string videoOutputPath);
    }
}
