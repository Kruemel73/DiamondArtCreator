using OfficeOpenXml.ConditionalFormatting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using GdiColor = System.Drawing.Color;

namespace Ravlyk.SAE.Drawing.Processor
{

    public class VisualPrintPreviewController : VisualBoxedController
	{
		public VisualPrintPreviewController(IImageProvider imageProvider, PatternGridPainter gridPainter, IList<CodedColor> orderedColors, Size imageBoxSize = default(Size))
			: base(imageProvider, imageBoxSize)
		{
			GridPainter = gridPainter;
			OrderedColors = orderedColors ?? SourceImage.Palette.OrderByDarknes().Cast<CodedColor>().ToList();
		}

		public bool isLizenzValid = SAEWizardSettings.Default.isLizenzValid;
		public bool isLizenzDemo = SAEWizardSettings.Default.isLizenzDemo;
		public bool isLizenzCommerc = SAEWizardSettings.Default.isLizenzCommerc;

		public bool showRulers = GridPainterSettings.Default.ShowRulers;
		public bool ShowLines = GridPainterSettings.Default.ShowLines;
		public bool cutLines = GridPainterSettings.Default.CutLines;


		IList<CodedColor> OrderedColors { get; }

		#region Properties

		public static int oldMax; //for correction colorcount in landscape mode

		const decimal MinCellSize = 2.0m;
		const decimal MaxCellSize = 20.0m;
		/// <summary>
		/// Printed cell size in mm.
		/// </summary>
		public decimal CellSizeMm
		{
			get { return cellSizeMm; }
			set
			{
				if (value < MinCellSize) { value = MinCellSize; }
				if (value > MaxCellSize) { value = MaxCellSize; }

				if (value != cellSizeMm)
				{
					cellSizeMm = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		decimal cellSizeMm = GridPainterSettings.Default.PrintCellSize;

		/// <summary>
		/// Number of pixels per mm.
		/// </summary>
		public decimal PixelsPerMm
		{
			get { return pixelsPerMm; }
			set
			{
				if (value < 1.0m) { value = 1.0m; }

				if (value != pixelsPerMm)
				{
					pixelsPerMm = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		decimal pixelsPerMm = 1.0m;

		/// <summary>
		/// Cell size in pixels.
		/// </summary>
		int CellWidth => (int)(CellSizeMm * PixelsPerMm);
		int RulerWidth => CellWidth * 2;

		int ColorRowHeight => (int)(4 * PixelsPerMm);  // Original is: (4 * PixelsPerMm)

		int ColorColumnNoWidth => ColorRowHeight * 2;
		int ColorColumnSymbolWidth => ColorRowHeight * 2;
		int ColorColumnCodeWidth => ColorRowHeight * 4;
		int ColorColumnColorWidth => ColorRowHeight * 4;
		int ColorColumnCountWidth => ColorRowHeight * 5;
		int ColorColumnBagsWidth => ColorRowHeight * 4;
		Size ColorRowSize => new Size(ColorColumnNoWidth + ColorColumnSymbolWidth + ColorColumnCodeWidth + ColorColumnColorWidth + ColorColumnCountWidth + ColorColumnBagsWidth, ColorRowHeight);
		Size ColorRowsPerPage => new Size(
			Math.Max((PagePrintSize.Width + ColorRowHeight) / (ColorRowSize.Width + ColorRowHeight), 1),
			Math.Max(PagePrintSize.Height / ColorRowSize.Height - 1, 1));

		/// <summary>
		/// Page printable size in pixels (after removing all margins).
		/// </summary>
		public Size PagePrintSize
		{
			get { return pagePrintSize; }
			set
			{
				if (value != pagePrintSize)
				{
					pagePrintSize = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		Size pagePrintSize;

		public Size PageCellsSize { get; private set; }

		public Size PrintSchemePagesCount { get; private set; }
		public int PrintPalettePagesCount { get; private set; }
		public int PagesCount => PrintSchemePagesCount.Width * PrintSchemePagesCount.Height + PrintPalettePagesCount;
		public int LegendCountColor = 0;

		public void SaveSettings()
		{
			//if (CellSizeMm == 2.9m) { CellSizeMm = 2.8m; }
			GridPainterSettings.Default.PrintCellSize = CellSizeMm;
			GridPainterSettings.Default.Save();
		}

		#endregion

		#region Update Visual Image

		public IDisposable SuspendChangePageCellsSize()
		{
			return DisposableLock.Lock(ref lockChangePageCellsSize);
		}

		DisposableLock lockChangePageCellsSize;

		protected override CodedImage CreateVisualImage()
		{
			return new CodedImage();
		}

		Size pageThumbnailSize;

		const int PreviewPagesMargin = 8;
		const int PreviewPageBorder = 4;

		readonly int BlackColorArgb = ColorBytes.ToArgb(0, 105, 105, 105);
		readonly int WhiteColorArgb = ColorBytes.ToArgb(0, 255, 255, 255);

		protected override void UpdateParameters()
		{
			base.UpdateParameters();

			if (PagePrintSize.Width <= 0 || PagePrintSize.Height <= 0) return;
			var newPageCellsSize = new Size((PagePrintSize.Width - RulerWidth) / CellWidth, (PagePrintSize.Height - RulerWidth) / CellWidth);
			if (!lockChangePageCellsSize.IsLocked() || newPageCellsSize.Width + 1 < PageCellsSize.Width || newPageCellsSize.Height + 1 < PageCellsSize.Height)
			{
				PageCellsSize = newPageCellsSize;
			}
			if (PageCellsSize.Width <= 0 || PageCellsSize.Height <= 0) return;

			PrintSchemePagesCount = new Size((SourceImage.Size.Width + PageCellsSize.Width - 1) / PageCellsSize.Width, (SourceImage.Size.Height + PageCellsSize.Height - 1) / PageCellsSize.Height);
			if (PrintSchemePagesCount.Width <= 0 || PrintSchemePagesCount.Height <= 0) return;

			PrintPalettePagesCount = Math.Max(((SourceImage.Palette.Count + 20 + ColorRowsPerPage.Width - 1) / ColorRowsPerPage.Width + ColorRowsPerPage.Height - 1) / ColorRowsPerPage.Height, 1);

			var palettePagesColumns = (PrintPalettePagesCount + PrintSchemePagesCount.Height - 1) / PrintSchemePagesCount.Height;
			var actualColumnsCount = PrintSchemePagesCount.Width + palettePagesColumns;

			var maxPageWidth = (ImageBoxSize.Width - PreviewPagesMargin * 3) / actualColumnsCount - PreviewPagesMargin;
			var maxPageHeight = (ImageBoxSize.Height - PreviewPagesMargin) / PrintSchemePagesCount.Height - PreviewPagesMargin;
			if (maxPageWidth <= 0 || maxPageHeight <= 0) return;

			pageThumbnailSize = PagePrintSize.Width * maxPageHeight > PagePrintSize.Height * maxPageWidth
				? new Size(maxPageWidth, PagePrintSize.Height * maxPageWidth / PagePrintSize.Width)
				: new Size(PagePrintSize.Width * maxPageHeight / PagePrintSize.Height, maxPageHeight);
			if (pageThumbnailSize.Width > maxPageWidth) { pageThumbnailSize.Width = maxPageWidth; }
			if (pageThumbnailSize.Height > maxPageHeight) { pageThumbnailSize.Height = maxPageHeight; }
		}

		protected override void UpdateVisualImageCore()
		{
			base.UpdateVisualImageCore();

			VisualImage.Size = ImageBoxSize;
			ImagePainter.FillRect(VisualImage, new Rectangle(0, 0, VisualImage.Size.Width, VisualImage.Size.Height), BlackColorArgb);

			if (PagePrintSize.Width <= 0 || PagePrintSize.Height <= 0 || PrintSchemePagesCount.Width <= 0 || PrintSchemePagesCount.Height <= 0 || pageThumbnailSize.Width <= 0 || pageThumbnailSize.Height <= 0)
			{
				return;
			}

			var imageVisualPartSize = new Size(pageThumbnailSize.Width - PreviewPageBorder * 2, pageThumbnailSize.Height - PreviewPageBorder * 2);

			var pageWithMarginHeight = pageThumbnailSize.Height + PreviewPagesMargin;
			var startY = (VisualImage.Size.Height - pageWithMarginHeight * PrintSchemePagesCount.Height + PreviewPagesMargin) / 2;

			var pageWithMarginWidth = pageThumbnailSize.Width + PreviewPagesMargin;
			var palettePagesColumns = (PrintPalettePagesCount + PrintSchemePagesCount.Height - 1) / PrintSchemePagesCount.Height;
			var actualColumnsCount = PrintSchemePagesCount.Width + palettePagesColumns;
			var startX = (VisualImage.Size.Width - pageWithMarginWidth * actualColumnsCount) / 2;
			if (startX < 0) { startX = 0; }

			var schemePagesCount = PrintSchemePagesCount.Width * PrintSchemePagesCount.Height;

			for (int row = 0, y = startY, srcY = 0;
				row < PrintSchemePagesCount.Height;
				row++, y += pageWithMarginHeight, srcY += PageCellsSize.Height)
			{
				for (int col = 0, x = startX, srcX = 0;
					col < actualColumnsCount;
					col++, x += pageWithMarginWidth, srcX += PageCellsSize.Width)
				{
					var actualX = x;
					if (col >= PrintSchemePagesCount.Width)
					{
						actualX += PreviewPageBorder * 2;
					}

					if (col < PrintSchemePagesCount.Width && imageVisualPartSize.Width > 0 && imageVisualPartSize.Height > 0)
					{
						ImagePainter.FillRect(VisualImage, new Rectangle(actualX, y, pageThumbnailSize.Width, pageThumbnailSize.Height), WhiteColorArgb);

						var sourceCropRect = new Rectangle(srcX, srcY, PageCellsSize.Width, PageCellsSize.Height);
						if (sourceCropRect.RightExclusive > SourceImage.Size.Width)
						{
							sourceCropRect.Width = SourceImage.Size.Width - srcX;
						}
						if (sourceCropRect.BottomExclusive > SourceImage.Size.Height)
						{
							sourceCropRect.Height = SourceImage.Size.Height - srcY;
						}
						var croppedImage = ImageCropper.Crop(SourceImage, sourceCropRect);

						var zoomSize = new Size(sourceCropRect.Width * imageVisualPartSize.Width / PageCellsSize.Width, sourceCropRect.Height * imageVisualPartSize.Height / PageCellsSize.Height);
						var zoomedPartImage = new ImageResampler().Resample(croppedImage, zoomSize, ImageResampler.FilterType.Box);

						ImageCopier.Copy(zoomedPartImage, VisualImage, new Point(actualX + PreviewPageBorder, y + PreviewPageBorder));

						PaintPageNumber(row * PrintSchemePagesCount.Width + col + 1, actualX, y);
					}
					else if (row * palettePagesColumns + col - PrintSchemePagesCount.Width + schemePagesCount < PagesCount)
					{
						ImagePainter.FillRect(VisualImage, new Rectangle(actualX, y, pageThumbnailSize.Width, pageThumbnailSize.Height), WhiteColorArgb);

						var lineX = actualX + PreviewPageBorder;
						var lineLength = pageThumbnailSize.Width / 2;
						var lastLineY = y + pageThumbnailSize.Height - PreviewPageBorder * 2;
						for (int lineY = y + PreviewPageBorder; lineY < lastLineY; lineY += 3)
						{
							ImagePainter.DrawHorizontalLine(VisualImage, lineX, lineY, lineLength, BlackColorArgb);
						}

						PaintPageNumber(schemePagesCount + row + (col - PrintSchemePagesCount.Width) * PrintSchemePagesCount.Height + 1, actualX, y);
					}
				}
			}
		}

		void PaintPageNumber(int number, int x, int y)
		{
			var numberString = number.ToString();
			var numberSize = PageNumberPainter.GetTextSize(numberString);
			ImagePainter.FillRect(VisualImage, new Rectangle(x, y, numberSize.Width + 4, numberSize.Height + 4), WhiteColorArgb);
			PageNumberPainter.PaintText(numberString, VisualImage.Pixels, VisualImage.Size, new Point(x + 2, y + 2));
		}

		TextPainter PageNumberPainter => pageNumberPainter ?? (pageNumberPainter = new TextPainter(GridPainter.SymbolsFont, 16));
		TextPainter pageNumberPainter;

		#endregion

		#region Paint Page

		public PatternGridPainter GridPainter { get; }

        public void PaintPrintPageImage(XGraphics gfx, IPainter painter, int page, Size pageSize, Rectangle margins, string appInfo, string fileInfo, bool isMystery,
			string isMysteryFile, bool SaveTemplateToImagefile, bool SymSizeFull, bool SymSizeNormal, bool line10Dbl, bool showRuler, bool showLines, bool cutLines,
			bool schemeLegend, bool schemeLogo, bool printScheme, bool legendLeft)
		{
			var lineWidth = Math.Max((int)(PixelsPerMm / 4m), 1);

			painter.FillRectangle(margins.Left, margins.Top, margins.Width, margins.Height, WhiteColorArgb);

			if (!schemeLegend) { PrintPalettePagesCount = 0; }
			using (painter.TranslateCoordinates(new Size(margins.Left, margins.Top)))
			{
				if (page < PagesCount - PrintPalettePagesCount)
				{
					//print a page of the scheme
					if (printScheme)
					{

						PaintSchemePage_Test(GridPainter, gfx, painter, page, new Size(margins.Width, margins.Height), new Rectangle(margins.Left, margins.Top, margins.Width, margins.Height),
							new Rectangle(0, 0, margins.Width, margins.Height), lineWidth, SymSizeFull, SymSizeNormal, line10Dbl, showRuler, showLines, cutLines, schemeLegend, 
							schemeLogo, legendLeft);
					}
				}
				else
				{
					// print color palette
					if (schemeLegend)
					{
						PaintPalettePage(gfx, painter, page, lineWidth, margins, fileInfo, isMystery, isMysteryFile, schemeLegend);
					}

				}
			}
			if (!SaveTemplateToImagefile)
			{
				var infoY = margins.BottomExclusive + (int)(CellSizeMm / 2);
				var textSize = (int)(PixelsPerMm * 3);
				var textPainter = new TextPainter(GridPainter.SymbolsFont, (int)(PixelsPerMm * 3));

				painter.PaintText(appInfo, new Point(margins.Left, infoY), textSize, spaceBetweenCharacters: lineWidth);
				if (fileInfo == null) { fileInfo = "preview";  }
				var fileInfoWidth = textPainter.GetTextSize(fileInfo, spaceBetweenCharacters: lineWidth).Width;
				painter.PaintText(fileInfo, new Point(margins.RightExclusive - fileInfoWidth, infoY), textSize, spaceBetweenCharacters: lineWidth);

				var pageNo = String.Format(Resources.PrintPageNo, page + 1, PagesCount);
				var pageNoWidth = textPainter.GetTextSize(pageNo, spaceBetweenCharacters: lineWidth).Width;
				painter.PaintText(pageNo, new Point(margins.Left + (margins.Width - pageNoWidth) / 2, infoY), textSize, spaceBetweenCharacters: lineWidth);
			}
			if (fileInfo == "preview") { fileInfo = null; }
		}

		public void PaintTopPage(XGraphics gfx, IPainter painter, Size pageSize, Rectangle margins, string fileInfo, string owner, string company, string logo, PdfPage page, bool isMystery, string isMysteryFile)
		{
			painter.FillRectangle(margins.Left, margins.Top, margins.Width, margins.Height, WhiteColorArgb);
			
			var zoomedColorRowHeight = ColorRowHeight;
			var zoomedColorRowWidth = ColorRowSize.Width;

			var thumbnailMaxSize = zoomedColorRowHeight * 30;
			var sourceImageMaxSize = Math.Max(SourceImage.Size.Width, SourceImage.Size.Height);
			var thumbnailSize = new Size(SourceImage.Size.Width * thumbnailMaxSize / sourceImageMaxSize, SourceImage.Size.Height * thumbnailMaxSize / sourceImageMaxSize);
			System.Drawing.Bitmap img;

			// Deckblatt Hochkant			
			if (page.Orientation == PdfSharp.PageOrientation.Portrait)
			{
                //var filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DiamondArtCreator\\PreviewImage.jpg";
				var filename = Path.Combine(Path.GetTempPath(), "PreviewImage.jpg");
                //print thumbnail Image
                if (isMystery)
				{
					img = new System.Drawing.Bitmap(isMysteryFile);
				}
				else
				{
					img = new System.Drawing.Bitmap(filename);
				}
				MemoryStream strm = new MemoryStream();
				img.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
				XImage xfoto = XImage.FromStream(strm);

				XRect rect = new XRect(0, 150, thumbnailSize.Width, thumbnailSize.Height);
				double rest = pageSize.Width / 2 - thumbnailSize.Width / 2;
				gfx.DrawImage(xfoto, rest, rect.Y, rect.Width, rect.Height);

				xfoto.Dispose();

				// print picture name
				var infoY = 70;
				var textSize = 24;
				rect = new XRect(margins.Left, 110, margins.Width, 400);
				XFont font = new XFont("Arial", 26);
				XBrush brush = XBrushes.Black;
				XStringFormat format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				gfx.DrawString(fileInfo, font, brush, rect, format);

				//
				//print owner
				//
				infoY = 0;
				textSize = 16;
				if (isLizenzValid && isLizenzCommerc || isLizenzDemo)
				{
					if (logo.Trim(' ', '/').EndsWith("jpg"))
					{
						XImage image = XImage.FromFile(logo.Trim(' ', '/'));
						XRect ret = new XRect(0, 0, image.Width, image.Height);
						double rt = pageSize.Width / 2 - image.Width / 2;
						gfx.DrawImage(image, rt, 15, ret.Width, ret.Height);
					}
					else
					{
						if (owner != "" || company != "")
						{
							rect = new XRect(margins.Left, 10, margins.Width, 20);
							font = new XFont("Arial", 15);
							string printText = "Erstellt von:";
							gfx.DrawString(printText, font, brush, rect, format);
							printText = "";
							rect = new XRect(margins.Left, 45, margins.Width, 30);
							font = new XFont("Arial", 25);
							if (owner != "") { printText = owner; }
							if (company != "") { printText = company; }
							if (owner != "" && company != "") { printText = owner + " / " + company; }
							gfx.DrawString(printText, font, brush, rect, format);
						}
					}
				}
				else
				{
					if (owner != "" || company != "")
					{
						rect = new XRect(margins.Left, 10, margins.Width, 20);
						font = new XFont("Arial", 15);
						string printText = "Erstellt von:";
						gfx.DrawString(printText, font, brush, rect, format);

						rect = new XRect(margins.Left, 45, margins.Width, 30);
						font = new XFont("Arial", 25);
						if (owner != "") { printText = owner; }
						if (company != "") { printText = company; }
						if (owner != "" && company != "") { printText = owner + " / " + company; }
						gfx.DrawString(printText, font, brush, rect, format);
					}
				}

				//print "Farbpalette: DMC"
				rect = new XRect(margins.Left, 520, margins.Width, 400);
				font = new XFont("Arial", 10);
				brush = XBrushes.Black;
				format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				gfx.DrawString(string.Format(Resources.PrintThreadsName, SourceImage.Palette.Name), font, brush, rect, format);

				// print scheme size
				infoY += 520 + zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, margins.Width, 400);
				int cmwidth = (int)(SourceImage.Size.Width * (cellSizeMm)) / 10;
				int cmheight = (int)(SourceImage.Size.Height * (cellSizeMm)) / 10;
				string text = string.Format("Groesse: " + SourceImage.Size.Width) + " x " + SourceImage.Size.Height + " Steine / " + cmwidth + " x " + cmheight + " Cm";
				gfx.DrawString(text, font, brush, rect, format);

				//print form of stones
				infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, margins.Width, 400);
				bool stormform = false;
				if (SAEWizardSettings.Default.RoundStones) { stormform = true; }
				if (SAEWizardSettings.Default.SquareStones) { stormform = false; }
				if (stormform) { text = string.Format("Steinform: Rund"); }
                if (!stormform) { text = string.Format("Steinform: Eckig"); }
                gfx.DrawString(text, font, brush, rect, format);


                //print colors total
                infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, margins.Width, 400);
				int colorstotal = SourceImage.Palette.Count;
				gfx.DrawString("Anzahl Farben: " + colorstotal, font, brush, rect, format);

				//print printed pages total
				infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, margins.Width, 400);
				gfx.DrawString("Anzahl Seiten (Ohne Deckblatt): " + PagesCount, font, brush, rect, format);

				//print site index
				infoY += 50;
				var indexTextSize = textSize * 2 / 3;
				var indexCellSize = new Size(textSize, indexTextSize * 4 / 3);
				rect = new XRect(margins.Left, infoY, margins.Width, 400);
				var indexY = infoY + 10;
				gfx.DrawString(Resources.PrintPagesIndex, font, brush, rect, format);

				indexY += zoomedColorRowHeight;
				for (int row = 0, index = 1; row < PrintSchemePagesCount.Height; row++, indexY += indexCellSize.Height)
				{
					for (int col = 0, indexX = margins.Left; col < PrintSchemePagesCount.Width; col++, index++, indexX += indexCellSize.Width)
					{
						rect = new XRect(indexX - ((PrintSchemePagesCount.Width * indexCellSize.Width) / 2), indexY, margins.Width, 400);
						font = new XFont("Arial", indexTextSize);
						gfx.DrawString(index.ToString(), font, brush, rect, format);
					}
				}

				// Deckblatt quer
			} else if (page.Orientation == PdfSharp.PageOrientation.Landscape)
			{
                //var filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DiamondArtCreator\\PreviewImage.jpg";
                var filename = Path.Combine(Path.GetTempPath(), "PreviewImage.jpg");
                //print thumbnail Image
                if (isMystery)
				{
					img = new System.Drawing.Bitmap(isMysteryFile);
				}
				else
				{
                    img = new System.Drawing.Bitmap(filename);
                    //img = new System.Drawing.Bitmap(SourceImage.SourceImageFileName);
				}
				MemoryStream strm = new MemoryStream();
				img.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
				XImage xfoto = XImage.FromStream(strm);

				XRect rect = new XRect(0, 150, thumbnailSize.Width, thumbnailSize.Height);
				double rest = pageSize.Width / 2 - thumbnailSize.Width / 2;
				gfx.DrawImage(xfoto, rest, rect.Y, rect.Width, rect.Height);
				var startY = thumbnailSize.Height / 2;
				xfoto.Dispose();

                // print picture name
                var infoY = 70;
				var textSize = 24;
				rect = new XRect(margins.Left, 110, margins.Width, 400);
				XFont font = new XFont("Arial", 30);
				XBrush brush = XBrushes.Black;
				XStringFormat format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				gfx.DrawString(fileInfo, font, brush, rect, format);

				// print owner
				infoY = 0;
				textSize = 16;
				if (isLizenzValid && isLizenzCommerc || isLizenzDemo)
				{
					if (logo.Trim(' ', '/').EndsWith("jpg"))
					{
						XImage image = XImage.FromFile(logo.Trim(' ', '/'));
						double x = (pageSize.Width - image.PixelWidth * 72 / image.HorizontalResolution) / 2;
						gfx.DrawImage(image, x, infoY + 15);
					}
					else
					{
						if (owner != "" || company != "")
						{
							rect = new XRect(margins.Left, 10, margins.Width, 20);
							font = new XFont("Arial", 15);
							string printText = "Erstellt von:";
							gfx.DrawString(printText, font, brush, rect, format);
							printText = "";
							rect = new XRect(margins.Left, 45, margins.Width, 30);
							font = new XFont("Arial", 25);
							if (owner != "") { printText = owner; }
							if (company != "") { printText = company; }
							if (owner != "" && company != "") { printText = owner + " / " + company; }
							gfx.DrawString(printText, font, brush, rect, format);
							//painter.PaintText(printText, new Point(margins.Left, infoY), textSize);
						}
					}
				}
				else
				{
					if (owner != "" || company != "")
					{
						rect = new XRect(margins.Left, 10, margins.Width, 20);
						font = new XFont("Arial", 15);
						string printText = "Erstellt von:";
						gfx.DrawString(printText, font, brush, rect, format);

						rect = new XRect(margins.Left, 45, margins.Width, 30);
						font = new XFont("Arial", 25);
						if (owner != "") { printText = owner; }
						if (company != "") { printText = company; }
						if (owner != "" && company != "") { printText = owner + " / " + company; }
						gfx.DrawString(printText, font, brush, rect, format);
						//painter.PaintText(printText, new Point(margins.Left, infoY), textSize);
					}
				}

				//print "Farbpalette: DMC"
				//infoY = 300;
				infoY = 150 + startY;
				rect = new XRect(margins.Left, infoY, 100, 100);
				font = new XFont("Arial", 10);
				brush = XBrushes.Black;
				format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				gfx.DrawString(string.Format(Resources.PrintThreadsName, SourceImage.Palette.Name), font, brush, rect, format);

				// print scheme size
				infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, 100, 100);
				int cmwidth = (int)(SourceImage.Size.Width * (cellSizeMm)) / 10;
				int cmheight = (int)(SourceImage.Size.Height * (cellSizeMm)) / 10;
				string text = string.Format("Groesse: " + SourceImage.Size.Width) + " x " + SourceImage.Size.Height + " Steine / " + cmwidth + " x " + cmheight + " Cm";
				gfx.DrawString(text, font, brush, rect, format);


				//print colors total
				infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, 100, 100);
				int colorstotal = SourceImage.Palette.Count;
				gfx.DrawString("Anzahl Farben: " + colorstotal, font, brush, rect, format);

				//print printed pages total
				infoY += zoomedColorRowHeight;
				rect = new XRect(margins.Left, infoY, 100, 400);
				gfx.DrawString("Anzahl Seiten (Ohne Deckblatt): " + PagesCount, font, brush, rect, format);

				//print site index
				infoY = 150 + startY;
				var indexTextSize = textSize * 2 / 3;
				var indexCellSize = new Size(textSize, indexTextSize * 4 / 3);
				rect = new XRect(margins.Width - 100, infoY, 100, 100);

				var indexY = infoY + 10;
				gfx.DrawString(Resources.PrintPagesIndex, font, brush, rect, format);
				//painter.PaintText(Resources.PrintPagesIndex, new Point(startIndexX, indexY), indexTextSize);

				indexY += zoomedColorRowHeight;
				for (int row = 0, index = 1; row < PrintSchemePagesCount.Height; row++, indexY += indexCellSize.Height)
				{
					for (int col = 0, indexX = margins.Width - 100; col < PrintSchemePagesCount.Width; col++, index++, indexX += indexCellSize.Width)
					{
						rect = new XRect(indexX - ((PrintSchemePagesCount.Width * indexCellSize.Width) / 2), indexY, 100, 100);
						font = new XFont("Arial", indexTextSize);
						gfx.DrawString(index.ToString(), font, brush, rect, format);
						//painter.PaintText(index.ToString(), new Point(indexX, indexY), indexTextSize);
					}
					//}
				}

			}
		}

		public void paintEtiketten(PatternGridPainter gridPainter, XGraphics pdfGraphics, IPainter painter, Size pageSize, Rectangle margins, int symbolCountX, int symbolCountY, Size EtiketteSize,
			int symbolPage, int pageCount, string appInfo, string fileInfo, bool EtiUmrandung, int VL, int VO)
		{
			//using (painter.TranslateCoordinates(new Size(margins.Left, margins.Top)))
			//{
			var lineWidth = Math.Max((int)(PixelsPerMm / 4m), 1);
			var gfx = pdfGraphics;
			XBrush brush = XBrushes.Black;
			XPen pen = new XPen(XColors.Black, 0.1);
			var maxSymbols = OrderedColors.Count;
			var startNo = symbolPage * symbolCountX * symbolCountY;
			//if (symbolPage != 0) { startNo += symbolPage * 15; }
			for (var y = 0; y < symbolCountY; y++)
			{
				for (var x = 0; x < symbolCountX; x++)
				{
					if (startNo < maxSymbols)
					{
						var xpos = 31 + VL + (x * (EtiketteSize.Width + 8));
						var ypos = 45 + VO + (y * (EtiketteSize.Height + 8));
						//gfx.DrawRectangle(pen, xpos, ypos, symbolSize.Width, symbolSize.Height);

						//draw Circle
						XRect rect = new XRect(xpos, ypos, 28, 28);
						if (EtiUmrandung)
						{
							gfx.DrawArc(pen, rect, 0, 360);
						}
						DrawSymbolsTable(gridPainter, gfx, painter, new Point(xpos + 4, ypos + 2), OrderedColors, startNo, GridPainter.NumbersArgb);
					}
					startNo++;
				}
				if (startNo >= maxSymbols) { break; }


			}
			//}
		}
		public void paintEtikettenTicTac(PatternGridPainter gridPainter, XGraphics pdfGraphics, IPainter painter, Size pageSize, Rectangle margins, int symbolCountX, int symbolCountY, Size EtiketteSize,
			int symbolPage, int pageCount, string appInfo, string fileInfo, bool EtiUmrandung, int VL, int VO)
		{
			var lineWidth = Math.Max((int)(PixelsPerMm / 4m), 1);
			var gfx = pdfGraphics;
			XBrush brush = XBrushes.Black;
			XPen pen = new XPen(XColors.Black, 0.1);
			var maxSymbols = OrderedColors.Count;
			var startNo = symbolPage * symbolCountX * symbolCountY;

			for (var y = 0; y < symbolCountY; y++)
			{
				for (var x = 0; x < symbolCountX; x++)
				{
					if (startNo < maxSymbols)
					{
						var xpos = 12 + VL + (x * (EtiketteSize.Width + 8));
						var ypos = 36 + VO + (y * (EtiketteSize.Height));
						//gfx.DrawRectangle(pen, xpos, ypos, symbolSize.Width, symbolSize.Height);

						//draw border
						XRect rect = new XRect(xpos, ypos, 50, 28);
						if (EtiUmrandung)
						{
							gfx.DrawRectangle(pen, rect);
						}
						DrawSymbolsTableTicTac(gridPainter, gfx, painter, new Point(xpos + 4, ypos + 2), OrderedColors, startNo, GridPainter.NumbersArgb);
					}
					startNo++;
				}
				if (startNo >= maxSymbols) { break; }


			}
		}


		static void DrawSymbolsTable(PatternGridPainter gridPainter, XGraphics gfx, IPainter painter, Point point, IList<CodedColor> orderedColors, int startNo, int numbersArgb)
		{
			DrawEtikett(gridPainter, gfx, painter, new Point(point.X, point.Y), orderedColors[startNo], 0, numbersArgb);
		}

		static void DrawSymbolsTableTicTac(PatternGridPainter gridPainter, XGraphics gfx, IPainter painter, Point point, IList<CodedColor> orderedColors, int startNo, int numbersArgb)
		{
			DrawEtikettTicTac(gridPainter, gfx, painter, new Point(point.X, point.Y), orderedColors[startNo], 0, numbersArgb);
		}

		static void DrawEtikett(PatternGridPainter gridPainter, XGraphics gfx, IPainter painter, Point point, CodedColor color, int startNo, int numbersArgb)
		{
			if (color != null)
			{
				XRect rect = new XRect(point.X, point.Y, 15, 15);
				XFont font = new XFont("DiamondArtCreator", 15);
				XBrush brush = XBrushes.Black;
				//XBrush brush = new XSolidBrush(XColor.FromArgb(gridPainter.GetBackColorArgb(color)));
				XPen pen = new XPen(XColors.Black);
				painter.FillRectangle(point.X + 3, point.Y + 3, 15, 12, gridPainter.GetBackColorArgb(color));
				//gfx.DrawEllipse(pen, brush, point.X + 1, point.Y - 2, 17, 17);

				font = new XFont("DiamondArtCreator", 18);
				rect = new XRect(point.X, point.Y, 18, 18);
				XStringFormat format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				var colorrgb = gridPainter.GetSymbolColorArgb(color);
				if (colorrgb == 16777215) { colorrgb = 16777210; }
				painter.PaintSymbol(color.SymbolChar, new Point(point.X + 4, point.Y + 2), 15, 15, colorrgb);


				rect = new XRect(point.X, point.Y + 17, 20, 10);
				font = new XFont("Arial", 6);
				brush = XBrushes.Black;
				if (!SAEWizardSettings.Default.WithoutDMC)
				{
					gfx.DrawString(color.ColorCode, font, brush, rect, format);
				}
			}
		}

		static void DrawEtikettTicTac(PatternGridPainter gridPainter, XGraphics gfx, IPainter painter, Point point, CodedColor color, int startNo, int numbersArgb)
		{
			if (color != null)
			{
				XRect rect = new XRect(point.X, point.Y, 15, 15);
				XFont font = new XFont("DiamondArtCreator", 15);
				XBrush brush = XBrushes.Black;
				//XBrush brush = new XSolidBrush(XColor.FromArgb(gridPainter.GetBackColorArgb(color)));
				XPen pen = new XPen(XColors.Black);
				painter.FillRectangle(point.X + 3, point.Y + 3, 15, 17, gridPainter.GetBackColorArgb(color));
				//gfx.DrawEllipse(pen, brush, point.X + 1, point.Y - 2, 17, 17);

				font = new XFont("DiamondArtCreator", 20);
				rect = new XRect(point.X, point.Y, 20, 20);
				XStringFormat format = new XStringFormat();
				format.Alignment = XStringAlignment.Center;
				var colorrgb = gridPainter.GetSymbolColorArgb(color);
				if (colorrgb == 16777215) { colorrgb = 16777210; }
				painter.PaintSymbol(color.SymbolChar, new Point(point.X + 3, point.Y + 3), 18, 18, colorrgb);


				rect = new XRect(point.X + 20, point.Y + 5, 20, 10);
				font = new XFont("Arial", 10);
				brush = XBrushes.Black;
				if (!SAEWizardSettings.Default.WithoutDMC)
				{
					gfx.DrawString(color.ColorCode, font, brush, rect, format);
				}
			}
		}
		void PaintSchemePage(IPainter painter, int page, Size canvasSize, Rectangle clipRect, int lineWidth)
		{
			var pageRow = page / PrintSchemePagesCount.Width;
			var pageCol = page - pageRow * PrintSchemePagesCount.Width;
			var gridRow = pageRow * PageCellsSize.Height;
			var gridCol = pageCol * PageCellsSize.Width;

			var originalCellSize = GridPainter.CellSize;
			var originalLineWidth = GridPainter.LineWidth;

			try
			{
				GridPainter.CellSize = CellWidth;
				GridPainter.LineWidth = lineWidth;
				GridPainter.Paint(painter, canvasSize, clipRect, new Point(gridCol, gridRow), false, PageCellsSize);
				try
				{
					var centerRow = SourceImage.Size.Height / 2;
					if (centerRow >= gridRow && centerRow < gridRow + PageCellsSize.Height)
					{
						var x = -CellWidth - lineWidth * 2;
						var y = GridPainter.RulerWidth + (centerRow - gridRow) * CellWidth;
						if (SourceImage.Size.Height % 2 == 0)
						{
							y -= CellWidth / 2;
						}

						painter.PaintSymbol('>', new Point(x, y), CellWidth, CellWidth, GridPainter.Line10Argb);
					}

					var centerCol = SourceImage.Size.Width / 2;
					if (centerCol >= gridCol && centerCol < gridCol + PageCellsSize.Width)
					{
						var x = GridPainter.RulerWidth + (centerCol - gridCol) * CellWidth;
						var y = -CellWidth - lineWidth * 2;
						if (SourceImage.Size.Width % 2 == 0)
						{
							x -= CellWidth / 2;
						}

						painter.PaintSymbol('v', new Point(x, y), CellWidth, CellWidth, GridPainter.Line10Argb);
					}
				}
				catch
				{
					//Markers are not important, and scheme itself is already painted, ignore any exception here.
				}
			}
			finally
			{
				GridPainter.LineWidth = originalLineWidth;
				GridPainter.CellSize = originalCellSize;
			}
		}

		void PaintSchemePage_Test(PatternGridPainter gridPainter, XGraphics gfx, IPainter painter, int page, Size canvasSize, Rectangle margins, Rectangle clipRect, int lineWidth, bool SymSizeFull,
			bool SymSizeNormal, bool line10Dbl, bool showRuler, bool showLines, bool cutLines, bool schemeLegend, bool schemeLogo, bool legendLeft)
		{
			int LegendSize = 14;
			int LegendStartColorCount = 0;
			int LegendMaxColorPerPage = 60;
			int ActualLegendColor = 0;
			int LegendPageCount = SourceImage.Palette.Count / LegendMaxColorPerPage;
			if ((LegendPageCount * LegendMaxColorPerPage) < SourceImage.Palette.Count)
			{
				LegendPageCount += 1;
			}

			var LineArgb = GridPainterSettings.Default.LineArgb.ToArgb();
			var Line5Argb = GridPainterSettings.Default.Line5Argb.ToArgb();
			var Line10Argb = GridPainterSettings.Default.Line10Argb.ToArgb();
			var Line10DblWidth = line10Dbl;
			var ShowRuler = showRuler;
			var ShowLines = showLines;
			var CutLines = cutLines;
			var SchemeLegend = schemeLegend;
			var SchemeLogo = schemeLogo;

			var Numbers = GridPainterSettings.Default.NumbersArgb.ToArgb();

			decimal CellSize = cellSizeMm * pixelsPerMm;

			var pageRow = page / PrintSchemePagesCount.Width;
			var pageCol = page - pageRow * PrintSchemePagesCount.Width;

			var CellsPerCol = 0;
			var CellsPerRow = 0;
			var StartCol = 0;
			var EndCol = 0;
			var StartRow = 0;
			var EndRow = 0;
			var LegendWidth = 100;

			if (isLizenzValid && legendLeft && pageCol == 0)
			{
				margins.Left += LegendWidth;
				canvasSize.Width -= LegendWidth;
				CellsPerCol = (int)Math.Floor(canvasSize.Height / CellSize);
				CellsPerRow = (int)Math.Floor(canvasSize.Width / CellSize);

				StartCol = pageRow * CellsPerCol;
				EndCol = pageRow * CellsPerCol + CellsPerCol;

				StartRow = pageCol * CellsPerRow;
				EndRow = pageCol * CellsPerRow + CellsPerRow;

                SAEWizardSettings.Default.EndRow = EndRow;
            }
			else
			{
                CellsPerCol = (int)Math.Floor(canvasSize.Height / CellSize);
                CellsPerRow = (int)Math.Floor(canvasSize.Width / CellSize);
                StartCol = pageRow * CellsPerCol;
				EndCol = pageRow * CellsPerCol + CellsPerCol;
				StartRow = pageCol * CellsPerRow;
				EndRow = pageCol * CellsPerRow + CellsPerRow;
				if (StartRow != 0)
				{
                    if (StartRow > SAEWizardSettings.Default.EndRow)
					{
                        int difference = StartRow - SAEWizardSettings.Default.EndRow;
                        StartRow -= difference;
                        EndRow -= difference;
                    }
				}
                SAEWizardSettings.Default.EndRow = EndRow;
            }
			if (EndCol >= GridPainter.Image.Size.Height)
			{
				EndCol = GridPainter.Image.Size.Height;
				//EndCol = image.Size.Height;
			}

			if (EndRow >= GridPainter.Image.Size.Width)
			{
				//EndRow = image.Size.Width;
				EndRow = GridPainter.Image.Size.Width;
			}



			decimal xStartPos = margins.Left;
			decimal yStartPos = margins.Top;
			int CS = (int)CellSize;
			if (SymSizeFull) { CS = (int)CellSize; }
			if (SymSizeNormal) { CS = (int)CellSize - 1; }

			var font = new XFont("DiamondArtCreator", CS);
			var brush = XBrushes.Black;
			XRect rect = new XRect();
			XStringFormat format = new XStringFormat();
			XPen pen = new XPen(XColor.FromArgb(LineArgb), 0.1);

			int maxColors = SourceImage.Palette.Count;
            
			// Array für Informationen (ColorCode, SymbolChar, ColorCount)
            (string ColorCode, char SymbolChar, int ColorCount)[] pageInfos = new (string, char, int)[maxColors];
			

            for (var x = StartRow; x <= EndRow - 1; x++)
			{
				pen = new XPen(XColor.FromArgb(LineArgb), 0.1);
				decimal xPosL = xStartPos + (x * CellSize - StartRow * CellSize);

				for (var y = StartCol; y <= EndCol - 1; y++)
				{
					var codedColor = GridPainter.Image[x, y];

					decimal yPosO = yStartPos + ((y * CellSize) - (StartCol * CellSize));

					rect = new XRect((double)xPosL, (double)yPosO, (double)CellSize, (double)CellSize);
					var bColor = GdiColor.FromArgb(GridPainter.GetBackColorArgb(codedColor));
					brush = new XSolidBrush(XColor.FromArgb(bColor.A, bColor.R, bColor.G, bColor.B));

					gfx.DrawRectangle(brush, rect);

					format.Alignment = XStringAlignment.Center;
					format.LineAlignment = XLineAlignment.Near;
					var cColor = GdiColor.FromArgb(GridPainter.GetSymbolColorArgb(codedColor));
					brush = new XSolidBrush(XColor.FromArgb(cColor.A, cColor.R, cColor.G, cColor.B));
					gfx.DrawString(codedColor.SymbolChar.ToString(), font, brush, rect, format);

                    // Überprüfen, ob ColorCode bereits im Array vorhanden ist
                    int existingIndex = Array.FindIndex(pageInfos, pInfo => pInfo.ColorCode != null && pInfo.ColorCode == codedColor.ColorCode);
                    if (existingIndex != -1)
                    {
                        // Wenn ColorCode bereits vorhanden ist, erhöhe ColorCount
                        pageInfos[existingIndex].ColorCount += 1;
                    }
                    else
                    {
                        // Wenn ColorCode nicht vorhanden ist, füge neue Informationen hinzu
                        AddNewPageInfo(pageInfos, codedColor.ColorCode, codedColor.SymbolChar, 1);
                    }
                }
                
            }
			if (isLizenzValid)
			{
				Array.Sort(pageInfos);

				int counter = 1;
				foreach (var pageInfo in pageInfos)
				{
					if (pageInfo.ColorCode != null)
					{
						Console.WriteLine($"Counter: {counter}, ColorCode: {pageInfo.ColorCode}, SymbolChar: {pageInfo.SymbolChar}, ColorCount: {pageInfo.ColorCount}");
					}
					counter++;
				}
                // write page infos to file
                var filename =  Path.Combine(Path.GetTempPath(), (page + 1) + "-site.sym");
                using (FileStream fs = File.OpenWrite(filename))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    foreach(var info in pageInfos)
					{
						if (info.ColorCode != null)
						{
							string WriteToLine = info.ColorCode.ToString() +","+ info.SymbolChar.ToString() +","+ info.ColorCount.ToString();
							sw.WriteLine(WriteToLine);
						}
					}
					sw.Close();
                }
            }

            if (ShowLines)
			{
				// vertical lines
				for (var x = 0; x <= CellsPerRow; x++)
				{
					double xPosL = (double)xStartPos + ((x * (double)CellSize));
					double yPosO = (double)yStartPos + ((EndCol * (double)CellSize) - StartCol * (double)CellSize);

					//if (x + StartRow > image.Size.Width)
					if (x + StartRow > GridPainter.Image.Size.Width)
					{
						break;
					}
					if ((x + StartRow) % 5 == 0)
					{
						if ((x + StartRow) % 10 == 0)
						{
							if (Line10DblWidth)
							{
								pen = new XPen(XColor.FromArgb(Line10Argb), 0.3);
								gfx.DrawLine(pen, xPosL, margins.Top, xPosL, yPosO);
							}
							else
							{
								pen = new XPen(XColor.FromArgb(Line10Argb), 0.1);
								gfx.DrawLine(pen, xPosL, margins.Top, xPosL, yPosO);
							}
						}
						else
						{
							pen = new XPen(XColor.FromArgb(Line5Argb), 0.1);
							gfx.DrawLine(pen, xPosL, margins.Top, xPosL, yPosO);
						}
					}
					else
					{
						pen = new XPen(XColor.FromArgb(LineArgb), 0.1);
						gfx.DrawLine(pen, xPosL, margins.Top, xPosL, yPosO);
					}

				}
				//horizontal lines
				for (var y = 0; y <= CellsPerCol; y++)
				{
					double yPosO = (double)yStartPos + ((y * (double)CellSize));
					double xPosR = (double)xStartPos + ((EndRow * (double)CellSize) - StartRow * (double)CellSize);

					//if(y + StartCol > image.Size.Height)
					if (y + StartCol > GridPainter.Image.Size.Height)
					{
						break;
					}
					if ((y + StartCol) % 5 == 0)
					{
						if ((y + StartCol) % 10 == 0)
						{
							if (Line10DblWidth)
							{
								pen = new XPen(XColor.FromArgb(Line10Argb), 0.3);
								gfx.DrawLine(pen, margins.Left, yPosO, xPosR, yPosO);
							}
							else
							{
								pen = new XPen(XColor.FromArgb(Line10Argb), 0.1);
								gfx.DrawLine(pen, margins.Left, yPosO, xPosR, yPosO);
							}
						}
						else
						{
							pen = new XPen(XColor.FromArgb(Line5Argb), 0.1);
							gfx.DrawLine(pen, margins.Left, yPosO, xPosR, yPosO);
						}
					}
					else
					{
						pen = new XPen(XColor.FromArgb(LineArgb), 0.1);
						gfx.DrawLine(pen, margins.Left, yPosO, xPosR, yPosO);
					}
				}


			}

			if (ShowRuler)
			{
				//PaintRulerCorner(painter, canvasClipRect);
				pen = new XPen(XColor.FromArgb(Numbers), 0.1);
				gfx.DrawLine(pen, margins.Left, margins.Top, margins.Left, margins.Top - 10);
				gfx.DrawLine(pen, margins.Left, margins.Top, margins.Left - 10, margins.Top);
                //PaintHRuler(painter, canvasClipRect, imageClipRect.Left, imageClipRect.RightExclusive);
                for (int xR = 1; xR <= CellsPerRow; xR++)
				{
					rect = new XRect((double)(margins.Left + ((xR - 1) * CellSize)), (double)margins.Top - 10, (double)CellSize, 10);

					var txt = StartRow + xR;

                    if (txt > GridPainter.Image.Size.Width)
					{
						break;
					}
					if (legendLeft && pageCol == 0)
					{
						painter.PaintText(txt.ToString(), new Point(LegendWidth + (int)((xR - 1) * CellSize), -2), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.VerticalUpward);
					}
					else
					{
                        //painter.PaintText(txt.ToString(), new Point((int)rect.X, (int)rect.Y - 2), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.VerticalUpward);
                        //painter.PaintText(txt.ToString(), new Point(margins.Left, margins.Top), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.VerticalUpward);
                        painter.PaintText(txt.ToString(), new Point((int)((xR - 1) * CellSize) + 1, -2), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.VerticalUpward);

                    }

                    gfx.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);

				}

				//PaintVRuler(painter, canvasClipRect, imageClipRect.Top, imageClipRect.BottomExclusive);
				for (int xR = 1; xR <= CellsPerCol; xR++)
				{
					rect = new XRect((double)(margins.Left - 10), (double)margins.Top + ((xR - 1) * (double)CellSize), 10, (double)CellSize);

					var txt = StartCol + xR;

					if (txt > GridPainter.Image.Size.Height)
					{
						break;
					}
					if (legendLeft && pageCol == 0)
					{
						painter.PaintText(txt.ToString(), new Point(LegendWidth - (int)rect.Width, 0 + (int)((xR - 1) * CellSize)), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.LeftToRight);
					}
					else
					{
                        painter.PaintText(txt.ToString(), new Point(0 - (int)rect.Width, 0 + (int)((xR - 1) * CellSize) + 1), 4, Numbers, spaceBetweenCharacters: 1, FontBasePainter.TextDirection.LeftToRight);
					}
					gfx.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y);

				}
			}
			if (CutLines)
			{
				pen = new XPen(XColor.FromArgb(Line10Argb), 0.2);
				//top left
				if (legendLeft && pageCol == 0)
				{
					gfx.DrawLine(pen, 0, margins.Top, margins.Left - LegendWidth, margins.Top);
					gfx.DrawLine(pen, margins.Left - LegendWidth, 0, margins.Left - LegendWidth, margins.Top);
				}
				else
				{
					gfx.DrawLine(pen, 0, margins.Top, margins.Left, margins.Top);
					gfx.DrawLine(pen, margins.Left, 0, margins.Left, margins.Top);
				}
				//top right
				double xPosR = (double)xStartPos + ((EndRow * (double)CellSize) - StartRow * (double)CellSize);
				gfx.DrawLine(pen, xPosR, 0, xPosR, margins.Top);
				gfx.DrawLine(pen, xPosR, margins.Top, xPosR + 30, margins.Top);
				// buttom left
				double yPosO = (double)yStartPos + ((EndCol * (double)CellSize) - StartCol * (double)CellSize);
				if (legendLeft && pageCol == 0)
				{
					gfx.DrawLine(pen, margins.Left - LegendWidth, yPosO, margins.Left - LegendWidth, yPosO + 30);
					gfx.DrawLine(pen, 0, yPosO, margins.Left - LegendWidth, yPosO);
				}
				else
				{
					gfx.DrawLine(pen, margins.Left, yPosO, margins.Left, yPosO + 30);
					gfx.DrawLine(pen, 0, yPosO, margins.Left, yPosO);
				}
				//buttom right
				yPosO = (double)yStartPos + ((EndCol * (double)CellSize) - StartCol * (double)CellSize);
				xPosR = (double)xStartPos + ((EndRow * (double)CellSize) - StartRow * (double)CellSize);
				gfx.DrawLine(pen, xPosR, yPosO, xPosR + 30, yPosO);
				gfx.DrawLine(pen, xPosR, yPosO, xPosR, yPosO + 30);
			}

			// Legend on scheme
			if (isLizenzCommerc || SAEWizardSettings.Default.FT6 || SAEWizardSettings.Default.FT8)
			{
				if (legendLeft && pageCol == 0)
				{
					int LegendCellSize = 10;
					int LegendTableWidth = LegendWidth - 20;

					int LegendColorNumberWidth = 20;
					int legendSymbolWidth = 20;
					int LegendColorDMCWidth = 40;

					int LegendTableStart = margins.Left - LegendWidth;

					int LegendColorNumberStart = LegendTableStart;
					int LegendSymbolStart = LegendTableStart + LegendColorNumberWidth;
					int LegendDMCStart = LegendTableStart + LegendColorNumberWidth + legendSymbolWidth;

					LegendMaxColorPerPage = margins.Height / LegendCellSize;

					pen = new XPen(XColors.Black, 0.2);
					brush = XBrushes.Black;

					if (LegendMaxColorPerPage > SourceImage.Palette.Count) { LegendMaxColorPerPage = SourceImage.Palette.Count; }
					int StartColor = 0 + LegendCountColor;

					if (StartColor < SourceImage.Palette.Count)
					{
						for (int LeftLegendColorNr = 0; LeftLegendColorNr < LegendMaxColorPerPage; LeftLegendColorNr++)
						{
							int LegendColorNr = LegendCountColor + 1;
							if (LegendColorNr > SourceImage.Palette.Count) { return; }

							// draw table
							gfx.DrawRectangle(pen, LegendColorNumberStart, margins.Top + (LeftLegendColorNr * LegendCellSize), LegendColorNumberWidth, LegendCellSize);
							gfx.DrawRectangle(pen, LegendSymbolStart, margins.Top + (LeftLegendColorNr * LegendCellSize), legendSymbolWidth, LegendCellSize);
							gfx.DrawRectangle(pen, LegendDMCStart, margins.Top + (LeftLegendColorNr * LegendCellSize), LegendColorDMCWidth, LegendCellSize);

							//draw color count
							painter.PaintText(LegendColorNr.ToString(), new Point(5, LeftLegendColorNr * LegendCellSize), 8, BlackColorArgb);
							
							//draw symbol && colored background
							
                            var zoomedColorRowHeight = ColorRowHeight;
                            var zoomedColorRowWidth = ColorRowSize.Width;
                            var zoomedColorRowWidthWithSpacer = zoomedColorRowWidth + zoomedColorRowHeight;

                            var startNo = LegendColorNr - 1;

                            var x = LegendTableStart;
                            var startY = (LeftLegendColorNr * LegendCellSize);

                            CodedColor color = OrderedColors[startNo];
							int numbersArgb = GridPainter.NumbersArgb;
                            var textArgb = color == null ? numbersArgb : 0;
                            var symbolSize = LegendCellSize;

							Point point = new Point(x, startY);

							if (color != null)
							{
								LineArgb = GridPainterSettings.Default.LineArgb.ToArgb();
                                font = new XFont("DiamondArtCreator", 8);
                                 brush = XBrushes.Black;
                                rect = new XRect();

                                format = new XStringFormat();

                                decimal yPosO = margins.Top + point.Y + 1;
                                x = x + LegendColorNumberWidth + symbolSize;

                                rect = new XRect((double)5 + LegendSymbolStart, (double)yPosO, (double)symbolSize, (double)symbolSize - 2);
                                var bColor = GdiColor.FromArgb(gridPainter.GetBackLegend(color));
                                brush = new XSolidBrush(XColor.FromArgb(bColor.A, bColor.R, bColor.G, bColor.B));
                                gfx.DrawRectangle(brush, rect);

                                format.Alignment = XStringAlignment.Center;
                                format.LineAlignment = XLineAlignment.Near;
                                var cColor = GdiColor.FromArgb(gridPainter.GetSymbolLegend(color));
                                brush = new XSolidBrush(XColor.FromArgb(cColor.A, cColor.R, cColor.G, cColor.B));
                                gfx.DrawString(color.SymbolChar.ToString(), font, brush, rect, format);

                                rect = new XRect((double)LegendDMCStart, (double)yPosO, (double)LegendColorDMCWidth, (double)symbolSize - 2);
                                brush = new XSolidBrush(XColor.FromName("Black"));
								format.Alignment = XStringAlignment.Center;
                                format.LineAlignment = XLineAlignment.Near;
                                gfx.DrawString(color != null ? color.ColorCode : Resources.PrintColumnCode, font, brush, rect, format);

                            }
                            //draw ColorCode
                            //painter.PaintText(color != null ? color.ColorCode : Resources.PrintColumnCode, new Point(5 + LegendDMCStart, point.Y), 7, BlackColorArgb);
                            
							LegendCountColor++;

						}
					}
				}
			}
		}

        static void AddNewPageInfo((string, char, int)[] pageInfos, string colorCode, char symbolChar, int colorCount)
        {
            // Fügt neue Informationen zum Array hinzu
            int index = Array.FindIndex(pageInfos, pInfo => pInfo.Item1 == null);

            if (index != -1)
            {
                pageInfos[index] = (colorCode, symbolChar, colorCount);
            }
            else
            {
                Console.WriteLine("Maximale Anzahl von Farben erreicht.");
            }
        }

        public void PrintPageInfos()
		{

		}

        void PaintPalettePage(XGraphics gfx, IPainter painter, int page, int lineWidth, Rectangle margins, string fileinfo, bool isMystery, string isMysteryFile, bool schemeLegend)
		{

			var palettePage = page - PagesCount + PrintPalettePagesCount;
			var zoomedColorRowHeight = ColorRowHeight;
			var zoomedColorRowWidth = ColorRowSize.Width;
			var zoomedColorRowWidthWithSpacer = zoomedColorRowWidth + zoomedColorRowHeight;

			if (palettePage == 0)
			{
				var thumbnailMaxSize = zoomedColorRowHeight * 9;
				var sourceImageMaxSize = Math.Max(SourceImage.Size.Width, SourceImage.Size.Height);
				var thumbnailSize = new Size(SourceImage.Size.Width * thumbnailMaxSize / sourceImageMaxSize, SourceImage.Size.Height * thumbnailMaxSize / sourceImageMaxSize);
				var thumbnailImage = new ImageResampler().Resample(SourceImage, thumbnailSize, ImageResampler.FilterType.Box);
				if (isMystery) 
				{ 
					var img = new System.Drawing.Bitmap(isMysteryFile);

					MemoryStream strm = new MemoryStream();
					img.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
					XImage xfoto = XImage.FromStream(strm);

					XRect rect = new XRect(0, 150, thumbnailSize.Width, thumbnailSize.Height);
					gfx.DrawImage(xfoto, 0, 0, rect.Width, rect.Height);
					xfoto.Dispose();
				}
				else
				{
					painter.PaintImage(thumbnailImage, new Point(0, 0));
				}

				var infoX = thumbnailMaxSize + zoomedColorRowHeight;
				var infoY = 0;
				var textSize = zoomedColorRowHeight - lineWidth * 2;

				// print "Farbpalette: DMC"
				painter.PaintText(string.Format("Bild: " + fileinfo, SourceImage.Palette.Name), new Point(infoX, infoY), textSize);

				infoY += zoomedColorRowHeight;
				int cmwidth = (int)(SourceImage.Size.Width * (cellSizeMm)) / 10;
				int cmheight = (int)(SourceImage.Size.Height * (cellSizeMm)) / 10;
				painter.PaintText(string.Format(Resources.PrintThreadsName, SourceImage.Palette.Name), new Point(infoX, infoY), textSize);

				// print scheme size
				infoY += zoomedColorRowHeight;
				cmwidth = (int)(SourceImage.Size.Width * (cellSizeMm)) / 10;
				cmheight = (int)(SourceImage.Size.Height * (cellSizeMm)) / 10;
				string text = string.Format("Groesse: " + SourceImage.Size.Width) + " x " + SourceImage.Size.Height + " Steine / " + cmwidth + " x " + cmheight + " Cm";
				painter.PaintText(text, new Point(infoX, infoY), textSize);

				//print colors total
				infoY += zoomedColorRowHeight;
				int colorstotal = SourceImage.Palette.Count;
				painter.PaintText("Anzahl Farben: " + colorstotal, new Point(infoX, infoY), textSize);

				//print printed pages total
				infoY += zoomedColorRowHeight;
				painter.PaintText("Anzahl Seiten: " + PagesCount, new Point(infoX, infoY), textSize);

				var indexTextSize = textSize * 2 / 3;
				var indexCellSize = new Size(textSize, indexTextSize * 4 / 3);
				var startIndexX = margins.Width - PrintSchemePagesCount.Width * indexCellSize.Width - 20;
				var indexY = 0;
				painter.PaintText(Resources.PrintPagesIndex, new Point(startIndexX, indexY), indexTextSize);
				indexY += textSize;
				for (int row = 0, index = 1; row < PrintSchemePagesCount.Height; row++, indexY += indexCellSize.Height)
				{
					for (int col = 0, indexX = startIndexX; col < PrintSchemePagesCount.Width; col++, index++, indexX += indexCellSize.Width)
					{
						painter.PaintText(index.ToString(), new Point(indexX, indexY), indexTextSize);
					}
				}
			}

			var startNo = palettePage * ColorRowsPerPage.Width * ColorRowsPerPage.Height;
			if (palettePage > 0)
			{
				startNo -= 20;
			}
			var startY = palettePage > 0 ? 0 : zoomedColorRowHeight * 10;
			var rowsPerPage = palettePage > 0 ? ColorRowsPerPage.Height : ColorRowsPerPage.Height - 10;

			for (int col = 0, x = 0; col < ColorRowsPerPage.Width; col++, x += zoomedColorRowWidthWithSpacer, startNo += rowsPerPage)
			{
				DrawColorsTable(painter, new Point(x, startY), OrderedColors, startNo, startNo + rowsPerPage,
					lineWidth, ColorRowSize, GridPainter.NumbersArgb, GridPainter.LineArgb, GridPainter.Line10Argb,
					ColorColumnNoWidth, ColorColumnSymbolWidth, ColorColumnCodeWidth, ColorColumnColorWidth, ColorColumnBagsWidth);
			}
		}

		#region Draw color rows
		public static void DrawColorsTable(IPainter painter, Point point, IList<CodedColor> orderedColors,
			int lineWidth, int rowHeight, int numbersArgb, int lineArgb, int line10Argb)
		{
			var colorColumnNoWidth = rowHeight * 3;
			var colorColumnSymbolWidth = rowHeight * 2;
			var colorColumnCodeWidth = rowHeight * 6;
			var colorColumnColorWidth = rowHeight * 4;
			var colorColumnCountWidth = rowHeight * 5;
			var colorColumnBagsWidth = rowHeight * 7;

			var rowSize = new Size(colorColumnNoWidth + colorColumnSymbolWidth + colorColumnCodeWidth + colorColumnColorWidth + colorColumnCountWidth + colorColumnBagsWidth, rowHeight);

			DrawColorsTable(painter, point, orderedColors, 0, orderedColors.Count,
				lineWidth, rowSize, numbersArgb, lineArgb, line10Argb,
				colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth, colorColumnBagsWidth);
		}

		public static int GetColorRowWidth(int rowHeight)
		{
			var colorColumnNoWidth = rowHeight * 3;
			var colorColumnSymbolWidth = rowHeight * 2;
			var colorColumnCodeWidth = rowHeight * 2;
			var colorColumnColorWidth = rowHeight * 2;
			var colorColumnCountWidth = rowHeight * 2;
			var colorColumnBagsWidth = rowHeight * 1;
			return colorColumnNoWidth + colorColumnSymbolWidth + colorColumnCodeWidth + colorColumnColorWidth + colorColumnCountWidth + colorColumnBagsWidth;
		}

		static void DrawColorsTable(IPainter painter, Point point, IList<CodedColor> orderedColors, int from, int toExclusive,
			int lineWidth, Size rowSize, int numbersArgb, int lineArgb, int line10Argb,
			int colorColumnNoWidth, int colorColumnSymbolWidth, int colorColumnCodeWidth, int colorColumnColorWidth, int colorColumnBagsWidth)
		{
			DrawColorRow(painter, new Point(point.X, point.Y), lineWidth, 0, null,
				rowSize, numbersArgb, lineArgb, line10Argb,
				colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth, colorColumnBagsWidth);

			if (from > oldMax) { from = oldMax; toExclusive -= 10; } // correct last color while printing in landscape
			for (int startNo = from, y = point.Y + rowSize.Height; startNo < toExclusive && startNo < orderedColors.Count; startNo++, y += rowSize.Height)
			{
				DrawColorRow(painter, new Point(point.X, y), lineWidth, startNo + 1, orderedColors[startNo],
					rowSize, numbersArgb, lineArgb, line10Argb,
					colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth, colorColumnBagsWidth);
			}
			oldMax = toExclusive; // save value for correcting last color while printing in landscape

		}

		static void DrawColorRow(IPainter painter, Point point, int lineWidth, int no, CodedColor color,
			Size rowSize, int numbersArgb, int lineArgb, int line10Argb,
			int colorColumnNoWidth, int colorColumnSymbolWidth, int colorColumnCodeWidth, int colorColumnColorWidth, int colorColumnBagsWidth)
		{
			var zoomedColorRowHeight = rowSize.Height;
			var zoomedColorRowWidth = rowSize.Width;

			var textArgb = color == null ? numbersArgb : 0;
			var textSize = zoomedColorRowHeight - lineWidth * 2;
			var symbolSize = zoomedColorRowHeight - lineWidth;

			var x = point.X;
			painter.PaintText(color != null ? no.ToString() : "#", new Point(x, point.Y), textSize, argb: textArgb);

			x += colorColumnNoWidth;
			if (color != null)
			{
				painter.PaintSymbol(color.SymbolChar, new Point(x, point.Y + lineWidth), symbolSize, zoomedColorRowHeight);
			}

			x += colorColumnSymbolWidth;
			painter.PaintText(color != null ? color.ColorCode : Resources.PrintColumnCode, new Point(x, point.Y), textSize, argb: textArgb);

			x += colorColumnCodeWidth;
			var colorColumnWidth = colorColumnColorWidth;
			if (color != null)
			{
				painter.FillRectangle(x, point.Y + lineWidth, colorColumnWidth * 2 / 3, zoomedColorRowHeight - lineWidth, color.Argb);
			}
			else
			{
				painter.PaintText(Resources.PrintColumnColor, new Point(x, point.Y), textSize, argb: textArgb);
			}

			x += colorColumnWidth;
			painter.PaintText(color?.OccurrencesCount.ToString() ?? Resources.PrintColumnCount, new Point(x, point.Y), textSize, argb: textArgb);

			x += colorColumnBagsWidth;
			if (color != null)
			{
				var bags = (color?.OccurrencesCount + ((color.OccurrencesCount / 100) * SAEWizardSettings.Default.Reserve)) / SAEWizardSettings.Default.Bagsize + 1;
				painter.PaintText(bags.ToString() ?? Resources.PrintColumnBags, new Point(x, point.Y), textSize, argb: textArgb);
			}
            else
            {
				painter.PaintText(Resources.PrintColumnBags, new Point(x, point.Y), textSize, argb: textArgb);
			}

				painter.DrawHorizontalLine(point.X, point.Y + zoomedColorRowHeight, zoomedColorRowWidth, color == null ? line10Argb : lineArgb, lineWidth);
			
		}

		#endregion
	}
    #endregion
}
