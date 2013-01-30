// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// Progress bar with text added to progress
/// </summary>
public class TextProgressBar : ProgressBar
{
    /// <summary>
    /// Message to be displayed with progress percent
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Brush for drawing progress string
    /// </summary>
    private SolidBrush brush = null;

    /// <summary>
    /// Default constructor
    /// </summary>
    public TextProgressBar()
    {
        // Set user paint - allows custom progress string to be drawn
        this.SetStyle(ControlStyles.UserPaint, true);

        // Set double buffering to reduce jittering on updating
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    /// <summary>
    /// Progress message is drawn on paint event
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        // Create brush if needed
        if (brush == null || brush.Color != this.ForeColor)
            brush = new SolidBrush(this.ForeColor);

        // Draw progress
        Rectangle rec = new Rectangle(0, 0, this.Width, this.Height);
        if (ProgressBarRenderer.IsSupported)
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);
        rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
        rec.Height = rec.Height - 4;
        e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);

        // Add progress percentage to bar
        string progress = this.Message + " [" + this.Value + "%]";
        Font font = new Font("Arial", (float)8.25, FontStyle.Regular);
        int msgWidth = (int)e.Graphics.MeasureString(progress, font).Width;
        e.Graphics.DrawString(progress, font, Brushes.Black, new PointF((this.Width - msgWidth)/2, this.Height / 2 - 7));

    }
}
