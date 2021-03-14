using System;
using Engine;
using Engine.Audio;

namespace Game
{
    public class ClickableWidget : Widget
    {
        public SoundBuffer Sound { get; set; }

        public Vector2 Size { get; set; } = new Vector2(float.PositiveInfinity);

        public bool IsPressed { get; private set; }

        public bool IsClicked { get; private set; }

        public bool IsTapped { get; private set; }

        public bool IsChecked { get; set; }

        public bool IsAutoCheckingEnabled { get; set; }

        public bool IsOkButton { get; set; }

        public bool IsCancelButton { get; set; }

        public override void UpdateCeases()
        {
            base.UpdateCeases();
            this.IsPressed = false;
            this.IsClicked = false;
            this.IsTapped = false;
        }

        public override void Update()
        {
            WidgetInput input = Input;
            this.IsPressed = false;
            this.IsTapped = false;
            this.IsClicked = false;
            if (input.Press != null && WidgetsManager.HitTest(input.Press.Value) == this)
            {
                this.IsPressed = true;
            }
            if (input.Tap != null && WidgetsManager.HitTest(input.Tap.Value) == this)
            {
                this.IsTapped = true;
            }
            if ((input.Click != null && WidgetsManager.HitTest(input.Click.Value.Start) == this && WidgetsManager.HitTest(input.Click.Value.End) == this) || (this.IsOkButton && input.Ok) || (this.IsCancelButton && (input.Cancel || input.Back)))
            {
                this.IsClicked = true;
                if (this.IsAutoCheckingEnabled)
                {
                    this.IsChecked = !this.IsChecked;
                }
                if (this.Sound != null)
                {
                    AudioManager.PlaySound(this.Sound, false, 1f, 1f, 0f);
                }
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            DesiredSize = this.Size;
        }
    }
}
