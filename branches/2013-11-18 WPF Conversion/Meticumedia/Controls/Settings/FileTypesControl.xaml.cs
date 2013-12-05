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
        public FileTypesControl()
        {
            InitializeComponent();            
        }

        #region Save/Load

        private FileTypesGroup videoFileTypes = new FileTypesGroup();
        private FileTypesGroup deleteFileTypes = new FileTypesGroup();
        private FileTypesGroup ignoreFileTypes = new FileTypesGroup();

        public void LoadSettings()
        {
            foreach (string str in Settings.VideoFileTypes.Types)
                this.videoFileTypes.Types.Add(str);
            foreach (string str in Settings.DeleteFileTypes.Types)
                this.deleteFileTypes.Types.Add(str);
            foreach (string str in Settings.IgnoreFileTypes.Types)
                this.ignoreFileTypes.Types.Add(str);

            ctntVideoFiles.Content = videoFileTypes;
            ctntDeleteFiles.Content = deleteFileTypes;
            ctntIgnoreFiles.Content = ignoreFileTypes;
        }

        public void SaveSettings()
        {
            Settings.VideoFileTypes.Types.Clear();
            foreach (string str in videoFileTypes.Types)
                Settings.VideoFileTypes.Types.Add(str);
            Settings.DeleteFileTypes.Types.Clear();
            foreach (string str in deleteFileTypes.Types)
                Settings.DeleteFileTypes.Types.Add(str);
            Settings.IgnoreFileTypes.Types.Clear();
            foreach (string str in ignoreFileTypes.Types)
                Settings.IgnoreFileTypes.Types.Add(str);

        }

        #endregion

        #region Event Handlers

        private void AddTypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            FileTypesGroup collection = (FileTypesGroup)button.DataContext;

            if (collection.Types.Contains(collection.NextItem))
            {
                MessageBox.Show("Extension already added.");
                return;
            }

            collection.Types.Add(collection.NextItem);
            collection.NextItem = string.Empty;
        }

        private void RemTypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            FileTypesGroup collection = (FileTypesGroup)button.DataContext;

            // TODO: remove selected item
        }

        #endregion
    }
}
