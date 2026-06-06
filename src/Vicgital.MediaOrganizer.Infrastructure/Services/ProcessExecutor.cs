using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Application.Models;

namespace Vicgital.MediaOrganizer.Infrastructure.Services
{
    public class ProcessExecutor(
        ILogger<ProcessExecutor> logger) : IProcessExecutor
    {
        private readonly ILogger<ProcessExecutor> _logger = logger;

        public ProcessResult Run(string executable, string arguments)
        {

            var psi = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            ProcessResult result;

            try
            {
                // Drain stderr so the process doesn't block on a full buffer
                string logErr = process.StandardError.ReadToEnd();
                string logOut = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                result = new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = logOut,
                    StandardError = logErr
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Occurred");

                result = new ProcessResult
                {
                    ExitCode = -1,
                    StandardOutput = string.Empty,
                    StandardError = ex.Message
                };
            }
            finally
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }

            return result;

            




        }
    }
}
