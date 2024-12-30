using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace DAC.Overlay
{
    public class Overlay
    {
        // "Old" signature overloaded to call new method w/extra parameters
        public static Bitmap TextOverlay(Image img, string OverlayText, Font OverlayFont,
                   Color OverlayColor, bool AddAlpha, bool AddShadow)
        {
            return TextOverlay(img, OverlayText, OverlayFont, OverlayColor, AddAlpha, AddShadow,
                ContentAlignment.MiddleCenter, 0.8F);
        }

        // Draw text directly onto an image (scaled for best-fit)
        // Written to be called from a module (just on the form for simplicity)
        public static Bitmap TextOverlay(Image img, string OverlayText, Font OverlayFont,
                Color OverlayColor, bool AddAlpha, bool AddShadow,
                System.Drawing.ContentAlignment Position, float PercentFill)
        {
            if (OverlayText != null && OverlayText.Length > 0 && PercentFill > 0)
            {
                // create bitmap and graphics used for drawing
                // "clone" image but use 24RGB format
                Bitmap bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(img, 0, 0);

                int alpha = 255;
                if (AddAlpha)
                {
                    // Compute transparency: Longer text should be less transparent or it gets lost.
                    alpha = 90 + (OverlayText.Length * 2);
                    if (alpha >= 255) alpha = 255;
                }
                // Create the brush based on the color and alpha
                SolidBrush b = new SolidBrush(Color.FromArgb(alpha, OverlayColor));

                // Measure the text to render (unscaled, unwrapped)
                StringFormat strFormat = StringFormat.GenericTypographic;
                SizeF s = g.MeasureString(OverlayText, OverlayFont, 100000, strFormat);

                // Enlarge font to specified fill (estimated by AREA)
                float zoom = (float)(Math.Sqrt(((double)(img.Width * img.Height) * PercentFill) / (double)(s.Width * s.Height)));
                FontStyle sty = OverlayFont.Style;
                Font f = new Font(OverlayFont.FontFamily, ((float)OverlayFont.Size) * zoom, sty);
                Console.WriteLine(String.Format("Starting Zoom: {0}, Font Size: {1}, Alpha: {2}", zoom, f.Size, alpha));

                // Measure using new font size, allow to wrap as needed.
                // Could rotate the overlay at a 30-45 deg. angle (trig would give correct angle).
                // Of course, then the area covered would be less than "straight" text.
                // I'll leave those calculations for someone else....
                int charFit;
                int linesFit;
                float SQRTFill = (float)(Math.Sqrt(PercentFill));
                strFormat.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap; //|| StringFormatFlags.LineLimit || StringFormatFlags.MeasureTrailingSpaces;
                strFormat.Trimming = StringTrimming.None;
                SizeF layout = new SizeF(((float)img.Width) * SQRTFill, ((float)img.Height) * 1.5F); // fit to width, allow height to go over
                Console.WriteLine(String.Format("Target size: {0}, {1}", layout.Width, layout.Height));
                s = g.MeasureString(OverlayText, f, layout, strFormat, out charFit, out linesFit);

                // Reduce size until it actually fits...
                // Most text only has to be reduced 1 or 2 times.
                if ((s.Height > (float)(img.Height) * SQRTFill) || (s.Width > layout.Width))
                {
                    do
                    {
                        Console.WriteLine(String.Format("Reducing font size: area required = {0}, {1}", s.Width, s.Height));
                        zoom = Math.Max((s.Height / (((float)img.Height) * SQRTFill)), s.Width / layout.Width);
                        zoom = f.Size / zoom;
                        if (zoom > 16F) zoom = (float)(Math.Floor(zoom)); // use a whole number to reduce "jaggies"
                        if (zoom >= f.Size) zoom -= 1;
                        f = new Font(OverlayFont.FontFamily, zoom, sty);
                        s = g.MeasureString(OverlayText, f, layout, strFormat, out charFit, out linesFit);
                        if (zoom <= 1) break; // bail
                    } while ((s.Height > (float)(img.Height) * SQRTFill) || (s.Width > layout.Width));
                }
                Console.WriteLine(String.Format("Final Font Size: {0}, area: {1}, {2}", f.Size, s.Width, s.Height));

                // Determine draw area based on placement
                RectangleF rect;
                switch (Position)
                {
                    case ContentAlignment.TopLeft: // =1
                        rect = new RectangleF(f.Size * 0.15F,
                            (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.TopCenter: // =2
                        rect = new RectangleF((bmp.Width - s.Width) / 2F,
                            (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.TopRight: // =4
                        rect = new RectangleF((bmp.Width - s.Width) - (f.Size * 0.1F),
                            (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.MiddleLeft: // =16  huh?  where's 8?
                        rect = new RectangleF(f.Size * 0.15F,
                            (bmp.Height - s.Height) / 2F,
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.MiddleCenter: // =32
                        rect = new RectangleF((bmp.Width - s.Width) / 2F,
                            (bmp.Height - s.Height) / 2F,
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.MiddleRight: //=64
                        rect = new RectangleF((bmp.Width - s.Width) - (f.Size * 0.1F),
                            (bmp.Height - s.Height) / 2F,
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.BottomLeft: // =256  and 128?;
                        rect = new RectangleF(f.Size * 0.15F,
                            (bmp.Height - s.Height) - (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.BottomCenter: // =512
                        rect = new RectangleF((bmp.Width - s.Width) / 2F,
                            (bmp.Height - s.Height) - (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    case ContentAlignment.BottomRight: // =1024
                        rect = new RectangleF((bmp.Width - s.Width) - (f.Size * 0.1F),
                            (bmp.Height - s.Height) - (f.Size * 0.1F),
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                    default:
                        ;
                        rect = new RectangleF((bmp.Width - s.Width) / 2F,
                            (bmp.Height - s.Height) / 2F,
                            layout.Width,
                            ((float)img.Height) * SQRTFill);
                        break;
                }

                // Add rendering hint (thx to Thomas)
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                if (AddShadow)
                {
                    // Add "drop shadow" at half transparency and offset by 1/10 font size
                    SolidBrush shadow = new SolidBrush(Color.FromArgb((int)(alpha / 2.0), OverlayColor));
                    RectangleF sRect = new RectangleF((float)rect.X - (f.Size * 0.1F), (float)rect.Y - (f.Size * 0.1F), rect.Width, rect.Height);
                    g.DrawString(OverlayText, f, shadow, sRect, strFormat);
                }

                // finally, draw centered text!
                g.DrawString(OverlayText, f, b, rect, strFormat);

                // clean-up
                g.Dispose();
                b.Dispose();
                f.Dispose();
                return bmp;
            }
            else
            {
                // nothing to overlay!  regurgitate image
                return new Bitmap(img);
            }
        }
    }
}