namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IFileSystemService
    {
        void EnsureDirectoryExists(string path);
        void MoveFile(string sourcePath, string destinationPath, bool overwrite = false);
        bool FileExists(string path);
        void DeleteFile(string path);
        long GetFileSize(string path);
    }
}
