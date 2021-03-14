using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 赠送命令对话框
    /// 继承:列表选择对话框
    /// </summary>
    internal class GiftOrderDialog : ListSelectionDialog
    {
        public GiftOrderDialog(HumanPlayer player, Planet targetPlanet, IEnumerable<Planet> selectedPlanets) : base("Gift Ships", CreateItems(player, targetPlanet, selectedPlanets), 68f, o => new GiftOrderDialog.OrderWidget(player, targetPlanet, selectedPlanets, (int)o), null)
        {
            GiftOrderDialog giftOrderDialog = this;
            this.SelectionColor = Player.GetColor(player.Faction);
            this.Player1 = player;
            this.TargetPlanet = targetPlanet;
            this.ContentSize = new Vector2(420f, 300f);
            this.SelectionHandler = this.SelectionHandler + (o =>
            {
                switch ((int)o)
                {
                    case 0:
                        giftOrderDialog.Player1.IssueMoveOrder(giftOrderDialog.TargetPlanet, selectedPlanets, 1f, giftOrderDialog.TargetPlanet.Faction);
                        break;
                    case 1:
                        giftOrderDialog.Player1.IssueMoveOrder(giftOrderDialog.TargetPlanet, selectedPlanets, 0.5f, giftOrderDialog.TargetPlanet.Faction);
                        break;
                    case 2:
                        giftOrderDialog.Player1.IssueMoveOrder(giftOrderDialog.TargetPlanet, selectedPlanets, 0.2f, giftOrderDialog.TargetPlanet.Faction);
                        break;
                }
            });
        }
        /*		public GiftOrderDialog(HumanPlayer player, Planet targetPlanet, IEnumerable<Planet> selectedPlanets) : base("Gift Ships", GiftOrderDialog.CreateItems(player, targetPlanet, selectedPlanets), 68f, (object o) => new GiftOrderDialog.OrderWidget(player, targetPlanet, selectedPlanets, (int)o), null)
		{
			GiftOrderDialog <>4__this = this;
			base.SelectionColor = Game.Player.GetColor(player.Faction);
			this.Player = player;
			this.TargetPlanet = targetPlanet;
			base.ContentSize = new Vector2(420f, 300f);
			this.SelectionHandler = (Action<object>)Delegate.Combine(this.SelectionHandler, new Action<object>(delegate(object o)
			{
				int num = (int)o;
				if (num == 0)
				{
					<>4__this.Player.IssueMoveOrder(<>4__this.TargetPlanet, selectedPlanets, 1f, <>4__this.TargetPlanet.Faction);
					return;
				}
				if (num == 1)
				{
					<>4__this.Player.IssueMoveOrder(<>4__this.TargetPlanet, selectedPlanets, 0.5f, <>4__this.TargetPlanet.Faction);
					return;
				}
				if (num == 2)
				{
					<>4__this.Player.IssueMoveOrder(<>4__this.TargetPlanet, selectedPlanets, 0.2f, <>4__this.TargetPlanet.Faction);
				}
			}));
		}*/
        /// <summary>
        /// 私有静态公开枚举数
        /// 创建项目
        /// 选择赠送的数目
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="targetPlanet"></param>
        /// <param name="selectedPlanets"></param>
        /// <returns></returns>
        private static IEnumerable<int> CreateItems(HumanPlayer player, Planet targetPlanet, IEnumerable<Planet> selectedPlanets)
        {
            yield return 0;
            yield return 1;
            yield return 2;
            yield break;
        }

        private HumanPlayer Player1;

        private Planet TargetPlanet;

        private class OrderWidget : StackPanelWidget
        {
            private HumanPlayer Player { get; }

            private Planet TargetPlanet { get; }

            private IEnumerable<Planet> SelectedPlanets { get; }

            private int OrderIndex { get; }

            private LabelWidget Label1 { get; }

            private LabelWidget Label2 { get; }

            public OrderWidget(HumanPlayer player, Planet targetPlanet, IEnumerable<Planet> selectedPlanets, int orderIndex)
            {
                this.Player = player;
                this.TargetPlanet = targetPlanet;
                this.SelectedPlanets = selectedPlanets;
                this.OrderIndex = orderIndex;
                Direction = LayoutDirection.Vertical;
                this.Children.Add(this.Label1 = new LabelWidget());
                WidgetsList children = this.Children;
                LabelWidget labelWidget = new LabelWidget();
                labelWidget.Font = Fonts.Small;
                labelWidget.Color = Colors.ForeDisabled;
                LabelWidget widget = labelWidget;
                this.Label2 = labelWidget;
                children.Add(widget);
            }

            public override void MeasureOverride(Vector2 parentAvailableSize)
            {
                IEnumerable<Planet> source = from p in this.SelectedPlanets where p != this.TargetPlanet select p;
                switch (this.OrderIndex) {
                    case 0:
                        this.Label1.Text = "GIFT ALL";
                        this.Label2.Text = string.Format("{0} ships", source.Sum((Planet p) => p.ShipsCount));
                        break;
                    case 1:
                        this.Label1.Text = "GIFT 50%";
                        this.Label2.Text = string.Format("{0} ships", MathUtils.Ceiling(source.Sum((Planet p) => p.ShipsCount) * 0.5f));
                        break;
                    case 2:
                        this.Label1.Text = "GIFT 20%";
                        this.Label2.Text = string.Format("{0} ships", MathUtils.Ceiling(source.Sum((Planet p) => p.ShipsCount) * 0.2f));
                        break;
                }
               /* if (this.OrderIndex == 0)
                {
                    this.Label1.Text = "GIFT ALL";
                    this.Label2.Text = string.Format("{0} ships", source.Sum((Planet p) => p.ShipsCount));
                }
                else if (this.OrderIndex == 1)
                {
                    this.Label1.Text = "GIFT 50%";
                    this.Label2.Text = string.Format("{0} ships", MathUtils.Ceiling((float)source.Sum((Planet p) => p.ShipsCount) * 0.5f));
                }
                else if (this.OrderIndex == 2)
                {
                    this.Label1.Text = "GIFT 20%";
                    this.Label2.Text = string.Format("{0} ships", MathUtils.Ceiling((float)source.Sum((Planet p) => p.ShipsCount) * 0.2f));
                }*/
                base.MeasureOverride(parentAvailableSize);
            }
        }
    }
}
