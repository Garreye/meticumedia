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
    /// Paramater for HTTP get.
    /// </summary>
    public class HttpGetParameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public HttpGetParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
