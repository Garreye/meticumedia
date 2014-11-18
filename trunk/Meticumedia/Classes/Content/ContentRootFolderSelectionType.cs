using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum ContentRootFolderSelectionType
    {
        [Description("Use Default")]
        Default,

        [Description("Child Genre Folders")]
        GenreChild,

        [Description("Rules Based")]
        Rules
    }
}
