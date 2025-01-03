#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
#if GDI
using System.Drawing.Drawing2D;
using GdiPen = System.Drawing.Pen;
#endif

namespace Ravlyk.Adopted.PdfSharp.Drawing
{
    // TODO Free GDI objects (pens, brushes, ...) automatically without IDisposable.
    /// <summary>
    /// Defines an object used to draw lines and curves.
    /// </summary>
    public sealed class XPen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XPen"/> class.
        /// </summary>
        public XPen(XColor color)
            : this(color, 1, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPen"/> class.
        /// </summary>
        public XPen(XColor color, double width)
            : this(color, width, false)
        { }

        internal XPen(XColor color, double width, bool immutable)
        {
            _color = color;
            _width = width;
            _lineJoin = XLineJoin.Miter;
            _lineCap = XLineCap.Flat;
            _dashStyle = XDashStyle.Solid;
            _dashOffset = 0f;
            _immutable = immutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPen"/> class.
        /// </summary>
        public XPen(XPen pen)
        {
            _color = pen._color;
            _width = pen._width;
            _lineJoin = pen._lineJoin;
            _lineCap = pen._lineCap;
            _dashStyle = pen._dashStyle;
            _dashOffset = pen._dashOffset;
            _dashPattern = pen._dashPattern;
            if (_dashPattern != null)
                _dashPattern = (double[])_dashPattern.Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        public XPen Clone()
        {
            return new XPen(this);
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public XColor Color
        {
            get { return _color; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _color != value;
                _color = value;
            }
        }
        internal XColor _color;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _width != value;
                _width = value;
            }
        }
        internal double _width;

        /// <summary>
        /// Gets or sets the line join.
        /// </summary>
        public XLineJoin LineJoin
        {
            get { return _lineJoin; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _lineJoin != value;
                _lineJoin = value;
            }
        }
        internal XLineJoin _lineJoin;

        /// <summary>
        /// Gets or sets the line cap.
        /// </summary>
        public XLineCap LineCap
        {
            get { return _lineCap; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _lineCap != value;
                _lineCap = value;
            }
        }
        internal XLineCap _lineCap;

        /// <summary>
        /// Gets or sets the miter limit.
        /// </summary>
        public double MiterLimit
        {
            get { return _miterLimit; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _miterLimit != value;
                _miterLimit = value;
            }
        }
        internal double _miterLimit;

        /// <summary>
        /// Gets or sets the dash style.
        /// </summary>
        public XDashStyle DashStyle
        {
            get { return _dashStyle; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _dashStyle != value;
                _dashStyle = value;
            }
        }
        internal XDashStyle _dashStyle;

        /// <summary>
        /// Gets or sets the dash offset.
        /// </summary>
        public double DashOffset
        {
            get { return _dashOffset; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _dirty = _dirty || _dashOffset != value;
                _dashOffset = value;
            }
        }
        internal double _dashOffset;

        /// <summary>
        /// Gets or sets the dash pattern.
        /// </summary>
        public double[] DashPattern
        {
            get
            {
                if (_dashPattern == null)
                    _dashPattern = new double[0];
                return _dashPattern;
            }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));

                int length = value.Length;
                //if (length == 0)
                //  throw new ArgumentException("Dash pattern array must not be empty.");

                for (int idx = 0; idx < length; idx++)
                {
                    if (value[idx] <= 0)
                        throw new ArgumentException("Dash pattern value must greater than zero.");
                }

                _dirty = true;
                _dashStyle = XDashStyle.Custom;
                _dashPattern = (double[])value.Clone();
            }
        }
        internal double[] _dashPattern;

        /// <summary>
        /// Gets or sets a value indicating whether the pen enables overprint when used in a PDF document.
        /// Experimental, takes effect only on CMYK color mode.
        /// </summary>
        public bool Overprint
        {
            get { return _overprint; }
            set
            {
                if (_immutable)
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                _overprint = value;
            }
        }
        internal bool _overprint;

#if GDI

        internal System.Drawing.Pen RealizeGdiPen()
        {
            if (_dirty)
            {
                if (_gdiPen == null)
                    _gdiPen = new System.Drawing.Pen(_color.ToGdiColor(), (float)_width);
                else
                {
                    _gdiPen.Color = _color.ToGdiColor();
                    _gdiPen.Width = (float)_width;
                }
                LineCap lineCap = XConvert.ToLineCap(_lineCap);
                _gdiPen.StartCap = lineCap;
                _gdiPen.EndCap = lineCap;
                _gdiPen.LineJoin = XConvert.ToLineJoin(_lineJoin);
                _gdiPen.DashOffset = (float)_dashOffset;
                if (_dashStyle == XDashStyle.Custom)
                {
                    int len = _dashPattern == null ? 0 : _dashPattern.Length;
                    float[] pattern = new float[len];
                    for (int idx = 0; idx < len; idx++)
                        pattern[idx] = (float)_dashPattern[idx];
                    _gdiPen.DashPattern = pattern;
                }
                else
                    _gdiPen.DashStyle = (System.Drawing.Drawing2D.DashStyle)_dashStyle;
            }
            return _gdiPen;
        }
#endif

        bool _dirty = true;
        readonly bool _immutable;
#if GDI
        GdiPen _gdiPen;
#endif
    }
}