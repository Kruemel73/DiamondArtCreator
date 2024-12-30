using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
//using Ravlyk.Adopted.OpenXmlPackaging;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Properties;
using GdiColor = System.Drawing.Color;

namespace Ravlyk.SAE.Drawing.Export
{
	/// <summary>
	/// Exports scheme image to Excel 2007+ (.xlsx).
	/// </summary>
	public static partial class ExcelExporter2
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
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using (var package = new ExcelPackage(fileName))
			{
				var ws = package.Workbook.Worksheets.Add("Legend");
				ws.Rows.Height = 25;
				ws.Rows.Style.Font.Size = 18;
				createColorListHeader(ws, 3, 1);

				int index = 0;
				foreach (var color in orderedColors)
				{
					index++;
					FillPaletteLine(color, index, ws, gridPainter, index + 4, 1);

				}
				//var range = ws.Cells[Address: "A4"].LoadFromCollection(orderedColors, false);
				//range.AutoFitColumns();
				package.SaveAsync();
			}
		}

		static void FillPaletteLine(CodedColor color, int index, ExcelWorksheet sheet, PatternGridPainter gridPainter, int row, int startCol)
		{
			SetTextCell(index.ToString(), sheet, row, startCol);
			SetSymbolCell(color.SymbolChar.ToString(), sheet, gridPainter, row, startCol + 1, color);
			sheet.Cells[row, startCol + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
			//sheet.Cells[row, startCol + 1].Style.Fill.BackgroundColor.SetColor(GdiColor.FromArgb(color.Argb));
			sheet.Cells[row, startCol + 1].Style.Fill.BackgroundColor.SetColor(GdiColor.FromArgb(gridPainter.GetBackColorArgb(color)));
			//bool isParsable = Int32.TryParse(color.ColorCode, out number);
			SetTextCell(color.ColorCode, sheet, row, startCol + 2);
			//SetTextCell(number, sheet, row, startCol + 2);
			SetTextCell(color.OccurrencesCount.ToString(), sheet, row, startCol + 3);
			//SetColorLineCell(" ", colorStyle, sheet, row, startCol + 3);
			//SetColorLineCell(color.ColorName, textStyle, sheet, row, startCol + 4);
			int Bags = (color.UsageOccurrences.Count + ((color.UsageOccurrences.Count / 100) * SAEWizardSettings.Default.Reserve)) / SAEWizardSettings.Default.Bagsize + 1;
			SetTextCell(Bags.ToString(), sheet, row, startCol + 4);
		}

        private static void SetTextCell(string text, ExcelWorksheet sheet, int row, int startCol)
        {
			sheet.Cells[row, startCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			sheet.Cells[row, startCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
			sheet.Cells[row, startCol].Style.Numberformat.Format = "0";
			sheet.Cells[row, startCol].Style.Font.Name = "Calibri";
			sheet.Cells[row, startCol].Value = text;
		}
		private static void SetSymbolCell(string text, ExcelWorksheet sheet, PatternGridPainter gridPainter, int row, int startCol, CodedColor codedColor)
        {
			sheet.Cells[row, startCol].Style.Font.Name = gridPainter.SymbolsFont.Name;
			sheet.Cells[row, startCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			sheet.Cells[row, startCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
			sheet.Cells[row, startCol].Style.Font.Color.SetColor(GdiColor.FromArgb(gridPainter.GetSymbolColorArgb(codedColor)));
			sheet.Cells[row, startCol].Value = text;
			

		}
		private static void SetColorCell(CodedColor color, ExcelWorksheet sheet, int row, int startCol)
        {
			sheet.Cells[row, startCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Border.Top.Color.SetColor(GdiColor.Black);
			sheet.Cells[row, startCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
			sheet.Cells[row, startCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromWin32(color.Argb));
		}

		private static void createColorListHeader(ExcelWorksheet ws, int startRow, int startCol)
        {
			SetColorHeaderCell("#", 15, ws, startRow, startCol);
			SetColorHeaderCell("Sym", 12, ws, startRow, startCol + 1);
			SetColorHeaderCell("Code", 12, ws, startRow, startCol + 2);
			SetColorHeaderCell("Count", 20, ws, startRow, startCol + 3);
			SetColorHeaderCell("Bags", 12, ws, startRow, startCol + 4);
			//SetColorHeaderCell("#", headerStyle, 10, sheet, startRow, startCol);
			//SetColorHeaderCell(" ", headerStyle, 6, sheet, startRow, startCol + 1);
			//SetColorHeaderCell("Code", headerStyle, 15, sheet, startRow, startCol);
			//SetColorHeaderCell("Color", headerStyle, 10, sheet, startRow, startCol + 3);
			//SetColorHeaderCell("Name", headerStyle, 25, sheet, startRow, startCol + 4);
			//SetColorHeaderCell("Count", headerStyle, 12, sheet, startRow, startCol + 1);
			//SetColorHeaderCell("Bags", headerStyle, 12, sheet, startRow, startCol + 2);
		}

		static void SetColorHeaderCell(string text, double width, ExcelWorksheet sheet, int row, int col)
		{
			var cell = sheet.Cells[row, col];
			sheet.Cells[row, col].Style.Font.Bold = true;
			sheet.Cells[row, col].Style.Font.UnderLine = true;
			sheet.Cells[row, col].Style.Font.Color.SetColor(Color.Black);
			cell.Value = text;
			sheet.Column(col).Width = width;
		}

	}		
}