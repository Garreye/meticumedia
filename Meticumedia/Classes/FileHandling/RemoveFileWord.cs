// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class for helping remove superfluous words from a string.
    /// </summary>
    public class RemoveFileWord
    {
        #region Properties

        /// <summary>
        /// Separator type required before word
        /// </summary>
        public Separator SeparatorBefore { get; set; }

        /// <summary>
        /// Separator type required after word
        /// </summary>
        public Separator SeparatorAfter { get; set; }

        /// <summary>
        /// Type of word that is being removed
        /// </summary>
        public FileWordType Type { get; set; }

        /// <summary>
        /// The word we want to remove from a string
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Whether to remove the word following if it is the last in the match string
        /// </summary>
        public bool RemoveFollowingEndWord { get; set; }

        /// <summary>
        /// Whether to remove all characters after word if matched
        /// </summary>
        public bool RemoveEverythingAfter { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RemoveFileWord()
        {
            this.SeparatorBefore = Separator.None;
            this.SeparatorAfter = Separator.None;
            this.Type = FileWordType.None;
            this.Word = string.Empty;
            this.RemoveFollowingEndWord = true;
        }

        /// <summary>
        /// Constructor with default RemoveAfterWords properties.
        /// </summary>
        /// <param name="SeperatorBefore">Separator type required before word</param>
        /// <param name="SeperatorAfter">Separator type required after word</param>
        /// <param name="Word">The word to remove from string</param>
        /// <param name="type">type of the word to remove</param>
        public RemoveFileWord(Separator seperatorBefore, Separator seperatorAfter, string word, FileWordType type)
        {
            this.SeparatorBefore = seperatorBefore;
            this.SeparatorAfter = seperatorAfter;
            this.Word = word;
            this.Type = type;
            this.RemoveFollowingEndWord = true;
            this.RemoveEverythingAfter = false;
        }

        /// <summary>
        /// Constructor with parameters for all properties
        /// </summary>
        /// <param name="seperatorBefore"></param>
        /// <param name="seperatorAfter"></param>
        /// <param name="word">The word to remove from string</param>
        /// <param name="type">type of the word to remove</param>
        /// <param name="removeAfterWords">Whether to also remove the word following if it is the last in the string</param>
        /// <param name="removeEverythingAfter">Whether to remove all characters after match</param>
        public RemoveFileWord(Separator seperatorBefore, Separator seperatorAfter, string word, FileWordType type, bool removeAfterWords, bool removeEverythingAfter) 
            : this(seperatorBefore, seperatorAfter, word, type)
        {
            this.RemoveFollowingEndWord = removeAfterWords;
            this.RemoveEverythingAfter = removeEverythingAfter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the word from a string and adds it to dictionary of all removed words indexed by type
        /// </summary>
        /// <param name="text">The text to remove word from</param>
        /// <param name="removeFileWords">Dictionary of all removed words from text to add to if word is removed</param>
        /// <returns>Text with word removed if found</returns>
        public bool RemoveWord(ref string text, Dictionary<FileWordType, List<string>> removeFileWords)
        {
            // Build regular expression for remove match
            string regEx = BuildRegExString();

            // Look for match, if found remove it
            Match match = Regex.Match(text, regEx, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // Remove each match
                for (int g = match.Groups.Count - 1; g > 0; g--)
                {
                    // Add removed word to dictionary of removed word based on type
                    if(g == 1 && this.Type != FileWordType.None)
                    {
                        // Check that key for word type is in dictionary, if not add it
                        if (!removeFileWords.ContainsKey(this.Type))
                            removeFileWords.Add(this.Type, new List<string>());

                        // Add the word to ditcionary (if not already in there)
                        string removeText = text.Substring(match.Groups[g].Index, match.Groups[g].Length);
                        if(!removeFileWords[this.Type].Contains(removeText))
                            removeFileWords[this.Type].Add(removeText);                       
                    }
                    
                    // Remove the word from the text
                    try
                    {
                        text = text.Remove(match.Groups[g].Index, match.Groups[g].Length);
                    }
                    catch
                    {
                    }
                }
                
                // Remove double spacing
                while (text.Contains("  "))
                    text = text.Replace("  ", " ");

                // Run this method recursively to remove all instances of the word!
                RemoveWord(ref text, removeFileWords);
                return true;
            }

             // No match, return unmodified text
            return false;
        }

        /// <summary>
        /// Build regular expression string for finding the word with
        /// required separators.
        /// </summary>
        /// <returns>The regular expression search string</returns>
        private string BuildRegExString()
        {
            // Initialize value
            string re = string.Empty;

            // Add beginning separator
            switch (this.SeparatorBefore)
            {
                case Separator.None:
                    break;
                case Separator.Nonnumeric:
                    re += "(?:^|\\D|\\W)";
                    break;
                case Separator.Whitespace:
                    re += "(?:^|\\W)";
                    break;
                default:
                    throw new Exception("Unknown seperator type");
            }

            // Add word
            re += "(" + this.Word + ")";

            // Add ending separator
            switch (this.SeparatorAfter)
            {
                case Separator.None:
                    if (RemoveEverythingAfter)
                        re += ".*$";
                    else if (RemoveFollowingEndWord)
                        re += "(?:(\\W+\\w+\\W*)$|$)?";
                    else
                        re += "$?";
                    break;
                case Separator.Nonnumeric:
                    if(RemoveEverythingAfter)
                        re += "(?:((\\D.*)|$)|$)";
                    else if (RemoveFollowingEndWord)
                        re += "(?:(\\W+\\w+\\W*)$|$|\\D|\\W)";
                    else
                        re += "(?:$|\\D|\\W)";
                    break;
                case Separator.Whitespace:
                    if (RemoveEverythingAfter)
                        re += "([^\\w].*)$";
                    else if (RemoveFollowingEndWord)
                        re += "(?:(\\W+\\w+\\W*)$|$|\\W)";
                    else
                        re += "(?:$|\\W)";
                    break;
                default:
                    throw new Exception("Unknown seperator type");
            }

            // Return regular expression string
            return re;
        }

        #endregion
    }
}
