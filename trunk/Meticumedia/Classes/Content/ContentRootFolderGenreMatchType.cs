using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum ContentRootFolderGenreMatchMissType
    {
        [Description("Use Default Folder")]
        Default,

        [Description("Prompt to Select")]
        Prompt,

        [Description("Automatically Create")]
        AutoCreate
    }
}
