using System;
using System.Xml.Linq;
using Engine.Content;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 繁忙对话框
    /// 继承:对话框
    /// </summary>
    internal class BusyDialog : Dialog
    {
        public BusyDialog(string largeMessage, string smallMessage)
        {
            XElement node = ContentCache.Get<XElement>("Dialogs/BusyDialog", true);
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.m_largeLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.LargeLabel", true);
            this.m_smallLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.SmallLabel", true);
            this.LargeMessage = largeMessage;
            this.SmallMessage = smallMessage;
        }

        public string LargeMessage
        {
            get
            {
                return this.m_largeLabelWidget.Text;
            }
            set
            {
                this.m_largeLabelWidget.Text = (value ?? string.Empty);
                this.m_largeLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        public string SmallMessage
        {
            get
            {
                return this.m_smallLabelWidget.Text;
            }
            set
            {
                this.m_smallLabelWidget.Text = (value ?? string.Empty);
                this.m_smallLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        public override void Update()
        {
            if (Input.Back)
            {
                Input.Clear();
            }
        }

        private LabelWidget m_largeLabelWidget;

        private LabelWidget m_smallLabelWidget;
    }
}
