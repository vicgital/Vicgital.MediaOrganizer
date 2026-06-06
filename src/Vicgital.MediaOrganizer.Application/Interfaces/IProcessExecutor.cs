using Vicgital.MediaOrganizer.Application.Models;

namespace Vicgital.MediaOrganizer.Application.Interfaces
{
    public interface IProcessExecutor
    {
        ProcessResult Run(string executable, string arguments);
    }
}
