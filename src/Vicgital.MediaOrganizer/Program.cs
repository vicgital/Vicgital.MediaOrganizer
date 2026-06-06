// read the Paths.txt and save the list of paths in a variable as a list
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Vicgital.MediaOrganizer.Application.Interfaces;
using Vicgital.MediaOrganizer.Application.Jobs;
using Vicgital.MediaOrganizer.Infrastructure.Configuration;
using Vicgital.MediaOrganizer.Infrastructure.Configuration.Extensions;
using Vicgital.MediaOrganizer.Infrastructure.Helpers;
using Vicgital.MediaOrganizer.Infrastructure.Logging.Configuration;
using Vicgital.MediaOrganizer.Infrastructure.Logging.Configuration.Extensions;
using Vicgital.MediaOrganizer.Infrastructure.Logging.Extensions;
using Vicgital.MediaOrganizer.Infrastructure.Processors;
using Vicgital.MediaOrganizer.Infrastructure.Services;

namespace Vicgital.MediaOrganizer
{
    internal static class Program
    {
        private static readonly string PathsFile = "Paths.txt";


        static async Task Main(string[] args)
        {
            var console = GetConsoleLogger();
            console.Information("Application started at {Time}", DateTimeOffset.Now);

            List<string> paths = await GetPathsFromFile();

            if (paths.Count == 0)
            {
                console.Information("No valid paths found in {File}.", PathsFile);
                console.Information("Enter the folder path:");
                var targetFolder = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(targetFolder))
                {
                    console.Warning("Error: No folder path provided.");
                    return;
                }

                if (!Directory.Exists(targetFolder))
                {
                    console.Warning("Error: The directory '{Directory}' does not exist.", targetFolder);
                    return;
                }
                else
                    paths.Add(targetFolder);
            }



            bool resizeVideo = true;
            console.Information("Skip Resize Videos? (Y/N) (Default: N)");
            var resizeVideosInput = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrEmpty(resizeVideosInput) && resizeVideosInput.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase))
                resizeVideo = false;

            console.Information($"Resize Videos: {(resizeVideo ? "Yes" : "No")}");



            console.Information("Processing the following paths:");
            foreach (var path in paths)
                console.Information("- {Path}", path);            
            


            foreach (var path in paths)
            {
                var services = GetServiceCollection(path).BuildServiceProvider();

                console.Information("-----------------------------------");
                console.Information("Processing path: {Path}", path);

                // Move Photos
                await services.GetRequiredKeyedService<IJob>("PhotoMover").Start(path);

                // Move Videos, Resize if applicable
                if (resizeVideo)
                {
                    await services.GetRequiredKeyedService<IJob>("VideoResizer").Start(path);
                }
                else 
                {
                    await services.GetRequiredKeyedService<IJob>("VideoMover").Start(path);
                }
            }
        }

        

        #region Helpers

        private static Logger GetConsoleLogger() =>
            LoggerConfigurationBuilder.BuildDefault(LogEventLevel.Information).CreateLogger();

        private static async Task<List<string>> GetPathsFromFile()
        {
            if (File.Exists(PathsFile))
            {
                var contents = await File.ReadAllLinesAsync(PathsFile);

                var paths = contents
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l));

                // validate that the paths exist and filter out any that don't
                paths = [.. paths.Where(p => Directory.Exists(p))];

                return [.. paths];
            }

            return [];
        }

        private static ServiceCollection GetServiceCollection(string path)
        {
            var services = new ServiceCollection();
            var config = ConfigurationBuilder.BuildConfiguration();
            var logger = LoggerConfigurationBuilder.BuildDefault(LogEventLevel.Information);
            logger.WriteToFile($"{path}/media_organizer.log", rollingInterval: RollingInterval.Infinite);

            services.AddAppConfiguration(config);
            services.AddSerilogLogging(logger.CreateLogger());
            services.AddScoped<IProcessExecutor, ProcessExecutor>();
            services.AddScoped<IFileSystemService, FileSystemService>();
            services.AddScoped<IReportWriter, CsvReportWriter>();
            services.AddScoped<IVideoProcessor, VideoProcessor>();
            services.AddScoped<IVideoDirectoryHelper, VideoDirectoryHelper>();
            services.AddScoped<IPhotoDirectoryHelper, PhotoDirectoryHelper>();
            services.AddKeyedScoped<IJob, VideoResizerJob>("VideoResizer");
            services.AddKeyedScoped<IJob, VideoMoverJob>("VideoMover");
            services.AddKeyedScoped<IJob, PhotoMoverJob>("PhotoMover");



            return services;
        }

        #endregion


    }
}
