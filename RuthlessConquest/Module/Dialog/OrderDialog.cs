// Decompiled with JetBrains decompiler
// Type: Game.OrderDialog
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class OrderDialog : ListSelectionDialog
    {
        private HumanPlayer Player1;
        private Planet TargetPlanet;

        public OrderDialog(
          HumanPlayer player,
          Planet targetPlanet,
          IEnumerable<Planet> selectedPlanets)
          : base("Issue Order", CreateItems(player, targetPlanet, selectedPlanets), 68f, o => new OrderDialog.OrderWidget(player, targetPlanet, selectedPlanets, (int)o), null)
        {
            OrderDialog orderDialog = this;
            this.SelectionColor = Player.GetColor(player.Faction);
            this.Player1 = player;
            this.TargetPlanet = targetPlanet;
            this.ContentSize = new Vector2(420f, 300f);
            this.SelectionHandler = this.SelectionHandler + (o =>
            {
                switch ((int)o)
                {
                    case 0:
                        orderDialog.Player1.IssueMoveOrder(orderDialog.TargetPlanet, selectedPlanets, 1f, Faction.None);
                        break;
                    case 1:
                        orderDialog.Player1.IssueMoveOrder(orderDialog.TargetPlanet, selectedPlanets, 0.2f, Faction.None);
                        break;
                    case 2:
                        orderDialog.Player1.IssueContinuousMoveOrder(orderDialog.TargetPlanet, selectedPlanets);
                        break;
                    case 3:
                        DialogsManager.ShowDialog(null, new GiftOrderDialog(orderDialog.Player1, orderDialog.TargetPlanet, selectedPlanets));
                        break;
                    case 4:
                        if (targetPlanet.CanLaunchSatellite(false))
                        {
                            orderDialog.Player1.IssueLaunchSatelliteOrder(targetPlanet);
                            break;
                        }
                        player.Game.Screen.MessagesListWidget.AddMessage("Cannot launch satellite", Planet.GetColor(player.Faction), true);
                        break;
                }
            });
        }

        private static IEnumerable<int> CreateItems(
          HumanPlayer player,
          Planet targetPlanet,
          IEnumerable<Planet> selectedPlanets)
        {
            if (selectedPlanets.Where<Planet>(p => p != targetPlanet).Count<Planet>() > 0)
            {
                yield return 0;
                yield return 1;
                yield return 2;
                if (player.Faction != targetPlanet.Faction)
                    yield return 3;
            }
            if (player.Faction == targetPlanet.Faction && targetPlanet.CanLaunchSatellite(true))
                yield return 4;
        }

        private class OrderWidget : StackPanelWidget
        {
            private HumanPlayer Player { get; }

            private Planet TargetPlanet { get; }

            private IEnumerable<Planet> SelectedPlanets { get; }

            private int OrderIndex { get; }

            private LabelWidget Label1 { get; }

            private LabelWidget Label2 { get; }

            public OrderWidget(
              HumanPlayer player,
              Planet targetPlanet,
              IEnumerable<Planet> selectedPlanets,
              int orderIndex)
            {
                this.Player = player;
                this.TargetPlanet = targetPlanet;
                this.SelectedPlanets = selectedPlanets;
                this.OrderIndex = orderIndex;
                this.Direction = LayoutDirection.Vertical;
                this.Children.Add(this.Label1 = new LabelWidget());
                WidgetsList children = this.Children;
                LabelWidget labelWidget1 = new LabelWidget();
                labelWidget1.Font = Fonts.Small;
                labelWidget1.Color = Colors.ForeDisabled;
                LabelWidget labelWidget2 = labelWidget1;
                this.Label2 = labelWidget1;
                LabelWidget labelWidget3 = labelWidget2;
                children.Add(labelWidget3);
            }

            public override void MeasureOverride(Vector2 parentAvailableSize)
            {
                IEnumerable<Planet> source = this.SelectedPlanets.Where<Planet>(p => p != this.TargetPlanet);
                if (this.OrderIndex == 0)
                {
                    this.Label1.Text = "SEND ALL";
                    this.Label2.Text = string.Format("{0} ships", source.Sum<Planet>(p => p.ShipsCount));
                }
                else if (this.OrderIndex == 1)
                {
                    this.Label1.Text = "SEND 20%";
                    this.Label2.Text = string.Format("{0} ships", MathUtils.Ceiling(source.Sum<Planet>(p => p.ShipsCount) * 0.2f));
                }
                else if (this.OrderIndex == 2)
                {
                    this.Label1.Text = "CONTINUOUS";
                    this.Label2.Text = "Sends ships as they are produced";
                }
                else if (this.OrderIndex == 3)
                {
                    this.Label1.Text = "GIFT SHIPS...";
                    this.Label2.Text = "Gift ships to another player";
                }
                else if (this.OrderIndex == 4)
                {
                    this.Label1.Text = this.TargetPlanet.Satellites.Count<Satellite>() == 0 ? "LAUNCH 1st SATELLITE" : "LAUNCH 2nd SATELLITE";
                    this.Label1.Color = this.TargetPlanet.CanLaunchSatellite(false) ? Colors.Fore : Colors.ForeDisabled;
                    this.Label2.Text = string.Format("Costs {0} ships", this.TargetPlanet.GetSatelliteCost());
                }
                base.MeasureOverride(parentAvailableSize);
            }
        }
    }
}
