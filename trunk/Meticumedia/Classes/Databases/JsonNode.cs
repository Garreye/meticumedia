// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Json HTTP response node. Recursively parse node and child nodes.
    /// </summary>
    public class JsonNode
        {
            #region Properties

            /// <summary>
            /// Name of node
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Value of node (all text after name)
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Child nodes contained in value.
            /// </summary>
            public List<JsonNode> ChildNodes { get; private set; }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor with known name and value.
            /// </summary>
            /// <param name="name">Node's name</param>
            /// <param name="value">Node's value</param>
            public JsonNode(string name, string value)
            {
                // Store properties
                this.Name = name;
                this.Value = value;

                // Parse value for child nodes
                BuildChildren();
            }

            #endregion

            #region Child Parsing

            /// <summary>
            /// Types of brackets that seperate out child nodes
            /// </summary>
            private enum BracketType { None, Sqiggly, Square }

            /// <summary>
            /// Parse value of node to build child nodes. This method will be called recursively as each
            /// child is created.
            /// Json generally has multiple nodes of the format name : value
            /// that are seperated by commas. In some cases (typically root nodes) there is no name, there
            /// is just a list of values separated by commas.
            /// The value portion can be made up of child nodes, which are parsed in this method.
            /// The name or value can be inside quotations, and the value can be inside square or curly brackets.
            /// </summary>
            private void BuildChildren()
            {
                // Init child nodes
                ChildNodes = new List<JsonNode>();
                
                // Init name/value for child
                string name = string.Empty;
                string value = string.Empty;

                // Init bracket/quotation tracking
                int bracketCnt = 0;
                BracketType bracketType = BracketType.None;
                bool insideQuotations = false;
                bool onValue = false;

                // Go through each character in value
                for (int i = 0; i < this.Value.Length; i++)
                {
                    // Grab current character
                    char c = this.Value[i];

                    // Square bracket checking - perform if not inside any brackets/quotations or if already inside square bracket and looking for end
                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Square))
                    {
                        // Opening bracket
                        if (c == '[')
                        {
                            // Set bracket type and start counting
                            bracketType = BracketType.Square;
                            bracketCnt++;

                            // If first bracket then it's the start of child node's value
                            if (bracketCnt == 1)
                            {
                                onValue = true;
                                continue;
                            }
                        }
                        // Closing bracket
                        else if (c == ']')
                        {
                            // Decrement bracket count
                            bracketCnt--;

                            // If final bracket it's the end of child value. 
                            if (bracketCnt == 0)
                            {
                                // Create child (will clear name, value, onValue variables) and clear bracket type.
                                CreateChild(ref name, ref value, ref onValue);
                                bracketType = BracketType.None;
                                continue;
                            }
                        }
                    }
                    // Squiggly bracket checking - perform if not inside any brackets/quotations or already inside sqiggly bracket and looking for end
                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Sqiggly))
                    {
                        // Opening bracket
                        if (c == '{')
                        {
                            // Set bracket type and start counting
                            bracketType = BracketType.Sqiggly;
                            bracketCnt++;

                            // If first bracket then it's the start of child node's value
                            if (bracketCnt == 1)
                            {
                                onValue = true;
                                continue;
                            }
                        }
                        // Closing bracket
                        else if (c == '}')
                        {
                            bracketCnt--;
                            if (bracketCnt == 0)
                            {
                                // Create child (will clear name, value, valuesSet variables) and clear bracket type.
                                CreateChild(ref name, ref value, ref onValue);
                                bracketType = BracketType.None;
                                continue;
                            }
                        }
                    }
                    // If not inside brackets, check for quotations
                    if (bracketType == BracketType.None)
                    {
                        if (c == '\"')
                        {
                            insideQuotations = !insideQuotations;
                            continue;
                        }
                    }

                    // Comma indicates end of child (if outside of quotations/brackets)
                    if (c == ',' && !insideQuotations && bracketCnt == 0)
                        CreateChild(ref name, ref value, ref onValue);
                    // Colong s separation between name and value
                    else if (c == ':' && !insideQuotations && !onValue)
                        onValue = true;
                    // Add character to name, if not on value yet
                    else if (!onValue)
                        name += c;
                    // Add character to value
                    else
                        value += c;
                }

                CreateChild(ref name, ref value, ref onValue);
            }

            /// <summary>
            /// Creates child node from name and value string and clears build variables.
            /// </summary>
            /// <param name="name">Name for child node</param>
            /// <param name="value">Value for child node</param>
            /// <param name="onValue">Whether child build is on value portion</param>
            private void CreateChild(ref string name, ref string value, ref bool onValue)
            {
                // Check that value is valid
                if (!string.IsNullOrEmpty(value))
                {
                    // Add child
                    ChildNodes.Add(new JsonNode(name, value));
                }

                // Clear child build variables
                name = string.Empty;
                value = string.Empty;
                onValue = false;
            }

            #endregion
        }
}
