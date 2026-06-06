using Vicgital.MediaOrganizer.Application.Interfaces;

namespace Vicgital.MediaOrganizer.Infrastructure.Services
{
    public class FileSystemService : IFileSystemService
    {
        public void EnsureDirectoryExists(string path) =>
            Directory.CreateDirectory(path);

        public void MoveFile(string sourcePath, string destinationPath, bool overwrite = false) =>
            File.Move(sourcePath, destinationPath, overwrite: overwrite);

        public bool FileExists(string path) =>
            File.Exists(path);

        public void DeleteFile(string path) =>
            File.Delete(path);

        public long GetFileSize(string path) =>
            new FileInfo(path).Length;
    }
}
