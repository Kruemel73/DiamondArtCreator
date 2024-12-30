using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
    /// <summary>
    /// Custom User Control which presents a 5x5 color matrix in a DataGridView object.
    /// The DataGridView is perfect because it manages the presentation and editing.
    /// 
    /// Author: Dennis Lang  2010     
    /// http://home.comcast.net/~lang.dennis/
    /// </summary>
    public partial class FilterGrid : UserControl
    {
        public FilterGrid(string label, Color bgColor)
        {
            InitializeComponent();

            Label = label;
            LabelBackColor = bgColor;
        }

        public FilterGrid()
        {
            InitializeComponent();

            Label = "";
            LabelBackColor = Color.LightBlue;
        }

        public void Set(FilterGrid fg)
        {
            Label = fg.Label;
            Source = fg.Source;
            Image = fg.Image;
        }

        public string Label
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public Color LabelBackColor
        {
            get { return label.BackColor; }
            set { label.BackColor = value; }
        }

        public Image Image
        {
            get { return panel.BackgroundImage; }
            set { panel.BackgroundImage = value; }
        }

        public DataGridView GridView
        {
            get { return this.dataGridView; }
            set { this.dataGridView = value; }
        }

        public object Source
        {
            get { return this.dataGridView.DataSource; }
            set {
                this.dataGridView.Columns.Clear();
                this.dataGridView.DataSource = value; 

                // Resize columns to page grid into viewer
                if (dataGridView.Columns.Count > 0)
                {
                    int extWidth = dataGridView.RowHeadersWidth + 1;
                    int colWidth = (dataGridView.Width - extWidth) / dataGridView.Columns.Count;
                    for (int col = 0; col < dataGridView.Columns.Count; col++)
                        dataGridView.Columns[col].Width = colWidth;
                }
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
            ClickEvent();
        }

        private void panel_Click(object sender, EventArgs e)
        {
            ClickEvent();
        }

        public EventHandler clickEvent;
        private void ClickEvent()
        {
            if (clickEvent != null)
                clickEvent(this, EventArgs.Empty);
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GridView.ReadOnly == true)
            {
                this.dataGridView.ClearSelection();
                ClickEvent();
            }
        }

     
    }
}
