using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
    /// <summary>
    /// Custom User Control which presents an image with several quick load buttons and either
    /// an image Load or Save button. Use the Buttons property to set the panel as a Load or Save panel.
    /// 
    /// Renders image over a checkerboard background to make transparency obvious.
    /// 
    /// Author: Dennis Lang  2010     
    /// http://home.comcast.net/~lang.dennis/
    /// </summary>
    public partial class ImagePanel : UserControl
    {
        public ImagePanel()
        {
            InitializeComponent();
        }

        public enum ButtonSet { Load, Save };
        public ButtonSet Buttons
        {
            set 
            {
                switch (value)
                {
                    case ButtonSet.Load:
                        this.button1.Visible = this.button2.Visible = this.button3.Visible = false;
                        this.button4.Visible = true;
                        this.button4.Text = "Laden...";
                        toolTip.SetToolTip(this.button4, "Bild laden...");
                        break;
                    case ButtonSet.Save:
                        this.button1.Visible = this.button2.Visible = this.button3.Visible = false;
                        this.button4.Visible = false;
                        this.button4.Text = "Übernehmen...";
                        toolTip.SetToolTip(this.button4, "Bild übernehmen...");
                        break;
                }
            }
        }

        //string Filename = Application.LocalUserAppDataPath + "\\background.png";
        string Filename = Application.StartupPath + "\\background.png";

        Image m_image;
        public class EventClick : EventArgs
        {
            string button;
            public EventClick(string b)
            {
                button = b;
            }
            public string Button
            {
                get { return button; }
                set { button = value; }
            }
        };

        /// <summary>
        /// Render image over checkerboard.
        /// </summary>
        private Image ImageCheckerboard(Image image)
        {
            Image bgImage = Image.FromFile(Filename);
            if (image != null)
            {
                Graphics g = Graphics.FromImage(bgImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                Rectangle rect = new Rectangle(Point.Empty, bgImage.Size);
                g.DrawImage(image, rect);
            }
            return bgImage;
        }

        public Image Image
        {
            get { return m_image; }
            set 
            {
                m_image = value;
                panel.BackgroundImage = ImageCheckerboard(m_image); 
            }
        }

        public string Label
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public void SetButtonImage(int button, Image image)
        {
            switch (button)
            {
                case 1: button1.BackgroundImage = image; break;
                case 2: button2.BackgroundImage = image; break;
                case 3: button3.BackgroundImage = image; break;
            }
        }

        public EventHandler buttonClick;

        private void button_Click(object sender, EventArgs e)
        {
            if (buttonClick != null)
                buttonClick(this, new EventClick((string)((Button)sender).Tag));
        }

        public EventHandler loadClick;
        private void loadBtn_Click(object sender, EventArgs e)
        {
            if (loadClick != null)
                loadClick(this, new EventArgs());
        }
    }
}
