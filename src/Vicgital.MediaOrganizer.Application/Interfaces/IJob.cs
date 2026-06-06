namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IJob
    {
        Task<bool> Start(string folderPath);
    }
}
