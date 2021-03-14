using System;
using Engine;

namespace Game
{
    public class DragHostWidget : ContainerWidget
    {
        public DragHostWidget()
        {
            this.IsHitTestVisible = false;
        }

        public bool IsDragInProgress
        {
            get
            {
                return this.m_dragWidget != null;
            }
        }

        public void BeginDrag(Widget dragWidget, object dragData, Action dragEndedHandler)
        {
            if (this.m_dragWidget == null)
            {
                this.m_dragWidget = dragWidget;
                this.m_dragData = dragData;
                this.m_dragEndedHandler = dragEndedHandler;
                this.Children.Add(this.m_dragWidget);
                this.UpdateDragPosition();
            }
        }

        public void EndDrag()
        {
            if (this.m_dragWidget != null)
            {
                this.Children.Remove(this.m_dragWidget);
                this.m_dragWidget = null;
                this.m_dragData = null;
                if (this.m_dragEndedHandler != null)
                {
                    this.m_dragEndedHandler();
                    this.m_dragEndedHandler = null;
                }
            }
        }

        public override void Update()
        {
            if (this.m_dragWidget != null)
            {
                this.UpdateDragPosition();
                IDragTargetWidget dragTargetWidget = WidgetsManager.HitTest(this.m_dragPosition, (Widget w) => w is IDragTargetWidget) as IDragTargetWidget;
                if (Input.Drag != null)
                {
                    if (dragTargetWidget != null)
                    {
                        dragTargetWidget.DragOver(this.m_dragWidget, this.m_dragData);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        if (dragTargetWidget != null)
                        {
                            dragTargetWidget.DragDrop(this.m_dragWidget, this.m_dragData);
                        }
                    }
                    finally
                    {
                        this.EndDrag();
                    }
                }
            }
        }

        public override void ArrangeOverride()
        {
            foreach (Widget widget in this.Children)
            {
                Vector2 parentDesiredSize = widget.ParentDesiredSize;
                parentDesiredSize.X = MathUtils.Min(parentDesiredSize.X, ActualSize.X);
                parentDesiredSize.Y = MathUtils.Min(parentDesiredSize.Y, ActualSize.Y);
                widget.Arrange(ScreenToWidget(this.m_dragPosition) - 0.5f * parentDesiredSize, parentDesiredSize);
            }
        }

        private void UpdateDragPosition()
        {
            if (Input.Drag != null)
            {
                this.m_dragPosition = Input.Drag.Value;
                this.m_dragPosition.X = MathUtils.Clamp(this.m_dragPosition.X, GlobalBounds.Min.X, GlobalBounds.Max.X - 1f);
                this.m_dragPosition.Y = MathUtils.Clamp(this.m_dragPosition.Y, GlobalBounds.Min.Y, GlobalBounds.Max.Y - 1f);
            }
        }

        private Widget m_dragWidget;

        private object m_dragData;

        private Action m_dragEndedHandler;

        private Vector2 m_dragPosition;
    }
}
