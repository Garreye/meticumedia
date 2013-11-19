// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Helper class for attempting to split string with no whitespace into words.
    /// </summary>
    public static class WordHelper
    {
        /// <summary>
        /// List of all words, loaded from file containing all words in dictionary
        /// </summary>
        private static List<string> words;

        /// <summary>
        /// Dictionary of all words, index by length (number of characters)
        /// </summary>
        private static Dictionary<int, List<string>> lengthWords;
        
        /// <summary>
        /// Initialize the helper. Load dictionary words into memory.
        /// </summary>
        public static void Initialize()
        {
            // Get all words
            words = File.ReadAllLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wordsEn.txt")).ToList();

            // Create dictionary of word list based on length
            lengthWords = new Dictionary<int, List<string>>();
            foreach (string word in words)
            {
                int length = word.Length;
                if (!lengthWords.ContainsKey(length))
                    lengthWords.Add(length, new List<string>());
                lengthWords[length].Add(word);
            }
        }

        /// <summary>
        /// Get words from dictionary of a given length
        /// </summary>
        /// <param name="length">Length of words to get</param>
        /// <returns>List of word with desired length</returns>
        public static List<string> GetWords(int length)
        {
            return lengthWords[length];
        }

        /// <summary>
        /// Check if a string matches a word in the dictionary
        /// </summary>
        /// <param name="input">String to match to dictionary word</param>
        /// <returns>Whether input string is a word in dictionary</returns>
        public static bool IsWord(string input)
        {
            // Don't look for empty words
            if (string.IsNullOrWhiteSpace(input) || !lengthWords.ContainsKey(input.Length))
                return false;

            // Compare input to all word of same length in dictionary
            foreach (string w in lengthWords[input.Length])
                if (input.ToLower() == w.ToLower())
                    return true;
            
            // No match, not a word!
            return false;
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
