using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum ContentRootFolderMatchRuleType
    {
        [Description("Equals")]
        Equals,        

        [Description("Contains")]
        Contains,

        [Description("Starts With")]
        StartsWith,

        [Description("Ends With")]
        EndsWith,

        [Description("Regular Expression")]
        RegularExpression,

        [Description("Between")]
        Between
    }
}
