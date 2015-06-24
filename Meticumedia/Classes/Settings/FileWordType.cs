// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{    
    /// <summary>
    /// Words (or portions) of file name - used for identifying substring of a file name and rebuilding it
    /// </summary>
    public enum FileWordType
    {
        None = 0,
        Tv = 32768,
        Movie = 65536,
        CustomString = 1 | Tv | Movie, 
        ShowName = 2 | Tv, 
        EpisodeNumber = 4 | Tv, 
        EpisodeName = 8 | Tv, 
        MovieName = 16 | Movie,
        VideoEncoding = 32 | Movie,
        AudioEncoding = 64 | Movie,
        VideoResolution = 128 | Movie,
        VideoQuality = 256 | Movie,
        RipType = 512 | Movie,
        Language = 1024 | Movie,
        ContentFormat = 2048 | Movie,
        LanguageSubstitution = 4096 |Movie,
        FilePart = 8192 | Movie,
        Year = 16384 | Movie
    }
}
