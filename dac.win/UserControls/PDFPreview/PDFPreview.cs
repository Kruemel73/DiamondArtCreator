using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PdfiumViewer;

namespace Ravlyk.SAE5.WinForms.Dialogs 
{ 
    public partial class PDFPreview : Form
    {
        private PdfDocument pdfDocument;
        private OpenFileDialog ofd;
        private string pdfFileName, color, outDirector, outDirFileName, fileNameOnly;
        private string imageFormat, extension;
        private int startPage, endPage, Dpi;
        private ImageFormat imgFormat;
        private Image img;
        private Size thumbSize;
        private PointF pt;

        private Rectangle thumBound;
        private Graphics g;

        private int splitterDistance;

        public int Cpn { get; set; }
        public int Fpn { get; set; }
        public int Lpn { get; set; }

        public PDFPreview(string fileName)
        {
            InitializeComponent();

            //Text = Program.NAME;
            listView1.OwnerDraw = true;
            listView1.MultiSelect = false;
            listView1.GridLines = true;
            listView1.AutoArrange = true;
            listView1.AllowDrop = false;
            listView1.View = View.Tile;

            splitterDistance = 300;
            splitContainer1.Width = 1;
            splitContainer1.SplitterWidth = 8;
            thumbSize = new Size(202, 200);

            pdfRenderer1.MouseWheel += new MouseEventHandler(pdfRenderer1_MouseWheel);

            pdfFileName = fileName;

            try
            {
                pdfDocument = PdfDocument.Load(pdfFileName);
            }
            catch
            {
                MessageBox.Show("Unsupported file format!\nThe file must be of .pdf type.", "Error");
                return;
            }
            pdfRenderer1.Load(pdfDocument);

            tabControl1.SelectedTab.Name = "tabPage1";
            splitContainer1.SplitterDistance = 320;
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);

            DisableToolBar();
            string fname = Path.GetFileName(pdfFileName);

            toolStripButton1_Click(this, EventArgs.Empty);//FitHeight

            Cpn = 0;
            listView1.View = View.Tile;
            listView1.TileSize = new Size(thumbSize.Width, thumbSize.Height);
            listView1.Clear();
            for (int i = 0; i < pdfDocument.PageCount; ++i)
            {
                listView1.Items.Add(i.ToString());
            }
            toolStripTextBoxSite.Text = "1";

            listView1.Items[0].Selected = true;
            listView1.Items[0].Focused = true;
            Fpn = 0;
            Lpn = pdfDocument.PageCount;
            toolStripTextBoxSite.Text = Cpn.ToString();
            toolStripLabel1.Text = " / " + Lpn.ToString();
            pdfRenderer1.Focus();
        }

        private void pdfRenderer1_LinkClick(object sender, LinkClickEventArgs e)
        {
            pdfRenderer1.DisplayRectangleChanged += pdfRenderer1_DisplayRectangleChanged;
            try
            {
                pdfRenderer1.Page = (int)(e.Link.TargetPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wrong Link Target Page - \n" + ex.ToString());
            }
            toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();

            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
        }

        private void pdfRenderer1_CursorChanged(object sender, EventArgs e)
        {
            pdfRenderer1.Cursor = Cursors.Default;
            pdfRenderer1.SetDisplayRectLocation(new Point(0, 0));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Checked = true;
            //fit height
            if (toolStripButton1.Checked == true)
            {
                toolStripButton2.Checked = false;
            }
            pdfRenderer1.Page = Cpn;
            pdfRenderer1.Zoom = 1;
            pdfRenderer1.ZoomMode = PdfViewerZoomMode.FitHeight;
            pdfRenderer1.Page = Cpn;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripButton2.Checked = true;
            //fit width
            if (toolStripButton2.Checked == true)
            {
                toolStripButton1.Checked = false;
            }
            pdfRenderer1.Page = Cpn;
            pdfRenderer1.Zoom = 1;
            pdfRenderer1.ZoomMode = PdfViewerZoomMode.FitWidth;
            pdfRenderer1.Page = Cpn;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //go to last page
            pdfRenderer1.Page = pdfDocument.PageCount;
            Cpn = pdfRenderer1.Page;
            toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //go to next page 
            pdfRenderer1.Page++;
            Cpn = pdfRenderer1.Page;
            toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //go to previous page
            pdfRenderer1.Page--;
            Cpn = pdfRenderer1.Page;
            toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            //go to first page
            pdfRenderer1.Page = 0;
            Cpn = pdfRenderer1.Page;
            toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //zoom in and keep selected page on top of the viewport
            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
            toolStripButton1.Checked = false;
            //pdfRenderer1.Page = Cpn;
            pdfRenderer1.ZoomIn();
            toolStripTextBoxSite_TextChanged(sender, e);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            //zoom out and keep selected page on top of the viewport
            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
            toolStripButton1.Checked = false;
            //pdfRenderer1.Page = Cpn;
            pdfRenderer1.ZoomOut();
            toolStripTextBoxSite_TextChanged(sender, e);
        }

        private void toolStripTextBoxSite_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBoxSite.Text == "0")
            {
                toolStripTextBoxSite.Text = "1";
                toolStripTextBoxSite.SelectionStart = toolStripTextBoxSite.Text.Length;
                return;
            }
            try
            {
                if (toolStripTextBoxSite.Text.Length > 0)
                {
                    //avoid out of range
                    if (int.Parse(toolStripTextBoxSite.Text) > pdfDocument.PageCount)
                    {
                        toolStripTextBoxSite.Text = pdfDocument.PageCount.ToString();
                        toolStripTextBoxSite.SelectionStart = toolStripTextBoxSite.Text.Length;
                    }
                    else
                    {
                        toolStripTextBoxSite.SelectionStart = toolStripTextBoxSite.Text.Length;

                        //render the page given by toolStripTextBox1
                        pdfRenderer1.Page = int.Parse(toolStripTextBoxSite.Text) - 1;
                        Cpn = pdfRenderer1.Page;
                        //listView1.Items[pdfRenderer1.Page].Selected = true;
                        listView1.Items[Cpn].Selected = true;
                        //Cpn = listView1.Items.IndexOf(listView1.SelectedItems[int.Parse(toolStripTextBox1.Text) - 1]);
                    }
                }

                SizeF mySize = new SizeF();

                // Use the textbox font
                Font myFont = toolStripTextBoxSite.Font;

                using (Graphics g = CreateGraphics())
                {
                    // Get the size given the string and the font
                    mySize = g.MeasureString("S" + toolStripTextBoxSite.Text + "S", myFont);
                }

                // Resize the textbox 
                toolStripTextBoxSite.Width = (int)Math.Round(mySize.Width, 0);
            }
            catch { }
        }

        private void splitContainer1_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer s = sender as SplitContainer;
            if (s != null)
            {
                s.Dock = DockStyle.Fill;
                int top = splitContainer1.Panel1.Height / 2 - 20;
                int bottom = splitContainer1.Panel1.Height / 2 + 20;
                int left = s.SplitterDistance;
                int right = left + s.SplitterWidth - 1;

                int cy = s.Height / 2;
                e.Graphics.DrawLine(Pens.Gray, left, cy - 9, right, cy - 9);
                e.Graphics.DrawLine(Pens.Gray, left, cy - 6, right, cy - 6);
                e.Graphics.DrawLine(Pens.Gray, left, cy - 3, right, cy - 3);
                e.Graphics.DrawLine(Pens.Gray, left, cy, right, cy);
                e.Graphics.DrawLine(Pens.Gray, left, cy + 3, right, cy + 3);
                e.Graphics.DrawLine(Pens.Gray, left, cy + 6, right, cy + 6);
                e.Graphics.DrawLine(Pens.Gray, left, cy + 9, right, cy + 9);

                //top = cy - 12;
                //bottom = cy + 12;
                //left = s.SplitterDistance;
                //right = left + s.SplitterWidth - 1;
                //e.Graphics.DrawLine(Pens.DimGray, left, top, left, bottom);
                //e.Graphics.DrawLine(Pens.DimGray, right, top, right, bottom);
            }
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            // This allows the splitter to be moved normally again
            ((SplitContainer)sender).IsSplitterFixed = false;
            splitContainer1.Refresh();
        }

        private void splitContainer1_MouseMove(object sender, MouseEventArgs e)
        {
            // This disables the normal move behavior
            if (((SplitContainer)sender).IsSplitterFixed)
            {
                // Make sure that the button used to move the splitter
                // is the left mouse button
                if (e.Button.Equals(MouseButtons.Left))
                {
                    // Checks to see if the splitter is aligned Vertically
                    if (((SplitContainer)sender).Orientation.Equals(Orientation.Vertical))
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.X > 0 && e.X < ((SplitContainer)sender).Width)
                        {
                            // Move the splitter & force a visual refresh
                            ((SplitContainer)sender).SplitterDistance = e.X;
                            ((SplitContainer)sender).Refresh();
                            splitterDistance = e.X;
                        }
                    }
                    // If it isn't aligned vertically then it must be
                    // horizontal
                    else
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.Y > 0 && e.Y < ((SplitContainer)sender).Height)
                        {
                            // Move the splitter & force a visual refresh
                            ((SplitContainer)sender).SplitterDistance = e.Y;
                            ((SplitContainer)sender).Refresh();
                        }
                    }
                }
                // If a button other than left is pressed or no button
                // at all
                else
                {
                    // This allows the splitter to be moved normally again
                    ((SplitContainer)sender).IsSplitterFixed = false;
                }
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (pdfDocument != null)
            {
                Rectangle bounds = e.Bounds;

                Rectangle pageRect = new Rectangle(bounds.Location, bounds.Size);

                Image im = pdfDocument.Render(e.ItemIndex, 20, 20, PdfRenderFlags.CorrectFromDpi);
                Bitmap bmp = ComputeThumbAspect(im, e.ItemIndex, thumbSize.Width, thumbSize.Height, listView1.BackColor);

                if (bmp != null) e.Graphics.DrawImageUnscaledAndClipped(bmp, pageRect);

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(46, Color.GreenYellow)))
                using (Pen pen1 = new Pen(Color.GreenYellow, 1))
                using (Pen pen2 = new Pen(Color.GreenYellow, 2))
                {

                    if (e.Item.Focused)
                    {
                        e.Graphics.FillRectangle(sb, e.Bounds);
                        e.Graphics.DrawRectangle(pen1, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }

                    if (e.Item.Selected)
                    {

                        Rectangle pagRect = listView1.GetItemRect(Cpn);
                        im = pdfDocument.Render(Cpn, 20, 20, PdfRenderFlags.CorrectFromDpi);
                        bmp = ComputeThumbAspect(im, Cpn, thumbSize.Width, thumbSize.Height, listView1.BackColor);

                        Graphics gr = Graphics.FromImage(bmp);
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        Rectangle r = new Rectangle(thumBound.Left - 5, thumBound.Top - 5, thumBound.Width + 10, thumBound.Height + 27);
                        //Rectangle r = new Rectangle(thumBound.Right - 10, thumBound.Bottom - 10, 10, 10);

                        //gr.FillRectangle(sb, r);
                        //gr.DrawRectangle(pen2, r);
                        //gr.FillEllipse(Brushes.Red, r);
                        gr.DrawRectangle(pen2, r);
                        using (Font font = new Font("Tahoma", 10))
                        using (SolidBrush br = new SolidBrush(Color.GreenYellow))
                        {
                            StringFormat format = new StringFormat();
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Near;
                            string txt = (Cpn + 1).ToString();
                            gr.DrawString(txt, font, br, pt, format);
                        }

                        e.Graphics.DrawImageUnscaledAndClipped(bmp, pagRect);

                        if (e.Item.Focused)
                        {
                            e.Graphics.FillRectangle(sb, e.Bounds);
                            e.Graphics.DrawRectangle(pen1, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                        }
                    }
                }
            }
        }

        private Bitmap ComputeThumbAspect(Image image, int index, int thRectW, int thRectH, Color listViewBack)
        {
            //these are target width, target height, the target x and y
            float targetx, targety;
            float targetw, targeth;
            float w = image.Width;
            float h = image.Height;

            //get the ratio of width / height
            float aspectRatio = w / h;

            //if width >= height
            if (image.Width >= image.Height)
            {
                //set the width to thRectW and compute the height
                targetw = thRectW - 30f;//30px is used for margins
                targeth = targetw / aspectRatio;
            }

            else
            {
                //otherwise set the height to thRectH and compute the width
                targeth = thRectH - 30f;//30px is used for draw page number
                targetw = targeth * aspectRatio;
            }

            //now we compute where in our final image we're going to draw it
            targetx = (thRectW - targetw) / 2f;
            targety = (thRectH - targeth - 30f);//30px is used for draw page number

            //create our final image - you can set the PixelFormat to anything that suits
            Bitmap thumb = new Bitmap(thRectW, thRectH, PixelFormat.Format24bppRgb);

            //get a Graphics from this final image
            Graphics g = Graphics.FromImage(thumb);

            //clear the final image to white (or anything else) for the areas that the input image doesn't cover
            g.Clear(listViewBack);
            g.InterpolationMode = InterpolationMode.Low;

            //and here is where everything happens
            //the first Rectangle is where (tx, ty) and what size (tw, th) we want to draw the image
            //the second Rectangle is the portion of the image we want to draw, in this case all of it
            //the drawing routine does the resizing
            RectangleF dstRect = new RectangleF(targetx - 2f, targety + 5f, targetw, targeth);
            int lef = (int)dstRect.Left;
            int top = (int)dstRect.Top;
            int rit = (int)targetw;
            int bot = (int)targeth;
            thumBound = new Rectangle(lef, top, rit, bot);

            RectangleF srcRect = new RectangleF(0, 0, w, h);
            g.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel);

            g.DrawRectangle(Pens.DarkGray, thumBound);

            //shadeBorder.Draw(g, thumBound);

            pt = new PointF(targetx + targetw / 2, targety + targeth + 5);
            using (Font font = new Font("Tahoma", 10))
            using (SolidBrush br = new SolidBrush(Color.White))
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Near;
                string txt = (index + 1).ToString();
                g.DrawString(txt, font, br, pt, format);
            }
            return thumb;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Cpn = listView1.Items.IndexOf(listView1.SelectedItems[0]);
                toolStripTextBoxSite.Text = (Cpn + 1).ToString();
            }
        }

        private void pdfRenderer1_SizeChanged(object sender, EventArgs e)
        {
            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
        }

        private void pdfRenderer1_Resize(object sender, EventArgs e)
        {
            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
        }

        private void pdfRenderer1_DisplayRectangleChanged(object sender, EventArgs e)
        {
            if (File.Exists(pdfFileName)) toolStripTextBoxSite.Text = (pdfRenderer1.Page + 1).ToString();
            Cpn = pdfRenderer1.Page;
        }

        private void pdfRenderer1_Scroll(object sender, ScrollEventArgs e)
        {
            //MessageBox.Show("Scroll event");
            pdfRenderer1.DisplayRectangleChanged += pdfRenderer1_DisplayRectangleChanged;
        }

        private void pdfRenderer1_ZoomChanged(object sender, EventArgs e)
        {
            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
        }

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            ((SplitContainer)sender).IsSplitterFixed = true;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            var iFormat = "jpg";
            imageSave(iFormat);
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            var iFormat = "png";
            imageSave(iFormat);
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            var iFormat = "tif";
            imageSave(iFormat);
        }

        private void imageSave(string iFormat)
        {
            int dpiX;
            int dpiY;

            dpiX = 96;
            dpiY = 96;

            string path;

            using (var form = new FolderBrowserDialog())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                    path = form.SelectedPath;                
            }

            var document = pdfRenderer1.Document;

            for (int i = 0; i < document.PageCount; i++)
            {
                using (var image = document.Render(i, (int)document.PageSizes[i].Width, (int)document.PageSizes[i].Height, dpiX, dpiY, true)) 
                {
                    if (iFormat == "jpg")
                    {
                        image.Save(Path.Combine(path, "Page " + i + ".jpg"), ImageFormat.Jpeg);
                    }

                    if (iFormat == "png")
                    {
                        image.Save(Path.Combine(path, "Page " + i + ".png"), ImageFormat.Png);
                    }

                    if (iFormat == "tif")
                    {
                        image.Save(Path.Combine(path, "Page " + i + ".tiff"), ImageFormat.Tiff);
                    }
                }
            }
                
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    splitContainer1.SplitterDistance = 300;
                    splitContainer1.SplitterWidth = 8;
                    break;
                case "tabPage2":
                    splitContainer1.Panel1MinSize = 38;
                    splitContainer1.SplitterDistance = 38;
                    splitContainer1.SplitterWidth = 1;
                    break;
            }
        }

        private void pdfRenderer1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
                //MessageBox.Show("Mouse Wheel event");
                pdfRenderer1.DisplayRectangleChanged += pdfRenderer1_DisplayRectangleChanged;
        }

        private void PDFPreview_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterWidth = 8;
            splitContainer1.Panel1MinSize = 38;
            splitContainer1.SplitterDistance = 320;
            splitContainer1.Panel1Collapsed = false;
            EnableToolBar();
        }

        private void PDFPreview_FormClosing(Object sender, FormClosingEventArgs e)
        {
            pdfDocument.Dispose();
            pdfRenderer1.Dispose();
            if (File.Exists(pdfFileName)) { File.Delete(pdfFileName); }
        }

        private void DisableToolBar()
        {
            tabControl1.Enabled = false;
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            toolStripButton5.Enabled = false;
            toolStripButton6.Enabled = false;
            toolStripButton7.Enabled = false;
            toolStripButton8.Enabled = false;

            toolStripTextBoxSite.Enabled = false;
        }

        private void EnableToolBar()
        {
            tabControl1.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = true;
            toolStripButton4.Enabled = true;
            toolStripButton5.Enabled = true;
            toolStripButton6.Enabled = true;
            toolStripButton7.Enabled = true;
            toolStripButton8.Enabled = true;

            toolStripTextBoxSite.Enabled = true;
        }

        private void PDFPreview_Resize(object sender, EventArgs e)
        {
            if (!File.Exists(pdfFileName)) return;
            if (File.Exists(pdfFileName)) splitContainer1.SplitterDistance = splitterDistance;
            else splitContainer1.SplitterDistance = 38;

            pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;

            pdfRenderer1.MouseWheel -= pdfRenderer1_MouseWheel;

            Cpn = int.Parse(toolStripTextBoxSite.Text) - 1;
            //Cpn = pdfRenderer1.Page;
            //toolStripTextBox1.Text = (Cpn + 1).ToString();
            //toolStripTextBox1.Text = (listView1.Items.IndexOf(listView1.SelectedItems[0]) + 1).ToString();
            if (WindowState == FormWindowState.Minimized)
            {
                pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
                pdfRenderer1.MouseWheel -= pdfRenderer1_MouseWheel;
                //splitContainer1.Panel1Collapsed = true;
            }
            if (WindowState == FormWindowState.Maximized || WindowState == FormWindowState.Normal)
            {
                pdfRenderer1.DisplayRectangleChanged -= pdfRenderer1_DisplayRectangleChanged;
                pdfRenderer1.MouseWheel -= pdfRenderer1_MouseWheel;
                //splitContainer1.Panel1Collapsed = false;
                //splitContainer1.SplitterDistance = splitdist;
            }
            //pdfRenderer1.DisplayRectangleChanged += pdfRenderer1_DisplayRectangleChanged;
            splitContainer1.Refresh();
        }
    }
}
