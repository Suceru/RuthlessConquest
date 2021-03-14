// Decompiled with JetBrains decompiler
// Type: Game.ContainerWidget
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using System.Collections.Generic;

namespace Game
{
    public abstract class ContainerWidget : Widget
    {
        public readonly WidgetsList Children;

        public IEnumerable<Widget> AllChildren
        {
            get
            {
                foreach (Widget child in this.Children)
                {
                    Widget childWidget = child;
                    yield return childWidget;
                    if (childWidget is ContainerWidget containerWidget)
                    {
                        foreach (Widget allChild in containerWidget.AllChildren)
                            yield return allChild;
                    }
                    childWidget = null;
                }
            }
        }

        protected ContainerWidget() => this.Children = new WidgetsList(this);

        public override void UpdateCeases()
        {
            foreach (Widget child in this.Children)
                child.UpdateCeases();
        }

        public virtual void WidgetAdded(Widget widget)
        {
        }

        public virtual void WidgetRemoved(Widget widget)
        {
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            foreach (Widget child in this.Children)
                child.Measure(Vector2.Max(parentAvailableSize - 2f * child.Margin, Vector2.Zero));
        }

        protected static void ArrangeChildWidgetInCell(Vector2 c1, Vector2 c2, Widget widget)
        {
            Vector2 zero1 = Vector2.Zero;
            Vector2 zero2 = Vector2.Zero;
            Vector2 vector2 = c2 - c1;
            Vector2 margin = widget.Margin;
            Vector2 parentDesiredSize = widget.ParentDesiredSize;
            parentDesiredSize.X = MathUtils.Min(parentDesiredSize.X, MathUtils.Max(vector2.X - 2f * margin.X, 0.0f));
            parentDesiredSize.Y = MathUtils.Min(parentDesiredSize.Y, MathUtils.Max(vector2.Y - 2f * margin.Y, 0.0f));
            if (float.IsPositiveInfinity(widget.Alignment.X))
            {
                zero1.X = c1.X + margin.X;
                zero2.X = MathUtils.Max(vector2.X - 2f * margin.X, 0.0f);
            }
            else
            {
                zero1.X = MathUtils.Lerp(c1.X + margin.X, c2.X - parentDesiredSize.X - margin.X, (float)(widget.Alignment.X / 2.0 + 0.5));
                zero2.X = parentDesiredSize.X;
            }
            if (float.IsPositiveInfinity(widget.Alignment.Y))
            {
                zero1.Y = c1.Y + margin.Y;
                zero2.Y = MathUtils.Max(vector2.Y - 2f * margin.Y, 0.0f);
            }
            else
            {
                zero1.Y = MathUtils.Lerp(c1.Y + margin.Y, c2.Y - parentDesiredSize.Y - margin.Y, (float)(widget.Alignment.Y / 2.0 + 0.5));
                zero2.Y = parentDesiredSize.Y;
            }
            widget.Arrange(zero1, zero2);
        }
    }
}


/*// Decompiled with JetBrains decompiler
// Type: Game.ContainerWidget
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using System.Collections.Generic;

namespace Game
{
  public abstract class ContainerWidget : Widget
  {
    public readonly WidgetsList Children;

    public IEnumerable<Widget> AllChildren
    {
      get
      {
        foreach (Widget child in this.Children)
        {
          Widget childWidget = child;
          yield return childWidget;
          if (childWidget is ContainerWidget containerWidget)
          {
            foreach (Widget allChild in containerWidget.AllChildren)
              yield return allChild;
          }
          childWidget = (Widget) null;
        }
      }
    }

    protected ContainerWidget() => this.Children = new WidgetsList(this);

    public override void UpdateCeases()
    {
      foreach (Widget child in this.Children)
        child.UpdateCeases();
    }

    public virtual void WidgetAdded(Widget widget)
    {
    }

    public virtual void WidgetRemoved(Widget widget)
    {
    }

    public override void MeasureOverride(Vector2 parentAvailableSize)
    {
      foreach (Widget child in this.Children)
        child.Measure(Vector2.Max(parentAvailableSize - 2f * child.Margin, Vector2.Zero));
    }

    protected static void ArrangeChildWidgetInCell(Vector2 c1, Vector2 c2, Widget widget)
    {
      Vector2 zero1 = Vector2.Zero;
      Vector2 zero2 = Vector2.Zero;
      Vector2 vector2 = c2 - c1;
      Vector2 margin = widget.Margin;
      Vector2 parentDesiredSize = widget.ParentDesiredSize;
      parentDesiredSize.X = MathUtils.Min(parentDesiredSize.X, MathUtils.Max(vector2.X - 2f * margin.X, 0.0f));
      parentDesiredSize.Y = MathUtils.Min(parentDesiredSize.Y, MathUtils.Max(vector2.Y - 2f * margin.Y, 0.0f));
      if (float.IsPositiveInfinity(widget.Alignment.X))
      {
        zero1.X = c1.X + margin.X;
        zero2.X = MathUtils.Max(vector2.X - 2f * margin.X, 0.0f);
      }
      else
      {
        zero1.X = MathUtils.Lerp(c1.X + margin.X, c2.X - parentDesiredSize.X - margin.X, (float) ((double) widget.Alignment.X / 2.0 + 0.5));
        zero2.X = parentDesiredSize.X;
      }
      if (float.IsPositiveInfinity(widget.Alignment.Y))
      {
        zero1.Y = c1.Y + margin.Y;
        zero2.Y = MathUtils.Max(vector2.Y - 2f * margin.Y, 0.0f);
      }
      else
      {
        zero1.Y = MathUtils.Lerp(c1.Y + margin.Y, c2.Y - parentDesiredSize.Y - margin.Y, (float) ((double) widget.Alignment.Y / 2.0 + 0.5));
        zero2.Y = parentDesiredSize.Y;
      }
      widget.Arrange(zero1, zero2);
    }
  }
}
*/