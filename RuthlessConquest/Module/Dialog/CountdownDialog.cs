using System;
using Engine;

namespace Game
{
    internal class CountdownDialog : Dialog
    {
        public CountdownDialog(Client client)
        {
            this.Client = client;
            this.Children.Add(new InterlaceWidget());
            this.Children.Add(this.MessageWidget = new LabelWidget());
        }

        public override void Update()
        {
            if (!this.Client.IsConnected)
            {
                DialogsManager.HideDialog(this, false);
                return;
            }
            if (!this.Client.Game.StepModule.IsGameStarted)
            {
                return;
            }
            if (Client.Game.CreationParameters.CountdownTicksCount * 0.5f - Client.Game.StepModule.CountdownStepsLeft * 0.0166666675f > 4f)
            {
                int num = (int)MathUtils.Ceiling(Client.Game.StepModule.CountdownStepsLeft * 0.0166666675f);
                if (num > 0)
                {
                    if (this.LastCounter < 0)
                    {
                        this.LastCounter = num;
                    }
                    if (num != this.LastCounter)
                    {
                        this.MessageWidget.Text = string.Format("START IN {0}", num);
                        this.MessageWidget.FontScale = 1f;
                        this.LastCounter = num;
                        AudioManager.PlaySound(Sounds.Countdown, false, 1f, 1f, 0f);
                        return;
                    }
                }
                else if (!this.IsHiding)
                {
                    this.IsHiding = true;
                    AudioManager.PlaySound(Sounds.Start, false, 1f, 1f, 0f);
                    this.MessageWidget.Text = "GO!";
                    this.MessageWidget.FontScale = 1.5f;
                    Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.6, delegate
                    {
                        DialogsManager.HideDialog(this, true);
                    });
                }
            }
        }

        private Client Client;

        private LabelWidget MessageWidget;

        private int LastCounter = -1;

        private bool IsHiding;
    }
}
