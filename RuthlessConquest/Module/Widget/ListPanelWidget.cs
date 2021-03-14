using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class ListPanelWidget : ScrollPanelWidget
    {
        public event Action<object> ItemClicked;

        public event Action SelectionChanged;

        public ListPanelWidget()
        {
            this.SelectionColor = Colors.HighDark;
            this.ItemWidgetFactory = ((object item) => new LabelWidget
            {
                Text = ((item != null) ? item.ToString() : string.Empty)
            });
            this.ItemSize = 48f;
        }

        public Func<object, Widget> ItemWidgetFactory { get; set; }

        public override LayoutDirection Direction
        {
            get
            {
                return base.Direction;
            }
            set
            {
                if (value != this.Direction)
                {
                    base.Direction = value;
                    this.m_widgetsDirty = true;
                }
            }
        }

        public override float ScrollPosition
        {
            get
            {
                return base.ScrollPosition;
            }
            set
            {
                if (value != this.ScrollPosition)
                {
                    base.ScrollPosition = value;
                    this.m_widgetsDirty = true;
                }
            }
        }

        public float ItemSize
        {
            get
            {
                return this.m_itemSize;
            }
            set
            {
                if (value != this.m_itemSize)
                {
                    this.m_itemSize = value;
                    this.m_widgetsDirty = true;
                }
            }
        }

        public int? SelectedIndex
        {
            get
            {
                return this.m_selectedItemIndex;
            }
            set
            {
                if (value != null && (value.Value < 0 || value.Value >= this.m_items.Count))
                {
                    value = null;
                }
                int? num = value;
                int? selectedItemIndex = this.m_selectedItemIndex;
                if (!(num.GetValueOrDefault() == selectedItemIndex.GetValueOrDefault() & num != null == (selectedItemIndex != null)))
                {
                    this.m_selectedItemIndex = value;
                    if (this.SelectionChanged != null)
                    {
                        this.SelectionChanged();
                    }
                }
            }
        }

        public object SelectedItem
        {
            get
            {
                if (this.m_selectedItemIndex == null)
                {
                    return null;
                }
                return this.m_items[this.m_selectedItemIndex.Value];
            }
            set
            {
                int num = this.m_items.IndexOf(value);
                this.SelectedIndex = ((num >= 0) ? new int?(num) : null);
            }
        }

        public ReadOnlyList<object> Items
        {
            get
            {
                return new ReadOnlyList<object>(this.m_items);
            }
        }

        public Color SelectionColor { get; set; }

        public void AddItem(object item)
        {
            this.m_items.Add(item);
            this.m_widgetsDirty = true;
        }

        public void RemoveItem(object item)
        {
            int num = this.m_items.IndexOf(item);
            if (num >= 0)
            {
                this.RemoveItemAt(num);
            }
        }

        public void RemoveItemAt(int index)
        {
            object obj = this.m_items[index];
            this.m_items.RemoveAt(index);
            this.m_widgetsByIndex.Clear();
            this.m_widgetsDirty = true;
            int? selectedIndex = this.SelectedIndex;
            if (index == selectedIndex.GetValueOrDefault() & selectedIndex != null)
            {
                this.SelectedIndex = null;
            }
        }

        public void ClearItems()
        {
            this.m_items.Clear();
            this.m_widgetsByIndex.Clear();
            this.m_widgetsDirty = true;
            this.SelectedIndex = null;
        }

        public override float CalculateScrollAreaLength()
        {
            return Items.Count * this.ItemSize;
        }

        public void ScrollToItem(object item)
        {
            int num = this.m_items.IndexOf(item);
            if (num >= 0)
            {
                float num2 = num * this.ItemSize;
                float num3 = (this.Direction == LayoutDirection.Horizontal) ? ActualSize.X : ActualSize.Y;
                if (num2 < this.ScrollPosition)
                {
                    this.ScrollPosition = num2;
                    return;
                }
                if (num2 > this.ScrollPosition + num3 - this.ItemSize)
                {
                    this.ScrollPosition = num2 - num3 + this.ItemSize;
                }
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            if (this.m_widgetsDirty)
            {
                this.m_widgetsDirty = false;
                this.CreateListWidgets((this.Direction == LayoutDirection.Horizontal) ? ActualSize.X : ActualSize.Y);
            }
            IsDrawRequired = true;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        widget.Measure(new Vector2(this.ItemSize, MathUtils.Max(parentAvailableSize.Y - 2f * widget.Margin.Y, 0f)));
                    }
                    else
                    {
                        widget.Measure(new Vector2(MathUtils.Max(parentAvailableSize.X - 2f * widget.Margin.X, 0f), this.ItemSize));
                    }
                }
            }
        }

        public override void ArrangeOverride()
        {
            if (ActualSize != this.lastActualSize)
            {
                this.m_widgetsDirty = true;
            }
            this.lastActualSize = ActualSize;
            int num = this.m_firstVisibleIndex;
            foreach (Widget widget in this.Children)
            {
                if (this.Direction == LayoutDirection.Horizontal)
                {
                    Vector2 vector = new Vector2(num * this.ItemSize - this.ScrollPosition, 0f);
                    ArrangeChildWidgetInCell(vector, vector + new Vector2(this.ItemSize, ActualSize.Y), widget);
                }
                else
                {
                    Vector2 vector2 = new Vector2(0f, num * this.ItemSize - this.ScrollPosition);
                    ArrangeChildWidgetInCell(vector2, vector2 + new Vector2(ActualSize.X, this.ItemSize), widget);
                }
                num++;
            }
        }

        public override void Update()
        {
            bool flag = this.ScrollSpeed != 0f;
            base.Update();
            if (Input.Tap != null && HitTestPanel(Input.Tap.Value))
            {
                this.m_clickAllowed = !flag;
            }
            if (Input.Click != null && this.m_clickAllowed && HitTestPanel(Input.Click.Value.Start) && HitTestPanel(Input.Click.Value.End))
            {
                int num = this.PositionToItemIndex(Input.Click.Value.End);
                if (this.ItemClicked != null && num >= 0 && num < this.m_items.Count)
                {
                    this.ItemClicked(this.Items[num]);
                }
                this.SelectedIndex = new int?(num);
                if (this.SelectedIndex != null)
                {
                    AudioManager.PlaySound(Sounds.Click2, false, 1f, 1f, 0f);
                }
            }
        }

        public override void Draw()
        {
            if (this.SelectedIndex != null && this.SelectedIndex.Value >= this.m_firstVisibleIndex && this.SelectedIndex.Value <= this.m_lastVisibleIndex)
            {
                Vector2 vector;
                if (this.Direction == LayoutDirection.Horizontal)
                {
                    vector = new Vector2(SelectedIndex.Value * this.ItemSize - this.ScrollPosition, 0f);
                }
                else
                {
                    vector = new Vector2(0f, SelectedIndex.Value * this.ItemSize - this.ScrollPosition);
                }
                FlatBatch2D flatBatch2D = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
                int count = flatBatch2D.TriangleVertices.Count;
                Vector2 v = (this.Direction == LayoutDirection.Horizontal) ? new Vector2(this.ItemSize, ActualSize.Y) : new Vector2(ActualSize.X, this.ItemSize);
                flatBatch2D.QueueQuad(vector, vector + v, 0f, this.SelectionColor * GlobalColorTransform);
                flatBatch2D.TransformTriangles(GlobalTransform, count, -1);
            }
            base.Draw();
        }

        private int PositionToItemIndex(Vector2 position)
        {
            Vector2 vector = ScreenToWidget(position);
            if (this.Direction == LayoutDirection.Horizontal)
            {
                return (int)((vector.X + this.ScrollPosition) / this.ItemSize);
            }
            return (int)((vector.Y + this.ScrollPosition) / this.ItemSize);
        }

        private void CreateListWidgets(float size)
        {
            this.Children.Clear();
            if (this.m_items.Count > 0)
            {
                int x = (int)MathUtils.Floor(this.ScrollPosition / this.ItemSize);
                int x2 = (int)MathUtils.Floor((this.ScrollPosition + size) / this.ItemSize);
                this.m_firstVisibleIndex = MathUtils.Max(x, 0);
                this.m_lastVisibleIndex = MathUtils.Min(x2, this.m_items.Count - 1);
                for (int i = this.m_firstVisibleIndex; i <= this.m_lastVisibleIndex; i++)
                {
                    object obj = this.m_items[i];
                    Widget widget;
                    if (!this.m_widgetsByIndex.TryGetValue(i, out widget))
                    {
                        widget = this.ItemWidgetFactory(obj);
                        widget.Tag = obj;
                        this.m_widgetsByIndex.Add(i, widget);
                    }
                    this.Children.Add(widget);
                }
            }
        }

        private List<object> m_items = new List<object>();

        private int? m_selectedItemIndex;

        private Dictionary<int, Widget> m_widgetsByIndex = new Dictionary<int, Widget>();

        private int m_firstVisibleIndex;

        private int m_lastVisibleIndex;

        private float m_itemSize;

        private bool m_widgetsDirty;

        private bool m_clickAllowed;

        private Vector2 lastActualSize = new Vector2(-1f);
    }
}
