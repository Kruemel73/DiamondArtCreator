using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Internal;
using PdfSharp.Pdf;
using Ravlyk.Common;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Drawing.Properties;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Rectangle = Ravlyk.Common.Rectangle;
using Size = Ravlyk.Common.Size;



namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class PrintUserControl : UserControl
	{
		public enum SizeUnit
		{
			Mm,
			Inch
		}

		static PrintUserControl()
		{
			SaeFontResolver.Setup();
		}

		public bool isLizenzValid;
		public bool isLizenzDemo;
		public bool isLizenzCommerc;
		public int PlateHight;
        public int PlateWidth;

        public PrintUserControl()
		{
			InitializeComponent();

			printDocument = new PrintDocument { DocumentName = Resources.SaeFileDescription };
			printDocument.PrintPage += PrintDocument_PrintPage;
			printDocument.DefaultPageSettings.Landscape = Settings.Default.PrintPageLandscape;
			printDocument.DefaultPageSettings.Margins = Settings.Default.PrintPageMargins;

			isLizenzValid = Settings.Default.isLizenzValid;
			isLizenzDemo = Settings.Default.isLizenzDemo;
			isLizenzCommerc = Settings.Default.isLizenzCommerc;


		}

		readonly PrintDocument printDocument;
		PrintPreviewControl previewControl;

		public IList<string> GetAvailablePrinters(out string defaultPrinter)
		{
			defaultPrinter = printDocument.PrinterSettings.PrinterName;
			return PrinterSettings.InstalledPrinters.Cast<string>().ToList();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Settings.Default.PrintPageMargins = printDocument.DefaultPageSettings.Margins;
				Settings.Default.PrintPageLandscape = printDocument.DefaultPageSettings.Landscape;
				Settings.Default.Save();

				Controller?.SaveSettings();

				printDocument?.Dispose();
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Controller

		public VisualPrintPreviewController Controller
		{
			get { return visualControlPrintPreview.Controller as VisualPrintPreviewController; }
			set
			{
				if (value != null && value != visualControlPrintPreview.Controller)
				{
					value.ImageBoxSize = new Size(visualControlPrintPreview.Width, visualControlPrintPreview.Height);

					using (value.SuspendUpdateVisualImage())
					{
						UpdatePagePrintSize(value);
					}

					printDocument.DocumentName = value.SourceImage.Description;
				}

				visualControlPrintPreview.Controller = value;
			}
		}

		public void UpdatePagePrintSize(VisualPrintPreviewController controller, bool keepPageCellsSize = false)
		{
			if (controller == null)
			{
				return;
			}
			var pageSettings = printDocument.DefaultPageSettings;
			using (controller.SuspendUpdateVisualImage())
			{
				var pageSize = new Size(
					pageSettings.Bounds.Width - pageSettings.Margins.Left - pageSettings.Margins.Right,
					pageSettings.Bounds.Height - pageSettings.Margins.Top - pageSettings.Margins.Bottom);
				if (ForPdf)
				{
					pageSize = new Size(pageSize.Width * 72 / 100, pageSize.Height * 72 / 100);
				}
				using (keepPageCellsSize ? controller.SuspendChangePageCellsSize() : null)
				{
					controller.PagePrintSize = pageSize;
					controller.PixelsPerMm = (ForPdf ? 72m : 100m) / 25.4m;
				}
			}

			printDocument.PrinterSettings.MinimumPage = 1;
			printDocument.PrinterSettings.FromPage = 1;
			printDocument.PrinterSettings.MaximumPage = controller.PagesCount;
			printDocument.PrinterSettings.ToPage = controller.PagesCount;
		}

		public bool ForPdf { get; set; }

		#endregion

		#region Print options

		public void SetPrinter(string printerName)
		{
			printDocument.PrinterSettings.PrinterName = printerName;
			UpdatePagePrintSize(Controller);
			ResetPreviewControl();
		}

		public decimal CellSize
		{
			get { return Controller.CellSizeMm / CellSizeToMmCoeff; }
			set
			{
				Controller.CellSizeMm = value * CellSizeToMmCoeff;
				UpdatePagePrintSize(Controller);
				ResetPreviewControl();
			}
		}

		public SizeUnit Unit { get; set; }

		decimal CellSizeToMmCoeff => Unit == SizeUnit.Inch ? 25.4m : 1m;

		public void OpenPageSetup()
		{
			ResetPreviewControl();
			using (var pageSetupDialog = new PageSetupDialog
			{
				Document = printDocument,
				EnableMetric = true,
				AllowPrinter = !ForPdf
			})
			{
				if (ForPdf)
				{
					SelectVirtualPrinter(printDocument.PrinterSettings);
				}

				if (pageSetupDialog.ShowDialog() == DialogResult.OK)
				{
					UpdatePagePrintSize(Controller);
				}
			}
		}

		static void SelectVirtualPrinter(PrinterSettings printreSettings)
		{
			var virtualPrinterNameParts = new[] { "PDF", "XPS" };
			if (virtualPrinterNameParts.Any(namePart => printreSettings.PrinterName.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0))
			{
				return;
			}

			foreach (var namePart in virtualPrinterNameParts)
			{
				var virtualPrinterName = PrinterSettings.InstalledPrinters.Cast<string>().FirstOrDefault(printerName => printerName.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0);
				if (!string.IsNullOrEmpty(virtualPrinterName))
				{
					printreSettings.PrinterName = virtualPrinterName;
					return;
				}
			}
		}

		#endregion

		#region Print preview

		bool showPreview;

		public void ShowPreview(bool show)
		{
			currentPage = 0;
			showPreview = show;
			UpdatePreviewControl();
		}

		void ResetPreviewControl()
		{
			showPreview = false;
			UpdatePreviewControl();
		}

		void UpdatePreviewControl()
		{
			if (showPreview)
			{
				visualControlPrintPreview.Visible = false;

				originalForPdf = ForPdf;
				ForPdf = false;
				UpdatePagePrintSize(Controller, originalForPdf);

				previewControl = new PrintPreviewControl
				{
					Document = printDocument,
					Dock = DockStyle.Fill
				};

				Controls.Add(previewControl);
				previewControl.BringToFront();
				previewControl.Hide();

			}
			else
			{
				if (previewControl != null)
				{
					ForPdf = originalForPdf;
					UpdatePagePrintSize(Controller);

					previewControl.Dispose();
					previewControl = null;
				}
				visualControlPrintPreview.Visible = true;
			}
		}

		bool originalForPdf;

		public int PreviewPage
		{
			get { return previewControl?.StartPage + 1 ?? 1; }
			set
			{
				if (previewControl != null && value > 0 && value <= Controller.PagesCount)
				{
					previewControl.StartPage = value - 1;
				}
			}
		}

		public void SetZoom(string zoomValue)
		{
			if (previewControl != null)
			{
				if (zoomValue == Resources.PrintPreviewZoomAuto)
				{
					previewControl.AutoZoom = true;
				}
				else
				{
					double newZoom;
					if (double.TryParse(zoomValue.Replace("%", "").Trim(), out newZoom))
					{
						previewControl.AutoZoom = false;
						previewControl.Zoom = newZoom / 100d;
					}
				}
			}
		}

		#endregion

		#region Print

		public void Print()
		{
			ResetPreviewControl();

			using (var printDialog =
				new PrintDialog
				{
					Document = printDocument,
					AllowSomePages = true,
					AllowCurrentPage = true,
					AllowSelection = true,
					UseEXDialog = true
				})
			{
				if (printDialog.ShowDialog() == DialogResult.OK)
				{
					UpdatePagePrintSize(Controller);
					currentPage = 0;
					Dock = DockStyle.Fill;
					printDocument.Print();

					Dispose();
				}
			}
		}

		void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			
			var realPage = currentPage;
			if (printDocument.PrinterSettings.PrintRange == PrintRange.SomePages)
			{
				realPage += printDocument.PrinterSettings.FromPage - 1;
			}

			using (var painter = new GdiPainter(e.Graphics, IndexedImageExtensions.ToBitmap, true)
			{
				FontFamily = FontHelper.GetFontFamily(Controller.GridPainter.SymbolsFont.Name)
			})
			{
				var document1 = new PdfDocument();
				var page1 = document1.AddPage();
				var pdfGraphics = XGraphics.FromPdfPage(page1);
				bool isMystery = Settings.Default.Mystery;
				string isMysteryFile = Settings.Default.UserPalettesLocation + @"\Mystery.jpg";
		
				
				Controller.PaintPrintPageImage(
					pdfGraphics,
					painter,
					realPage,
					new Size(e.PageBounds.Width, e.PageBounds.Height),
					new Rectangle(e.MarginBounds.X, e.MarginBounds.Y, e.MarginBounds.Width, e.MarginBounds.Height),
					AppInfo.AppDescription,
					Controller.SourceImage.Description ?? Resources.UnsavedImageDescription,
					isMystery,
					isMysteryFile, true,
					Settings.Default.SymFull,
					Settings.Default.SymNormal,
					Settings.Default.Line10Dbl,
					Settings.Default.ShowRulers,
					Settings.Default.ShowLines,
					Settings.Default.CutLines,
					Settings.Default.SchemeLegend,
					Settings.Default.SchemeLogo,
                    Settings.Default.PrintScheme,
					Settings.Default.LegendLeft
                    ); // the last true only if this is called
			}

			currentPage++;
			e.HasMorePages =
				currentPage < Controller.PagesCount &&
				(printDocument.PrinterSettings.PrintRange == PrintRange.AllPages || realPage < printDocument.PrinterSettings.ToPage - 1);
			
		}

		int currentPage;

		#endregion

		#region Pdf

		internal string PreselectedPdfFileName { get; set; }

		public void SaveToPdf(CodedImage image, PatternGridPainter gridPainter, bool SaveTemplateToImagefile)
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			if (SaveTemplateToImagefile)
			{
				saveDialog.Filter = Resources.FileFilterImage;
			}
			else
			{
				saveDialog.Filter = Resources.FileFilterPDF;
			}
			if (showPreview || SaveTemplateToImagefile)
			{
				saveDialog.FileName = Settings.Default.UserPalettesLocation + "\\preview.pdf";
				if(File.Exists(saveDialog.FileName)) { File.Delete(saveDialog.FileName); }
				var fileName = saveDialog.FileName;
				CreatePDF(image, gridPainter, fileName, SaveTemplateToImagefile);
			}
			else
			{
				using (saveDialog = new SaveFileDialog { Filter = Resources.FileFilterPDF, FilterIndex = 0, FileName = PreselectedPdfFileName })
				{
					foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
					{
						var ps = new PrinterSettings { PrinterName = installedPrinter };
						var maxResolution = ps.DefaultPageSettings.PaperSize.ToString();
						Console.WriteLine("{0}: {1}x{2}", installedPrinter, maxResolution, null);
					}
					
					if (saveDialog.ShowDialog() == DialogResult.OK)
					{

						PreselectedPdfFileName = string.Empty;
						var wasForPdf = ForPdf;

						Cursor = Cursors.WaitCursor;
						try
						{
							CreatePDF(image, gridPainter, saveDialog.FileName, false);
						}
						catch (IOException ex)
						{
							MessageBox.Show(Resources.ErrorSavePdf + Environment.NewLine + ex.Message);
							ForPdf = wasForPdf;
							UpdatePagePrintSize(Controller);
							Cursor = DefaultCursor;
						}
					}
				}
			}
		}

		void CreatePDF(CodedImage image, PatternGridPainter gridPainter, string fileName, bool SaveTemplateToImagefile)
        {
			var document = new PdfDocument();
            Settings.Default.PictureName = image.Description;
            //show pdf-settings page
            if (isLizenzValid)
			{
				if (!showPreview && (Settings.Default.FT3 || Settings.Default.FT6 || Settings.Default.FT10 || isLizenzCommerc || isLizenzDemo))
				{
					using (var pdfSettings = new PDFSettings())
					{
						DialogResult dr = pdfSettings.ShowDialog();
						if (dr == DialogResult.OK)
						{
							pdfSettings.Dispose();
						}
						else
						{
							// exit pdf-save when clicked on cancel
							this.Dispose();
							return;
						}
					}
				}
				else
                {
					Settings.Default.Mystery = false;
					Settings.Default.TicTacEti = false;
					Settings.Default.RundEti = false;
					Settings.Default.PrintDeckblatt = false;
					Settings.Default.PrintScheme = true;
					Settings.Default.SchemeLegend = true;
					Settings.Default.LegendLeft = false;
                }
				if (Settings.Default.PictureName == "")
				{
					document.Info.Title = Controller.SourceImage.Description;
				}
				else
				{
					document.Info.Title = Settings.Default.PictureName;
				}

				if (Settings.Default.Owner == "")
				{
					document.Info.Author = SystemInformation.UserName;
				}
				else
				{
					document.Info.Author = Settings.Default.Owner;
				}
				if (Settings.Default.OwnerPassword != "")
				{
					document.SecuritySettings.OwnerPassword = Settings.Default.OwnerPassword;
					document.Info.Creator = AppInfo.AppDescription + " (" + Settings.Default.Company + ")";
					document.SecuritySettings.PermitPrint = Settings.Default.PermitPrint;
					document.SecuritySettings.PermitModifyDocument = Settings.Default.PermitModifyDocument;
					document.SecuritySettings.PermitAssembleDocument = Settings.Default.PermitAssembleDocument;
					document.SecuritySettings.PermitExtractContent = Settings.Default.PermitExtractContent;
					document.SecuritySettings.PermitAccessibilityExtractContent = Settings.Default.PermitAccessibilityExtractContent;
					document.SecuritySettings.PermitAnnotations = Settings.Default.PermitAnnotations;
					document.SecuritySettings.PermitFormsFill = Settings.Default.PermitFormsFill;
					document.SecuritySettings.PermitFullQualityPrint = Settings.Default.PermitFullQualityPrint;
				}
				if (Settings.Default.PDFPassword != "")
				{
					document.SecuritySettings.UserPassword = Settings.Default.PDFPassword;
				}
			}
			else
			{
				Settings.Default.EtiUmrandung = false;
				Settings.Default.PrintEtiketten = false;
				Settings.Default.PrintDeckblatt = false;
				Settings.Default.PrintScheme = true;
				Settings.Default.LegendLeft = false;
				Settings.Default.SchemeLegend = true;

				document.Info.Title = Controller.SourceImage.Description;
				document.Info.Author = SystemInformation.UserName;
				document.Info.Creator = AppInfo.AppDescription + " (" + AppInfo.AppVersion + ")";
			}

			//if saving to TIFF
			if (SaveTemplateToImagefile)
			{
                Settings.Default.EtiUmrandung = false;
                Settings.Default.PrintEtiketten = false;
                Settings.Default.PrintDeckblatt = false;
                Settings.Default.PrintScheme = true;
            }

			document.Info.CreationDate = DateTime.Now;
			document.Info.ModificationDate = document.Info.CreationDate;
			document.Info.Subject = Resources.SaeFileDescription;
			
			var pageSettings = printDocument.DefaultPageSettings;
			var pageMargins = pageSettings.Margins;

			var pageWidth = new XUnit(pageSettings.PaperSize.Width * 72.0 / 100.0, XGraphicsUnit.Point);
			var pageHeight = new XUnit(pageSettings.PaperSize.Height * 72.0 / 100.0, XGraphicsUnit.Point);

			if (pageSettings.Landscape)
			{
				var x = pageWidth;
				pageWidth = pageHeight;
				pageHeight = x;
			}
			var pageSize = new Size((int)pageWidth.Point, (int)pageHeight.Point);

			var marginsRect = new Rectangle(
				pageMargins.Left * 72 / 100,
				pageMargins.Top * 72 / 100,
				pageSize.Width - (pageMargins.Left + pageMargins.Right) * 72 / 100,
				pageSize.Height - (pageMargins.Top + pageMargins.Bottom) * 72 / 100);

			ForPdf = true;
			UpdatePagePrintSize(Controller);

			string ptext;
			if (showPreview)
            {
				ptext = Resources.CreatePreview;
            }
            else
            {
				ptext = Resources.PrintSavingToPdf;
            }

			using (var form = new Form
			{
				Text = ptext,
				Size = new System.Drawing.Size(200, 130),
				TopMost = true,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				ShowIcon = false,
				ControlBox = false
			})
			{
				using (var painter = new PdfSharpPainter(IndexedImageExtensions.ToBitmap)
				{
					FontName = Controller.GridPainter.SymbolsFont.Name,
					FontFamily = FontHelper.GetFontFamily(Controller.GridPainter.SymbolsFont.Name)
				})
				{
					var progressLabel = new Label { Location = new System.Drawing.Point(32, 24), AutoSize = true };
					form.Controls.Add(progressLabel);
					form.Show();

					//Deckblatt
					if (Settings.Default.PrintDeckblatt)
					{
						if (isLizenzValid)
						{
							if (isLizenzDemo || isLizenzCommerc || Settings.Default.FT3 || Settings.Default.FT6)
							{

								progressLabel.Text = Resources.Deckblatt + " " + Controller.SourceImage.Description;
								form.Invalidate();
								form.Update();
								var page = document.AddPage();
								var pdfGraphics = XGraphics.FromPdfPage(page);

								page.Orientation = pageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
								page.Width = pageWidth;
								page.Height = pageHeight;
								painter.PdfGraphics = pdfGraphics; // Keep same painter for different pages to share fonts


								string PDFOwner = Settings.Default.Owner;
								string PDFCompany = Settings.Default.Company;
								string PDFLogo = Settings.Default.Logo;
								bool isMystery1 = Settings.Default.Mystery;
								string isMysteryFile1 = Settings.Default.UserPalettesLocation + @"\Mystery.jpg";
								if (Settings.Default.MysteryPic != "")
								{
									isMysteryFile1 = Settings.Default.MysteryPic;
								}

								Controller.PaintTopPage(pdfGraphics, painter, pageSize, marginsRect, Settings.Default.PictureName, PDFOwner, PDFCompany, PDFLogo, page, isMystery1, isMysteryFile1);
                                
                            }
						}
					}
					// Scheme- and Color Pages
					bool isMystery = Settings.Default.Mystery;
					string isMysteryFile = Settings.Default.UserPalettesLocation + @"\Mystery.jpg";
					if (Settings.Default.FT10)
					{
						if (Settings.Default.MysteryPic != "")
						{
							isMysteryFile = Settings.Default.MysteryPic;
						}
					}

					var j = Controller.PrintPalettePagesCount;
					if (Settings.Default.PrintScheme || Settings.Default.SchemeLegend || !isLizenzValid)
					{
						for (int i = 0; i < Controller.PagesCount; i++)
						{
							if (Settings.Default.SchemeLegend && !Settings.Default.PrintScheme)
							{
								i = Controller.PagesCount - j;
								j -= 1;
							}
							if (SaveTemplateToImagefile)
							{
								form.Text = "Creating Image-File";
							}

							progressLabel.Text = string.Format(Resources.PrintPageNo, i + 1, Controller.SourceImage.Description);
							form.Invalidate();
							form.Update();

							var page = document.AddPage();

							page.Orientation = pageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
							page.Width = pageWidth;
							page.Height = pageHeight;

							using (var pdfGraphics = XGraphics.FromPdfPage(page))
							{
								painter.PdfGraphics = pdfGraphics; // Keep same painter for different pages to share fonts
								if (isLizenzValid || Settings.Default.FT1)
								{
									string PDFOwner = Settings.Default.Owner;
									string PDFCompany = Settings.Default.Company;
									if (PDFOwner == "") { PDFOwner = AppInfo.AppDescription; }
									if (PDFCompany == "") { PDFCompany = ""; }
									Controller.PaintPrintPageImage(
										pdfGraphics,
										painter,
										i,
										pageSize,
										marginsRect,
										PDFOwner + " / " + PDFCompany,
										Settings.Default.PictureName,
										isMystery,
										isMysteryFile,
										SaveTemplateToImagefile,
										Settings.Default.SymFull,
										Settings.Default.SymNormal,
										Settings.Default.Line10Dbl,
										Settings.Default.ShowRulers,
										Settings.Default.ShowLines,
										Settings.Default.CutLines,
										Settings.Default.SchemeLegend,
										Settings.Default.SchemeLogo,
										Settings.Default.PrintScheme,
										Settings.Default.LegendLeft
										);
								}
								else
								{
									Settings.Default.SymFull = false;
									Settings.Default.SymNormal = true;
									Settings.Default.Line10Dbl = GridPainterSettings.Default.Line10DoubleWidth;
									Settings.Default.ShowRulers = GridPainterSettings.Default.ShowRulers;
									Settings.Default.ShowLines = GridPainterSettings.Default.ShowLines;
									Settings.Default.CutLines = false;

									Controller.PaintPrintPageImage(
										pdfGraphics,
										painter,
										i,
										pageSize,
										marginsRect,
										AppInfo.AppDescription,
										Controller.SourceImage.Description ?? Resources.UnsavedImageDescription,
										isMystery,
										isMysteryFile,
										SaveTemplateToImagefile,
										Settings.Default.SymFull,
										Settings.Default.SymNormal,
										Settings.Default.Line10Dbl,
										Settings.Default.ShowRulers,
										Settings.Default.ShowLines,
										Settings.Default.CutLines,
										Settings.Default.SchemeLegend,
										Settings.Default.SchemeLogo,
										Settings.Default.PrintScheme,
										Settings.Default.LegendLeft
										);

									pdfGraphics.Dispose();
									string watermark = "DiamondArtCreator";
									XFont font = new XFont("DiamondArtCreator", 55);

									// Variation 1: Draw watermark as text string
									// Get an XGraphics object for drawing beneath the existing content
									XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

									// Get the size (in point) of the text
									XSize size = gfx.MeasureString(watermark, font);

									// Define a rotation transformation at the center of the page
									gfx.TranslateTransform(page.Width / 2, page.Height / 2);
									gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
									gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);

									// Create a string format
									XStringFormat format = new XStringFormat();
									format.Alignment = XStringAlignment.Near;
									format.LineAlignment = XLineAlignment.Near;

									// Create a dimmed red brush
									//XBrush brush = new XSolidBrush(XColor.FromArgb(128, 255, 0, 0));
									XBrush brush = new XSolidBrush(XColor.FromArgb(128, 184, 210, 230));
									// Draw the string
									gfx.DrawString(watermark, font, brush,
									  new XPoint((page.Width - size.Width) / 2, (page.Height - size.Height) / 2),
									  format);

								}
							}

						}
					}

					// runde etiketten
					if (Settings.Default.RundEti)
					{
						if (isLizenzValid)
						{
							if (isLizenzValid || isLizenzDemo || Settings.Default.FT3 || Settings.Default.FT6)
							{
								//round etiketts
								var EtiketteSize = new Size(28, 28);
								var symbolCountX = 15;
								var symbolCountY = 21;
								var maxSymbolPerPage = symbolCountX * symbolCountY;
								var symbolPages = Controller.SourceImage.Palette.Count / maxSymbolPerPage;
								for (var i = 0; i <= symbolPages; i++)
								{
									progressLabel.Text = Resources.RoundEtikett + " " + (i + 1);
									form.Invalidate();
									form.Update();
									var page = document.AddPage();
									page.Orientation = pageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
									if (page.Orientation == PageOrientation.Landscape)
									{
										symbolCountX = 21;
										symbolCountY = 15;
									}
									page.Width = pageWidth;
									page.Height = pageHeight;
									string PDFOwner = Settings.Default.Owner;
									string PDFCompany = Settings.Default.Company;
									if (PDFOwner == "") { PDFOwner = ""; }
									if (PDFCompany == "") { PDFCompany = ""; }
									var VL = Settings.Default.VL;
									var VO = Settings.Default.S;
									using (var pdfGraphics = XGraphics.FromPdfPage(page))
									{
										painter.PdfGraphics = pdfGraphics; // Keep same painter for different pages to share fonts
										Controller.paintEtiketten(gridPainter,
											pdfGraphics,
											painter,
											pageSize,
											marginsRect,
											symbolCountX,
											symbolCountY,
											EtiketteSize,
											i,
											symbolPages,
											PDFOwner + PDFCompany,
											Settings.Default.PictureName,
											Settings.Default.EtiUmrandung,
											VL,
											VO);
									}
								}
							}
						}
					}
					if (Settings.Default.TicTacEti)
					{
						if (isLizenzCommerc || isLizenzDemo || Settings.Default.FT3 || Settings.Default.FT6)
						{

							//tictac-etiketts
							var EtiketteSize = new Size(50, 28);
							var symbolCountX = 10;
							var symbolCountY = 27;
							var maxSymbolPerPage = symbolCountX * symbolCountY;
							var symbolPages = Controller.SourceImage.Palette.Count / maxSymbolPerPage;
							var VL = Settings.Default.VL;
							var VO = Settings.Default.S;
							for (int i = 0; i <= symbolPages; i++)
							{
								progressLabel.Text = Resources.TicTacEtikett + " " + (i + 1);
								form.Invalidate();
								form.Update();
								var page = document.AddPage();
								page.Orientation = PageOrientation.Portrait;// pageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;

								page.Width = pageWidth;
								page.Height = pageHeight;
								string PDFOwner = Settings.Default.Owner;
								string PDFCompany = Settings.Default.Company;
								if (PDFOwner == "") { PDFOwner = ""; }
								if (PDFCompany == "") { PDFCompany = ""; }

								using (var pdfGraphics = XGraphics.FromPdfPage(page))
								{
									painter.PdfGraphics = pdfGraphics; // Keep same painter for different pages to share fonts
									Controller.paintEtikettenTicTac(gridPainter,
										pdfGraphics,
										painter,
										pageSize,
										marginsRect,
										symbolCountX,
										symbolCountY,
										EtiketteSize,
										i,
										symbolPages,
										PDFOwner + PDFCompany,
										Settings.Default.PictureName,
										Settings.Default.EtiUmrandung,
										VL,
										VO);
								}
							}
						}
					}
				}

				document.Save(fileName);
            }

			// show PDFPreview
			if (File.Exists(fileName))
			{
				if (showPreview && !SaveTemplateToImagefile)
				{
					//string fileName = saveDialog.FileName;
					using (var pdfPreview = new PDFPreview(fileName))
					{
						pdfPreview.ShowDialog();
						pdfPreview.Dispose();
					}
				}
				else
				{
					if (!SaveTemplateToImagefile)
					{
						Process.Start(fileName);
					}// if only when saving as TIFF or image-file
				}
			}

			Dispose();
		}
		#endregion
	}
}
