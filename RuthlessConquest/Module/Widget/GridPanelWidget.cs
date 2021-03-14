using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    public class GridPanelWidget : ContainerWidget
    {
        public GridPanelWidget()
        {
            this.ColumnsCount = 1;
            this.RowsCount = 1;
        }

        public int ColumnsCount
        {
            get
            {
                return this.m_columns.Count;
            }
            set
            {
                this.m_columns = new List<GridPanelWidget.Column>(this.m_columns.GetRange(0, MathUtils.Min(this.m_columns.Count, value)));
                while (this.m_columns.Count < value)
                {
                    this.m_columns.Add(new GridPanelWidget.Column());
                }
            }
        }

        public int RowsCount
        {
            get
            {
                return this.m_rows.Count;
            }
            set
            {
                this.m_rows = new List<GridPanelWidget.Row>(this.m_rows.GetRange(0, MathUtils.Min(this.m_rows.Count, value)));
                while (this.m_rows.Count < value)
                {
                    this.m_rows.Add(new GridPanelWidget.Row());
                }
            }
        }

        public Point2 GetWidgetCell(Widget widget)
        {
            Point2 result;
            this.m_cells.TryGetValue(widget, out result);
            return result;
        }

        public void SetWidgetCell(Widget widget, Point2 cell)
        {
            this.m_cells[widget] = cell;
        }

        public static void SetCell(Widget widget, Point2 cell)
        {
            GridPanelWidget gridPanelWidget = widget.ParentWidget as GridPanelWidget;
            if (gridPanelWidget != null)
            {
                gridPanelWidget.SetWidgetCell(widget, cell);
            }
        }

        public override void WidgetRemoved(Widget widget)
        {
            this.m_cells.Remove(widget);
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            foreach (GridPanelWidget.Column column in this.m_columns)
            {
                column.ActualWidth = 0f;
            }
            foreach (GridPanelWidget.Row row in this.m_rows)
            {
                row.ActualHeight = 0f;
            }
            foreach (Widget widget in this.Children)
            {
                widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
                Point2 widgetCell = this.GetWidgetCell(widget);
                GridPanelWidget.Column column2 = this.m_columns[widgetCell.X];
                column2.ActualWidth = MathUtils.Max(column2.ActualWidth, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
                GridPanelWidget.Row row2 = this.m_rows[widgetCell.Y];
                row2.ActualHeight = MathUtils.Max(row2.ActualHeight, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
            }
            Vector2 zero = Vector2.Zero;
            foreach (GridPanelWidget.Column column3 in this.m_columns)
            {
                column3.Position = zero.X;
                zero.X += column3.ActualWidth;
            }
            foreach (GridPanelWidget.Row row3 in this.m_rows)
            {
                row3.Position = zero.Y;
                zero.Y += row3.ActualHeight;
            }
            DesiredSize = zero;
        }

        public override void ArrangeOverride()
        {
            foreach (Widget widget in this.Children)
            {
                Point2 widgetCell = this.GetWidgetCell(widget);
                GridPanelWidget.Column column = this.m_columns[widgetCell.X];
                GridPanelWidget.Row row = this.m_rows[widgetCell.Y];
                ArrangeChildWidgetInCell(new Vector2(column.Position, row.Position), new Vector2(column.Position + column.ActualWidth, row.Position + row.ActualHeight), widget);
            }
        }

        private List<GridPanelWidget.Column> m_columns = new List<GridPanelWidget.Column>();

        private List<GridPanelWidget.Row> m_rows = new List<GridPanelWidget.Row>();

        private Dictionary<Widget, Point2> m_cells = new Dictionary<Widget, Point2>();

        private class Column
        {
            public float Position;

            public float ActualWidth;
        }

        private class Row
        {
            public float Position;

            public float ActualHeight;
        }
    }
}
