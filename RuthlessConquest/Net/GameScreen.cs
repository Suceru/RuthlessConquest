// Decompiled with JetBrains decompiler
// Type: GameScreen
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class GameScreen : Screen
    {
        private PlayerLabelWidget[] InternalPlayerLabels = new PlayerLabelWidget[7];
        private bool DisconnectedDialogShown;

        public Server Server { get; private set; }

        public Client Client { get; private set; }

        public GameWidget GameWidget { get; private set; }

        public IReadOnlyList<PlayerLabelWidget> PlayerLabels => InternalPlayerLabels;

        public MessagesListWidget MessagesListWidget { get; private set; }

        public MessageWidget ConnectionInterruptionMessageWidget { get; private set; }

        public AliensAttackMessageWidget AliensAttackMessageWidget { get; private set; }

        public ReinforcementsMessageWidget ReinforcementsMessageWidget { get; private set; }

        public GameScreen()
        {
            this.IsDrawRequired = true;
            this.IsOverdrawRequired = true;
            this.Children.Add(new BackgroundWidget()
            {
                ShiftSpeed = 0.0f,
                Density = 0.6f,
                Brightness = 0.7f
            });
            this.Children.Add(this.GameWidget = new GameWidget());
            for (int labelIndex = 0; labelIndex < this.InternalPlayerLabels.Length; ++labelIndex)
            {
                this.InternalPlayerLabels[labelIndex] = new PlayerLabelWidget(this, labelIndex);
                this.Children.Add(this.InternalPlayerLabels[labelIndex]);
            }
            WidgetsList children1 = this.Children;
            MessagesListWidget messagesListWidget1 = new MessagesListWidget();
            messagesListWidget1.Alignment = (0.0f, 1f);
            messagesListWidget1.Margin = (0.0f, 36f);
            MessagesListWidget messagesListWidget2 = messagesListWidget1;
            this.MessagesListWidget = messagesListWidget1;
            MessagesListWidget messagesListWidget3 = messagesListWidget2;
            children1.Add(messagesListWidget3);
            WidgetsList children2 = this.Children;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.IsHitTestVisible = false;
            stackPanelWidget.Direction = LayoutDirection.Vertical;
            stackPanelWidget.Children.Add(this.ConnectionInterruptionMessageWidget = new MessageWidget());
            stackPanelWidget.Children.Add(this.AliensAttackMessageWidget = new AliensAttackMessageWidget());
            stackPanelWidget.Children.Add(this.ReinforcementsMessageWidget = new ReinforcementsMessageWidget());
            children2.Add(stackPanelWidget);
        }

        public override void Enter(object[] parameters)
        {
            this.Server = (Server)parameters[0];
            this.Client = (Client)parameters[1];
            this.Client.Game.Screen = this;
            this.GameWidget.Client = this.Client;
            this.MessagesListWidget.ClearMessages();
            if (this.Client.Game.CreationParameters.MaxHumanPlayersCount <= 1)
            {
                this.Client.StartGame();
                DialogsManager.ShowDialog(GameWidget, new CountdownDialog(this.Client), false, new Color(0, 0, 0, 64));
            }
            else
                DialogsManager.ShowDialog(this, new WaitingForPlayersDialog(this.Client), false);
            if (this.Server == null || !this.Server.IsUsingInProcessTransmitter || SettingsManager.DontShowInstructions)
                return;
            DialogsManager.ShowDialog(null, new GameMenuDialog(this, true));
        }

        public override void Leave()
        {
            foreach (PlayerLabelWidget playerLabel in PlayerLabels)
                playerLabel.Player = null;
            MusicManager.CurrentMix = MusicManager.Mix.Menu;
            this.GameWidget.Client = null;
            this.Client.Dispose();
            this.Client = null;
            this.Server?.Dispose();
            this.Server = null;
        }

        public override void Update()
        {
            if (!this.Client.IsConnected)
            {
                if (this.DisconnectedDialogShown)
                    return;
                this.DisconnectedDialogShown = true;
                DialogsManager.ShowDialog(this, new MessageDialog("DISCONNECTED", "You have been disconnected from the game.", "OK", null, b => ScreensManager.SwitchScreen("MainMenu")));
            }
            else
                this.DisconnectedDialogShown = false;
        }

        public override void Draw()
        {
            if (ScreensManager.IsAnimating)
                return;
            Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
        }

        public override void Overdraw()
        {
            if (this.Server == null || !this.Server.Games.Any<ServerGame>(g => g.DesyncDetected))
                return;
            WidgetsManager.PrimitivesRenderer2D.FontBatch(layer: 1000).QueueText("Desync", Vector2.Zero, 0.0f, Color.Gray);
        }

        public void Disconnect()
        {
            this.DisconnectedDialogShown = true;
            this.Client.DisconnectFromGame();
            ScreensManager.SwitchScreen("MainMenu");
        }

        public PlayerLabelWidget FindPlayerLabel(Player player)
        {
            foreach (PlayerLabelWidget playerLabel in PlayerLabels)
            {
                if (playerLabel.Player == player)
                    return playerLabel;
            }
            return null;
        }
    }
}

