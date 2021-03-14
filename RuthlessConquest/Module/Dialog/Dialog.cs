using System;

namespace Game
{
    internal class Dialog : CanvasWidget
    {
        public Dialog()
        {
            this.IsHitTestVisible = true;
        }

        public DialogCoverWidget DialogCoverWidget
        {
            get
            {
                if (ParentWidget != null)
                {
                    int num = ParentWidget.Children.IndexOf(this);
                    return (DialogCoverWidget)ParentWidget.Children[num - 1];
                }
                return null;
            }
        }

        public virtual void DialogShown()
        {
        }

        public virtual void DialogHidden()
        {
        }
    }
}
