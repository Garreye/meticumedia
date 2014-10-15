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
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for FileTypeControl.xaml
    /// </summary>
    public partial class FileTypesControl : UserControl
    {
        public FileTypesControlViewModel ViewModel
        {
            get
            {
                return this.DataContext as FileTypesControlViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
        
        public FileTypesControl()
        {
            InitializeComponent();            
        }
    }
}
