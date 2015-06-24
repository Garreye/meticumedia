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
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Collections.ObjectModel;
using Meticumedia.Classes;
using System.Collections.Specialized;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ScanFoldersControl.xaml
    /// </summary>
    public partial class ScanFoldersControl : UserControl
    {
        #region Constructor

        public ScanFoldersControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Variables


        #endregion

        #region Save/Load 


        public void SaveSettings()
        {           
            //Meticumedia.Classes.Settings.ScanDirectories.Clear();
            //foreach (OrgFolder fldr in folders)
            //   Meticumedia.Classes.Settings.ScanDirectories.Add(fldr);
        }

        #endregion

        #region Event Handlers

        //private void btnAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    // Open folder browser
        //    VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();

        //    // Add folder if valid folder selected
        //    if ((bool)folderSel.ShowDialog() && Directory.Exists(folderSel.SelectedPath))
        //        folders.Add(new OrgFolder(folderSel.SelectedPath));
        //}

        //private void btnRemove_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lbScanFolders.SelectedItem != null)
        //        folders.Remove((OrgFolder)lbScanFolders.SelectedItem);
        //}

        //private void btnClear_Click(object sender, RoutedEventArgs e)
        //{
        //    folders.Clear();
        //}

        //private void btnFolderSel_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    var dataContext = button.DataContext;
        //    ListBoxItem clickedListBoxItem = lbScanFolders.ItemContainerGenerator.ContainerFromItem(dataContext) as ListBoxItem;
        //    OrgFolder folder = (OrgFolder)clickedListBoxItem.Content;
        //    clickedListBoxItem.IsSelected = true;

        //    VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();
        //    folderSel.SelectedPath = folder.FolderPath;

        //    if ((bool)folderSel.ShowDialog() && Directory.Exists(folderSel.SelectedPath))
        //        folder.FolderPath = folderSel.SelectedPath;

        //}

        #endregion
    }
}
