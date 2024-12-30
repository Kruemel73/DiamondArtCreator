// Copyright (C) Jari Hautio <jari.hautio@iki.fi> 2008. Licensed under GPLv2. See LICENSE.txt in solution folder.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace RyijyApp
{
    /// <summary>
    /// Image processor class manages image resizing, color palette conversion and color reduction.
    /// </summary>
    class ImageProcessor
    {
        /// <summary>
        /// Image after resizing. Color manipulation is done on this image.
        /// </summary>
        Bitmap scaledImage = null;
        /// <summary>
        /// cached image with right palette, ready for color removal
        /// </summary>
        Bitmap paletteImage = null;
        /// <summary>
        /// Stores original color count
        /// </summary>
        int originalColors = 0;
        /// <summary>
        /// Color historgram for resized image
        /// </summary>
        ColorLib.ColorHistogram scaledHistogram = new ColorLib.ColorHistogram();
        /// <summary>
        /// Color histogram for target palette image
        /// </summary>
        ColorLib.ColorHistogram paletteHistogram = new ColorLib.ColorHistogram();

        public ColorLib.ColorHistogram PaletteHistogram { get { return paletteHistogram; } }
        /// <summary>
        /// Color histogram for final image
        /// </summary>
        ColorLib.ColorHistogram targetHistogram = new ColorLib.ColorHistogram();
        /// <summary>
        /// Returns color histogram for final image.
        /// </summary>
        public ColorLib.ColorHistogram TargetHistogram { get { return targetHistogram; } }
        /// <summary>
        /// Stores original image.
        /// </summary>
        private Bitmap originalImage = null;
        /// <summary>
        /// Number of colors after palette conversion.
        /// </summary>
        private int targetDepthColors;

        /// <summary>
        /// Custom palette to be used in stead for color depth.
        /// </summary>
        private CustomPalette customPalette = null;

        /// <summary>
        /// Gets or sets custom color palette.
        /// </summary>
        public CustomPalette CustomColorPalette
        {
            get { return customPalette; }
            set { customPalette = value; paletteImage = null; }
        }	
        ColorPicker.DistanceMode distanceMode = ColorPicker.DistanceMode.YCbCr;
        public ColorPicker.DistanceMode ColorDistanceMode { get { return distanceMode; } set { distanceMode = value; } }

        /// <summary>
        /// Reset event is fires when Original Image changes.
        /// </summary>
        public event EventHandler ResetHandler;
        /// <summary>
        /// Calss Reset event handlers
        /// </summary>
        /// <param name="e">not used</param>
        protected void OnReset(EventArgs e)
        {
            if (ResetHandler != null)
                ResetHandler(this, e);
        }
	
        /// <summary>
        /// Gets number of colors after palette conversion
        /// </summary>
        public int TargetDepthColors
        {
            get { return targetDepthColors; }
        }
	
        /// <summary>
        /// Gets or sets original source image. 
        /// Resizes back to original image size.
        /// </summary>
        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set 
            {
                originalImage = value;
                ryijyProps.OriginalSize.Height = originalImage != null ? originalImage.Height : 0;
                ryijyProps.OriginalSize.Width = originalImage != null ? originalImage.Width : 0;
                Reset();
            }
        }
        /// <summary>
        /// Final image after all processing is done.
        /// </summary>
        private Bitmap targetImage = null;

        /// <summary>
        /// Gets or sets final image.
        /// </summary>
        public Bitmap TargetImage
        {
            get { return targetImage; }
            set { targetImage = value; }
        }

        /// <summary>
        /// Size properties.
        /// </summary>
        private RyijyProp ryijyProps = new RyijyProp();

        /// <summary>
        /// Gets or sets image size properties.
        /// </summary>
        public RyijyProp RyijyProperties
        {
            get { return ryijyProps; }
            set { ryijyProps = value; }
        }
        /// <summary>
        /// Gets number of colors in final image.
        /// </summary>
        public int TargetColors { get { return targetHistogram.Count; } }
        /// <summary>
        /// Gets number of colors in original image.
        /// </summary>
        public int OriginalColors { get { return originalColors; } }
        /// <summary>
        /// Gets number of colors in resized image.
        /// </summary>
        public int ScaledColors { get { return scaledHistogram.Count; } }

        /// <summary>
        /// Default color depth conversion target.
        /// </summary>
        private const int defaultTargetDepth = 256;
        /// <summary>
        /// Color depth conversion target
        /// </summary>
        private int targetDepth = defaultTargetDepth;

        /// <summary>
        /// Gets or sets color depth conversion target
        /// </summary>
        public int TargetDepth
        {
            get => targetDepth;
            set
            {
                if (targetDepth != value)
                {
                    targetDepth = value;
                    removedColors = MaxRemovedColors;
                    paletteImage = null; // invalidate cached palette image
                    UpdateTargetImage();
                }
            }
        }
        /// <summary>
        /// Color reduction target
        /// </summary>
        private int removedColors = 0;
        /// <summary>
        /// Maximum for removed colors.
        /// </summary>
        public int MaxRemovedColors { get { return TargetDepthColors == 0 ? 1 : TargetDepthColors; } }
        /// <summary>
        /// Mininum for removed colors.
        /// </summary>
        public int MinRemovedColors { get { return 1; } }

        /// <summary>
        /// Get or set color removal target.
        /// MinRemovedColors and MaxRemovedColors define the interval.
        /// This is number of colors left in image
        /// </summary>
        public int RemovedColors
        {
            get { return removedColors; }
            set { if (removedColors != value) { removedColors = value; UpdateTargetImage(); }  }
        }
        private System.Drawing.Imaging.ColorMatrix colorMatrix = null;

        public System.Drawing.Imaging.ColorMatrix ColorMatrix
        {
            get { return colorMatrix; }
            set { colorMatrix = value; paletteImage = null; removedColors = MaxRemovedColors; UpdateTargetImage(); }
        }
	
        /// <summary>
        /// Event for notifying thayt colors in the target image have been changed
        /// </summary>
        public event EventHandler ColorsChanged;
        /// <summary>
        /// Fires ColorsChanged event.
        /// </summary>
        protected void OnColorsChanged()
        {
            if (ColorsChanged != null)
                ColorsChanged(this, EventArgs.Empty);
        }

        private double diversityFactor = 1.0;

        public double DiversityFactor
        {
            get { return diversityFactor; }
            set { diversityFactor = value; }
        }

        protected virtual bool IsUpdateCancelled() { return false; }
        /// <summary>
        /// Reprocesses the target image from scaled image.
        /// Sets palette and removes colors.
        /// </summary>
        public virtual void UpdateTargetImage()
        {
            if (scaledImage == null) return;
            if (paletteImage == null)
            {
                if (colorMatrix != null)
                {
                    paletteImage = new Bitmap(scaledImage.Width, scaledImage.Height, scaledImage.PixelFormat);
                    paletteImage.SetResolution( scaledImage.HorizontalResolution, scaledImage.VerticalResolution);
                    ImageAttributes img = new ImageAttributes();
                    img.SetColorMatrix(colorMatrix);
                    Rectangle r = new Rectangle(0,0,scaledImage.Width, scaledImage.Height);
                    using (Graphics g = Graphics.FromImage(paletteImage))
                    g.DrawImage(scaledImage, r, 0,0,r.Width, r.Height, GraphicsUnit.Pixel, img);
                }
                else 
                {
                    paletteImage =(Bitmap)scaledImage.Clone();
                }
                paletteHistogram = ReduceDepth(paletteImage, targetDepth);
            }
            if (IsUpdateCancelled()) return;

            targetImage = (Bitmap)paletteImage.Clone();            
            targetHistogram = paletteHistogram.CloneHistogram();
            bool useAllColors = (removedColors == MaxRemovedColors);
            targetDepthColors = targetHistogram.Count;
            if (removedColors > targetDepthColors || useAllColors)
                removedColors = targetDepthColors;

            if (IsUpdateCancelled()) return;

            if (removedColors != MaxRemovedColors)
                RemoveColors(targetImage, removedColors, targetHistogram);

            if (IsUpdateCancelled()) return;

            OnColorsChanged();
        }

        #region Public functions
        /// <summary>
        /// Resizes image using original image as starting image.
        /// Color palette and removed colors are reset.
        /// TODO: Change to keep palette and removal target.
        /// </summary>
        /// <param name="w">New width</param>
        /// <param name="h">New height</param>
        public void ResizeImage(int w, int h)
        {
            if (originalImage == null) return;

            Bitmap bmPhoto = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(originalImage.HorizontalResolution,
                                    originalImage.VerticalResolution);
            // first add some borders to original image


            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            ImageAttributes img = new ImageAttributes();
            img.SetWrapMode(WrapMode.TileFlipXY);
            grPhoto.DrawImage(originalImage,
                new Rectangle(0, 0, w, h),
                0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel, img);

            grPhoto.Dispose();

            // After resize, reset coloring options to full colors image
            scaledImage = bmPhoto;
            targetImage = bmPhoto;
            paletteImage = bmPhoto;
            scaledHistogram = CountColors(scaledImage);
            paletteHistogram = scaledHistogram;
            targetHistogram = scaledHistogram;
            targetDepth = defaultTargetDepth;
            targetDepthColors = scaledHistogram.Count;
            removedColors = MaxRemovedColors;

            OnColorsChanged();
        }
	

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageProcessor()
        {
            Reset();
        }

        /// <summary>
        /// Resets all image and color tranformations
        /// </summary>
        public void Reset()
        {
            scaledImage = originalImage;
            targetImage = scaledImage != null ? scaledImage.Clone() as Bitmap : null;
            paletteImage = scaledImage;
            scaledHistogram = CountColors(scaledImage);
            paletteHistogram = scaledHistogram;
            originalColors = scaledHistogram.Count;
            targetHistogram = scaledHistogram;
            targetDepth = defaultTargetDepth;
            targetDepthColors = scaledHistogram.Count;
            removedColors = MaxRemovedColors;
            OnReset(EventArgs.Empty);
            OnColorsChanged();
        }
        #endregion


        #region Utility functions
        /// <summary>
        /// Creates color historgram from a bitmap.
        /// </summary>
        /// <param name="bm">Color histogram</param>
        /// <returns></returns>
        unsafe ColorLib.ColorHistogram CountColors(Bitmap bm)
        {
            ColorLib.ColorHistogram colors = new ColorLib.ColorHistogram();
            CountColors(bm, colors);
            return colors;
        }

        unsafe void CountColors(Bitmap bm, ColorLib.ColorHistogram colors)
        {
            
            if (bm == null) return;
            colors.Clear();
            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            byte* ptr = (byte*)bd.Scan0.ToPointer();
            int offset = bd.Stride - bm.Width * 3;
            int height = bm.Height;
            int width = bm.Width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int c = Color.FromArgb(ptr[2], ptr[1], ptr[0]).ToArgb();
					colors.AddCount(Color.FromArgb(ptr[2], ptr[1], ptr[0]));
                    ptr += 3;
                }
                ptr += offset;
            }
            bm.UnlockBits(bd);
        }
        unsafe void FastReduceColors(Bitmap bm, ColorLib.ColorHistogram colors, int numColors)
        {
            if (bm == null) return;
            colors.Clear();
            Dictionary<int, int> cd = new Dictionary<int, int>(10000);
            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            byte* ptr = (byte*)bd.Scan0.ToPointer();
            int offset = bd.Stride - bm.Width * 3;
            int height = bm.Height;
            int width = bm.Width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c0 = Color.FromArgb(ptr[2], ptr[1], ptr[0]);
                    int r = Convert.ToInt32(Math.Round(numColors * c0.R / 255.0) / numColors * 255);
                    int g = Convert.ToInt32(Math.Round(numColors * c0.G / 255.0) / numColors * 255);
                    int b = Convert.ToInt32(Math.Round(numColors * c0.B / 255.0) / numColors * 255);
                    Color c2 = Color.FromArgb(r, g, b);
                    int c = c2.ToArgb();
                    ptr[2] = c2.R;
                    ptr[1] = c2.G;
                    ptr[0] = c2.B;
                    if (cd.ContainsKey(c))
                    {
                        ++cd[c];
                    }
                    else
                    {
                        cd.Add(c, 1);
                    }
                    ptr += 3;
                }
                ptr += offset;
            }
            bm.UnlockBits(bd);
            foreach (KeyValuePair<int, int> p in cd)
                colors.Add(Color.FromArgb(p.Key), p.Value);
        }

        /// <summary>
        /// Reduces color depth in target image.
        /// If custom palette is defined, it is used instead of given
        /// color depth.
        /// </summary>
        /// <param name="bm">Target bitmap</param>
        /// <param name="numColors">new number of colors </param>
        ColorLib.ColorHistogram ReduceDepth(Bitmap bm, int numColors)
        {
            // update histogram
            ColorLib.ColorHistogram dc = new ColorLib.ColorHistogram();
            if (bm == null) return dc;

            if (customPalette == null)
            {

                if (numColors > 256) numColors = 256;
                if (numColors < 1) numColors = 1;
                FastReduceColors(bm, dc, numColors);

            }
            else
            {
				//TODO: optimize this
				dc = new ColorLib.ColorHistogram(customPalette.Process(bm));
            }
            return dc;
        }
        /// <summary>
        /// Removee colors in targetImage
        /// </summary>
        /// <param name="bm">Target Bitmap</param>
        /// <param name="numColors">New number of colors</param>
        unsafe void RemoveColors(Bitmap bm, int numColors, ColorLib.ColorHistogram colors)
        {
            if (bm == null || numColors == MaxRemovedColors) return;

            // make histogram if needed
            
            if (colors == null) colors = new ColorLib.ColorHistogram();
            if (colors.Count == 0)
            {
                CountColors(bm, colors);
            }
            if (IsUpdateCancelled()) return;
            // Pick best colors
            int colorCount = numColors;
            if (colorCount < 1) colorCount = 1;
            System.DateTime ta = System.DateTime.Now;
            /*
            ColorPicker cp = new ColorPicker(colors);
            cp.DiversityFactor = DiversityFactor;
            cp.ColorDistanceMode = distanceMode;
            System.DateTime t0 = System.DateTime.Now;
            // drop infrequently used colors with good candidate colors
            cp.DropColors2(4, colorCount > 100 ? 8 : 10 , colors.Keys.Count - colorCount);
            // drop more, if lot of color are still left
            int needToDrop = colors.Keys.Count - colorCount - cp.DroppedColors.Count;
            if (needToDrop > 500)
            {
                cp.DropColors2(8, 12, needToDrop);
            }
            System.DateTime t1 = System.DateTime.Now;

            List<Color> picked = cp.PickColors(colorCount);
            System.DateTime t2 = System.DateTime.Now;

            Dictionary<Color,Color> colorMap = cp.GetColorMap();
            System.DateTime t3 = System.DateTime.Now;
*/
            ColorLib.CubePicker cp = new ColorLib.CubePicker(colors, ColorLib.CubePicker.CubeMode.Percent,ColorLib.CubePicker.ColorMode.Lab);
            List<Color> picked = cp.Select(colorCount);
            Dictionary<Color, Color> colorMap = cp.GetColorMap();
            // remap colors in the image
            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            byte *ptr = (byte*)bd.Scan0.ToPointer();
            int offset = bd.Stride - bm.Width * 3;
            Color newColor;
            for (int i = 0; i < bm.Height; i++)
            {
                for (int j = 0; j < bm.Width; j++)
                {
                    Color c = Color.FromArgb(ptr[2], ptr[1], ptr[0]);
                    if (colorMap.TryGetValue(c, out newColor))
                    {
                        ptr[2] = newColor.R;
                        ptr[1] = newColor.G;
                        ptr[0] = newColor.B;
                    }
                    ptr += 3;
                }
                ptr += offset;
            }
            bm.UnlockBits(bd);
            System.DateTime t4 = System.DateTime.Now;

			colors.RemapColors(colorMap);
            System.DateTime t5 = System.DateTime.Now;
#if TIME_REMOVE_COLOR
            System.Windows.Forms.MessageBox.Show(
                String.Format("cons={0} drop={1} pick={2} get={3} replace={4} hist={5}",
               new object[] { t0-ta,t1 - t0, t2 - t1, t3 - t2, t4 - t3, t5 - t4 }));
#endif

        }

        #endregion

    }
}
