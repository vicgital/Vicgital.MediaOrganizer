# Vicgital.MediaOrganizer

A .NET 10 console application that automatically organizes media files in a target folder. It sorts photos and videos into dedicated subfolders and optionally transcodes videos to a smaller format using FFmpeg.

## What It Does

Point the app at any folder containing a mix of media files and it will:

1. **Move photos** into a `Photos/` subfolder
2. **Move videos** into a `Videos/` subfolder — or optionally:
3. **Transcode videos** using FFmpeg (H.265 by default), archive the originals in `Raw_Videos/`, and write a CSV report with compression metrics

The app supports batch processing: provide multiple folder paths and it runs through each one in sequence, writing a separate log file per folder.

### Output structure (after a run with resizing enabled)

```
TargetFolder/
├── Photos/                        # All photo files
├── Videos/                        # Transcoded video files
├── Raw_Videos/                    # Original video files (pre-transcoding)
├── media_organizer.log            # Per-run log
└── video_resize_results_*.csv     # Compression report (file name, sizes, %, duration)
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **FFmpeg** — must be installed and the paths to `ffmpeg.exe` and `ffprobe.exe` must be configured in `appsettings.json`. FFmpeg is used for video transcoding and duration extraction. Download it from [ffmpeg.org](https://ffmpeg.org/download.html).

## Configuration

Edit `src/Vicgital.MediaOrganizer/appsettings.json` before running:

```json
{
  "FFMPEG_PATH": "C:\\Tools\\ffmpeg\\bin\\ffmpeg.exe",
  "FFPROBE_PATH": "C:\\Tools\\ffmpeg\\bin\\ffprobe.exe",
  "FFMPEG_ARGS": "-c:v libx265 -crf 24 -preset ultrafast -c:a copy",
  "ALLOWED_VIDEO_EXTENSIONS": ".mp4,.mov,.mkv,.avi,.m4v",
  "ALLOWED_PHOTO_EXTENSIONS": ".jpg,.jpeg,.png,.gif,.bmp,.hevc,.heic"
}
```

| Setting                      | Description                                                       |
|------------------------------|-------------------------------------------------------------------|
| `FFMPEG_PATH`                | Full path to the `ffmpeg` executable                              |
| `FFPROBE_PATH`               | Full path to the `ffprobe` executable                             |
| `FFMPEG_ARGS`                | FFmpeg encoding arguments (codec, quality, preset, audio)         |
| `ALLOWED_VIDEO_EXTENSIONS`   | Comma-separated list of video extensions to process               |
| `ALLOWED_PHOTO_EXTENSIONS`   | Comma-separated list of photo extensions to process               |

All settings can also be overridden with environment variables. The `ASPNETCORE_ENVIRONMENT` variable selects an `appsettings.{env}.json` override file (defaults to `dev`).

## How to Run

### Option 1 — Use Paths.txt

Add the directories you want to process to `src/Vicgital.MediaOrganizer/Paths.txt`, one path per line:

```
C:\Users\You\Pictures\Vacation 2025
D:\Footage\Wedding
```

Then run:

```bash
dotnet run --project src/Vicgital.MediaOrganizer
```

### Option 2 — Enter paths interactively

Run without a `Paths.txt` (or leave it empty) and the app will prompt you to enter folder paths one at a time in the console.

### Resize prompt

After paths are loaded, the app asks:

```
Do you want to resize videos? (y/n)
```

- **y** — transcode videos with FFmpeg, archive originals, generate CSV report
- **n** — move videos as-is without transcoding

## Building

```bash
dotnet build
```

## Project Structure

```
src/
├── Vicgital.MediaOrganizer/              # Console entry point
├── Vicgital.MediaOrganizer.Domain/       # Models and constants
├── Vicgital.MediaOrganizer.Application/  # Business logic and interfaces
└── Vicgital.MediaOrganizer.Infrastructure/ # File I/O, FFmpeg, logging, config
```

See `CLAUDE.md` for a detailed architecture reference.
