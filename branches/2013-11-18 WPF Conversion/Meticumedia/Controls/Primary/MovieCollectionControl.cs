using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Controls
{
    class MovieCollectionControl : ContentCollectionControl
    {
        public MovieCollectionControl()
            : base(Classes.ContentType.Movie)
        {
        }
    }
}
