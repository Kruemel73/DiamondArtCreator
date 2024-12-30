using System;
using System.Collections.Generic;
using Ravlyk.Adopted.OpenXmlPackaging;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Properties;
using GdiColor = System.Drawing.Color;

namespace Ravlyk.SAE.Drawing.Export
{
	/// <summary>
	/// Exports scheme image to Excel 2007+ (.xlsx).
	/// </summary>
	public static class ExcelExporter
	{
		/// <summary>
		/// Exports scheme image to Excel 2007+ (.xlsx).
		/// </summary>
		/// <param name="image">Scheme image.</param>
		/// <param name="fileName">Target file name.</param>
		/// <param name="gridPainter">Grid painter with current paint options.</param>
		/// <param name="orderedColors">List of colors in current order.</param>
		public static void ExportToExcel(CodedImage image, string fileName, PatternGridPainter gridPainter, IList<CodedColor> orderedColors)
		{
			using (var document = new SpreadsheetDocument(fileName))
			{
				CreateSchemeSheet(image, document, gridPainter);							// not important for most Pixler...
				//CreatePaletteSheet(image, orderedColors, document, gridPainter);
			}
		}

		#region Scheme

		static void CreateSchemeSheet(CodedImage image, SpreadsheetDocument document, PatternGridPainter gridPainter)
		{
			int startRowH;
			int startColH;
			int startRowV;
			int startColV;
			int startCountH;
			int startCountV;
			int platesHeight = SAEWizardSettings.Default.stickPlateHeight;
			int platesWidth = SAEWizardSettings.Default.stickPlateWidth;
			int platesWidthOld;
			int platesHeightOld;
			int countPlatesH;
			int countPlatesV;
			int fillStartH = 0;
			int fillStartV = 0;
			platesWidthOld = platesWidth;

			startRowH = 1;
			startColH = 2;
			startRowV = 2;
			startColV = 1;
			startCountH = 1;
			startCountV = 1;

			countPlatesH = image.Size.Width / platesWidth;
			if (countPlatesH * platesWidth < image.Size.Width ) { countPlatesH += 1; }
			countPlatesV = image.Size.Height / platesHeight;
			if (countPlatesV * platesHeight < image.Size.Height) { countPlatesV += 1; }

			for (int y = 0; y < countPlatesV; y++)
			{
				if (y == (countPlatesV - 1))
				{
					platesHeightOld = platesHeight;
					platesHeight = image.Size.Height - (y * platesHeight);
				}
				for (int x = 0; x < countPlatesH; x++)
				{
					if (x == (countPlatesH - 1))
					{
						platesWidthOld = platesWidth;
						platesWidth = image.Size.Width - (x * platesWidth);
					}
					var sheet = document.Worksheets.Add(Resources.ExcelSheetScheme + (x + 1) + (y + 1));

					CreateHorizontalRuler(image, document, sheet, gridPainter, startRowH, startColH, startCountH, platesWidth);


					CreateVerticalRuler(image, document, sheet, gridPainter, startRowV, startColV, startCountV, platesHeight);


					FillScheme(image, document, sheet, gridPainter, 2, 2, platesHeight, platesWidth, fillStartH, fillStartV);
					startCountH += platesWidth;
					fillStartH += platesWidth;
				}
				fillStartV += platesHeight;
				fillStartH = 0;
				startCountV += platesHeight;
				platesWidth = platesWidthOld;
				startCountH = 1;
			}


			//var sheet = document.Worksheets.Add(Resources.ExcelSheetScheme);

			//CreateHorizontalRuler(image, document, sheet, gridPainter, 1, 2);
			//CreateVerticalRuler(image, document, sheet, gridPainter, 2, 1);
			//FillScheme(image, document, sheet, gridPainter, 2, 2);
		}

        #region Ruler
        static void CreateHorizontalRuler(CodedImage image, SpreadsheetDocument document, Worksheet sheet, PatternGridPainter gridPainter, int startRowH, 
			int startColH, int startCountH, int platesWidth)
		{
			for (int x = 0; x < platesWidth; x++)
			{
				sheet.SetColumnWidth(x + startColH, 3);
				var cell = sheet.Cells[startRowH, x + startColH];
				cell.Style = GetHorizontalRulerCellStyle(document.Stylesheet, image, x, gridPainter);
				cell.Value = (startCountH + x);
			}

			sheet.Cells[startRowH, startColH].Value = " ";
			sheet.Cells[startRowH, startColH].Row.Height = 28;
		}

		static Style GetHorizontalRulerCellStyle(Stylesheet stylesheet, CodedImage image, int x, PatternGridPainter gridPainter)
		{
			var style = new Style();

			style.Alignment = new Alignment { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom, WrapText = false, Rotation = 90 };
			style.Font = new Font { Size = 10, Color = GridPainterSettings.Default.NumbersArgb };

			style.Borders = new Borders();

			style.Borders.Left.BorderStyle = GetBorderStyle(x, image.Size.Width);
			style.Borders.Left.Color = GetBorderColor(x, image.Size.Width);

			style.Borders.Right.BorderStyle = GetBorderStyle(x + 1, image.Size.Width);
			style.Borders.Right.Color = GetBorderColor(x + 1, image.Size.Width);

			style.Borders.Top.BorderStyle = BorderStyles.None;

			style.Borders.Bottom.BorderStyle = BorderStyles.Medium;
			style.Borders.Bottom.Color = GridPainterSettings.Default.Line10Argb;

			style.Borders.Diagonal.BorderStyle = BorderStyles.None;

			return stylesheet.AddStyle(style);
		}

		static void CreateVerticalRuler(CodedImage image, SpreadsheetDocument document, Worksheet sheet, PatternGridPainter gridPainter, int startRowV, int startColV, 
			int startCountV, int platesHeight)
		{
			sheet.SetColumnWidth(startColV, 6);

			for (int y = 0; y < platesHeight; y++)
			{
				var cell = sheet.Cells[y + startRowV, startColV];
				cell.Style = GetVerticalRulerCellStyle(document.Stylesheet, image, y, gridPainter);
				cell.Value = startCountV + y;
				cell.Row.Height = 14;
			}
		}

		static Style GetVerticalRulerCellStyle(Stylesheet stylesheet, CodedImage image, int y, PatternGridPainter gridPainter)
		{
			var style = new Style();

			style.Alignment = new Alignment { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Middle, WrapText = false };
			style.Font = new Font { Size = 10, Color = GridPainterSettings.Default.NumbersArgb };

			style.Borders = new Borders();

			style.Borders.Top.BorderStyle = GetBorderStyle(y, image.Size.Height);
			style.Borders.Top.Color = GetBorderColor(y, image.Size.Height);

			style.Borders.Bottom.BorderStyle = GetBorderStyle(y + 1, image.Size.Height);
			style.Borders.Bottom.Color = GetBorderColor(y + 1, image.Size.Height);

			style.Borders.Left.BorderStyle = BorderStyles.None;

			style.Borders.Right.BorderStyle = BorderStyles.Medium;
			style.Borders.Right.Color = GridPainterSettings.Default.Line10Argb;

			style.Borders.Diagonal.BorderStyle = BorderStyles.None;

			return stylesheet.AddStyle(style);
		}
        #endregion

        static void FillScheme(CodedImage image, SpreadsheetDocument document, Worksheet sheet, PatternGridPainter gridPainter, int startRow, int startCol,
			int platesHeight, int platesWidth, int fillStartH, int fillStartV)
		{
			for (int x = 0; x < platesWidth; x++)
			{
				for (int y = 0; y < platesHeight; y++)
				{
					var cell = sheet.Cells[y + startRow, x + startCol];
					cell.Style = GetCrossCellStyle(document.Stylesheet, image, x, y, gridPainter);
					cell.Value = image[x + fillStartH, y + fillStartV].SymbolChar.ToString();
				}
			}
		}

		static Style GetCrossCellStyle(Stylesheet stylesheet, CodedImage image, int x, int y, PatternGridPainter gridPainter)
		{
			var codedColor = image[x, y];

			var style = new Style();

			//style.NumberFormat = new NumberFormat("");
			style.Alignment = new Alignment { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Middle, WrapText = false };
			style.Fill = new Fill { Color = GdiColor.FromArgb(gridPainter.GetBackColorArgb(codedColor)) };
			style.Font = new Font { Name = gridPainter.SymbolsFont.Name, Size = 10, Color = GdiColor.FromArgb(gridPainter.GetSymbolColorArgb(codedColor)) };

			style.Borders = new Borders();

			style.Borders.Top.BorderStyle = GetBorderStyle(y, image.Size.Height);
			style.Borders.Top.Color = GetBorderColor(y, image.Size.Height);

			style.Borders.Bottom.BorderStyle = GetBorderStyle(y + 1, image.Size.Height);
			style.Borders.Bottom.Color = GetBorderColor(y + 1, image.Size.Height);

			style.Borders.Left.BorderStyle = GetBorderStyle(x, image.Size.Width);
			style.Borders.Left.Color = GetBorderColor(x, image.Size.Width);

			style.Borders.Right.BorderStyle = GetBorderStyle(x + 1, image.Size.Width);
			style.Borders.Right.Color = GetBorderColor(x + 1, image.Size.Width);

			style.Borders.Diagonal.BorderStyle = BorderStyles.None;

			return stylesheet.AddStyle(style);
		}

		static BorderStyles GetBorderStyle(int x, int width)
		{
			if ((x == width || x % 10 == 0) && GridPainterSettings.Default.Line10DoubleWidth)
			{
				return BorderStyles.Medium;
			}
			else
			{
				return BorderStyles.Thin;
			}
		}

		static GdiColor GetBorderColor(int x, int width)
		{
			if (x == width || x % 10 == 0)
			{
				return GridPainterSettings.Default.Line10Argb;
			}
			else if (x % 5 == 0)
			{
				return GridPainterSettings.Default.Line5Argb;
			}
			else
			{
				return GridPainterSettings.Default.LineArgb;
			}
		}

		#endregion

		#region Palette

		static void CreatePaletteSheet(CodedImage image, IList<CodedColor> orderedColors, SpreadsheetDocument document, PatternGridPainter gridPainter)
		{
			var sheet = document.Worksheets.Add(Resources.ExcelSheetPalette);

			//sheet.Cells[1, 1].Value = image.Palette.Name;						// Palettename in first cell

			CreateColorsListHeader(sheet, document.Stylesheet, 3, 1);

			int index = 0;
			foreach (var color in orderedColors)
			{
				index++;
				FillPaletteLine(color, index, sheet, document.Stylesheet, gridPainter, index + 3, 1);

			}
		}

		static void CreateColorsListHeader(Worksheet sheet, Stylesheet stylesheet, int startRow, int startCol)
		{
			var headerStyle = stylesheet.AddStyle(
				new Style
				{
					Font = new Font { Style = FontStyles.Bold, Color = GridPainterSettings.Default.NumbersArgb }
				});

			//SetColorHeaderCell("#", headerStyle, 10, sheet, startRow, startCol);
			//SetColorHeaderCell(" ", headerStyle, 6, sheet, startRow, startCol + 1);
			SetColorHeaderCell("Code", headerStyle, 15, sheet, startRow, startCol);
			//SetColorHeaderCell("Color", headerStyle, 10, sheet, startRow, startCol + 3);
			//SetColorHeaderCell("Name", headerStyle, 25, sheet, startRow, startCol + 4);
			SetColorHeaderCell("Count", headerStyle, 12, sheet, startRow, startCol + 1);
			SetColorHeaderCell("Bags", headerStyle, 12, sheet, startRow, startCol + 2);
		}

		static void SetColorHeaderCell(string text, Style style, double width, Worksheet sheet, int row, int col)
		{
			sheet.SetColumnWidth(col, width);
			var cell = sheet.Cells[row, col];
			cell.Style = style;
			cell.Value = text;
		}

		static void FillPaletteLine(CodedColor color, int index, Worksheet sheet, Stylesheet stylesheet, PatternGridPainter gridPainter, int row, int startCol)
		{
			var textStyle = new Style();
			textStyle.Borders = new Borders();
			textStyle.Borders.Top.BorderStyle = BorderStyles.Thin;
			textStyle.Borders.Top.Color = GridPainterSettings.Default.Line10Argb;
			textStyle = stylesheet.AddStyle(textStyle);

			var symbolStyle = new Style();
			symbolStyle.Borders = new Borders();
			symbolStyle.Borders.Top.BorderStyle = BorderStyles.Thin;
			symbolStyle.Borders.Top.Color = GridPainterSettings.Default.Line10Argb;
			symbolStyle.Font = new Font { Name = gridPainter.SymbolsFont.Name };
			symbolStyle = stylesheet.AddStyle(symbolStyle);

			var colorStyle = new Style();
			colorStyle.Borders = new Borders();
			colorStyle.Borders.Top.BorderStyle = BorderStyles.Thin;
			colorStyle.Borders.Top.Color = GridPainterSettings.Default.Line10Argb;
			colorStyle.Fill = new Fill(GdiColor.FromArgb(color.Argb));
			colorStyle = stylesheet.AddStyle(colorStyle);

			//SetColorLineCell(index.ToString(), textStyle, sheet, row, startCol);
			//SetColorLineCell(color.SymbolChar.ToString(), symbolStyle, sheet, row, startCol + 1);
			SetColorLineCell(color.ColorCode, textStyle, sheet, row, startCol);
			//SetColorLineCell(" ", colorStyle, sheet, row, startCol + 3);
			//SetColorLineCell(color.ColorName, textStyle, sheet, row, startCol + 4);
			SetColorLineCell(color.OccurrencesCount.ToString(), textStyle, sheet, row, startCol + 1);
			//int Bags = color.OccurrencesCount / 190 + 1;
			int Bags = (color.UsageOccurrences.Count + ((color.UsageOccurrences.Count / 100) * SAEWizardSettings.Default.Reserve)) / SAEWizardSettings.Default.Bagsize + 1;
			SetColorLineCell(Bags.ToString(), textStyle, sheet, row, startCol +2);
		}

		static void SetColorLineCell(string text, Style style, Worksheet sheet, int row, int col)
		{
			var cell = sheet.Cells[row, col];
			cell.Style = style;
			cell.Value = text;
		}

		#endregion
	}
}