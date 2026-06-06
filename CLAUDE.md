# Vicgital.MediaOrganizer — Architecture Reference

## Overview

Console app (.NET 10) that organizes media files by sorting photos and videos into subfolders, with optional video transcoding via FFmpeg. Processes one or more target directories in batch.

## Solution Structure

```
Vicgital.MediaOrganizer.slnx
└── src/
    ├── Vicgital.MediaOrganizer/              # Entry point (console)
    ├── Vicgital.MediaOrganizer.Domain/       # Models, constants, exceptions
    ├── Vicgital.MediaOrganizer.Application/  # Interfaces, jobs (business logic)
    └── Vicgital.MediaOrganizer.Infrastructure/ # Implementations, config, logging
```

All projects target **net10.0**.

## Layer Responsibilities

### Domain
No external dependencies. Contains:
- `Models/MediaFileInfo.cs` — file metadata (path, name, extension, size)
- `Models/VideoProcessedResult.cs` — FFmpeg result (success, error, elapsed time, sizes)
- `Models/VideoResizedResult.cs` — combines `MediaFileInfo` + `VideoProcessedResult`
- `Constants/ConfigurationKeys.cs` — string keys for `appsettings.json`
- `Exceptions/VideoFilePathMissingException.cs` — custom exception

### Application
Depends on Domain only. Contains:
- `Interfaces/` — contracts for all external concerns (`IJob`, `IFileSystemService`, `IProcessExecutor`, `IVideoProcessor`, `IReportWriter`, `IVideoDirectoryHelper`, `IPhotoDirectoryHelper`, `IWorker`)
- `Jobs/PhotoMoverJob.cs` — discovers photos, moves them to `Photos/`
- `Jobs/VideoMoverJob.cs` — discovers videos, moves them to `Videos/`
- `Jobs/VideoResizerJob.cs` — discovers videos, transcodes with FFmpeg, archives originals to `Raw_Videos/`, writes CSV report

### Infrastructure
Depends on Application + Domain. Contains:
- `Services/FileSystemService.cs` — wraps `System.IO` file operations
- `Services/ProcessExecutor.cs` — wraps `System.Diagnostics.Process` for FFmpeg/ffprobe calls
- `Services/CsvReportWriter.cs` — writes CSV with compression metrics per video
- `Processors/VideoProcessor.cs` — calls FFmpeg to encode and ffprobe to get duration
- `Helpers/VideoDirectoryHelper.cs` — enumerates video files by allowed extensions
- `Helpers/PhotoDirectoryHelper.cs` — enumerates photo files by allowed extensions
- `Configuration/` — loads `appsettings.json` + env vars via `Microsoft.Extensions.Configuration`
- `Logging/` — Serilog setup with file sink per target folder + console sink

### Entry Point (Vicgital.MediaOrganizer)
- `Program.cs` — reads paths from `Paths.txt` or stdin, builds per-folder DI containers, orchestrates jobs
- `appsettings.json` — FFmpeg paths, codec args, allowed file extensions
- `Paths.txt` — optional newline-separated list of directories to process

## Execution Flow

```
Program.Main()
 ├─ Read paths from Paths.txt (or prompt user)
 └─ For each path:
     ├─ Build DI container (per folder, with per-folder log file)
     ├─ Run PhotoMoverJob  →  Photos/
     └─ Run VideoResizerJob (user opted in)  →  Videos/ + Raw_Videos/ + CSV report
         or VideoMoverJob (no resize)        →  Videos/
```

Output after execution on a target folder:
```
TargetFolder/
├── Photos/
├── Videos/
├── Raw_Videos/              (only when resizing)
├── media_organizer.log
└── video_resize_results_*.csv  (only when resizing)
```

## Dependency Injection

Services registered per folder in `Program.GetServiceCollection()`:

| Lifetime  | Interface               | Implementation        |
|-----------|-------------------------|-----------------------|
| Singleton | `IAppConfiguration`     | `AppConfiguration`    |
| Scoped    | `IProcessExecutor`      | `ProcessExecutor`     |
| Scoped    | `IFileSystemService`    | `FileSystemService`   |
| Scoped    | `IReportWriter`         | `CsvReportWriter`     |
| Scoped    | `IVideoProcessor`       | `VideoProcessor`      |
| Scoped    | `IVideoDirectoryHelper` | `VideoDirectoryHelper`|
| Scoped    | `IPhotoDirectoryHelper` | `PhotoDirectoryHelper`|
| Scoped    | `IJob` ("VideoResizer") | `VideoResizerJob`     |
| Scoped    | `IJob` ("VideoMover")   | `VideoMoverJob`       |
| Scoped    | `IJob` ("PhotoMover")   | `PhotoMoverJob`       |

Jobs are resolved by key using the keyed services API.

## Configuration

All settings live in `appsettings.json` (overridable via environment variables):

| Key                        | Description                          | Default Example                         |
|----------------------------|--------------------------------------|-----------------------------------------|
| `FFMPEG_PATH`              | Absolute path to ffmpeg.exe          | `C:\Tools\ffmpeg\bin\ffmpeg.exe`        |
| `FFPROBE_PATH`             | Absolute path to ffprobe.exe         | `C:\Tools\ffmpeg\bin\ffprobe.exe`       |
| `FFMPEG_ARGS`              | Encoder arguments passed to FFmpeg   | `-c:v libx265 -crf 24 -preset ultrafast -c:a copy` |
| `ALLOWED_VIDEO_EXTENSIONS` | Comma-separated video extensions     | `.mp4,.mov,.mkv,.avi,.m4v`             |
| `ALLOWED_PHOTO_EXTENSIONS` | Comma-separated photo extensions     | `.jpg,.jpeg,.png,.gif,.bmp,.hevc,.heic` |

Environment: `ASPNETCORE_ENVIRONMENT` (defaults to `dev`). Supports `appsettings.{env}.json` overrides.

## Key Design Patterns

- **Clean Architecture** — strict inward dependency rule; Domain has no dependencies
- **Strategy / Keyed DI** — `PhotoMoverJob`, `VideoMoverJob`, `VideoResizerJob` all implement `IJob`; resolved by key at runtime
- **Adapter** — `ProcessExecutor` adapts `System.Diagnostics.Process` to `IProcessExecutor`
- **Dependency Inversion** — Application defines interfaces; Infrastructure provides implementations

## External Dependencies

**NuGet:**
- `Microsoft.Extensions.*` (Configuration, Logging) — v10.0.x
- `Serilog` + sinks/enrichers — file + console logging with contextual enrichment

**System tools (must be pre-installed):**
- `ffmpeg` — video encoding
- `ffprobe` — video metadata (duration extraction)
