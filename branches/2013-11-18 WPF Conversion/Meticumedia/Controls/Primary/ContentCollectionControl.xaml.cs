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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ContentControl.xaml
    /// </summary>
    public partial class ContentCollectionControl : UserControl
    {
        #region Constructor

        public ContentCollectionControl(ContentType contentType)
        {
            InitializeComponent();
            this.DataContext = new ContentCollectionControlViewModel(contentType);
        }

        #endregion
    }
}
