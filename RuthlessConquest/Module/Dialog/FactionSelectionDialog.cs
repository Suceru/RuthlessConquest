using System;
using System.Linq;
using Engine;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 功能选择对话框
    /// 继承:列表选择对话框
    /// </summary>
    internal class FactionSelectionDialog : ListSelectionDialog
    {
        private static Widget ItemWidgetFactory(object item)
        {
            Faction faction = (Faction)item;
            return new CanvasWidget
            {
                Margin = new Vector2(0f, 0f),
                Children =
                {
                    new BevelledRectangleWidget
                    {
                        Size = new Vector2(72f, 72f),
                        CenterColor = Player.GetColor(faction),
                        BevelColor = Player.GetColor(faction)
                    },
                    new RectangleWidget
                    {
                        Size = new Vector2(56f, 56f),
                        FillColor = Color.Black,
                        OutlineColor = Color.Transparent,
                        Subtexture = Ship.GetTexture(faction)
                    }
                }
            };
        }

        public FactionSelectionDialog(Action<object> selectionHandler) : base("Select Faction", Enumerable.Range(0, 6), 96f, (object i) => ItemWidgetFactory(i), selectionHandler)
        {
            Direction = LayoutDirection.Horizontal;
            ContentSize = new Vector2(640f, 120f);
        }
    }
}
