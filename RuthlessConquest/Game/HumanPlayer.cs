using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
    internal class HumanPlayer : Player
    {
        public bool BlinkPlanets { get; private set; }

        private HumanPlayer()
        {
        }

        public HumanPlayer(PlayerDescription playerDescription) : base(playerDescription)
        {
        }

        public void AddOrderToOutgoingList(Planet sourcePlanet, IEnumerable<Planet> route, int shipsCount, bool launchSatellite, Faction giftToFaction)
        {
            int num = MathUtils.Min(shipsCount, sourcePlanet.ShipsCount);
            if ((num > 0 && route.Count<Planet>() > 0) || launchSatellite)
            {
                StepModule.OutgoingOrders.Add(new Order
                {
                    ShipsCount = num,
                    PlanetIndex = PlanetsModule.Planets.IndexOf(sourcePlanet),
                    RouteIndexes = (from p in route
                                    select PlanetsModule.Planets.IndexOf(p)).ToDynamicArray<int>(),
                    LaunchSatellite = launchSatellite,
                    GiftToFaction = giftToFaction
                });
            }
        }

        public void IssueMoveOrder(Planet targetPlanet, IEnumerable<Planet> sourcePlanets, float strength, Faction giftToFaction)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = true;
            using (IEnumerator<Planet> enumerator = sourcePlanets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Planet sourcePlanet = enumerator.Current;
                    if (sourcePlanet != targetPlanet)
                    {
                        if (sourcePlanet.ContinuousOrderRoute != null)
                        {
                            flag = true;
                            sourcePlanet.ContinuousOrderRoute = null;
                        }
                        IEnumerable<Planet> planets;
                        if (giftToFaction == Faction.None)
                        {
                            planets = PlanetsModule.Planets;
                        }
                        else
                        {
                            planets = from p in PlanetsModule.Planets
                                      where p.Faction == sourcePlanet.Faction || p.Faction == giftToFaction
                                      select p;
                        }
                        DynamicArray<Planet> dynamicArray = RouteFinderModule.FindRoute(sourcePlanet, targetPlanet, planets);
                        if (dynamicArray.Count > 0)
                        {
                            flag3 = false;
                            flag2 = true;
                            this.AddOrderToOutgoingList(sourcePlanet, dynamicArray, (int)MathUtils.Ceiling(sourcePlanet.ShipsCount * strength), false, giftToFaction);
                            dynamicArray.Insert(0, sourcePlanet);
                            this.AddVisualRoute(new VisualRoute(dynamicArray, Ship.GetColor((giftToFaction == Faction.None) ? Faction : giftToFaction)));
                            AudioManager.PlaySound(Sounds.Order, true, 1f, 1f, 0f);
                        }
                    }
                }
            }
            if (flag)
            {
                Game.Screen.MessagesListWidget.AddMessage("Continuous order cleared", GetColor(Faction), true);
            }
            if (flag3)
            {
                Game.Screen.MessagesListWidget.AddMessage("No route found", GetColor(Faction), true);
            }
            if (flag2)
            {
                this.DeselectAllPlanets();
            }
        }

        public void IssueContinuousMoveOrder(Planet targetPlanet, IEnumerable<Planet> sourcePlanets)
        {
            bool flag = false;
            foreach (Planet planet in sourcePlanets)
            {
                if (planet != targetPlanet)
                {
                    DynamicArray<Planet> dynamicArray = RouteFinderModule.FindRoute(planet, targetPlanet, true);
                    if (dynamicArray.Count > 0)
                    {
                        this.AddOrderToOutgoingList(planet, dynamicArray, planet.ShipsCount, false, Faction.None);
                        flag = true;
                        planet.ContinuousOrderRoute = dynamicArray.ToDynamicArray<Planet>();
                        dynamicArray.Insert(0, planet);
                        this.AddVisualRoute(new VisualRoute(dynamicArray, Ship.GetColor(Faction)));
                        AudioManager.PlaySound(Sounds.Order, true, 1f, 1f, 0f);
                    }
                }
            }
            if (flag)
            {
                Game.Screen.MessagesListWidget.AddMessage("Ships will be continuously sent", Planet.GetColor(Faction), true);
                this.DeselectAllPlanets();
            }
        }

        public void IssueLaunchSatelliteOrder(Planet planet)
        {
            if (planet.CanLaunchSatellite(false))
            {
                this.AddOrderToOutgoingList(planet, new Planet[0], 0, true, Faction.None);
                this.DeselectAllPlanets();
            }
        }

        protected internal override void OnAdded()
        {
            this.BlinkPlanets = IsControllingPlayer;
        }

        public override void Update()
        {
            if (PlayersModule.ControllingPlayer != this)
            {
                return;
            }
            if (StepModule.IsGameStarted && StepModule.CountdownStepsLeft <= 0 && !PlayersModule.IsGameFinished)
            {
                WidgetInput input = Game.Screen.GameWidget.Input;
                if (input.Drag != null)
                {
                    if (this.DragStartPosition == null)
                    {
                        Widget widget = WidgetsManager.HitTest(input.Drag.Value);
                        if (widget == Game.Screen.GameWidget || (widget is PlayerLabelWidget && widget.IsChildWidgetOf(Game.Screen)))
                        {
                            this.DragStartPosition = input.Drag;
                        }
                    }
                    if (this.DragStartPosition != null)
                    {
                        this.DragEndPosition = new Vector2?(input.Drag.Value);
                        this.HandleDrag(new Segment2(this.DragStartPosition.Value, this.DragEndPosition.Value));
                    }
                }
                else
                {
                    this.DragStartPosition = null;
                    this.DragEndPosition = null;
                    if (input.Hold != null && WidgetsManager.HitTest(input.Hold.Value) == Game.Screen.GameWidget)
                    {
                        this.HandleHold(input.Hold.Value);
                    }
                    else if (input.Click != null && WidgetsManager.HitTest(input.Click.Value.Start) == Game.Screen.GameWidget)
                    {
                        this.HandleClick(input.Click.Value);
                    }
                    else if (input.SecondaryClick != null && WidgetsManager.HitTest(input.SecondaryClick.Value.Start) == Game.Screen.GameWidget)
                    {
                        this.HandleSecondaryClick(input.SecondaryClick.Value);
                    }
                }
            }
            else
            {
                this.DragStartPosition = null;
                this.DragEndPosition = null;
            }
            PlayerLabelWidget playerLabelWidget = Game.Screen.FindPlayerLabel(this);
            if (playerLabelWidget != null && playerLabelWidget.IsSatelliteButtonClicked)
            {
                Game.StepModule.OutgoingOrders.Add(new Order
                {
                    EnableDisableSatellites = new bool?(!AreSatellitesEnabled)
                });
                if (AreSatellitesEnabled)
                {
                    Game.Screen.MessagesListWidget.AddMessage("Satellites disabled", GetColor(Faction), false);
                    AudioManager.PlaySound(Sounds.Shutdown, true, 0.5f, 1f, 0f);
                }
                else
                {
                    Game.Screen.MessagesListWidget.AddMessage("Satellites enabled", GetColor(Faction), false);
                    AudioManager.PlaySound(Sounds.Startup, true, 0.5f, 1f, 0f);
                }
            }
            if (!this.GameResultDialogShown && Status != FactionStatus.Undecided && Time.FrameStartTime > StatusChangeTime + 2.0)
            {
                this.GameResultDialogShown = true;
                DialogsManager.ShowDialog(null, new GameResultDialog(this), true);
            }
        }

        public override void Draw(Color colorTransform)
        {
            if (PlayersModule.ControllingPlayer != this)
            {
                return;
            }
            this.TmpVisualRoutes.Clear();
            this.TmpVisualRoutes.AddRange(this.VisualRoutes);
            foreach (VisualRoute visualRoute in this.TmpVisualRoutes)
            {
                visualRoute.Draw(colorTransform);
                if (visualRoute.TimeToLive <= 0f)
                {
                    this.RemoveVisualRoute(visualRoute);
                }
            }
            if (this.DragStartPosition != null && this.DragEndPosition != null)
            {
                Vector2 corner = Vector2.Transform(this.DragStartPosition.Value, CameraModule.ScreenToWorldMatrix);
                Vector2 corner2 = Vector2.Transform(this.DragEndPosition.Value, CameraModule.ScreenToWorldMatrix);
                FlatBatch2D flatBatch2D = CameraModule.PrimitivesRenderer.FlatBatch(10, null, null, null);
                flatBatch2D.QueueQuad(corner, corner2, 0f, Color.White * 0.2f);
                flatBatch2D.QueueRectangle(corner, corner2, 0f, Color.White * 0.5f);
            }
        }

        private void HandleClick(Segment2 click)
        {
            IEnumerable<Planet> enumerable = from p in PlanetsModule.Planets
                                             where p.IsSelected
                                             select p;
            Planet planet = PlanetsModule.PickPlanet(click.Start);
            Planet planet2 = PlanetsModule.PickPlanet(click.End);
            Planet planet3 = (planet == planet2) ? planet : null;
            if (planet3 != null)
            {
                if (Keyboard.IsKeyDown(Key.Shift) && planet3.Faction == Faction)
                {
                    this.BlinkPlanets = false;
                    planet3.IsSelected = !planet3.IsSelected;
                    AudioManager.PlaySound(planet3.IsSelected ? Sounds.Select : Sounds.Deselect, false, 1f, 1f, 0f);
                    return;
                }
                if (!Keyboard.IsKeyDown(Key.Shift) && enumerable.Count<Planet>() > 0)
                {
                    if (enumerable.Count<Planet>() > 1 || enumerable.ElementAt(0) != planet3)
                    {
                        this.IssueMoveOrder(planet3, enumerable, SettingsManager.DefaultFleetStrength, Faction.None);
                        return;
                    }
                    if (enumerable.Count<Planet>() == 1 && enumerable.ElementAt(0) == planet3)
                    {
                        this.DeselectAllPlanets();
                        AudioManager.PlaySound(Sounds.Deselect, false, 1f, 1f, 0f);
                        if (planet3.ContinuousOrderRoute != null)
                        {
                            planet3.ContinuousOrderRoute = null;
                            Game.Screen.MessagesListWidget.AddMessage("Continuous order cleared", GetColor(Faction), true);
                            return;
                        }
                    }
                }
                else if (planet3.Faction == Faction)
                {
                    this.DeselectAllPlanets();
                    this.BlinkPlanets = false;
                    planet3.IsSelected = true;
                    AudioManager.PlaySound(Sounds.Select, false, 1f, 1f, 0f);
                    return;
                }
            }
            else if (!Keyboard.IsKeyDown(Key.Shift))
            {
                if (enumerable.Count<Planet>() > 0)
                {
                    AudioManager.PlaySound(Sounds.Deselect, false, 1f, 1f, 0f);
                }
                this.DeselectAllPlanets();
            }
        }

        private void HandleSecondaryClick(Segment2 click)
        {
            Planet planet = PlanetsModule.PickPlanet(click.Start);
            Planet planet2 = PlanetsModule.PickPlanet(click.End);
            Planet planet3 = (planet == planet2) ? planet : null;
            if (planet3 != null)
            {
                OrderDialog orderDialog = new OrderDialog(this, planet3, from p in PlanetsModule.Planets
                                                                         where p.IsSelected
                                                                         select p);
                if (orderDialog.ListWidget.Items.Count > 0)
                {
                    AudioManager.PlaySound(Sounds.Click, false, 1f, 1f, 0f);
                    DialogsManager.ShowDialog(Game.Screen, orderDialog, true);
                    this.BlinkPlanets = false;
                }
            }
        }

        private void HandleHold(Vector2 hold)
        {
            Planet planet = PlanetsModule.PickPlanet(hold);
            if (planet != null)
            {
                OrderDialog orderDialog = new OrderDialog(this, planet, from p in PlanetsModule.Planets
                                                                        where p.IsSelected
                                                                        select p);
                if (orderDialog.ListWidget.Items.Count > 0)
                {
                    AudioManager.PlaySound(Sounds.Click, false, 1f, 1f, 0f);
                    DialogsManager.ShowDialog(Game.Screen, orderDialog, true);
                    this.BlinkPlanets = false;
                }
            }
        }

        private void HandleDrag(Segment2 drag)
        {
            int num = (from p in PlanetsModule.Planets
                       where p.IsSelected
                       select p).Count<Planet>();
            this.DeselectAllPlanets();
            Vector2 v = Vector2.Transform(drag.Start, CameraModule.ScreenToWorldMatrix);
            Vector2 v2 = Vector2.Transform(drag.End, CameraModule.ScreenToWorldMatrix);
            BoundingRectangle boundingRectangle = new BoundingRectangle(Vector2.Min(v, v2), Vector2.Max(v, v2));
            foreach (Planet planet in PlanetsModule.Planets)
            {
                if (planet.Faction == Faction && boundingRectangle.Intersection(new BoundingCircle(new Vector2(planet.Position), planet.Radius)))
                {
                    this.BlinkPlanets = false;
                    planet.IsSelected = true;
                }
            }
            if ((from p in PlanetsModule.Planets
                 where p.IsSelected
                 select p).Count<Planet>() > num)
            {
                AudioManager.PlaySound(Sounds.Select, true, 1f, 1f, 0f);
            }
        }

        private void DeselectAllPlanets()
        {
            foreach (Planet planet in PlanetsModule.Planets)
            {
                planet.IsSelected = false;
            }
        }

        private void AddVisualRoute(VisualRoute visualRoute)
        {
            if (visualRoute.Game != null)
            {
                throw new InvalidOperationException();
            }
            this.VisualRoutes.Add(visualRoute);
            visualRoute.Game = Game;
        }

        private void RemoveVisualRoute(VisualRoute visualRoute)
        {
            if (visualRoute.Game != Game)
            {
                throw new InvalidOperationException();
            }
            this.VisualRoutes.Remove(visualRoute);
            visualRoute.Game = null;
        }

        private bool GameResultDialogShown;

        private DynamicArray<VisualRoute> VisualRoutes = new DynamicArray<VisualRoute>();

        private DynamicArray<VisualRoute> TmpVisualRoutes = new DynamicArray<VisualRoute>();

        private Vector2? DragStartPosition;

        private Vector2? DragEndPosition;
    }
}
