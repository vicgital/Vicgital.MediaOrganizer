using System;
using System.Collections.Generic;
using System.Text;

namespace Vicgital.MediaOrganizer.Domain.Models
{
    public class VideoFileInfo
    {
        public required string FilePath { get; init; }
        public required string FileName { get; init; }
        public required string Extension { get; init; }
        public long Size { get; init; }
    }
}
