using System;
using System.Xml.Linq;
using Engine.Content;

namespace Game
{
    internal class TextBoxDialog : Dialog
    {
        public TextBoxDialog(string title, string text, int maximumLength, Action<string> handler)
        {
            this.m_handler = handler;
            XElement node = ContentCache.Get<XElement>("Dialogs/TextBoxDialog", true);
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.m_titleWidget = this.Children.Find<LabelWidget>("TextBoxDialog.Title", true);
            this.m_textBoxWidget = this.Children.Find<TextBoxWidget>("TextBoxDialog.TextBox", true);
            this.m_okButtonWidget = this.Children.Find<BevelledButtonWidget>("TextBoxDialog.OkButton", true);
            this.m_cancelButtonWidget = this.Children.Find<BevelledButtonWidget>("TextBoxDialog.CancelButton", true);
            this.m_okButtonWidget.Sound = Sounds.Click2;
            this.m_okButtonWidget.IsOkButton = true;
            this.m_cancelButtonWidget.Sound = Sounds.Click2;
            this.m_cancelButtonWidget.IsCancelButton = true;
            this.m_titleWidget.IsVisible = !string.IsNullOrEmpty(title);
            this.m_titleWidget.Text = (title ?? string.Empty);
            this.m_textBoxWidget.MaximumLength = maximumLength;
            this.m_textBoxWidget.Text = (text ?? string.Empty);
            this.m_textBoxWidget.Title = title;
            this.m_textBoxWidget.Title = title;
            this.m_textBoxWidget.HasFocus = true;
            this.AutoHide = true;
        }

        public bool AutoHide { get; set; }

        public override void Update()
        {
            if (Input.Cancel)
            {
                this.Dismiss(null);
                return;
            }
            if (Input.Ok)
            {
                this.Dismiss(this.m_textBoxWidget.Text);
                return;
            }
            if (this.m_okButtonWidget.IsClicked)
            {
                this.Dismiss(this.m_textBoxWidget.Text);
                return;
            }
            if (this.m_cancelButtonWidget.IsClicked || DialogCoverWidget.Clickable.IsClicked)
            {
                this.Dismiss(null);
            }
        }

        private void Dismiss(string result)
        {
            if (this.AutoHide)
            {
                DialogsManager.HideDialog(this, true);
            }
            Action<string> handler = this.m_handler;
            if (handler == null)
            {
                return;
            }
            handler(result);
        }

        private Action<string> m_handler;

        private LabelWidget m_titleWidget;

        private TextBoxWidget m_textBoxWidget;

        private BevelledButtonWidget m_okButtonWidget;

        private BevelledButtonWidget m_cancelButtonWidget;
    }
}
