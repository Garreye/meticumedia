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
    /// Interaction logic for ContentEditorWindow.xaml
    /// </summary>
    public partial class ContentEditorWindow : Window
    {

        public Content Results
        {
            get
            {
                ContentEditorWindowViewModel vm = this.DataContext as ContentEditorWindowViewModel;
                if (vm.ResultsOk)
                    return vm.Content;
                else
                    return null;
            }
        }
        
        public ContentEditorWindow(Content content)
        {
            InitializeComponent();

            ContentEditorWindowViewModel vm = new ContentEditorWindowViewModel(content);
            vm.ResultsSet += vm_ResultsSet;
            this.DataContext = vm;
        }

        void vm_ResultsSet(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
