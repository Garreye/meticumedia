using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class DebugNotificationArgs
    {
        public string Notification { get; set; }

        public DebugNotificationArgs(string notification)
        {
            this.Notification = notification;
        }
    }
}
