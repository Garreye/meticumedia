using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum ContentRootFolderSelectionCriteria
    {
        [Description("Always Use Default")]
        Default,

        [Description("Sub-Folders as Genre Folders")]
        GenreChild,

        [Description("Rules List")]
        Rules
    }
}
