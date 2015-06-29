using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Types of options that can be applied to case of portion
    /// </summary>
    public enum CaseOptionType
    {
        [Description("None")]
        None,

        [Description("Lower Case")]
        LowerCase,

        [Description("Upper Case")]
        UpperCase
    }
}
