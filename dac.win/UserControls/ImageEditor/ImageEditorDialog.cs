using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Ravlyk.SAE5.WinForms.Properties;
using DAC.Overlay;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
    public partial class ImageEditorDialog : Form
    {
        Microsoft.Win32.RegistryKey regKey; // Our registry key section
        int cb;
        Font arialFont;
        float arialSize;

        public ImageEditorDialog()
        {
            this.filterGrid = new FilterGrid("", this.BackColor);
            InitializeComponent();
            string editImage1Filename = Settings.Default.UserPalettesLocationSafe + "\\croppedpic.jpg";

            imagePanel1.Label = "Hauptbild";
            imagePanel1.Buttons = ImagePanel.ButtonSet.Load;
            imagePanel1.loadClick += new EventHandler(panelLoad_Click);
            imagePanel1.Image = Image.FromFile(editImage1Filename);

            imagePanel2.Label = "Overlay-Bild";
            imagePanel2.Buttons = ImagePanel.ButtonSet.Load;
            imagePanel2.loadClick += new EventHandler(panelLoad_Click);

            imagePanel3.Label = "Komposition";
            imagePanel3.Buttons = ImagePanel.ButtonSet.Save;
            imagePanel3.loadClick += new EventHandler(panelSave_Click);

            // Load defaults from registry, values set when program closes.
            this.regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\" + Application.ProductName);

            string defFilterPath = Path.GetDirectoryName(Settings.Default.UserPalettesLocationSafe) + @"\Filters";
            loadFilterDialog.InitialDirectory = this.regKey.GetValue("FilterPath", defFilterPath) as string;
            if (loadFilterDialog.InitialDirectory.Length == 0)
                loadFilterDialog.InitialDirectory = Path.GetDirectoryName(Settings.Default.UserPalettesLocationSafe) + @"\Filters";

            loadImageDialog.InitialDirectory = this.regKey.GetValue("ImagePath",
               Path.GetDirectoryName(Application.ExecutablePath) + @"\Images") as string;

            LoadDefaultFilter();
            ApplyFilter();

            try
            {
                // Load built-in filters
                Assembly a = Assembly.GetExecutingAssembly();
                Stream zipStream = a.GetManifestResourceStream("Ravlyk.SAE5.WinForms.filters.gzip");
                LoadCompressedFilters(zipStream);

                // Load filters on disk.
                DirectoryInfo dirInfo = new DirectoryInfo(loadFilterDialog.InitialDirectory);
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.xml"))
                {
                    LoadFilterGridFlow(fileInfo.FullName);
                }
            }
            catch { }
        }

        private void LoadCompressedFilters(Stream fileStream)
        {

            GZipStream decompressStream = new GZipStream(fileStream, CompressionMode.Decompress, false);

            BinaryFormatter bFormatter = new BinaryFormatter();

            while (true)
            {
                try
                {
                    string label = (string)bFormatter.Deserialize(decompressStream);
                    DataTable dt = (DataTable)bFormatter.Deserialize(decompressStream);
                    Image image = (Image)bFormatter.Deserialize(decompressStream);
                    FilterGrid fg = new FilterGrid(label, Color.LightBlue);
                    fg.GridView.ReadOnly = true;

                    fg.Source = dt;
                    fg.Image = image;
                    fg.clickEvent += new System.EventHandler(GridSelectionChanged);

                    this.filterFlow.Controls.Add(fg);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    break;
                }
            }

            decompressStream.Close();
        }

        private void LoadFilterGridFlow(string filename)
        {
            FilterGrid fg = LoadFilterGrid(filename);
            if (fg != null)
            {
                fg.clickEvent += new System.EventHandler(GridSelectionChanged);
                fg.GridView.ReadOnly = true;

                bool duplicate = false;
                foreach (FilterGrid fgScan in filterFlow.Controls)
                {
                    if (dgvEqual(fg.GridView, fgScan.GridView))
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                    this.filterFlow.Controls.Add(fg);
            }
        }
        private bool dgvEqual(DataGridView dgv1, DataGridView dgv2)
        {
            bool eq = (dgv1.Rows.Count == dgv2.Rows.Count &&
                dgv1.ColumnCount == dgv2.ColumnCount);
            if (eq)
            {
                for (int row = 0; row < dgv1.Rows.Count; row++)
                {
                    for (int col = 0; col < dgv1.ColumnCount; col++)
                    {
                        Single f1 = (Single)dgv1.Rows[row].Cells[col].Value;
                        Single f2 = (Single)dgv2.Rows[row].Cells[col].Value;
                        if (f1 != f2)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private void GridSelectionChanged(object obj, EventArgs e)
        {
            if (checkBoxMain.Checked)
            {
                FilterGrid activeFg = (FilterGrid)obj;
                DataTable dt = (DataTable)activeFg.Source;
                this.filterGrid.Source = dt.Copy();
                this.filterGrid.Label = activeFg.Label;
                this.filterGrid.Image = new Bitmap(activeFg.Image);

                ApplyFilter();
                //ApplyFilter2();
            }
            if (checkBoxOverlay.Checked)
            {
                FilterGrid activeFg = (FilterGrid)obj;
                DataTable dt1 = (DataTable)activeFg.Source;
                this.filterGrid1.Source = dt1.Copy();
                this.filterGrid1.Label = activeFg.Label;
                this.filterGrid1.Image = new Bitmap(activeFg.Image);

                //ApplyFilter();
                ApplyFilter2();
            }

        }
        private FilterGrid LoadFilterGrid(string filename)
        {
            FilterGrid fg = null;
            try
            {
                DataTable dt = new DataTable();
                dt.ReadXml(filename);

                fg = new FilterGrid(Path.GetFileName(filename), Color.LightBlue);
                fg.Source = dt;

                try
                {
                    Image image = Bitmap.FromFile(Path.ChangeExtension(filename, ".png"));
                    fg.Image = image;
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return fg;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                this.regKey.SetValue("FilterPath", Path.GetDirectoryName(loadFilterDialog.FileName));
                this.regKey.SetValue("ImagePath", Path.GetDirectoryName(loadImageDialog.FileName));
            }
            catch { }
            imagePanel1.Dispose();
            imagePanel2.Dispose();
            imagePanel3.Dispose();
            base.OnClosing(e);
        }

        Single[][] m_f2Array;                // Array to store color matrix cell values (image 1)
        DataTable m_dt;                     // DataTable used to manipulate Grid values. (image 1)

        Single[][] m_f2Array2;                // Array to store color matrix cell values
        DataTable m_dt2;                     // DataTable used to manipulate Grid values.

        Single[][] filterArray;

        Bitmap tempImage;

        private void ApplyFilter()
        {
            m_dt = (DataTable)filterGrid.Source;
            for (int r = 0; r < m_f2Array.Length; r++)
            {
                m_dt.Rows[r].ItemArray.CopyTo(m_f2Array[r], 0);
            }
            Bitmap newImage = new Bitmap(imagePanel1.Image);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            DrawImage(g, imagePanel1.Image);

            imagePanel3.Image = newImage;
            tempImage = new Bitmap(newImage);
            filterGrid.Image = newImage;
        }

        private void ApplyFilter2()
        {
            m_dt2 = (DataTable)filterGrid1.Source;
            for (int r = 0; r < m_f2Array2.Length; r++)
            {
                m_dt2.Rows[r].ItemArray.CopyTo(m_f2Array2[r], 0);
            }
            Bitmap newImage = new Bitmap(tempImage);
            //Bitmap newImage = new Bitmap(imagePanel2.Image);
            Graphics g2 = Graphics.FromImage(newImage);
            g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            DrawImage2(g2, imagePanel2.Image);
            imagePanel3.Image = newImage;
            filterGrid1.Image = newImage;
        }

        private void DrawImage(Graphics g, Image image)
        {
            Single gamma = 1.0f;         // no change in gamma
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(m_f2Array), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);

            Rectangle rect = new Rectangle(Point.Empty, image.Size);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        private void DrawImage2(Graphics g, Image image)
        {
            Single gamma = 1.0f;         // no change in gamma
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(m_f2Array2), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);

            Rectangle rect = new Rectangle(Point.Empty, image.Size);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
        }


        private void LoadDefaultFilter()
        {
            Single brightness = 1.0f;    // no change in brightness
            Single contrast = 1.0f;      // normal, unit value

            Single adjBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            m_f2Array = new Single[][] {
                    new Single[] {contrast, 0, 0, 0, 0}, // scale red
                    new Single[] {0, contrast, 0, 0, 0}, // scale green
                    new Single[] {0, 0, contrast, 0, 0}, // scale blue
                    new Single[] {0, 0, 0, 1.0f, 0},     // don't scale alpha
                    new Single[] {adjBrightness, adjBrightness, adjBrightness, 0, 1}};

            // dataGridView.Rows.Clear();
            // dataGridView.Columns.Clear();
            // dataGridView.DataBindings.Clear();
            m_dt = Fill(new DataTable("dt"), m_f2Array);
            filterGrid.Source = m_dt;
        }

        private void LoadDefaultFilter2()
        {
            Single brightness = 1.0f;    // no change in brightness
            Single contrast = 1.0f;      // normal, unit value

            Single adjBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            m_f2Array2 = new Single[][] {
                    new Single[] {contrast, 0, 0, 0, 0}, // scale red
                    new Single[] {0, contrast, 0, 0, 0}, // scale green
                    new Single[] {0, 0, contrast, 0, 0}, // scale blue
                    new Single[] {0, 0, 0, 1.0f, 0},     // don't scale alpha
                    new Single[] {adjBrightness, adjBrightness, adjBrightness, 0, 1}};

            // dataGridView.Rows.Clear();
            // dataGridView.Columns.Clear();
            // dataGridView.DataBindings.Clear();
            m_dt2 = Fill2(new DataTable("dt2"), m_f2Array2);
            filterGrid1.Source = m_dt2;
        }

        private DataTable Fill(DataTable dt, Single[][] f2Array)
        {
            dt.Columns.Add("Red", typeof(Single));
            dt.Columns.Add("Green", typeof(Single));
            dt.Columns.Add("Blue", typeof(Single));
            dt.Columns.Add("Alpha", typeof(Single));
            dt.Columns.Add("Int", typeof(Single));

            foreach (Single[] fRow in f2Array)
            {
                DataRow dRow = dt.NewRow();
                for (int col = 0; col < fRow.Length; col++)
                    dRow[col] = fRow[col];
                dt.Rows.Add(dRow);
            }

            dt.AcceptChanges();
            return dt;
        }
        private DataTable Fill2(DataTable dt2, Single[][] f2Array2)
        {
            dt2.Columns.Add("Red", typeof(Single));
            dt2.Columns.Add("Green", typeof(Single));
            dt2.Columns.Add("Blue", typeof(Single));
            dt2.Columns.Add("Alpha", typeof(Single));
            dt2.Columns.Add("Int", typeof(Single));

            foreach (Single[] fRow in f2Array2)
            {
                DataRow dRow = dt2.NewRow();
                for (int col = 0; col < fRow.Length; col++)
                    dRow[col] = fRow[col];
                dt2.Rows.Add(dRow);
            }

            dt2.AcceptChanges();
            return dt2;
        }
        private void panelLoad_Click(object sender, EventArgs e)
        {
            Image image = LoadImage();
            if (image != null)
            {
                ImagePanel ImagePanel = (ImagePanel)sender;
                ImagePanel.Image = image;
                LoadDefaultFilter2();
                ApplyFilter2();
            }
        }
        private void panelSave_Click(object sender, EventArgs e)
        {
            string editImage1Filename = Application.LocalUserAppDataPath + "\\editpic.jpg";
            try
            {
                ImagePanel ipanel = (ImagePanel)sender;
                Image image = ipanel.Image;
                image.Save(editImage1Filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Image LoadImage()
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Filter = "";
            Dlg.Title = "Datei wählen...";
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = Bitmap.FromFile(Dlg.FileName);
                    return image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return null;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (imagePanel2.Image == null)
            {
                ApplyFilter();
            }
            else
            {
                ApplyFilter();
                ApplyFilter2();
            }
        }

        private void buttonApply2_Click(object sender, EventArgs e)
        {
            ApplyFilter();
            ApplyFilter2();
        }

        private void buttonLoadOverlay_Click(object sender, EventArgs e)
        {
            loadFilterDialog.Multiselect = false;
            if (loadFilterDialog.ShowDialog() == DialogResult.OK)
            {
                FilterGrid fg = LoadFilterGrid(loadFilterDialog.FileName);
                if (fg != null)
                {
                    filterGrid1.Set(fg);
                }
            }
        }

        private void buttonSaveMain_Click(object sender, EventArgs e)
        {
            if (saveFilterDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filename = saveFilterDialog.FileName;
                    m_dt.WriteXml(filename, XmlWriteMode.WriteSchema);
                    imagePanel3.Image.Save(Path.ChangeExtension(filename, ".png"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void buttonSaveOverlay_Click(object sender, EventArgs e)
        {
            if (saveFilterDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filename = saveFilterDialog.FileName;
                    m_dt2.WriteXml(filename, XmlWriteMode.WriteSchema);
                    imagePanel3.Image.Save(Path.ChangeExtension(filename, ".png"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            imagePanel1.Image.Dispose();
            if (imagePanel2.Image != null) { imagePanel2.Image.Dispose(); }
            imagePanel3.Image.Dispose();
            File.Delete(Settings.Default.UserPalettesLocationSafe + "\\croppedpic.jpg");
            File.Delete(Settings.Default.UserPalettesLocationSafe + "\\editpic2.jpg");
            this.Dispose();
            this.Close();
        }

        private void buttonToPreview_Click(object sender, EventArgs e)
        {
            string file = Settings.Default.UserPalettesLocationSafe + "\\editpic1.jpg";
            if (!File.Exists(file))
            {
                imagePanel3.Image.Save(file, ImageFormat.Jpeg);
            }
            else
            {
                File.Delete(file);
                imagePanel3.Image.Save(file, ImageFormat.Jpeg);
            }
            MessageBox.Show("Komposition wurde als Vorlage übernommen !");
        }

        private void flatButton1_Click(object sender, EventArgs e)
        {
            //FontDialog fDialog = new FontDialog();

            //if (fDialog.ShowDialog() == DialogResult.OK & !String.IsNullOrEmpty(txtOverlayText.Text))
            //{
                string overlayText = txtOverlayText.Text;
                var imgHeight = imagePanel1.Image.Height;
                var imgWidht = imagePanel1.Image.Width;
            if (imagePanel2.Image != null)
            {
                imagePanel2.Image.Dispose();
            }

            if (arialFont == null) { arialFont = DefaultFont; }

                ContentAlignment position = ContentAlignment.MiddleCenter;

                switch (cboPosition.SelectedIndex + 1)
                {
                    case 1:
                        position = ContentAlignment.TopRight;
                        break;
                    case 2:
                        position = ContentAlignment.TopCenter;
                        break;
                    case 3:
                        position = ContentAlignment.TopLeft;
                        break;
                    case 4:
                        position = ContentAlignment.MiddleLeft;
                        break;
                    case 5:
                        position = ContentAlignment.MiddleCenter;
                        break;
                    case 6:
                        position = ContentAlignment.MiddleRight;
                        break;
                    case 7:
                        position = ContentAlignment.BottomRight;
                        break;
                    case 8:
                        position = ContentAlignment.BottomCenter;
                        break;
                    case 9:
                        position = ContentAlignment.BottomLeft;
                        break;
                }
                
                //create image
                var bitmap = new Bitmap(imgWidht, imgHeight);
                for (int y = 0; y < imgHeight; y++)
                {
                    for (int x = 0; x < imgWidht; x++)
                    {
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                }

                //create Overlay-Text
                bitmap = Overlay.TextOverlay(bitmap, overlayText, arialFont, Color.Gray, false, false, Position: position, 1.0f);

                //Make text transparant and save file for forther work
                bitmap.MakeTransparent(Color.Gray);
                string file = Settings.Default.UserPalettesLocationSafe + "\\editpic2.jpg";
                //save image
                if (!File.Exists(file))
                {
                    bitmap.Save(file, ImageFormat.Png);
                    bitmap.Dispose();
                }
                else
                {
                    //File.Delete(file);
                    bitmap.Save(file, ImageFormat.Png);
                    bitmap.Dispose();
                }
                bitmap.Dispose();

                //set image to imagePanel2
                Image image = Bitmap.FromFile(file);
                imagePanel2.Image = image;

                LoadDefaultFilter2();
                ApplyFilter2();

                //image.Dispose();
            //}
        }

        private void redbar_ValueChanged(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void ChangeBar()
        {
            if (cb == 2)
            {
                Single changeredO = redbarO.Value * 0.1f;
                Single changegreenO = greenbarO.Value * 0.1f;
                Single changeblueO = bluebarO.Value * 0.1f;
                Single brightnessO = TrackBarBrightnessO.Value * 0.01f;
                float contrastO = trackBarContrastO.Value * 0.01f;
                float alphaO = trackBarAlphaO.Value * 0.1f;

                txtBoxRedO.Text = changeredO.ToString();
                txtBoxGreenO.Text = changegreenO.ToString();
                txtBoxBlueO.Text = changeblueO.ToString();
                txtBoxBrightnessO.Text = TrackBarBrightness.Value.ToString();
                txtBoxKontrastO.Text = trackBarContrast.Value.ToString();
                txtBoxAlphaO.Text = trackBarAlphaO.Value.ToString();

                m_f2Array2 = new float[][] {
                    new float[] {1 + changeredO, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1 + changegreenO, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1 + changeblueO, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1 + alphaO, 0},     // don't scale alpha
                    new float[] { brightnessO, brightnessO, brightnessO, 0, 1}};
                m_dt2 = Fill(new DataTable("dt2"), m_f2Array2);
                this.filterGrid1.Source = m_dt2;
                ApplyFilter();
                ApplyFilter2();
            }
            else if (cb == 1)
            {
                float changered = redbar.Value * 0.1f;
                float changegreen = greenbar.Value * 0.1f;
                float changeblue = bluebar.Value * 0.1f;
                float brightness = TrackBarBrightness.Value * 0.01f;
                float contrast = trackBarContrast.Value * 0.01f;
                float alpha = trackBarAlpha.Value * 0.1f;

                txtBoxRed.Text = changered.ToString();
                txtBoxGreen.Text = changegreen.ToString();
                txtBoxBlue.Text = changeblue.ToString();
                txtBoxBrightness.Text = TrackBarBrightness.Value.ToString();
                txtBoxKontrast.Text = trackBarContrast.Value.ToString();
                txtBoxAlpha.Text = trackBarAlpha.Value.ToString();

                m_f2Array = new float[][] {
                    new float[] {1 + changered, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1 + changegreen, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1 + changeblue, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1+ alpha, 0},     // don't scale alpha
                    new float[] { brightness, brightness, brightness, 0, 1}};
                m_dt = Fill(new DataTable("dt"), m_f2Array);
                this.filterGrid.Source = m_dt;
                if (imagePanel2.Image != null)
                {
                    ApplyFilter();
                    ApplyFilter2();
                }
                else
                {
                    ApplyFilter();
                }
            }

        }

        private void TrackBarBrightness_Scroll(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void trackBarContrast_Scroll(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void greenbar_ValueChanged(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void bluebar_ValueChanged(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void redbarO_ValueChanged(object sender, EventArgs e)
        {
            cb = 2;
            ChangeBar();
        }

        private void trackBarAlpha_ValueChanged(object sender, EventArgs e)
        {
            cb = 1;
            ChangeBar();
        }

        private void trackBarAlphaO_ValueChanged(object sender, EventArgs e)
        {
            cb = 2;
            ChangeBar();
        }

        private void ChangeFilter2()
        {
            m_dt2 = Fill(new DataTable("dt2"), m_f2Array2);
            this.filterGrid1.Source = m_dt2;

            if (imagePanel2.Image != null)
            {
                ApplyFilter();
                ApplyFilter2();
            }
            else
            {
                ApplyFilter();
            }
        }

        private void ChangeFilter()
        {
            m_dt = Fill(new DataTable("dt"), m_f2Array);
            this.filterGrid.Source = m_dt;

            if (imagePanel2.Image != null)
            {
                ApplyFilter();
                ApplyFilter2();
            }
            else
            {
                ApplyFilter();
            }
        }

        private void ckbGray_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {0.299f, 0.299f, 0.299f, 0, 0}, // scale red
                    new float[] {0.587f, 0.587f, 0.587f, 0, 0}, // scale green
                    new float[] {0.114f, 0.114f, 0.114f, 0, 0 }, // scale blue
                    new float[] {0, 0, 0, 1, 0},     // don't scale alpha
                    new float[] {0, 0, 0, 0, 1}};
            
            if (ckbGray.Checked || ckbOVGray.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void resetFilter()
        {
            if (tcFiltergallery.SelectedIndex == 0)
            {
                LoadDefaultFilter();
                ChangeFilter();
            }
            if (tcFiltergallery.SelectedIndex == 1)
            {
                LoadDefaultFilter2();
                ChangeFilter2();
            }
        }

        private void setFilter(float[][] filterArray)
        {
            if (tcFiltergallery.SelectedIndex == 0)
            {
                m_f2Array = filterArray;
                ChangeFilter();
            }
            if (tcFiltergallery.SelectedIndex == 1)
            {
                m_f2Array2 = filterArray;
                ChangeFilter2();
            }
        }

        private void checkBoxBlue_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {0,0,0,0,0}, // scale red
                    new float[] {0,0,0,0,0}, // scale green
                    new float[] {0,0,1,0,0}, // scale blue
                    new float[] {0,0,0,1,0},     // don't scale alpha
                    new float[] {0,0,0,0,1}};
            if (checkBoxBlue.Checked || ckbOVBlue.Checked)
                {
                    setFilter(filterArray);
                }
            else
                {
                    resetFilter();
                }
        }

        private void checkBoxGreen_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {0,0,0,0,0}, // scale red
                    new float[] {0,1,0,0,0}, // scale green
                    new float[] {0,0,0,0,0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxGreen.Checked || ckbOVGreen.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxRed_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1,0,0,0,0}, // scale red
                    new float[] {0,0,0,0,0}, // scale green
                    new float[] {0,0,0,0,0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxRed.Checked || ckbOVRed.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxNegativ_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {-1, 0, 0, 0, 0}, // scale red
                    new float[] {0, -1, 0, 0, 0}, // scale green
                    new float[] {0, 0, -1, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1, 0},     // don't scale alpha
                    new float[] {1, 1, 1, 0, 1}};
            if (checkBoxNegativ.Checked || ckbOVNegativ.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxSepia_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {.393f, .349f, .272f, 0, 0}, // scale red
                    new float[] {.769f, .686f, .534f, 0, 0}, // scale green
                    new float[] {.189f, .168f, .131f, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1, 0},     // don't scale alpha
                    new float[] {0, 0, 0, 0, 1}};
            if (checkBoxSepia.Checked || ckbOVSepia.Checked)
                {
                    setFilter(filterArray);
                }
            else
                {
                    resetFilter();
                }
        }

            private void checkBoxWhiteHalftransparent_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1,1,1,0,0}, // scale red
                    new float[] {1,1,1,0,0}, // scale green
                    new float[] {1,1,1,0,0}, // scale blue
                    new float[] {0,0,0,.5f,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxWhiteHalftransparent.Checked || ckbOVWHT.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxArtistic_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1, 0},     // don't scale alpha
                    new float[] {0, 0, 1, 0, 1}};
            if (checkBoxArtistic.Checked || ckbOVArtistic.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxDarkHalftransparent_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {0.5f,0.5f,0.5f,0,0}, // scale red
                    new float[] {0.5f,0.5f,0.5f,0,0}, // scale green
                    new float[] {0.5f,0.5f,0.5f,0,0}, // scale blue
                    new float[] {0,0,0,0.5f,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxDarkHalftransparent.Checked || ckbOVDHT.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxHalftransparent_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1,0,0,0,0}, // scale red
                    new float[] {0,1,0,0,0}, // scale green
                    new float[] {0,0,1,0,0}, // scale blue
                    new float[] {0,0,0,0.5f,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxHalftransparent.Checked || ckbOVHT.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxSpike_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1 + 0.3f, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1 + 0.7f, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1 + 1.3f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxSpike.Checked || ckbOVSpike.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxFlash_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1 + 0.9f, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1 + 1.5f, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1 + 1.3f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxFlash.Checked || ckbOVFlash.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxFrozen_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {1 + 0.3f, 0, 0, 0, 0}, // scale red
                    new float[] {0, 1 + 0f, 0, 0, 0}, // scale green
                    new float[] {0, 0, 1 + 5f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxFrozen.Checked || ckbOVFrozen.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxSuji_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {.393f, .349f + 0.5f, .272f, 0, 0}, // scale red
                    new float[] {.769f + 0.3f, .686f, .534f, 0, 0}, // scale green
                    new float[] {.189f, .168f, .131f + 0.5f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxSuji.Checked || ckbOVSuji.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxDramatic_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {.393f + 0.3f, .349f, .272f, 0, 0}, // scale red
                    new float[] {.769f, .686f + 0.2f, .534f, 0, 0}, // scale green
                    new float[] {.189f, .168f, .131f + 0.9f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxDramatic.Checked || ckbOVDramatic.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void checkBoxKakao_CheckedChanged(object sender, EventArgs e)
        {
            filterArray = new float[][] {
                    new float[] {.393f, .349f, .272f + 1.3f, 0, 0}, // scale red
                    new float[] {.769f, .686f + 0.5f, .534f, 0, 0}, // scale green
                    new float[] {.189f + 2.3f, .168f, .131f, 0, 0}, // scale blue
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}};
            if (checkBoxKakao.Checked || ckbOVKakao.Checked)
            {
                setFilter(filterArray);
            }
            else
            {
                resetFilter();
            }
        }

        private void TrackBarBrightnessO_Scroll(object sender, EventArgs e)
        {
            cb = 2;
            ChangeBar();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            FontDialog fDialog = new FontDialog();
            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                arialFont = fDialog.Font;
                arialSize = fDialog.Font.Size;
            }
        }

        private void ckbOVGray_CheckedChanged(object sender, EventArgs e)
        {
            ckbGray_CheckedChanged(sender, e);
        }

        private void ckbOVBlue_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxBlue_CheckedChanged(sender, e);
        }

        private void ckbOVGreen_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxGreen_CheckedChanged(sender, e);
        }

        private void ckbOVRed_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRed_CheckedChanged(sender, e);
        }

        private void ckbOVNegativ_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxNegativ_CheckedChanged(sender, e);
        }

        private void ckbOVWHT_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxWhiteHalftransparent_CheckedChanged(sender, e);
        }

        private void ckbOVArtistic_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxArtistic_CheckedChanged(sender, e);
        }

        private void ckbOVDHT_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxDarkHalftransparent_CheckedChanged(sender, e);
        }

        private void ckbOVHT_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxHalftransparent_CheckedChanged(sender, e);
        }

        private void ckbOVSpike_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSpike_CheckedChanged(sender, e);
        }

        private void ckbOVFlash_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxFlash_CheckedChanged(sender, e);
        }

        private void ckbOVDramatic_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxDramatic_CheckedChanged(sender, e);
        }

        private void ckbOVKakao_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxKakao_CheckedChanged(sender, e);
        }

        private void ckbOVSuji_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSuji_CheckedChanged(sender, e);
        }

        private void ckbOVFrozen_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxFrozen_CheckedChanged(sender, e);
        }

        private void ckbOVSepia_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSepia_CheckedChanged(sender, e);
        }

        private void saveImageDialog_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
