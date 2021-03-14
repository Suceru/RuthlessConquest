using System;
using System.Collections;
using System.Xml.Linq;
using Engine;
using Engine.Content;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 列表选择对话框
    /// 继承:对话框
    /// </summary>
    internal class ListSelectionDialog : Dialog
    {
        public ListPanelWidget ListWidget { get; }

        public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, Widget> itemWidgetFactory, Action<object> selectionHandler)
        {
            this.SelectionHandler = selectionHandler;
            XElement node = ContentCache.Get<XElement>("Dialogs/ListSelectionDialog", true);
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.TitleLabelWidget = this.Children.Find<LabelWidget>("ListSelectionDialog.Title", true);
            this.ListWidget = this.Children.Find<ListPanelWidget>("ListSelectionDialog.List", true);
            this.ContentWidget = this.Children.Find<CanvasWidget>("ListSelectionDialog.Content", true);
            this.TitleLabelWidget.Text = title;
            this.TitleLabelWidget.IsVisible = !string.IsNullOrEmpty(title);
            this.ListWidget.ItemSize = itemSize;
            if (itemWidgetFactory != null)
            {
                this.ListWidget.ItemWidgetFactory = itemWidgetFactory;
            }
            foreach (object item in items)
            {
                this.ListWidget.AddItem(item);
            }
            this.OptimizeSize();
        }

        public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, string> itemToStringConverter, Action<object> selectionHandler) : this(title, items, itemSize, (object item) => new LabelWidget
        {
            Text = itemToStringConverter(item)
        }, selectionHandler)
        {
        }

        private void OptimizeSize()
        {
            for (int i = this.ListWidget.Items.Count; i >= 0; i--)
            {
                float num = MathUtils.Min(i + 0.5f, ListWidget.Items.Count);
                if (this.Direction == LayoutDirection.Vertical)
                {
                    if (num * this.ListWidget.ItemSize <= this.ContentWidget.Size.Y)
                    {
                        this.ContentWidget.Size = new Vector2(this.ContentWidget.Size.X, num * this.ListWidget.ItemSize);
                        return;
                    }
                }
                else if (num * this.ListWidget.ItemSize <= this.ContentWidget.Size.X)
                {
                    this.ContentWidget.Size = new Vector2(num * this.ListWidget.ItemSize, this.ContentWidget.Size.Y);
                    return;
                }
            }
        }

        public Color SelectionColor
        {
            get
            {
                return this.ListWidget.SelectionColor;
            }
            set
            {
                this.ListWidget.SelectionColor = value;
            }
        }

        public LayoutDirection Direction
        {
            get
            {
                return this.ListWidget.Direction;
            }
            set
            {
                if (value != this.ListWidget.Direction)
                {
                    this.ListWidget.Direction = value;
                    this.OptimizeSize();
                }
            }
        }

        public Vector2 ContentSize
        {
            get
            {
                return this.ContentWidget.Size;
            }
            set
            {
                if (value != this.ContentWidget.Size)
                {
                    this.ContentWidget.Size = value;
                    this.OptimizeSize();
                }
            }
        }

        public override void Update()
        {
            if (Input.Back || Input.Cancel)
            {
                this.DismissTime = new double?(0.0);
            }
            else if (Input.Tap != null && !this.ListWidget.HitTest(Input.Tap.Value))
            {
                this.DismissTime = new double?(0.0);
            }
            else if (this.DismissTime == null && this.ListWidget.SelectedItem != null)
            {
                this.DismissTime = new double?(Time.FrameStartTime + 0.05000000074505806);
            }
            if (this.DismissTime != null && Time.FrameStartTime >= this.DismissTime.Value)
            {
                this.Dismiss(this.ListWidget.SelectedItem);
            }
        }

        private void Dismiss(object result)
        {
            if (!this.IsDismissed)
            {
                this.IsDismissed = true;
                DialogsManager.HideDialog(this, true);
                if (this.SelectionHandler != null && result != null)
                {
                    this.SelectionHandler(result);
                }
            }
        }

        private LabelWidget TitleLabelWidget;

        private CanvasWidget ContentWidget;

        private double? DismissTime;

        private bool IsDismissed;

        public Action<object> SelectionHandler;
    }
}
