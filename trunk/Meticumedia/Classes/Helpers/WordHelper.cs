// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Helper class for attempting to split string into words.
    /// </summary>
    public static class WordHelper
    {
        /// <summary>
        /// List of all words, loaded from file containing all words in dictionary
        /// </summary>
        private static List<string> words;
        
        /// <summary>
        /// Initialize the helper. Load dictionary words into memory.
        /// </summary>
        public static void Initialize()
        {
            words = File.ReadAllLines(Path.Combine(Application.StartupPath, "wordsEn.txt")).ToList();
        }

        /// <summary>
        /// Get words from dictionary of a given length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<string> GetWords(int length)
        {
            return words.FindAll(delegate(string s) { return s.Length == length; });
        }

        /// <summary>
        /// Check is a word is in the dictionary
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            List<string> matches = words.FindAll(delegate(string s) { return s.Equals(word.ToLower()); });
            return matches.Count > 0;
        }

        /// <summary>
        /// Attemps to split a string with no spaces into string with spaces between words
        /// </summary>
        /// <param name="text">string with no spaces</param>
        /// <param name="splitText">resulting string with added spaces between words</param>
        /// <returns>whether split was sucessful</returns>
        public static bool TrySplitWords(string text, out string splitText)
        {
            // Create list of words
            List<string> textWords = new List<string>();
            
            // Create local variables
            int currentPos = 0;
            int i;
            bool wordFound = false;

            // Movie through un-spaced string and try to make words 
            do
            {
                // Go through each char in string
                wordFound = false;
                for (i = text.Length; i > currentPos; i--)
                {
                    // Try to create word starting with whole string from current position to end, removing a letter on each iteration of loop
                    string testWord = text.Substring(currentPos, i - currentPos);
                    if (IsWord(testWord))
                    {
                        // Word match found, add to word list
                        textWords.Add(testWord);
                        wordFound = true;
                        break;
                    }
                }

                // If no match found it could be due to incorrect match of previous word (e.g. "brickstore" -> "bricks" was matches, now can't match "tore")
                if (!wordFound)
                    while (textWords.Count > 0)
                    {
                        // Continuously shorten previous match and see if a new word is found
                        string lastWord = textWords[textWords.Count - 1];
                        for (int len = lastWord.Length - 1; len > 0; len--)
                        {
                            string newWord = lastWord.Substring(0, len);

                            // Update last word if substring is a word
                            if (IsWord(newWord))
                            {
                                textWords[textWords.Count - 1] = newWord;
                                currentPos -= lastWord.Length - newWord.Length;
                                wordFound = true;
                                break;
                            }

                        }

                        if (!wordFound)
                        {
                            textWords.Remove(lastWord);
                            currentPos -= lastWord.Length;
                        }
                        else
                            break;
                    }

                // Set position to how far we made it into string on matching above
                else
                    currentPos = i;


            } while (currentPos < text.Length && wordFound);

            // Build string with spaces from words list, if splitting was sucessful
            if (wordFound)
            {
                splitText = string.Empty;
                foreach (string word in textWords)
                    splitText += word + " ";
                splitText.Trim();
                return true;
            }

            // Couldn't sucessfully split string into words
            splitText = text;
            return false;
        }

    }
}
