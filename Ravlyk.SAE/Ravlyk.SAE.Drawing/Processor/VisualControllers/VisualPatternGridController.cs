using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.UndoRedo;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using PdfiumViewer;
using PdfSharp.Drawing;
using Ravlyk.SAE.Drawing.Properties;


namespace Ravlyk.SAE.Drawing.Processor
{
    public class VisualPatternGridController : VisualScrollableController, INotifyPropertyChanged
    {
        public VisualPatternGridController(IImageProvider imageProvider, TrueTypeFont symbolsFont, IndexedImage whiteCrosses, Ravlyk.Common.Size imageBoxSize = default(Ravlyk.Common.Size))
            : base(imageProvider, imageBoxSize)
        {
            WhiteCrosses = whiteCrosses;
            SymbolsFont = symbolsFont;
            imageProvider.Image.PixelChanged += Image_PixelChanged;
        }


        #region Properties


        public PatternGridPainter.StitchesPaintMode PaintMode
        {
            get { return GridPainter.PaintMode; }
            set
            {
                if (GridPainter.PaintMode != value)
                {
                    GridPainter.PaintMode = value;
                    UpdateVisualImage();
                    OnPropertyChanged(nameof(PaintMode));
                }
            }
        }

        public PatternGridPainter.LegendPaintMode LegendPaint
        {
            get { return GridPainter.LegendPaint; }
            set
            {
                if (GridPainter.LegendPaint != value)
                {
                    GridPainter.LegendPaint = value;
                    UpdateVisualImage();
                    OnPropertyChanged(nameof(LegendPaint));
                }
            }
        }

        public int CellSize
        {
            get { return GridPainter.CellSize; }
            set
            {
                if (GridPainter.CellSize != value)
                {
                    GridPainter.CellSize = value;
                    UpdateParametersAndVisualImage();
                    OnPropertyChanged(nameof(CellSize));
                }
            }
        }
        public Ravlyk.Common.Point SchemeImageCell
        {
            get { return touchedCell; }
            set
            {
                var point = touchedCell = CellFromImagePoint(new Ravlyk.Common.Point(initialTouchImagePoint.X, initialTouchImagePoint.Y));
            }
        }
        public bool ShowRulers
        {
            get { return GridPainter.ShowRulers; }
            set
            {
                if (GridPainter.ShowRulers != value)
                {
                    GridPainter.ShowRulers = value;
                    UpdateVisualImage();
                    OnPropertyChanged(nameof(ShowRulers));
                }
            }
        }

        public bool ShowLines
        {
            get { return GridPainter.ShowLines; }
            set
            {
                if (GridPainter.ShowLines != value)
                {
                    GridPainter.ShowLines = value;
                    UpdateVisualImage();
                    OnPropertyChanged(nameof(ShowLines));
                }
            }
        }

        public Ravlyk.Common.Rectangle SelectedRect
        {
            get { return GridPainter.SelectedRect; }
            set
            {
                if (!GridPainter.SelectedRect.Equals(value))
                {
                    GridPainter.SelectedRect = value;
                    UpdateVisualImage();
                    OnPropertyChanged(nameof(SelectedRect));
                }
            }
        }

        bool ShowSelectedRect
        {
            get { return GridPainter.ShowSelectedRect; }
            set
            {
                if (value != GridPainter.ShowSelectedRect)
                {
                    GridPainter.ShowSelectedRect = value;
                    if (value)
                    {
                        SelectedRect = default(Ravlyk.Common.Rectangle);
                    }
                    UpdateVisualImage();
                }
            }
        }

        #endregion

        #region Visual Image

        internal int VisibleColumns { get; private set; }
        internal int VisibleColumnsWidth { get; private set; }
        internal int VisibleRows { get; private set; }
        internal int VisibleRowsHeight { get; private set; }
        internal bool RequiresShift { get; private set; }

        protected override void UpdateParameters()
        {
            base.UpdateParameters();

            VisibleColumns = (ImageBoxSize.Width - GridPainter.RulerWidth) / CellSize;
            VisibleColumnsWidth = VisibleColumns * CellSize;

            VisibleRows = (ImageBoxSize.Height - GridPainter.RulerWidth) / CellSize;
            VisibleRowsHeight = VisibleRows * CellSize;

            RequiresShift = VisibleColumns < SourceImage.Size.Width || VisibleRows < SourceImage.Size.Height;

            PixelsShift = PixelsShift;
        }

        protected override CodedImage CreateVisualImage()
        {
            return new CodedImage { Size = ImageBoxSize };
        }

        protected override void UpdateVisualImageCore()
        {
            if (SymbolsFont != null && CellSize > 0)
            {
                VisualImage.Size = ImageBoxSize;

                Painter.Canvas = VisualImage;
                using (Painter.Clip(new Ravlyk.Common.Rectangle(0, 0, ImageBoxSize.Width, ImageBoxSize.Height)))
                {
                    GridPainter.Paint(Painter, VisualImage.Size, Painter.ClipRect, new Ravlyk.Common.Point(CellsShift.Width, CellsShift.Height));
                }
            }
        }

        #region Painter

        public TrueTypeFont SymbolsFont { get; }

        public IndexedImage WhiteCrosses
        {
            get { return GridPainter.WhiteCrosses; }
            set { GridPainter.WhiteCrosses = value; }
        }

        public PatternGridPainter GridPainter
        {
            get
            {
                if (gridPainter == null)
                {
                    gridPainter = new PatternGridPainter(SourceImage, SymbolsFont);
                }
                else if (SymbolsFont != null && gridPainter.SymbolsFont != SymbolsFont)
                {
                    gridPainter.SymbolsFont = SymbolsFont;
                }

                return gridPainter;
            }
        }
        PatternGridPainter gridPainter;


        IndexedImagePainter Painter
        {
            get
            {
                if (painter == null)
                {
                    painter = new IndexedImagePainter();
                }
                painter.SymbolsFont = SymbolsFont;
                return painter;
            }
        }
        IndexedImagePainter painter;

        #endregion

        #endregion

        #region Drawing

        public enum MouseActionMode
        {
            Shift,
            Pen,
            Fill,
            Select,
            MoveSelection
        }

        public MouseActionMode MouseMode
        {
            get { return mouseMode; }
            set
            {
                if (value == mouseMode)
                {
                    return;
                }

                var oldMouseMode = mouseMode;
                mouseMode = value;

                using (SuspendUpdateVisualImage())
                {
                    if (oldMouseMode == MouseActionMode.MoveSelection)
                    {
                        FinishMoveSelection();
                    }

                    ShowSelectedRect = mouseMode == MouseActionMode.Select || mouseMode == MouseActionMode.MoveSelection;
                    GridPainter.ShowSelectedRectPoints = mouseMode == MouseActionMode.Select;
                    GridPainter.AllowExceedingSelection = mouseMode == MouseActionMode.MoveSelection;
                    if (mouseMode == MouseActionMode.Select)
                    {
                        SelectedRect = default(Ravlyk.Common.Rectangle);
                    }
                    if (mouseMode != MouseActionMode.MoveSelection)
                    {
                        GridPainter.InsertedBlock = null;
                    }
                }

                OnPropertyChanged(nameof(MouseMode));
            }
        }
        MouseActionMode mouseMode = MouseActionMode.Shift;

        public CodedColor MouseColor { get; set; }

        public void Pen(Ravlyk.Common.Point point)
        {
            if (MouseColor != null && point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
            {
                using (SuspendUpdateVisualImage())
                using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
                {
                    SourceImage[point.X, point.Y] = MouseColor;
                    if (!suspendImageChangedEventOnTouch)
                    {
                        SourceImage.TriggerImageChanged();
                    }
                }
            }
        }

        public void Fill(Ravlyk.Common.Point point)
        {
            if (MouseColor != null && point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
            {
                using (SuspendUpdateVisualImage())
                using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
                using (UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionFillRegion))
                {
                    ImagePainter.Fill(SourceImage, MouseColor, point.X, point.Y);
                    SourceImage.TriggerImageChanged();
                }
            }
        }

        void Image_PixelChanged(object sender, PixelChangedEventArgs e)
        {
            UpdateVisualImage();
        }

        #region Move Selection

        Ravlyk.Common.Point initialTouchImagePoint;

        public void InsertBlockAndBeginMoveSelection(CodedImage block)
        {
            using (SuspendUpdateVisualImage())
            {
                if (MouseMode == MouseActionMode.MoveSelection)
                {
                    FinishMoveSelection();
                }

                GridPainter.InsertedBlock = block;
                MouseMode = MouseActionMode.MoveSelection;
                SelectedRect = new Ravlyk.Common.Rectangle(CellsShift.Width, CellsShift.Height, block.Size.Width, block.Size.Height);
                UpdateVisualImage();
            }
        }

        void MoveSelection(Ravlyk.Common.Size shift)
        {
            SelectedRect = new Ravlyk.Common.Rectangle(
                Math.Max(-SelectedRect.Width + 1, Math.Min(SourceImage.Size.Width - 1, SelectedRect.Left + shift.Width)),
                Math.Max(-SelectedRect.Height + 1, Math.Min(SourceImage.Size.Height - 1, SelectedRect.Top + shift.Height)),
                SelectedRect.Width,
                SelectedRect.Height);
        }

        public void FinishMoveSelection()
        {
            if (GridPainter.InsertedBlock != null)
            {
                using (SuspendUpdateVisualImage())
                using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
                using (UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionPaste))
                {
                    ImageCopier.CopyWithPalette(GridPainter.InsertedBlock, SourceImage, new Ravlyk.Common.Point(SelectedRect.Left, SelectedRect.Top));

                    GridPainter.InsertedBlock = null;

                    if (MouseMode == MouseActionMode.MoveSelection)
                    {
                        MouseMode = MouseActionMode.Shift;
                    }

                    UpdateVisualImage();
                    SourceImage.TriggerImageChanged();
                }
            }
        }

        // remove colors with less than x stones
        public void RemoveColorsLess(CodedColor col, Ravlyk.Common.Point Pos)
        {
            var set = 0;
            //search for different colors nearby
            for (var sx = -1; sx < 2; sx++)
            {
                for (var sy = -1; sy < 2; sy++)
                {
                    if (Pos.X + sx <= 0) { sx = 0; };
                    if (Pos.Y + sy <= 0) { sy = 0; };
                    if (Pos.X + sx < SourceImage.Size.Width - 1 && Pos.Y + sy < SourceImage.Size.Height - 1)
                    {
                        if (SourceImage[Pos.X + sx, Pos.Y + sy].ColorCode != col.ColorCode)
                        {
                            SourceImage[Pos.X, Pos.Y] = SourceImage[Pos.X + sx, Pos.Y + sy];
                            set = 1;
                            break;
                        }
                    }
                }
                if (set != 0) { break; }
            }

        }

        #endregion

        #endregion

        #region UndoRedo

        public UndoRedoProvider UndoRedo
        {
            get
            {
                if (undoRedo == null)
                {
                    undoRedo = new UndoRedoProvider(SourceImage);
                }
                return undoRedo;
            }
        }
        UndoRedoProvider undoRedo;

        public void Undo()
        {
            using (SuspendUpdateVisualImage())
            {
                if (MouseMode == MouseActionMode.MoveSelection)
                {
                    FinishMoveSelection();
                }

                if (UndoRedo.CanUndo)
                {
                    UndoRedo.Undo();
                    SourceImage.TriggerImageChanged();
                }
            }
        }

        public void Redo()
        {
            using (SuspendUpdateVisualImage())
            {
                if (MouseMode == MouseActionMode.MoveSelection)
                {
                    FinishMoveSelection();
                }

                if (UndoRedo.CanRedo)
                {
                    UndoRedo.Redo();
                    SourceImage.TriggerImageChanged();
                }
            }
        }

        #endregion

        #region Touch actions

        public Ravlyk.Common.Size CellsShift
        {
            get { return cellsShift; }
            private set
            {
                if (value.Width < 0) { value.Width = 0; }
                if (value.Height < 0) { value.Height = 0; }

                var maxHorizontalShift = SourceImage.Size.Width - VisibleColumns;
                if (value.Width > maxHorizontalShift && maxHorizontalShift > 0) { value.Width = maxHorizontalShift; }

                var maxVerticalShift = SourceImage.Size.Height - VisibleRows;
                if (value.Height > maxVerticalShift && maxVerticalShift > 0) { value.Height = maxVerticalShift; }

                if (value != cellsShift)
                {
                    var oldCellShift = cellsShift;
                    cellsShift = value;
                    var shiftDelta = new Ravlyk.Common.Size(cellsShift.Width - oldCellShift.Width, cellsShift.Height - oldCellShift.Height);

                    if (!synchronizingShifts)
                    {
                        synchronizingShifts = true;
                        PixelsShift = new Ravlyk.Common.Size(value.Width * CellSize, value.Height * CellSize);
                        synchronizingShifts = false;
                    }

                    if (!IsUpdateVisualImageSuspended && Math.Abs(shiftDelta.Width) < VisibleColumns && Math.Abs(shiftDelta.Height) < VisibleRows)
                    {
                        GridPainter.ShiftImageAndUpdateRest(Painter, VisualImage, new Ravlyk.Common.Point(CellsShift.Width, CellsShift.Height), shiftDelta);
                        OnVisualImageChanged();
                    }
                    else
                    {
                        UpdateVisualImage();
                    }
                }
            }
        }
        Ravlyk.Common.Size cellsShift;

        public Ravlyk.Common.Size PixelsShift
        {
            get { return pixelsShift; }
            set
            {
                if (value.Width < 0) { value.Width = 0; }
                if (value.Height < 0) { value.Height = 0; }

                var maxHorizontalShift = SourceImage.Size.Width * CellSize - VisibleColumnsWidth;
                if (value.Width > maxHorizontalShift) { value.Width = maxHorizontalShift; }

                var maxVerticalShift = SourceImage.Size.Height * CellSize - VisibleRowsHeight;
                if (value.Height > maxVerticalShift) { value.Height = maxVerticalShift; }

                if (value != pixelsShift)
                {
                    pixelsShift = value;
                    if (!synchronizingShifts)
                    {
                        synchronizingShifts = true;
                        CellsShift = new Ravlyk.Common.Size(value.Width / CellSize, value.Height / CellSize);
                        synchronizingShifts = false;
                    }
                }
            }
        }
        Ravlyk.Common.Size pixelsShift;
        bool synchronizingShifts;

        Ravlyk.Common.Point touchedCell;
        TouchPointerStyle touchedPointerStyle;
        VisualZoomCropController.TouchPoint touchedPoint;

        protected override void OnTouchedCore(Ravlyk.Common.Point imagePoint)
        {
            if (MouseMode == MouseActionMode.Shift)
            {
                base.OnTouchedCore(imagePoint);
                var point = touchedCell = CellFromImagePoint(imagePoint);

                //Getting and temp-saving Cell-Infos for viewing on Grid-CellToolTip(label)
                //if (GridPainterSettings.Default.NotTemplate == false)
                //{
                var ClickedCell = new Ravlyk.Common.Point(point.X + 1, point.Y + 1);
                var GetColorCode = SourceImage[point.X, point.Y];
                var ClickedColorCode = GetColorCode.ColorCode;
                SAEWizardSettings.Default.ClickedCell = new System.Drawing.Point(ClickedCell.X, ClickedCell.Y);
                SAEWizardSettings.Default.ClickedColorCode = ClickedColorCode;
                SAEWizardSettings.Default.ClickedSymbol = GetColorCode.SymbolChar.ToString();
                SAEWizardSettings.Default.SourceImageHeight = SourceImage.Size.Height;
                SAEWizardSettings.Default.SourceImageWidth = SourceImage.Size.Width;
                //GridPainterSettings.Default.Save();
                
                //}
            }
            else if (MouseMode == MouseActionMode.Pen)
            {
                suspendImageChangedEventOnTouch = true;
                Pen(CellFromImagePoint(imagePoint));
            }
            else if (MouseMode == MouseActionMode.Fill)
            {
                Fill(CellFromImagePoint(imagePoint));
            }
            else if (MouseMode == MouseActionMode.Select)
            {
                touchedPointerStyle = GetTouchPointerStyleCore(imagePoint); // Should be saved calculated first
                touchedPoint = GetTouchPoint(imagePoint);
                var point = touchedCell = CellFromImagePoint(imagePoint);
                if (point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
                {
                    if (touchedPoint == VisualZoomCropController.TouchPoint.None || SelectedRect.Width == 0 || SelectedRect.Height == 0)
                    {
                        SelectedRect = new Ravlyk.Common.Rectangle(point.X, point.Y, 1, 1);
                    }
                }
            }
            else if (MouseMode == MouseActionMode.MoveSelection)
            {
                var cellPoint = CellFromImagePoint(imagePoint);
                if (!SelectedRect.ContainsPoint(cellPoint))
                {
                    FinishMoveSelection();
                }
                else
                {
                    initialTouchImagePoint = cellPoint;
                }
            }
        }

        protected override void OnHoverCore(Common.Point imagePoint)
        {
            base.OnHoverCore(imagePoint);
        }
        protected override void OnShiftCore(Ravlyk.Common.Point imagePoint, Ravlyk.Common.Size shiftSize)
        {
            if (IsTouching)
            {
                if (MouseMode == MouseActionMode.Shift && RequiresShift)
                {
                    PixelsShift = new Ravlyk.Common.Size(PixelsShift.Width - shiftSize.Width, PixelsShift.Height - shiftSize.Height);
                }
                else if (MouseMode == MouseActionMode.Pen)
                {
                    Pen(CellFromImagePoint(new Ravlyk.Common.Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height)));
                }
                else if (MouseMode == MouseActionMode.Select)
                {
                    var newPoint = CellFromImagePoint(new Ravlyk.Common.Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height));
                    var x1 = SelectedRect.Left;
                    var y1 = SelectedRect.Top;
                    var x2 = SelectedRect.RightExclusive - 1;
                    var y2 = SelectedRect.BottomExclusive - 1;

                    switch (touchedPoint)
                    {
                        case VisualZoomCropController.TouchPoint.Left:
                            x1 = newPoint.X;
                            break;
                        case VisualZoomCropController.TouchPoint.Right:
                            x2 = newPoint.X;
                            break;
                        case VisualZoomCropController.TouchPoint.Top:
                            y1 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.Bottom:
                            y2 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.LeftTop:
                            x1 = newPoint.X;
                            y1 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.RightTop:
                            x2 = newPoint.X;
                            y1 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.LeftBottom:
                            x1 = newPoint.X;
                            y2 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.RightBottom:
                            x2 = newPoint.X;
                            y2 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.None:
                            x1 = touchedCell.X;
                            y1 = touchedCell.Y;
                            x2 = newPoint.X;
                            y2 = newPoint.Y;
                            break;
                        case VisualZoomCropController.TouchPoint.Middle:
                            x1 = SelectedRect.Left + newPoint.X - touchedCell.X;
                            y1 = SelectedRect.Top + newPoint.Y - touchedCell.Y;
                            x2 = x1 + SelectedRect.Width - 1;
                            y2 = y1 + SelectedRect.Height - 1;
                            touchedCell = newPoint;
                            break;
                    }

                    if (x1 > x2)
                    {
                        var t = x1;
                        x1 = x2;
                        x2 = t;
                    }
                    if (y1 > y2)
                    {
                        var t = y1;
                        y1 = y2;
                        y2 = t;
                    }
                    SelectedRect = new Ravlyk.Common.Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                }
                else if (MouseMode == MouseActionMode.MoveSelection)
                {
                    var newPoint = CellFromImagePoint(new Ravlyk.Common.Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height));
                    MoveSelection(new Ravlyk.Common.Size(newPoint.X - initialTouchImagePoint.X, newPoint.Y - initialTouchImagePoint.Y));
                    initialTouchImagePoint = newPoint;
                }
            }
            else
            {
                base.OnShiftCore(imagePoint, shiftSize);
            }
        }

        protected override void OnUntouchedCore(Ravlyk.Common.Point imagePoint)
        {
            touchedPointerStyle = TouchPointerStyle.None;
            touchedPoint = VisualZoomCropController.TouchPoint.None;
            touchedCell = new Ravlyk.Common.Point(-1, -1);

            if (suspendImageChangedEventOnTouch)
            {
                SourceImage.TriggerImageChanged();
                suspendImageChangedEventOnTouch = false;
            }
            base.OnUntouchedCore(imagePoint);
        }

        public Ravlyk.Common.Point CellFromImagePoint(Ravlyk.Common.Point imagePoint)
        {
            var rulerWidth = GridPainter.RulerWidth;
            return new Ravlyk.Common.Point(
                imagePoint.X >= rulerWidth ? (imagePoint.X - rulerWidth) / CellSize + CellsShift.Width : -1,
                imagePoint.Y >= rulerWidth ? (imagePoint.Y - rulerWidth) / CellSize + CellsShift.Height : -1);
        }

        Ravlyk.Common.Point ImageFromCellPoint(Ravlyk.Common.Point cellPoint)
        {
            var rulerWidth = GridPainter.RulerWidth;
            return new Ravlyk.Common.Point(
                cellPoint.X >= 0 && cellPoint.X < SourceImage.Size.Width ? (cellPoint.X - CellsShift.Width) * CellSize + rulerWidth : -1,
                cellPoint.Y >= 0 && cellPoint.Y < SourceImage.Size.Height ? (cellPoint.Y - CellsShift.Height) * CellSize + rulerWidth : -1);
        }

        protected override void OnMouseWheelCore(int delta)
        {
            if (RequiresShift)
            {
                PixelsShift = new Ravlyk.Common.Size(PixelsShift.Width, PixelsShift.Height - delta);
            }
            else
            {
                base.OnMouseWheelCore(delta);
            }
        }

        bool suspendImageChangedEventOnTouch;

        protected override TouchPointerStyle GetTouchPointerStyleCore(Ravlyk.Common.Point imagePoint)
        {
            var point = CellFromImagePoint(imagePoint);
            if (point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
            {
                if (MouseMode == MouseActionMode.MoveSelection)
                {
                    if (SelectedRect.ContainsPoint(point))
                    {
                        return TouchPointerStyle.Shift;
                    }
                }
                else if (MouseMode == MouseActionMode.Select)
                {
                    if (IsTouching)
                    {
                        return touchedPointerStyle;
                    }

                    if (SelectedRect.Width > 0 && SelectedRect.Height > 0)
                    {
                        var imageRectLeftTop = ImageFromCellPoint(new Ravlyk.Common.Point(SelectedRect.Left, SelectedRect.Top));
                        var imageRectRightBottom = ImageFromCellPoint(new Ravlyk.Common.Point(SelectedRect.RightExclusive, SelectedRect.BottomExclusive));

                        if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, imageRectLeftTop.Y) ||
                            IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, imageRectRightBottom.Y))
                        {
                            return TouchPointerStyle.ResizeLeftTop_RightBottom;
                        }
                        if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, imageRectRightBottom.Y) ||
                            IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, imageRectLeftTop.Y))
                        {
                            return TouchPointerStyle.ResizeRightTop_LeftBottom;
                        }

                        var midX = (imageRectLeftTop.X + imageRectRightBottom.X) / 2;
                        var midY = (imageRectLeftTop.Y + imageRectRightBottom.Y) / 2;

                        if (IsInsideRadius(imagePoint.X, imagePoint.Y, midX, imageRectLeftTop.Y) ||
                            IsInsideRadius(imagePoint.X, imagePoint.Y, midX, imageRectRightBottom.Y))
                        {
                            return TouchPointerStyle.ResizeVertical;
                        }
                        if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, midY) ||
                            IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, midY))
                        {
                            return TouchPointerStyle.ResizeHorizontal;
                        }

                        if (SelectedRect.ContainsPoint(point))
                        {
                            return TouchPointerStyle.ResizeAll;
                        }
                    }

                    return TouchPointerStyle.Cross;
                }
            }

            return base.GetTouchPointerStyleCore(imagePoint);
        }

        VisualZoomCropController.TouchPoint GetTouchPoint(Ravlyk.Common.Point imagePoint)
        {
            if (MouseMode != MouseActionMode.Select && MouseMode != MouseActionMode.MoveSelection)
            {
                return VisualZoomCropController.TouchPoint.None;
            }

            var imageRectLeftTop = ImageFromCellPoint(new Ravlyk.Common.Point(SelectedRect.Left, SelectedRect.Top));
            var imageRectRightBottom = ImageFromCellPoint(new Ravlyk.Common.Point(SelectedRect.RightExclusive, SelectedRect.BottomExclusive));

            return VisualZoomCropController.GetTouchPoint(imagePoint, GetTouchPointerStyleCore(imagePoint),
                new Ravlyk.Common.Rectangle(imageRectLeftTop.X, imageRectLeftTop.Y, imageRectRightBottom.X - imageRectLeftTop.X, imageRectRightBottom.Y - imageRectLeftTop.Y));
        }

        static bool IsInsideRadius(int x1, int y1, int x2, int y2, int radius = 3)
        {
            return VisualZoomCropController.IsInsideRadius(x1, x2, radius) && VisualZoomCropController.IsInsideRadius(y1, y2, radius);
        }

        #endregion

        #region VisualScrollableController

        public override bool ShowVScroll => RequiresShift;

        public override int MaxVSteps => SourceImage.Size.Height;

        public override int BigVStep => VisibleRows;

        public override int VPosition
        {
            get { return CellsShift.Height; }
            set { CellsShift = new Ravlyk.Common.Size(CellsShift.Width, value); }
        }

        public override bool ShowHScroll => RequiresShift;

        public override int MaxHSteps => SourceImage.Size.Width;

        public override int BigHStep => VisibleColumns;

        public override int HPosition
        {
            get { return CellsShift.Width; }
            set { CellsShift = new Ravlyk.Common.Size(value, CellsShift.Height); }
        }

        #endregion

        #region INotifyPropertyChanged

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


    }
}
