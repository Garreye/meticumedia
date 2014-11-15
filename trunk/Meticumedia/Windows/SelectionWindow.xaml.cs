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
using System.Windows.Shapes;

namespace Meticumedia.Windows
{   
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        #region Properties

        /// <summary>
        /// Results from selection. Empty string when cancelled.
        /// </summary>
        public string Results { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for generic selection window.
        /// </summary>
        /// <param name="title">Title for form</param>
        /// <param name="options">List of options for selection</param>
        public SelectionWindow(string title, string[] options)
        {
            InitializeComponent();

            // Setup display
            this.Title = title;

            foreach (string option in options)
                lbOptions.Items.Add(option);

            // Clear results
            this.Results = string.Empty;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Listbox selection enables OK button
        /// </summary>
        private void lbOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOk.IsEnabled = lbOptions.SelectedItems.Count > 0;
        }

        /// <summary>
        /// Double-click on an item in listbox selects it as the
        /// results and closes the form.
        /// </summary>
        private void lbOptions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbOptions.SelectedItems.Count > 0)
            {
                this.Results = lbOptions.SelectedItem.ToString();
                this.Close();
            }
        }

        /// <summary>
        /// OK button sets current selection to results and closes form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Results = lbOptions.SelectedItem.ToString();
            this.Close();
        }

        /// <summary>
        /// Cancel closes form. Results will be empty.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
