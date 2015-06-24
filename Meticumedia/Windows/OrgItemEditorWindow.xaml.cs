using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Meticumedia.Classes;

namespace Meticumedia.Windows
{
    /// <summary>
    /// Interaction logic for OrgItemEditorWindow.xaml
    /// </summary>
    public partial class OrgItemEditorWindow : Window
    {
        public OrgItem Results
        {
            get
            {
                if (viewModel.ResultsOk)
                    return viewModel.Item;
                else
                    return null;
            }
        }
        
        private OrgItemEditorWindowViewModel viewModel;
        
        public OrgItemEditorWindow(OrgItem item)
        {
            InitializeComponent();
            viewModel = new OrgItemEditorWindowViewModel(item);
            this.DataContext = viewModel;
            viewModel.ResultsSet += viewModel_ResultsSet;
        }

        void viewModel_ResultsSet(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
