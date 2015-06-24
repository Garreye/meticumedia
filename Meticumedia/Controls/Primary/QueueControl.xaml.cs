using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for QueueControl.xaml
    /// </summary>
    public partial class QueueControl : UserControl
    {        
        public QueueControl()
        {
            InitializeComponent();

            OrgItemQueueableViewModel.ItemsToQueue += OrgItemQueueableViewModel_ItemsToQueue;
        }

        void OrgItemQueueableViewModel_ItemsToQueue(object sender, Classes.ItemsToQueueArgs e)
        {
            TabItem tab = LogicalTreeHelper.GetParent(this) as TabItem;
            tab.Focus();
        }
    }
}
