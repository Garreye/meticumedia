using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Collection of OrgItems; created this way to allow XAML binding to CollectionViewSource
    /// </summary>
    public class OrgItemCollection : ObservableCollection<OrgItem>
    {

    }
}
