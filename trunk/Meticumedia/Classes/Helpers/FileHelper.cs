// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace Meticumedia
{
    /// <summary>
    /// Static helper class with methods for aiding in actions related to files.
    /// </summary>
    public static class FileHelper
    {
        #region General

        /// <summary>
        /// String used in destination for items to be delete
        /// </summary>
        public static readonly string DELETE_DIRECTORY = "Never Never Land (Permanent Delete)";

        /// <summary>
        /// Categories of useful file types
        /// </summary>
        public enum FileCategory { Unknown, Ignored, TvVideo, NonTvVideo, Trash, Custom, Folder };

        /// <summary>
        /// Categorize a file based on extension
        /// </summary>
        /// <param name="file">The file name string</param>
        /// <returns>The file category</returns>
        public static FileCategory CategorizeFile(OrgPath file)
        {
            // Get file extension
            string extenstion = Path.GetExtension(file.Path).ToLower();
            
            // Check if ignored
            if (file.OrgFolder != null && file.OrgFolder.IsIgnored(file.Path))
                return FileCategory.Ignored;

            // Check against each type
            foreach (string ext in Settings.VideoFileTypes)
                if (extenstion == ext.ToLower())
                {
                    if (IsTvEpisode(file.Path))
                        return FileCategory.TvVideo;
                    else
                        return FileCategory.NonTvVideo;
                }
            foreach (string ext in Settings.DeleteFileTypes)
                if (extenstion == ext.ToLower())
                    return FileCategory.Trash;

            foreach (string ext in Settings.IgnoreFileTypes)
                if (extenstion == ext.ToLower())
                    return FileCategory.Ignored;

            return FileCategory.Unknown;
        }

        /// <summary>
        /// Remove invalid file name characters from string
        /// </summary>
        /// <param name="name">original file name</param>
        /// <returns>resulting name without invalid characters</returns>
        public static string GetSafeFileName(string name)
        {
            string newName = name.Replace("*", "#");
            newName = newName.Replace("|","-");
            newName = newName.Replace("\\","-");
            newName = newName.Replace("/","-");
            newName = newName.Replace(":"," -");
            newName = newName.Replace(">","");
            newName = newName.Replace(">","");
            newName = newName.Replace("?","");
            newName = newName.Replace("\"", "'");

            return newName;
        }

        /// <summary>
        /// Gets year of movie from file name.
        /// </summary>
        /// <param name="fileName">The file name string to get year from</param>
        /// <returns>Year found in file name is any, otherwise -1</returns>
        public static int GetYear(string fileName)
        {
            Match match = Regex.Match(fileName, @"\D((?:19|20)\d{2})\D");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].ToString());
            }

            return -1;
        }

        /// <summary>
        /// Compares two strings to check if they are very similar or equivalent.
        /// </summary>
        /// <param name="s1">The first string</param>
        /// <param name="s2">The second string</param>
        /// <returns>Whether the 2 strings are very similar or equivalen</returns>
        public static bool CompareStrings(string s1, string s2)
        {
            // Both string to lower case
            s1 = s1.ToLower();
            s2 = s2.ToLower();

            // If equal compare is good
            if (s1.Equals(s2))
                return true;

            int s1Count;
            int s2Count;
            List<string> diff;
            GetStringDiff(s1, s2, out s1Count, out s2Count, out diff);

            if (diff.Count == 1 && diff[0] == "the")
                return true;

            // Return unmatched if too many different words
            //if (diff.Count >= s1Count - 1 || diff.Count >= s2Count - 1)
            if (diff.Count > 0)
                return false;

            // Return matched
            return true;
        }

        /// <summary>
        /// Get list different between two strings - based on seperation by whitespace
        /// </summary>
        /// <param name="s1">The first string</param>
        /// <param name="s2">The second string</param>
        /// <returns>list of differences</returns>
        public static List<string> GetStringDiff(string s1, string s2)
        {
            int s1Count;
            int s2Count;
            List<string> diff;
            GetStringDiff(s1, s2, out s1Count, out s2Count, out diff);

            return diff;
        }

        /// <summary>
        /// Get list different between two strings - based on seperation by whitespace
        /// </summary>
        /// <param name="s1">The first string</param>
        /// <param name="s2">The second string</param>
        /// <param name="s1Count">Count of words in first string</param>
        /// <param name="s2Count">Count of words in second string</param>
        /// <param name="diff">list of differences</param>
        private static void GetStringDiff(string s1, string s2, out int s1Count, out int s2Count, out List<string> diff)
        {
            // Get words out of strings
            IEnumerable<string> set1 = s1.Split(' ').Distinct();
            IEnumerable<string> set2 = s2.Split(' ').Distinct();

            // Count words in strings
            s1Count = set1.Count();
            s2Count = set2.Count();

            // Get the words that are different between the two strings

            if (s2Count > s1Count)
                diff = set2.Except(set1).ToList();
            else
                diff = set1.Except(set2).ToList();
        }

        #endregion

        #region TV

        /// <summary>
        /// Array of regular expressions to match season and episode information
        /// from a file name.
        /// </summary>
        private static string[] SeasonEpMatch = new string[4] 
        {
            @"(^|\W)s(eason)?\W?(?<s>\d+)(?<sep1>.?)e(pisode)?\W?(?<e1>\d+)((?<sep2>[^e]?)(e(pisode)?)?\W?(?<e2>\d+))?(?:\W|$)", // s/e match: s01e02 or season1episode02 s01e02e03 or s01e0203 or s01.e02 or s01_e02, etc.
            @"(^|\W)e(pisode)?(?<e1>\d+).?(e(pisode)?\W?(?<e2>\d+))?\W?s(eason)?\W?(?<s>\d+)(?:\W|$)", // e/s match: e01s02 or episode02seasson02 e01e02s03 or e0102s03 or e01.s02 or e01_s02, etc.
            @"(^|\W)(?<s>\d{1,2})(?<e1>\d{2})(?<e2>\d{2})?(?:\W|$)", // no s/e: 120203, 503, 50304            
            @"(^|\W)(?<s>\d{1,2})(?<sep1>.)(?<e1>\d{2})((?<sep2>.)(?<e2>\d{2}))?(?:\W|$)" // no s/e with seperators: 5x03, 5_03, 5_03_04, 12x02x03 etc.
        };

        /// <summary>
        /// Attempts to retrieve the season and episode information from a file name string.
        /// </summary>
        /// <param name="file">The file name string</param>
        /// <param name="season">The season number of the episode</param>
        /// <param name="episode1">The episode number</param>
        /// <param name="episode2">The 2nd episode number for multipart files</param>
        /// <returns>true if episode information is found</returns>
        public static bool GetEpisodeInfo(string file, string showName, out int season, out int episode1, out int episode2)
        {
            string unused, unused2;
            return GetEpisodeInfo(file, showName, out season, out episode1, out episode2, out unused, out unused2);
        }

        /// <summary>
        /// Removes episode season/episode number portion from file name string.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string RemoveEpisodeInfo(string file)
        {
            int season, episode1, episode2;
            string fileWithoutEp, fileBeforeEp;
            GetEpisodeInfo(file, string.Empty, out season, out episode1, out episode2, out fileWithoutEp, out fileBeforeEp);

            return fileWithoutEp;
        }

        /// <summary>
        /// Trims a file name down to the portion that come before the season/episode numbers.
        /// </summary>
        /// <param name="file">The file name string</param>
        /// <returns>The trimmed file string</returns>
        public static string TrimFromEpisodeInfo(string file)
        {
            int season, episode1, episode2;
            string fileWithoutEp, fileBeforeEp;
            GetEpisodeInfo(file, string.Empty, out season, out episode1, out episode2, out fileWithoutEp, out fileBeforeEp);

            return fileBeforeEp;
        }

        /// <summary>
        /// Attempts to retrieve the season and episode information from a file name string.
        /// </summary>
        /// <param name="file">The file name string</param>
        /// <param name="season">The season number of the episode</param>
        /// <param name="episode1">The episode number</param>
        /// <param name="episode2">The 2nd episode number for multipart files</param>
        /// <param name="fileWithoutEp">File name string with season/episode part removed</param>
        /// <param name="fileBeforeEp">Portion of file name string that came before the season/episode</param>
        /// <returns>true if episode information is found</returns>
        private static bool GetEpisodeInfo(string file, string showName, out int season, out int episode1, out int episode2, out string fileWithoutEp, out string fileBeforeEp)
        {
            // Default value to empty string
            season = -1;
            episode1 = -1;
            episode2 = -1;

            // Get simpplified file name for easier matching
            string simpleName = System.IO.Path.GetFileNameWithoutExtension(file);
            simpleName = BasicSimplify(simpleName, true);

            // Remove show name
            string[] showNameWord = showName.Split(' ');
            foreach (string word in showNameWord)
                if (!string.IsNullOrEmpty(word))
                {
                    Match m;
                    while(true)
                    {
                        m = Regex.Match(simpleName, @"(\W|^)(" + word + @")(\W|$)", RegexOptions.IgnoreCase);
                        if (m.Groups.Count > 2)
                            simpleName = simpleName.Remove(m.Groups[2].Index, m.Groups[2].Length);
                        else
                            break;
                    }
                }

            // Attempt to match against eaching matching expression
            foreach (string reStr in SeasonEpMatch)
            {
                MatchCollection matches = Regex.Matches(simpleName, reStr, RegexOptions.IgnoreCase);
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    string matchStr = simpleName.Substring(matches[i].Index, matches[i].Length);
                    if (showName.Split(' ').ToList().Contains(matchStr))
                        continue;

                    // Get values
                    bool seasonParsed = int.TryParse(matches[i].Groups["s"].Value, out season);
                    bool episode1Parsed = int.TryParse(matches[0].Groups["e1"].Value, out episode1);
                    if (!int.TryParse(matches[0].Groups["e2"].Value, out episode2))
                        episode2 = -1;

                    // Don't match 2nd episode if seperators don't match
                    if (episode2 >= 0)
                    {
                        string sep1 = matches[0].Groups["sep1"].Value;
                        string sep2 = matches[0].Groups["sep2"].Value;

                        if (sep1 != sep2 && sep1 != null & sep2 != null)
                            episode2 = -1;
                    }

                    // Return true if values are good
                    if (seasonParsed && episode1Parsed)
                    {
                        Match m = Regex.Match(simpleName, reStr);
                        if (m.Success)
                            fileWithoutEp = simpleName.Remove(m.Index, m.Length).Replace("-", "");
                        else
                            fileWithoutEp = simpleName;

                        // Get string before episode number (e.g. "The Office" from "Then Office S07E01 BDRip XviD-CLUE"
                        fileBeforeEp = simpleName.Remove(matches[0].Index);

                        return episode1 > 0 || episode2 > 0;
                    }
                }
            }

            fileWithoutEp = file;
            fileBeforeEp = file;
            return false;
        }

        /// <summary>
        /// Determines whether a file is a TV episode based on whether season/episode information
        /// can be extracted from the file name.
        /// </summary>
        /// <param name="file">The file name string</param>
        /// <returns>true if TV episode information is found in file name</returns>
        public static bool IsTvEpisode(string file)
        {
            int season, episode1, episode2;
            return GetEpisodeInfo(file, string.Empty, out season, out episode1, out episode2);
        }

        #endregion

        #region File Name Simplifying

        /// <summary>
        /// Array of words to be removed from file names to make processing easier.
        /// </summary>
        private static RemoveFileWord[] AlwaysRemoveWords = new RemoveFileWord[]
        {
            new RemoveFileWord(Separator.None, Separator.None, "repack", FileWordType.None),
            new RemoveFileWord(Separator.None, Separator.None, "internal", FileWordType.None),
            new RemoveFileWord(Separator.None, Separator.None, "(?:extended|final|directors)(?:\\W+cut)?", FileWordType.ContentFormat),
            new RemoveFileWord(Separator.None, Separator.None, "audio", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "cam", FileWordType.VideoQuality),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "dub", FileWordType.LanguageSubstitution),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "dubbed", FileWordType.LanguageSubstitution),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "cut", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "rc", FileWordType.None),
            new RemoveFileWord(Separator.None, Separator.None, "remastered", FileWordType.ContentFormat),
            new RemoveFileWord(Separator.None, Separator.None, "unrated", FileWordType.ContentFormat),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "dts", FileWordType.AudioEncoding),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "hq", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "subs", FileWordType.LanguageSubstitution),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "bluray", FileWordType.VideoQuality),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "dvd", FileWordType.VideoQuality),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "\\w*rips?", FileWordType.VideoQuality),
            new RemoveFileWord(Separator.Nonnumeric, Separator.Nonnumeric, "(?:DD\\W?)?5.1", FileWordType.AudioEncoding),
            new RemoveFileWord(Separator.Nonnumeric, Separator.Nonnumeric, "1.5", FileWordType.None),
            new RemoveFileWord(Separator.Nonnumeric, Separator.None, "480p", FileWordType.VideoResolution),
            new RemoveFileWord(Separator.Nonnumeric, Separator.None, "720p", FileWordType.VideoResolution),
            new RemoveFileWord(Separator.Nonnumeric, Separator.None, "1080p", FileWordType.VideoResolution),
            new RemoveFileWord(Separator.None, Separator.Nonnumeric, "x264", FileWordType.VideoEncoding),
            new RemoveFileWord(Separator.None, Separator.None, "xvid", FileWordType.VideoEncoding),
            new RemoveFileWord(Separator.None, Separator.None, "divx", FileWordType.VideoEncoding),
            new RemoveFileWord(Separator.None, Separator.Nonnumeric, "AC3", FileWordType.AudioEncoding),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "hd", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "3d", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "gb", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "eng?(?:lish)?", FileWordType.Language),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "srt", FileWordType.LanguageSubstitution),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "r5", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "cd\\d", FileWordType.FilePart),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "vost", FileWordType.LanguageSubstitution),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "hdtv", FileWordType.None),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "lol", FileWordType.None),
        };

        /// <summary>
        /// Array of words that to be removed from file names to make SOMETIMES processing easier.
        /// </summary>
        private static RemoveFileWord[] OptionalRemoveWords = new RemoveFileWord[]
        {
            new RemoveFileWord(Separator.Nonnumeric, Separator.Nonnumeric, @"(?:19|20)\d{2}", FileWordType.Year, true, false),
            new RemoveFileWord(Separator.Nonnumeric, Separator.Nonnumeric, @"(?:19|20)\d{2}", FileWordType.Year, false, true),
            new RemoveFileWord(Separator.Whitespace, Separator.Whitespace, "(?:and|&)", FileWordType.None, false, false)
        };

        /// <summary>
        /// Optional remove file word types - matches OptionalRemoveWords items
        /// </summary>
        public enum OptionalSimplifyRemoves { None = 0, Year = 1, YearAndFollowing = 2, And = 4 }

        /// <summary>
        /// Result from simplification process of a string.
        /// </summary>
        public class SimplifyStringResults
        {
            /// <summary>
            /// Simplified string value.
            /// </summary>
            public string SimplifiedString { get; set; }

            /// <summary>
            /// Dictionary of words that were removed from original string, indexed by word type
            /// </summary>
            public Dictionary<FileWordType, List<string>> RemovedWords { get; set; }

            /// <summary>
            /// Constructor with known properties
            /// </summary>
            /// <param name="text">Simplified string</param>
            /// <param name="removedWords">Dictionary of words that were removed</param>
            public SimplifyStringResults(string text, Dictionary<FileWordType, List<string>> removedWords)
            {
                this.SimplifiedString = text;
                this.RemovedWords = removedWords;
            }

            /// <summary>
            /// Overrides to output simplified string value.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return this.SimplifiedString;
            }
        }

        /// <summary>
        /// Creates of list of simplified strings from an input string (multiple options for which words to remove creates multiple results).
        /// </summary>
        /// <param name="input">String to be simplified</param>
        /// <returns>List of simplified string results</returns>
        public static List<SimplifyStringResults> SimplifyString(string input)
        {
            // Create list of simplified strings
            List<SimplifyStringResults> simpliedStrings = new List<SimplifyStringResults>();

            // Set number of optional combinations for simplifying string with
            int optionCombinations = (int)Math.Pow(2, OptionalRemoveWords.Length + 2);

            // Loop twice: with and without word splitting
            for (int i = 0; i < 2; i++)
            {
                // Go through all combinations of optional removes
                for (int j = 0; j < optionCombinations; j++)
                {
                    // Build options
                    OptionalSimplifyRemoves options = (OptionalSimplifyRemoves)j;

                    // Don't do both year removes
                    if ((options & OptionalSimplifyRemoves.Year) > 0 && (options & OptionalSimplifyRemoves.YearAndFollowing) > 0)
                        continue;

                    // Get results
                    SimplifyStringResults simpleRes = BuildSimplifyResults(input, (j & 1) > 0, (j & 2) > 0, options, false, i == 1, true);

                    // Don't allow result that is only the year
                    if(Regex.IsMatch(simpleRes.SimplifiedString, @"^(19|20)\d{2}$") && !simpleRes.RemovedWords.ContainsKey(FileWordType.Year))
                        continue;

                    // Don't let common single words through
                    if (!simpleRes.SimplifiedString.Contains(' ') && simpleRes.SimplifiedString.Length < 4 && WordHelper.IsWord(simpleRes.SimplifiedString))
                        continue;


                    // Add to list of simplified strings
                    bool exists = false;
                    foreach (SimplifyStringResults simplifyRes in simpliedStrings)
                        if (simplifyRes.SimplifiedString == simpleRes.SimplifiedString)
                        {
                            exists = true;
                            break;
                        }

                    // Check that simplification doesn't already exist!
                    if (!exists && !string.IsNullOrEmpty(simpleRes.SimplifiedString))
                        simpliedStrings.Add(simpleRes);
                }
            }

            return simpliedStrings;
        }

        /// <summary>
        /// Simplifies an input string with various options.
        /// </summary>
        /// <param name="input">String to be simplified</param>
        /// <param name="removeFirst">Whether to always remove first word during simplification</param>
        /// <param name="removeLast">Whether to always remove last word during simplification</param>
        /// <param name="options">Selection of optional remove words</param>
        /// <param name="disableRemAfter">Disable removing of words that follow defined words to remove</param>
        /// <param name="wordSplitEn">Enables splitting words in the string using dictionary (e.g. "howimetyourmother" split to "how i met your mother"</param>
        /// <param name="removeWhitespace">Whether to remove extra whitespace</param>
        /// <returns>Simplified string results</returns>
        public static SimplifyStringResults BuildSimplifyResults(string input, bool removeFirst, bool removeLast, OptionalSimplifyRemoves options, bool disableRemAfter, bool wordSplitEn, bool removeWhitespace)
        {
            // All lowercase
            string simplifiedName = input.ToLower();

            // Remove unneeded characters: ',!,?,(,),:
            simplifiedName = Regex.Replace(simplifiedName, @"[']+", "");
            simplifiedName = Regex.Replace(simplifiedName, @"[!\?\u0028\u0029\:\]\[]+", " ");

            // Replace seperators with spaces
            if (removeWhitespace)
                simplifiedName = Regex.Replace(simplifiedName, @"\W+|_", " ");

            Dictionary<FileWordType, List<string>> removeFileWords = new Dictionary<FileWordType, List<string>>();

            // Remove first word (enabled by bit 0)
            if (removeFirst)
            {
                Match firstWordMatch = Regex.Match(simplifiedName, @"^\W*\w+");
                if (firstWordMatch.Success)
                    simplifiedName = simplifiedName.Remove(firstWordMatch.Index, firstWordMatch.Length);
            }

            // Remove Last word (enabled by bit 1)
            else if (removeLast)
            {
                Match lastWordMatch = Regex.Match(simplifiedName, @"(\w+\W*)$");
                if (lastWordMatch.Success)
                    simplifiedName = simplifiedName.Remove(lastWordMatch.Index, lastWordMatch.Length);
            }
            // Don't remove first and last
            else if (removeFirst && removeLast)
                return null;

            // Remove each optional remove word (enabled by bit 2 + word #)
            for (int j = 0; j < OptionalRemoveWords.Length; j++)
                if (((int)options & (int)Math.Pow(2, j)) > 0)
                    simplifiedName = RemoveWord(disableRemAfter, simplifiedName, removeFileWords, OptionalRemoveWords[j]);

            // Remove always remove words
            foreach (RemoveFileWord remWord in AlwaysRemoveWords)
                simplifiedName = RemoveWord(disableRemAfter, simplifiedName, removeFileWords, remWord);

            // Word splitting
            if (wordSplitEn)
            {
                // Seperate input by whitespace
                string[] words = simplifiedName.Split(' ');

                // Build new string with words split up
                simplifiedName = string.Empty;
                foreach (string word in words)
                {
                    string newWord;
                    WordHelper.TrySplitWords(word, out newWord);
                    simplifiedName += newWord + " ";
                }
            }

            // Trim
            simplifiedName = simplifiedName.Trim().Replace("  ", " ");

            return new SimplifyStringResults(simplifiedName, removeFileWords);
        }

        /// <summary>
        /// Removes word from string with extra options.
        /// </summary>
        /// <param name="disableRemAfter">Disable removing of words that follow defined words to remove</param>
        /// <param name="input">Input string to remove words from</param>
        /// <param name="removeFileWords">List of removed words from string to add to when removing more</param>
        /// <param name="remWord">Word to be removed</param>
        /// <returns>string with word removed</returns>
        private static string RemoveWord(bool disableRemAfter, string input, Dictionary<FileWordType, List<string>> removeFileWords, RemoveFileWord remWord)
        {
            // Store remove after state
            bool remEverything = remWord.RemoveEverythingAfter;
            bool remAfterWord = remWord.RemoveFollowingEndWord;

            // Disable remove after if needed
            if (disableRemAfter)
            {
                remWord.RemoveEverythingAfter = false;
                remWord.RemoveFollowingEndWord = false;
            }

            // Perform word remove
            remWord.RemoveWord(ref input, removeFileWords);

            // Restore remove after state
            if (disableRemAfter)
            {
                remWord.RemoveEverythingAfter = remEverything;
                remWord.RemoveFollowingEndWord = remAfterWord;
            }

            // Return results with removed word
            return input;
        }

        /// <summary>
        /// Simplifies a file name string for easier matching against database results.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The simplified file name string</returns>
        public static string BasicSimplify(string fileName, bool removeYear)
        {
            // All lowercase
            string simplifiedName = fileName.ToLower();

            // Remove unneeded characters: ',!,?,(,),:
            simplifiedName = Regex.Replace(simplifiedName, @"[']+", "");
            simplifiedName = Regex.Replace(simplifiedName, @"[!\?\u0028\u0029\:\]\[]+", " ");

            // Replace seperators with spaces
            simplifiedName = Regex.Replace(simplifiedName, @"\W+|_", " ");

            if (removeYear)
            {
                RemoveFileWord remYear = new RemoveFileWord(Separator.Nonnumeric, Separator.Nonnumeric, @"(?:19|20)\d{2}", FileWordType.Year, false, false);
                remYear.RemoveWord(ref simplifiedName, new Dictionary<FileWordType, List<string>>());
            }

            return simplifiedName;
        }

        /// <summary>
        /// Simplifies a file name for easier processing.
        /// </summary>
        /// <param name="fileName">File name to simplify</param>
        /// <param name="removeYear">Whether to remove year during simplification</param>
        /// <param name="removeWhitespace">Whether to remove extra whitespace</param>
        /// <returns>Simplified file name</returns>
        public static string SimplifyFileName(string fileName, bool removeYear, bool removeWhitespace)
        {
            OptionalSimplifyRemoves options = removeYear ? OptionalSimplifyRemoves.Year : OptionalSimplifyRemoves.None;
            return BuildSimplifyResults(fileName, false, false, options, true, false, removeWhitespace).SimplifiedString;
        }

        /// <summary>
        /// Simplifies a file name for easier processing (with default options)
        /// </summary>
        /// <param name="fileName">File name to simplify</param>
        /// <returns>Simplified file name</returns>
        public static string SimplifyFileName(string fileName)
        {
            return SimplifyFileName(fileName, false, true);
        }

        #endregion
    }
}

