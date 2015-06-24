// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Column for a listview of OrgItem instances
    /// </summary>
    public class OrgItemColumn
    {
        /// <summary>
        /// Type of data contained in the column
        /// </summary>
        public OrgColumnType Type { get; set; }

        /// <summary>
        /// The header tied to the listview
        /// </summary>
        public ColumnHeader Header { get; set; }

        /// <summary>
        /// Order of the column
        /// </summary>
        public int Order = -1;

        /// <summary>
        /// Build the column based on the type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="order"></param>
        public OrgItemColumn(OrgColumnType type, int order)
        {
            // Set properties
            this.Type = type; 
            this.Order = order;

            // Build header from type
            Header = new ColumnHeader();
            Header.Text = type.ToString().Replace('_', ' ');
            
            // Set width based on type
            switch (type)
            {
                case OrgColumnType.DateTime:
                    Header.Width = 150;
                    break;
                case OrgColumnType.Status:
                    Header.Width = 75;
                    break;
                case OrgColumnType.Show:
                    Header.Width = 200;
                    break;
                case OrgColumnType.Season:
                    Header.Width = 50;
                    break;
                case OrgColumnType.Episode:
                    Header.Width = 50;
                    break;
                case OrgColumnType.Movie:
                    Header.Width = 200;
                    break;
                case OrgColumnType.Source_Folder:
                    Header.Width = 300;
                    break;
                case OrgColumnType.Source_File:
                    Header.Width = 250;
                    break;
                case OrgColumnType.Category:
                    Header.Width = 100;
                    break;
                case OrgColumnType.Action:
                    Header.Width = 100;
                    break;
                case OrgColumnType.Destination_Folder:
                    Header.Width = 300;
                    break;
                case OrgColumnType.Destination_File:
                    Header.Width = 250;
                    break;
                case OrgColumnType.Progress:
                    Header.Width = 75;
                    break;
                case OrgColumnType.Number:
                    Header.Width = 50;
                    break;
                default:
                    throw new Exception("Unknown column type");
            }
        }
    }

}
